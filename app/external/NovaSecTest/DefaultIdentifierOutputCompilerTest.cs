using System;
using System.Collections.Generic;
using NovaSec.Parser.AbstractSyntaxTree;
using NovaSec.Compiler.Compilers;
using NovaSec.Compiler.Resolvers;
using NUnit.Framework;
using System.Linq.Expressions;

namespace NovaSecTest
{
    public class DefaultIdentifierOutputResolverTest
    {
        private AccessTestClass _inputObject;
        private DefaultIdentifierOutputCompiler<AccessTestClass, DefaultIdentifierInputCompiler, TargetMethodInputResolver> _compiler;

        [SetUp]
        public void Setup()
        {
            _inputObject = new AccessTestClass()
            {
                Nested = new AccessTestClass()
                {
                    BooleanProperty = true,
                }
            };
            _compiler = new DefaultIdentifierOutputCompiler<AccessTestClass, DefaultIdentifierInputCompiler, TargetMethodInputResolver>(new DefaultIdentifierInputCompiler());
        }

        private T CompileAndRun<T>(Identifier identifier, ResultObjectResolver<AccessTestClass> input)
        {
            ParameterExpression identifierResolverParam = Expression.Parameter(typeof(ResultObjectResolver<AccessTestClass>));
            var body = _compiler.Compile(identifier, identifierResolverParam);
            var lambda = Expression.Lambda<Func<ResultObjectResolver<AccessTestClass>, T>>(
                body, false, new List<ParameterExpression>() { identifierResolverParam }
            );
            var func = lambda.Compile();
            return func(input);
        }

        [Test]
        public void ResolveOutputObject()
        {
            var identifier = new Identifier("resultObject.Nested.BooleanProperty");
            var input = new ResultObjectResolver<AccessTestClass>(_inputObject);
            Assert.IsTrue(CompileAndRun<bool>(identifier, input));
        }
    }
}