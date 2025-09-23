using NovaSec.Attributes;
using NovaSec.Exceptions;
using NovaSec.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NovaSec.Compiler;
using NovaSec.Compiler.Compilers;
using NovaSec.Compiler.Resolvers;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec
{
    public class SecurityContext
    {
        private readonly SecurityExpressionParser _parser = new SecurityExpressionParser();
        private readonly ISecurityExpressionRoot _securityExpressionRoot;
        private readonly IInjectResolver _injectResolver;
        
        private readonly Dictionary<string, IExpression> _absTreeCache = new Dictionary<string, IExpression>();

        public SecurityContext(ISecurityExpressionRoot securityExpressionRoot, IInjectResolver injectResolver)
        {
            _securityExpressionRoot = securityExpressionRoot;
            _injectResolver = injectResolver;
        }

        /// <summary>
        /// Checks prior to invocation, if the method should be invoked in accordance with the parameter values provided
        /// and the associated security expressions
        /// </summary>
        /// <param name="methodInfo">MethodInfo of the method being called</param>
        /// <param name="parameters">Parameters provided to the method call</param>
        /// <param name="attributes">Security expressions associated with the method call</param>
        /// <param name="observer">Optional observer for measuring performance security.</param>
        /// <returns>False if the method should be blocked from executing.</returns>
        public bool PreAuthorize(MethodInfo methodInfo, object[] parameters, List<PreAuthorizeAttribute> attributes, 
            ISecurityContextObserver observer = null)
        {
            var identifierCompiler = new DefaultIdentifierInputCompiler(methodInfo);
            var identityResolver = new TargetMethodInputResolver(methodInfo, parameters);
            return PreAuthorize(identifierCompiler, identityResolver, attributes);
        }

        internal bool PreAuthorize<TIdentifierInstanceResolver, TIdentifierCompiler>(
            TIdentifierCompiler identifierCompiler, TIdentifierInstanceResolver instanceResolver,
            List<PreAuthorizeAttribute> attributes, ISecurityContextObserver observer = null)
            where TIdentifierInstanceResolver : IIdentifierResolver
            where TIdentifierCompiler : IIdentifierCompiler<TIdentifierInstanceResolver>
        {
            observer?.BeginPreAuthorize();
            var result = true;
            if (attributes != null && attributes.Any())
            {
                // make evaluator for given parameters and functions
                var functionCompiler = new DefaultFunctionCompiler<TIdentifierInstanceResolver, TIdentifierCompiler>
                    (_securityExpressionRoot, _injectResolver);
                var expressionCompiler = new ExpressionCompiler<TIdentifierCompiler, DefaultFunctionCompiler<TIdentifierInstanceResolver, TIdentifierCompiler>, TIdentifierInstanceResolver>();

                var executableExpressions = new Dictionary<PreAuthorizeAttribute, Func<TIdentifierInstanceResolver, bool>>();
                // parse all attributes
                foreach (var attribute in attributes)
                {
                    PrepareAttributeExpression(attribute);
                    observer?.BeginExpressionCompilation(attribute);
                    Func<TIdentifierInstanceResolver, bool> executableExpression =
                        expressionCompiler.Compile(attribute.TreeExpression, identifierCompiler, functionCompiler);
                    observer?.EndExpressionCompilation(attribute);
                    executableExpressions.Add(attribute, executableExpression);
                }

                // evaluate until a predicate is true or sequence is empty
                result = attributes.Any(attribute =>
                {
                    observer?.BeginExpressionEvaluation(attribute);
                    bool attributeResult = executableExpressions[attribute](instanceResolver);
                    observer?.EndExpressionEvaluation(attribute);
                    return attributeResult;
                });
            }

            observer?.EndPreAuthorize();
            return result;
        }
        
        private void PrepareAttributeExpression(BaseAttribute attribute)
        {
            if (attribute.Key == null)
            {
                throw new SecurityExpressionAttributeException($"Failed to extract non-empty key for attribute of type ${attribute.GetType().Name} with expression '${attribute.SecurityExpression}'");
            }

            if (!_absTreeCache.ContainsKey(attribute.Key))
            {
                _absTreeCache.Add(attribute.Key, _parser.Parse(attribute.SecurityExpression));
            }

            attribute.TreeExpression = _absTreeCache[attribute.Key];
        }

        #region Redaction

        private class VarTree
        {
            private readonly string _name;
            private List<VarTree> _children;

            public VarTree(string name)
            {
                _name = name;
                _children = new List<VarTree>();
            }

            private VarTree GetChild(string name)
            {
                return _children.FirstOrDefault(c => c._name == name);
            }

            private VarTree EnsureChild(string name)
            {
                var result = GetChild(name);
                if (result == null)
                {
                    result = new VarTree(name);
                    _children.Add(result);
                }
                return result;
            }

            private void Prune()
            {
                _children = new List<VarTree>();
            }

            public void Add(List<string> segments, int idx = 0)
            {
                if (idx < segments.Count)
                {
                    var segment = segments[idx];
                    var child = GetChild(segment);
                    if (child == null || child._children.Any())
                    {
                        child = EnsureChild(segment);
                        child.Add(segments, idx + 1);
                    }
                    else
                    {
                        // this redaction already exists and is more general than the one being added
                    }
                }
                else
                {
                    // the redaction being added is more general, remove any sub-redaction
                    Prune();
                }
            }

            public List<string> GetPaths()
            {
                var result = new List<string>();
                if (_children.Any())
                {
                    foreach (var child in _children)
                    {
                        result.AddRange(child.GetPaths().Select(childPath => _name != null ? $"{_name}.{childPath}" : childPath));
                    }
                }
                else if (_name != null)
                {
                    result.Add(_name);
                }
                return result;
            }
        }

        internal List<string> FindCommonRedactionPaths(List<List<string>> redactionList)
        {
            var tree = new VarTree(null);
            foreach (var redaction in redactionList)
            {
                foreach (var redactionPath in redaction)
                {
                    var cur = tree;
                    var segments = redactionPath.Split('.').ToList();
                    tree.Add(segments);
                }
            }

            return tree.GetPaths();
        }

        internal void RedactObject(object target, List<string> redaction)
        {
            if (target == null)
            {
                throw new SecurityExpressionRedactionException(
                    $"Could not redact field on result object. Target object was null");
            }
            foreach (var redactionPath in redaction)
            {
                var cur = target;
                var segments = redactionPath.Split('.').ToList();
                for (var i = 0; i < segments.Count; i++)
                {
                    var segment = segments[i];
                    var isLeaf = i == segments.Count - 1;

                    var prop = cur.GetType().GetField(segment) as MemberInfo ??
                               cur.GetType().GetProperty(segment) as MemberInfo;

                    if (prop == null)
                    {
                        throw new SecurityExpressionRedactionException(
                            $"Could not redact field on result object, no such property on {redactionPath} segment index {i} for type {target.GetType()}");
                    }

                    if (isLeaf)
                    {
                        // do the redaction
                        if (prop is PropertyInfo pi)
                        {
                            try
                            {
                                pi.SetValue(cur, null);
                            }
                            catch (ArgumentException e)
                            {
                                throw new SecurityExpressionRedactionException(
                                    $"Could not redact property at {redactionPath} for type {target.GetType()}. Does it have a public setter?",
                                    e);
                            }
                        }
                        else if (prop is FieldInfo fi)
                        {
                            fi.SetValue(cur, null);
                        }

                    }
                    else
                    {
                        if (prop is PropertyInfo pi)
                        {
                            cur = pi.GetValue(cur, null);
                        }
                        else if (prop is FieldInfo fi)
                        {
                            cur = fi.GetValue(cur);
                        }
                        if (cur == null)
                        {
                            // already "redacted"
                            break;
                        }
                        if (cur is IEnumerable curEnumerable)
                        {
                            foreach (var item in curEnumerable)
                            {
                                var newRedactionPath = segments.GetRange(i + 1, segments.Count - (i + 1));
                                var newRedaction = new List<string>
                                {
                                    string.Join('.', newRedactionPath)
                                };
                                RedactObject(item, newRedaction);
                            }
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Do a filtering and/or redaction of the result objects returned from a method call according to the associated security expressions.
        /// </summary>
        /// <typeparam name="T">The type of the method result objects</typeparam>
        /// <param name="resultCollection">Collection of result objects to be filtered</param>
        /// <param name="attributes">Security expressions associated with the method</param>
        /// <param name="observer">Optional observer for measuring performance security.</param>
        /// <returns>The filtered and redacted collection.</returns>
        public IEnumerable<T> PostFilter<T>(
            IEnumerable<T> resultCollection,
            List<PostFilterAttribute> attributes, 
            ISecurityContextObserver observer = null)
        {
            return PostFilter(resultCollection, attributes, null, null, observer);
        }

        public IEnumerable<T> PostFilter<T>(
            IEnumerable<T> resultCollection, 
            List<PostFilterAttribute> attributes, 
            MethodInfo methodInfo, 
            object[] parameters, 
            ISecurityContextObserver observer = null)
        {
            observer?.BeginPostFilter();
            // filter entries
            if (attributes != null && attributes.Any())
            {
                var identifierInputCompiler = new DefaultIdentifierInputCompiler(methodInfo);
                var identifierOutputCompiler = new DefaultIdentifierOutputCompiler<T, DefaultIdentifierInputCompiler, TargetMethodInputResolver>(identifierInputCompiler);
                var functionCompiler = new DefaultFunctionCompiler<ResultObjectResolver<T>, DefaultIdentifierOutputCompiler<T, DefaultIdentifierInputCompiler, TargetMethodInputResolver>>
                    (_securityExpressionRoot, _injectResolver);
                var expressionCompiler = new ExpressionCompiler<
                    DefaultIdentifierOutputCompiler<T, DefaultIdentifierInputCompiler, TargetMethodInputResolver>,
                    DefaultFunctionCompiler<ResultObjectResolver<T>, DefaultIdentifierOutputCompiler<T, DefaultIdentifierInputCompiler, TargetMethodInputResolver>>,
                    ResultObjectResolver<T>>();

                var executableExpressions = new Dictionary<BaseAttribute, Func<ResultObjectResolver<T>, bool>>();
                // parse all attributes and compile these to executable expressions
                foreach (var attribute in attributes)
                {
                    PrepareAttributeExpression(attribute);
                    observer?.BeginExpressionCompilation(attribute);
                    Func<ResultObjectResolver<T>, bool> executableExpression =
                        expressionCompiler.Compile(attribute.TreeExpression, identifierOutputCompiler, functionCompiler);
                    observer?.EndExpressionCompilation(attribute);
                    executableExpressions.Add(attribute, executableExpression);
                }

                var filteredResults = new List<T>();
                foreach (var resultObject in resultCollection)
                {
                    var wasTrue = new List<PostFilterAttribute>();
                    foreach (var attribute in attributes)
                    {
                        var input = new ResultObjectResolver<T>(resultObject, methodInfo, parameters);
                        
                        observer?.BeginExpressionEvaluation(attribute);
                        bool securityExpressionResult = executableExpressions[attribute](input);
                        observer?.EndExpressionEvaluation(attribute);
                        
                        if (securityExpressionResult)
                        {
                            wasTrue.Add(attribute);
                        }
                    }

                    if (!wasTrue.Any())
                    {
                        observer?.OnPostFilterFail(resultObject);
                        continue;
                    }

                    var redaction = wasTrue.Select(a => a.RedactionPaths).ToList();
                    if (redaction.Count > 0 && redaction.All(r => r != null))
                    {
                        var effectiveRedaction =
                            FindCommonRedactionPaths(redaction);
                        RedactObject(resultObject, effectiveRedaction);
                    }
                    
                    observer?.OnPostFilterPass(resultObject);
                    filteredResults.Add(resultObject);
                }

                observer?.EndPostFilter();
                return filteredResults;
            }

            observer?.EndPostFilter();
            return resultCollection;
        }
    }
}
