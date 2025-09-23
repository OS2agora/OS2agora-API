using Moq;
using NovaSec.Exceptions;
using NovaSec.Parser;
using NovaSec.Compiler.Compilers;
using NovaSec.Compiler.Resolvers;
using NovaSec.Parser.AbstractSyntaxTree;
using NUnit.Framework;
using System.Linq.Expressions;

namespace NovaSecTest
{
    public class ExpressionCompilerTest
    {
        private SecurityExpressionParser _parser;
        private NovaSec.Compiler.ExpressionCompiler<IIdentifierCompiler<TargetMethodInputResolver>, IFunctionCompiler<TargetMethodInputResolver, IIdentifierCompiler<TargetMethodInputResolver>>, TargetMethodInputResolver> _systemUnderTest;
        private Mock<IIdentifierCompiler<TargetMethodInputResolver>> _identifierCompiler;
        private Mock<TargetMethodInputResolver> _identifierResolver;
        private Mock<IFunctionCompiler<TargetMethodInputResolver, IIdentifierCompiler<TargetMethodInputResolver>>>
            _functionCompiler;
        
        [SetUp]
        public void Setup()
        {
            _identifierCompiler = new Mock<IIdentifierCompiler<TargetMethodInputResolver>>();
            _identifierResolver = new Mock<TargetMethodInputResolver>();
            _functionCompiler =
                new Mock<IFunctionCompiler<TargetMethodInputResolver, IIdentifierCompiler<TargetMethodInputResolver>>>();
            _parser = new SecurityExpressionParser();
            _systemUnderTest = new NovaSec.Compiler.ExpressionCompiler<IIdentifierCompiler<TargetMethodInputResolver>, IFunctionCompiler<TargetMethodInputResolver, IIdentifierCompiler<TargetMethodInputResolver>>, TargetMethodInputResolver>();
        }

        private bool CompileAndRun(IExpression absTree)
        {
            var func = _systemUnderTest.Compile(absTree, _identifierCompiler.Object, _functionCompiler.Object);
            return func(_identifierResolver.Object);
        }

        [Test]
        public void BooleanLiteralTrueTest()
        {
            var input = "true";
            Assert.IsTrue(CompileAndRun(_parser.Parse(input)));
        }

        [Test]
        public void BooleanLiteralFalseTest()
        {
            var input = "false";
            Assert.IsFalse(CompileAndRun(_parser.Parse(input)));
        }

        [Test]
        public void EqualTest()
        {
            var stringEq = "'string' == identifier1";
            var stringNeq = "'string' == 'strung'";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier1"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant("string"));
            var decimalEq = "1 == identifier2";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier2"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant((decimal) 1.0));
            var decimalNeq = "1 == 1.1";
            var charEq = "identifier3 == identifier4";
            var charNeq = "identifier3 == identifier5";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier3"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant('a'));
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier4"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant('a'));
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier5"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant('b'));

            Assert.IsTrue(CompileAndRun(_parser.Parse(stringEq)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(stringNeq)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(decimalEq)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(decimalNeq)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(charEq)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(charNeq)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void EqualTypeMismatchTest()
        {
            var mismatch = "'string' == identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(1));
            Assert.Throws<SecurityExpressionEvaluationException>(() =>
                CompileAndRun(_parser.Parse(mismatch)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void NotEqualTest()
        {
            var stringEq = "'string' != identifier1";
            var stringNeq = "'string' != 'strung'";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier1"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant("string"));
            var decimalEq = "1 != identifier2";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier2"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant((decimal) 1.0));
            var decimalNeq = "1 != 1.1";
            var charEq = "identifier3 != identifier4";
            var charNeq = "identifier3 != identifier5";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier3"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant('a'));
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier4"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant('a'));
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier5"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant('b'));


            Assert.IsFalse(CompileAndRun(_parser.Parse(stringEq)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(stringNeq)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(decimalEq)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(decimalNeq)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(charEq)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(charNeq)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void NotEqualsTypeMismatchTest()
        {
            var mismatch = "'string' != identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(1));
            Assert.Throws<SecurityExpressionEvaluationException>(() =>
                CompileAndRun(_parser.Parse(mismatch)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void GreaterThanTest()
        {
            var isGreater = "2 > identifier";
            var isNotGreater = "2 > 2";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant((decimal) 1));

            Assert.IsTrue(CompileAndRun(_parser.Parse(isGreater)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(isNotGreater)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void GreaterThanTypeMismatchTest()
        {
            var mismatch1 = "'string' > identifier";
            var mismatch2 = "1 > identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(1));
            Assert.Throws<SecurityExpressionParseException>(() => CompileAndRun(_parser.Parse(mismatch1)));
            Assert.Throws<SecurityExpressionEvaluationException>(() => CompileAndRun(_parser.Parse(mismatch2)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void GreaterThanOrEqualTest()
        {
            var isGreater = "2 >= identifier";
            var isEqual = "2 >= 2";
            var isLesser = "1 >= 2";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant((decimal) 1));

            Assert.IsTrue(CompileAndRun(_parser.Parse(isGreater)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(isEqual)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(isLesser)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void GreaterThanOrEqualTypeMismatchTest()
        {
            var mismatch1 = "'string' >= identifier";
            var mismatch2 = "1 >= identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(1));
            Assert.Throws<SecurityExpressionParseException>(() => CompileAndRun(_parser.Parse(mismatch1)));
            Assert.Throws<SecurityExpressionEvaluationException>(() => CompileAndRun(_parser.Parse(mismatch2)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void LessThanTest()
        {
            var isLesser = "2 < identifier";
            var isNotLesser = "2 < 2";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant((decimal) 3));

            Assert.IsTrue(CompileAndRun(_parser.Parse(isLesser)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(isNotLesser)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void LessThanTypeMismatchTest()
        {
            var mismatch1 = "'string' < identifier";
            var mismatch2 = "1 < identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(1));
            Assert.Throws<SecurityExpressionParseException>(() => CompileAndRun(_parser.Parse(mismatch1)));
            Assert.Throws<SecurityExpressionEvaluationException>(() => CompileAndRun(_parser.Parse(mismatch2)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void LessThanOrEqualTest()
        {
            var isLesser = "2 <= identifier";
            var isEqual = "2 <= 2";
            var isGreater = "3 <= 2";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant((decimal) 3));

            Assert.IsTrue(CompileAndRun(_parser.Parse(isLesser)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(isEqual)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(isGreater)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void LessThanOrEqualTypeMismatchTest()
        {
            var mismatch1 = "'string' <= identifier";
            var mismatch2 = "1 <= identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(1));
            Assert.Throws<SecurityExpressionParseException>(() => CompileAndRun(_parser.Parse(mismatch1)));
            Assert.Throws<SecurityExpressionEvaluationException>(() => CompileAndRun(_parser.Parse(mismatch2)));
            _identifierCompiler.VerifyAll();
        }

        [Test]
        public void TopLevelFunctionReturnsBoolTest()
        {
            var input = "func()";
            _functionCompiler.Setup(r => r.Compile(It.Is<Function>(f => f.Name == "func"), It.IsAny<IIdentifierCompiler<TargetMethodInputResolver>>(), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(true));
            Assert.IsTrue(CompileAndRun(_parser.Parse(input)));
        }

        [Test]
        public void TopLevelFunctionReturnsNotBoolTest()
        {
            var input = "func()";
            _functionCompiler.Setup(r => r.Compile(It.Is<Function>(f => f.Name == "func"), It.IsAny<IIdentifierCompiler<TargetMethodInputResolver>>(), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant("This is not a boolean"));
            Assert.Throws<SecurityExpressionEvaluationException>(() => CompileAndRun(_parser.Parse(input)));
        }

        [Test]
        public void TopLevelIdentifierReturnsBoolTest()
        {
            var input = "identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(true));
            Assert.IsTrue(CompileAndRun(_parser.Parse(input)));
        }

        [Test]
        public void TopLevelIdentifierReturnsNotBoolTest()
        {
            var input = "identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant("This is not a boolean"));
            Assert.Throws<SecurityExpressionEvaluationException>(() => CompileAndRun(_parser.Parse(input)));
        }

        [Test]
        public void AndTest()
        {
            var true1 = "true && true";
            var false1 = "true && false";
            var false2 = "false && true";
            var false3 = "false && false";

            Assert.IsTrue(CompileAndRun(_parser.Parse(true1)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(false1)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(false2)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(false3)));
        }

        [Test]
        public void AndTypeMismatchTest()
        {
            var withStr = "true && 'string'";
            var withInt = "identifier && false";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(1));

            Assert.Throws<SecurityExpressionParseException>(() => CompileAndRun(_parser.Parse(withStr)));
            Assert.Throws<SecurityExpressionEvaluationException>(() => CompileAndRun(_parser.Parse(withInt)));
        }

        [Test]
        public void OrTest()
        {
            var true1 = "true || true";
            var true2 = "false || true";
            var true3 = "true || false";
            var false1 = "false || false";

            Assert.IsTrue(CompileAndRun(_parser.Parse(true1)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(true2)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(true3)));
            Assert.IsFalse(CompileAndRun(_parser.Parse(false1)));
        }

        [Test]
        public void OrTypeMismatchTest()
        {
            var withStr = "true || 'string'";
            var withInt = "identifier || false";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(1));

            Assert.Throws<SecurityExpressionParseException>(() => CompileAndRun(_parser.Parse(withStr)));
            Assert.Throws<SecurityExpressionEvaluationException>(() => CompileAndRun(_parser.Parse(withInt)));
        }

        [Test]
        public void NotTest()
        {
            var input1 = "!true";
            var input2 = "!identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant(false));

            Assert.IsFalse(CompileAndRun(_parser.Parse(input1)));
            Assert.IsTrue(CompileAndRun(_parser.Parse(input2)));
        }

        [Test]
        public void NotTypeMismatchTest()
        {
            var input = "!identifier";
            _identifierCompiler.Setup(r => r.Compile(It.Is<Identifier>(i => i.Name == "identifier"), It.IsAny<ParameterExpression>()))
                .Returns(Expression.Constant("Not a boolean"));
            Assert.Throws<SecurityExpressionEvaluationException>(() => CompileAndRun(_parser.Parse(input)));
        }
    }
}