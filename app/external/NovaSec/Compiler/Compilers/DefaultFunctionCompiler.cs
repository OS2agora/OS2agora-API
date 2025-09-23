using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NovaSec.Compiler.Resolvers;
using NovaSec.Exceptions;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Compiler.Compilers
{
    internal class DefaultFunctionCompiler<TIdentifierInstanceResolver, TIdentifierCompiler> : 
        IFunctionCompiler<TIdentifierInstanceResolver, TIdentifierCompiler>
        where TIdentifierInstanceResolver : IIdentifierResolver
        where TIdentifierCompiler : IIdentifierCompiler<TIdentifierInstanceResolver>
    {
        private readonly ISecurityExpressionRoot _root;
        private readonly IInjectResolver _injectResolver;

        public DefaultFunctionCompiler(ISecurityExpressionRoot root, IInjectResolver injectResolver)
        {
            _root = root;
            _injectResolver = injectResolver;
        }

        private MethodInfo FindMethodInfo(Type t, string methodName, Function function)
        {
            var methods = t.GetMethods().Where(m => m.Name == methodName).ToArray();
            var numMethods = methods.Count();

            if (numMethods == 0)
            {
                throw new SecurityExpressionResolveException(
                    $"{this.GetType()} unable to resolve function {function.Name} on type {t} to method {methodName}. No method by that name was found");
            }
            else
            {
                foreach (var method in methods)
                {
                    var i = 0;
                    var didFail = false;
                    foreach (var parameter in method.GetParameters())
                    {
                        if (i < function.Arguments.Count)
                        {
                            var funArg = function.Arguments[i];
                            var paramType = parameter.ParameterType;
                            if (funArg is Identifier)
                            {
                                // not trying to guess the identifier type, this is happy go lucky
                                continue;
                            }

                            if (funArg is StringArgument && paramType == typeof(string))
                            {
                                continue;
                            }

                            if (funArg is ListArgument && paramType.IsGenericType &&
                                paramType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                continue;
                            }
                        }
                        else if (parameter.HasDefaultValue)
                        {
                            continue;
                        }

                        didFail = true;
                        break;
                    }

                    if (!didFail)
                    {
                        return method;
                    }
                }
            }

            throw new SecurityExpressionResolveException(
                $"{this.GetType()} unable to resolve function {function.Name} on type {t} to method {methodName}. Could not find method with matching parameter types.");
        }

        private IEnumerable<Expression> ResolveFunctionParameters(
            Function function,
            TIdentifierCompiler identifierCompiler,
            ParameterExpression identifierResolverInstance)
        {
            return function.Arguments.Select((arg, index) =>
            {
                switch (arg)
                {
                    case StringArgument sa:
                        return (Expression) Expression.Constant(sa.Value);
                    case ListArgument la:
                    {
                        var addMethod = typeof(List<string>).GetMethod("Add");
                        var listItems = la.List.Select((listItem, listItemIndex) => listItem switch
                        {
                            StringArgument lsa => (Expression) Expression.Constant(lsa.Value),
                            Identifier li => identifierCompiler.Compile(li, identifierResolverInstance),
                            _ => throw new SecurityExpressionResolveException(
                                $"{this.GetType()} unable to resolve list item for function argument for function {function.Name} argument index {index} list item index {listItemIndex}"),
                        }).ToArray();
                        return Expression.ListInit(Expression.New(typeof(List<string>)), addMethod, listItems);
                    }
                    case Identifier i:
                        return identifierCompiler.Compile(i, identifierResolverInstance);
                    default:
                        throw new SecurityExpressionResolveException(
                            $"{this.GetType()} unable to resolve function argument for function {function.Name} argument index {index}");
                }
            }).ToList();
        }

        public Expression Compile(
            Function function,
            TIdentifierCompiler identifierCompiler,
            ParameterExpression identifierResolverInstance)
        {
            var functionParameters = ResolveFunctionParameters(function, identifierCompiler, identifierResolverInstance);
            Expression instance = null;
            MethodInfo target = null;

            var path = function.Name.Split('.').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            // if the function identifier starts with a '@' the object defining the function to be called is expected to have been injected from the application during setup of the SecurityContext
            if (function.Name.StartsWith('@'))
            {
                path[0] = path[0].Substring(1);
                if (!_injectResolver.Exists(path[0]))
                {
                    throw new SecurityExpressionResolveException(
                        $"{this.GetType()} unable to resolve injected function with name {function.Name}");
                }

                var obj = _injectResolver.Resolve(path[0]);
                instance = Expression.Constant(obj);
                var t = obj.GetType();
                for (var i = 1; i < path.Length - 1; i++)
                {
                    t = t.GetProperty(path[i]).PropertyType;
                    instance = Expression.PropertyOrField(instance, path[i]);
                }

                target = FindMethodInfo(t, path.Last(), function);
            }
            // assume this function is considered built-in and search for it in the SecurityExpressionRoot provided on setup
            else if (typeof(ISecurityExpressionRoot).GetMethod(path[0]) != null)
            {
                instance = Expression.Constant(_root);
                target = FindMethodInfo(typeof(ISecurityExpressionRoot), path[0], function);
            }

            if (instance == null || target == null)
            {
                throw new SecurityExpressionResolveException(
                    $"{this.GetType()} unable to resolve function with name {function.Name}");
            }

            var targetParameterTypes = target.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToList();
            functionParameters = functionParameters.Select((param, paramIndex) =>
                Expression.Convert(param, targetParameterTypes[paramIndex]));
            return Expression.Call(instance, target, functionParameters);
        }
    }
}