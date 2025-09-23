using NovaSec.Parser.AbstractSyntaxTree;
using NovaSec.Compiler.Resolvers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NovaSec.Compiler.Compilers;

namespace NovaSecTest
{
    public class DefaultIdentifierInputCompilerTest
    {
        private AccessTestClass _inputObject;
        private MethodInfo _callMeMethodInfo;
        private DefaultIdentifierInputCompiler _compiler;

        [SetUp]
        public void Setup()
        {
            _inputObject = new AccessTestClass()
            {
                BooleanProperty = true,
                BooleanField = false,
                Nested = new AccessTestClass()
                {
                    BooleanProperty = false,
                    BooleanField = true
                }
            };
            _callMeMethodInfo = typeof(AccessTestClass).GetMethod("CallMe");
            _compiler = new DefaultIdentifierInputCompiler(_callMeMethodInfo);
        }

        private T CompileAndRun<T>(Identifier identifier, TargetMethodInputResolver input)
        {
            ParameterExpression identifierResolverParam = Expression.Parameter(typeof(TargetMethodInputResolver));
            var body = _compiler.Compile(identifier, identifierResolverParam);
            var lambda = Expression.Lambda<Func<TargetMethodInputResolver, T>>(
                body, false, new List<ParameterExpression>() { identifierResolverParam }
            );
            var func = lambda.Compile();
            return func(input);
        }

        [Test]
        public void AccessInputParameterTest()
        {
            var boolProp = new Identifier("#input.BooleanProperty");
            var boolField = new Identifier("#input.BooleanField");
            var nestedBoolProp = new Identifier("#input.Nested.BooleanProperty");
            var nestedBoolField = new Identifier("#input.Nested.BooleanField");

            var input = new TargetMethodInputResolver(_callMeMethodInfo, new object[] { _inputObject });

            Assert.IsTrue(CompileAndRun<bool>(boolProp, input));
            Assert.IsFalse(CompileAndRun<bool>(boolField, input));
            Assert.IsFalse(CompileAndRun<bool>(nestedBoolProp, input));
            Assert.IsTrue(CompileAndRun<bool>(nestedBoolField, input));
        }

        [Test]
        public void NonExistingInputParameterTest()
        {
            var nonExistingLeaf = new Identifier("#input.Nested.DoesNotExist");
            var nonExistingNode = new Identifier("#input.DoesNotExist.BooleanProperty");

            var input = new TargetMethodInputResolver(_callMeMethodInfo, new object[] { _inputObject });

            Assert.Throws<ArgumentException>(() => CompileAndRun<object>(nonExistingLeaf, input));
            Assert.Throws<ArgumentException>(() => CompileAndRun<object>(nonExistingNode, input));
        }

        [Test]
        public void NestedNullInputParameterTest()
        {
            var nullLeaf = new Identifier("#input.Nested.Nested.Nested");
            var input = new TargetMethodInputResolver(_callMeMethodInfo, new object[] { _inputObject });
            Assert.Throws<NullReferenceException>(() => CompileAndRun<AccessTestClass>(nullLeaf, input));
        }
    }
}