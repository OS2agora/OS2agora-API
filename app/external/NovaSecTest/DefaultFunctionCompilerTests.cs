using System;
using Moq;
using NovaSec.Exceptions;
using NovaSec.Parser.AbstractSyntaxTree;
using NovaSec.Compiler.Compilers;
using NovaSec.Compiler.Resolvers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NovaSec.Compiler;

namespace NovaSecTest
{
    public class DefaultFunctionCompilerTests
    {
        private AccessTestClass _inputObject;
        private MethodInfo _callMeMethodInfo;
        private Mock<IInjectedTestObject> _injectObjectMock;
        private Mock<ISecurityExpressionRoot> _rootMock;
        private StaticInjectResolver _injectResolver;
        private DefaultIdentifierInputCompiler _identifierInputCompiler;
        private DefaultFunctionCompiler<TargetMethodInputResolver, DefaultIdentifierInputCompiler> _functionCompiler;
        private TargetMethodInputResolver _inputResolver;

        [SetUp]
        public void Setup()
        {
            _inputObject = new AccessTestClass
            {
                BooleanProperty = true,
                BooleanField = false,
                Nested = new AccessTestClass
                {
                    BooleanProperty = false,
                    BooleanField = true
                }
            };
            _callMeMethodInfo = typeof(AccessTestClass).GetMethod("CallMe");
            _inputResolver = new TargetMethodInputResolver(_callMeMethodInfo, new[] { _inputObject });
            _injectObjectMock = new Mock<IInjectedTestObject>(MockBehavior.Strict);
            _rootMock = new Mock<ISecurityExpressionRoot>(MockBehavior.Strict);
            _injectResolver = new StaticInjectResolver(new Dictionary<string, object> {{"injectObject", _injectObjectMock.Object}});

            _identifierInputCompiler = new DefaultIdentifierInputCompiler(_callMeMethodInfo);
            _functionCompiler = new DefaultFunctionCompiler<TargetMethodInputResolver, DefaultIdentifierInputCompiler>
                (_rootMock.Object, _injectResolver);
        }

        private T CompileAndRun<T>(Function function, TargetMethodInputResolver input)
        {
            ParameterExpression identifierResolverParam = Expression.Parameter(typeof(TargetMethodInputResolver));
            var body = _functionCompiler.Compile(function, _identifierInputCompiler, identifierResolverParam);
            var lambda = Expression.Lambda<Func<TargetMethodInputResolver, T>>(
                body, false, new List<ParameterExpression>() { identifierResolverParam }
            );
            var func = lambda.Compile();
            return func(input);
        }

        [Test]
        public void ResolveRootFunctionTest()
        {
            // arrange
            var hasRoleInput1 = new Function("HasRole", new List<IFunctionArgument> { new StringArgument("'ROLE1'") });
            var hasRoleInput2 = new Function("HasRole", new List<IFunctionArgument> { new StringArgument("'ROLE2'") });
            var hasAllRolesInput = new Function("HasAllRoles",
                new List<IFunctionArgument> { new ListArgument(new List<IListItem> { new StringArgument("'ROLE3'") }) });
            var hasAnyRoleInput = new Function("HasAnyRole",
                new List<IFunctionArgument> { new ListArgument(new List<IListItem> { new StringArgument("'ROLE4'") }) });
            _rootMock.Setup(r => r.HasRole("ROLE1")).Returns(true);
            _rootMock.Setup(r => r.HasRole("ROLE2")).Returns(false);
            _rootMock.Setup(r => r.HasAllRoles(It.IsAny<List<string>>())).Returns(true);
            _rootMock.Setup(r => r.HasAnyRole(It.IsAny<List<string>>())).Returns(false);

            // act
            var hasRoleResult1 = CompileAndRun<bool>(hasRoleInput1, _inputResolver);
            var hasRoleResult2 = CompileAndRun<bool>(hasRoleInput2, _inputResolver);
            var hasAllRolesResult = CompileAndRun<bool>(hasAllRolesInput, _inputResolver);
            var hasAnyRoleResult = CompileAndRun<bool>(hasAnyRoleInput, _inputResolver);

            // assert - note that this is also act, because expressions get evaluated here
            Assert.IsTrue(hasRoleResult1);
            Assert.IsFalse(hasRoleResult2);
            Assert.IsTrue(hasAllRolesResult);
            Assert.IsFalse(hasAnyRoleResult);
            _rootMock.VerifyAll();
        }

        [Test]
        public void ResolveNonExistingRootFunctionTest()
        {
            // arrange
            var nonExistingFunctionInput = new Function("DoesNotExist", new List<IFunctionArgument>());

            // act / assert
            Assert.Throws<SecurityExpressionResolveException>(() =>
                CompileAndRun<bool>(nonExistingFunctionInput, _inputResolver));
        }

        [Test]
        public void ResolveFunctionOnInjectedObjectTest()
        {
            var returnBooleanInput = new Function("@injectObject.ReturnBoolean",
                new List<IFunctionArgument> { new Identifier("#input.BooleanProperty") });
            _injectObjectMock.Setup(r => r.ReturnBoolean(true)).Returns(false);
            var returnStringInput = new Function("@injectObject.ReturnString",
                new List<IFunctionArgument> { new StringArgument("'inputstring'") });
            _injectObjectMock.Setup(r => r.ReturnString("inputstring")).Returns("resultstring");

            var returnBooleanResult = CompileAndRun<bool>(returnBooleanInput, _inputResolver);
            var returnStringResult = CompileAndRun<string>(returnStringInput, _inputResolver);

            Assert.IsFalse(returnBooleanResult);
            Assert.AreEqual("resultstring", returnStringResult);
            _injectObjectMock.VerifyAll();
        }


        [Test]
        public void ResolveNonExistentFunctionOnInjectedObjectTest()
        {
            var nonExistentFunctionInput = new Function("@injectObject.DoesNotExist", new List<IFunctionArgument>());

            Assert.Throws<SecurityExpressionResolveException>(() =>
                CompileAndRun<bool>(nonExistentFunctionInput, _inputResolver));
        }

        [Test]
        public void ResolveFunctionOnMissingInjectedObjectTest()
        {
            var nonExistentFunctionInput = new Function("@isNotInjected.DoesNotExist", new List<IFunctionArgument>());
            Assert.Throws<SecurityExpressionResolveException>(() =>
                CompileAndRun<bool>(nonExistentFunctionInput, _inputResolver));
        }

        [Test]
        public void ResolveFailureBecauseOfNoMatchingParametersOnInjectedObjectMethodTest()
        {
            var input = new Function("@injectObject.InputStringList",
                new List<IFunctionArgument> { new StringArgument("'inputstring'") });
            Assert.Throws<SecurityExpressionResolveException>(() =>
                CompileAndRun<bool>(input, _inputResolver));
        }
    }
}