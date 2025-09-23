using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NovaSec.Compiler.Resolvers;
using NovaSec.Exceptions;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Compiler.Compilers
{
    public class DefaultIdentifierInputCompiler : IIdentifierCompiler<TargetMethodInputResolver>
    {
        private readonly MethodInfo _targetMethodInfo;

        public DefaultIdentifierInputCompiler()
        {
        }

        public DefaultIdentifierInputCompiler(MethodInfo targetMethodInfo)
        {
            _targetMethodInfo = targetMethodInfo;
        }

        internal int GetParameterIndex(string argumentName, MethodInfo targetMethodInfo)
        {
            if (targetMethodInfo == null)
            {
                throw new SecurityExpressionResolveException(
                    $"{this.GetType()} cannot resolve input parameter for {argumentName} because identifier resolver was not given a MethodInfo object and thus have no method from where the parameter can be deduced");
            }
            var targetMethodParams = targetMethodInfo.GetParameters();
            var paramIdx = Array.FindIndex(targetMethodParams, p => p.Name == argumentName);
            if (paramIdx < 0)
            {
                throw new SecurityExpressionResolveException(
                    $"{this.GetType()} cannot resolve input parameter for {argumentName} because the parameter was not found on the target method! Available parameters were {string.Join(", ", targetMethodParams.Select(p => p.Name))}");
            }

            return paramIdx;
        }
        public Expression Compile(Identifier identifier, ParameterExpression identifierInstanceResolver)
        {
            // is method parameter. use the MethodInfo provided to identify which parameter it is, extract the value of it from the resolver and type cast it accordingly
            if (identifier.Name.StartsWith('#'))
            {
                var path = identifier.Name.Substring(1).Split('.').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                if (path.Length == 0)
                {
                    throw new SecurityExpressionResolveException(
                        $"{this.GetType()} cannot resolve input parameter for {identifier.Name} because name is empty!");
                }

                var targetMethodArgumentName = path[0];
                MethodInfo getArgValueByNameMethod = typeof(TargetMethodInputResolver).GetMethod(nameof(TargetMethodInputResolver.GetValueByName), new[] {typeof(string)})!;
                Expression getValue = Expression.Call(identifierInstanceResolver, getArgValueByNameMethod,
                    Expression.Constant(targetMethodArgumentName));
                var argType =
                    _targetMethodInfo.GetParameters()[GetParameterIndex(targetMethodArgumentName, _targetMethodInfo)]
                        .ParameterType;
                Expression result = Expression.Convert(getValue, argType);

                // dig into the nested property if identifier is dotted
                for (var i = 1; i < path.Length; i++)
                {
                    result = Expression.PropertyOrField(result, path[i]);
                }

                return result;
            }

            throw new SecurityExpressionResolveException(
                $"{this.GetType()} unable to resolve variable for identifier with name {identifier.Name}");
        }
    }
}