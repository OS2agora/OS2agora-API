using NovaSec.Exceptions;
using NovaSec.Parser;
using NovaSec.Parser.AbstractSyntaxTree;
using NUnit.Framework;
using System.Collections.Generic;

namespace NovaSecTest
{
    public class LanguageTests
    {
        private SecurityExpressionParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new SecurityExpressionParser();
        }

        [Test]
        public void AndOrPrecedenceTest()
        {
            // arrange
            var input1 = "true || true && true";
            var input2 = "true && true || true";

            // act
            var tree1 = _parser.Parse(input1);
            var tree2 = _parser.Parse(input2);

            // assert
            var tree1TopNode = tree1 as Logical;
            var tree2TopNode = tree2 as Logical;

            Assert.IsNotNull(tree1TopNode);
            Assert.IsNotNull(tree2TopNode);
            Assert.AreEqual(LogicalOperator.Or, tree1TopNode.Operator);
            Assert.AreEqual(LogicalOperator.Or, tree2TopNode.Operator);

            var tree1RightLeg = tree1TopNode.Right as Logical;
            Assert.IsNotNull(tree1RightLeg);
            Assert.AreEqual(LogicalOperator.And, tree1RightLeg.Operator);

            var tree2LeftLeg = tree2TopNode.Left as Logical;
            Assert.IsNotNull(tree2LeftLeg);
            Assert.AreEqual(LogicalOperator.And, tree2LeftLeg.Operator);
        }

        [Test]
        public void SyntaxErrorTest()
        {
            // arrange
            var input = "1 < 2 < 3";

            // act and assert
            Assert.Throws<SecurityExpressionParseException>(() => _parser.Parse(input));
        }

        [Test]
        public void ParenthesisTest()
        {
            var input = "true && (true || true)";

            var tree = _parser.Parse(input);

            var topNode = tree as Logical;
            Assert.IsNotNull(topNode);
            Assert.AreEqual(LogicalOperator.And, topNode.Operator);
        }

        [Test]
        public void IdentifierTest()
        {
            // arrange
            var startWithUpperCase = "Identifier";
            var startWithLowerCase = "identifier";
            var startWithSharp = "#identifier";
            var startWithUnderscore = "_identifier";
            var includeNumbers = "i123dentifier";
            var includeUnderscore = "identi_fier";
            var includeDots = "iden.ti.fier";
            var startsWithAt = "@identifier";

            // act
            var startWithUpperCaseResult = _parser.Parse(startWithUpperCase);
            var startWithLowerCaseResult = _parser.Parse(startWithLowerCase);
            var startWithSharpResult = _parser.Parse(startWithSharp);
            var startWithUnderscoreResult = _parser.Parse(startWithUnderscore);
            var includeNumbersResult = _parser.Parse(includeNumbers);
            var includeUnderscoreResult = _parser.Parse(includeUnderscore);
            var includeDotsResult = _parser.Parse(includeDots);
            var startsWithAtResult = _parser.Parse(startsWithAt);

            // assert
            Assert.IsInstanceOf<Identifier>(startWithUpperCaseResult);
            Assert.AreEqual(startWithUpperCase, ((Identifier)startWithUpperCaseResult).Name);

            Assert.IsInstanceOf<Identifier>(startWithLowerCaseResult);
            Assert.AreEqual(startWithLowerCase, ((Identifier)startWithLowerCaseResult).Name);

            Assert.IsInstanceOf<Identifier>(startWithSharpResult);
            Assert.AreEqual(startWithSharp, ((Identifier)startWithSharpResult).Name);

            Assert.IsInstanceOf<Identifier>(startWithUnderscoreResult);
            Assert.AreEqual(startWithUnderscore, ((Identifier)startWithUnderscoreResult).Name);

            Assert.IsInstanceOf<Identifier>(includeNumbersResult);
            Assert.AreEqual(includeNumbers, ((Identifier)includeNumbersResult).Name);

            Assert.IsInstanceOf<Identifier>(includeUnderscoreResult);
            Assert.AreEqual(includeUnderscore, ((Identifier)includeUnderscoreResult).Name);

            Assert.IsInstanceOf<Identifier>(includeDotsResult);
            Assert.AreEqual(includeDots, ((Identifier)includeDotsResult).Name);

            Assert.IsInstanceOf<Identifier>(startsWithAtResult);
            Assert.AreEqual(startsWithAt, ((Identifier)startsWithAtResult).Name);
        }

        [Test]
        public void ComparatorIdentifierAndDecimalInputsTest()
        {
            var differentInputs = "1 < 1.0 && identifier1 < identifier2 && identifier3 < 0.9 && 12.34 < identifier4";

            var tree = _parser.Parse(differentInputs);

            var thirdAnd = tree as Logical;
            Assert.IsNotNull(thirdAnd);
            var fourthCmd = thirdAnd.Right as Comparator;
            Assert.IsNotNull(fourthCmd);
            var secondAnd = thirdAnd.Left as Logical;
            Assert.IsNotNull(secondAnd);
            var thirdCmd = secondAnd.Right as Comparator;
            Assert.IsNotNull(thirdCmd);
            var firstAnd = secondAnd.Left as Logical;
            Assert.IsNotNull(firstAnd);
            var firstCmp = firstAnd.Left as Comparator;
            Assert.IsNotNull(firstCmp);
            var secondCmp = firstAnd.Right as Comparator;
            Assert.IsNotNull(secondCmp);

            // 1 < 1.0
            Assert.IsInstanceOf<DecimalValue>(firstCmp.Left);
            Assert.IsInstanceOf<DecimalValue>(firstCmp.Right);

            // identifier1 < identifier2
            Assert.IsInstanceOf<Identifier>(secondCmp.Left);
            Assert.IsInstanceOf<Identifier>(secondCmp.Right);

            // identifier3 < 0.9
            Assert.IsInstanceOf<Identifier>(thirdCmd.Left);
            Assert.IsInstanceOf<DecimalValue>(thirdCmd.Right);

            // 12.34 < identifier4
            Assert.IsInstanceOf<DecimalValue>(fourthCmd.Left);
            Assert.IsInstanceOf<Identifier>(fourthCmd.Right);
        }

        [Test]
        public void ComparatorStringInputTest()
        {
            var input1 = "'string' == 'string'";
            var input2 = "'string' != 'string'";

            var cmp1 = _parser.Parse(input1) as Comparator;
            var cmp2 = _parser.Parse(input2) as Comparator;

            Assert.IsNotNull(cmp1);
            Assert.IsInstanceOf<StringArgument>(cmp1.Left);
            Assert.IsInstanceOf<StringArgument>(cmp1.Right);

            Assert.IsNotNull(cmp2);
            Assert.IsInstanceOf<StringArgument>(cmp2.Left);
            Assert.IsInstanceOf<StringArgument>(cmp2.Right);
        }

        [Test]
        public void ComparatorRejectBadComparisonTest()
        {
            // string can only use == and !=
            var inputs = new List<string>()
            {
                "'string' < identifier",
                "'string' <= identifier",
                "'string' > identifier",
                "'string' >= identifier",
                "identifier < 'string'",
                "identifier <= 'string'",
                "identifier > 'string'",
                "identifier >= 'string'",
                "'string' == 1.2",
                "1.2 != 'string'",
            };

            foreach (var input in inputs)
            {
                Assert.Throws<SecurityExpressionParseException>(() => _parser.Parse(input));
            }
        }

        [Test]
        public void ComparatorAllOperatorsTest()
        {
            // arrange
            var lt = "1 < 1";
            var gt = "1 > 1";
            var leq = "1 <= 1";
            var geq = "1 >= 1";
            var eq = "1 == 1";
            var neq = "1 != 1";

            // act
            var ltResult = _parser.Parse(lt);
            var gtResult = _parser.Parse(gt);
            var leqResult = _parser.Parse(leq);
            var geqResult = _parser.Parse(geq);
            var eqResult = _parser.Parse(eq);
            var neqResult = _parser.Parse(neq);

            // assert
            Assert.IsInstanceOf<Comparator>(ltResult);
            Assert.AreEqual(ComparatorOperator.Lt, ((Comparator)ltResult).Operator);

            Assert.IsInstanceOf<Comparator>(gtResult);
            Assert.AreEqual(ComparatorOperator.Gt, ((Comparator)gtResult).Operator);

            Assert.IsInstanceOf<Comparator>(leqResult);
            Assert.AreEqual(ComparatorOperator.Leq, ((Comparator)leqResult).Operator);

            Assert.IsInstanceOf<Comparator>(geqResult);
            Assert.AreEqual(ComparatorOperator.Geq, ((Comparator)geqResult).Operator);

            Assert.IsInstanceOf<Comparator>(eqResult);
            Assert.AreEqual(ComparatorOperator.Eq, ((Comparator)eqResult).Operator);

            Assert.IsInstanceOf<Comparator>(neqResult);
            Assert.AreEqual(ComparatorOperator.Neq, ((Comparator)neqResult).Operator);
        }

        [Test]
        public void DecimalTest()
        {
            // arrange
            var input = "1 < 1.0";

            var result = _parser.Parse(input);

            var cpm = result as Comparator;
            Assert.IsNotNull(cpm);
            Assert.IsInstanceOf<DecimalValue>(cpm.Left);
            Assert.IsInstanceOf<DecimalValue>(cpm.Right);
            Assert.AreEqual(1, ((DecimalValue)cpm.Left).Value);
            Assert.AreEqual(1, ((DecimalValue)cpm.Right).Value);
        }

        [Test]
        public void BoolExprTest()
        {
            var t = "TruE";
            var f = "fAlSe";
            var tResult = _parser.Parse(t) as BooleanValue;
            var fResult = _parser.Parse(f) as BooleanValue;

            Assert.IsNotNull(tResult);
            Assert.IsNotNull(fResult);
            Assert.IsTrue(tResult.Value);
            Assert.IsFalse(fResult.Value);
        }

        [Test]
        public void FunctionStringArgumentTest()
        {
            var simpleSingleStr = "func('string')";
            var simpleDoubleStr = "func(\"string\")";
            var dontEscapeDoubleInSingle = "func('string\\\"')";
            var dontEscapeSingleInDouble = "func(\"string\\'\")";
            var escapeSingleInSingle = "func('string\\'')";
            var escapeDoubleInDouble = "func(\"string\\\"\")";
            var normalEscapes = "func('\\b\\t\\n\\f\\r\\\\')";
            var octalEscapes = "func('\\0\\12\\123')";
            var unicodeEscape = "func('\\u0000\\u000a\\u00a1\\u0a1B')";
            var stringsInList = "func(['string1', identifier, 'string2'])";

            var simpleSingleStrResult = _parser.Parse(simpleSingleStr) as Function;
            var simpleDoubleStrResult = _parser.Parse(simpleDoubleStr) as Function;
            var escapeSingleInSingleResult = _parser.Parse(escapeSingleInSingle) as Function;
            var escapeDoubleInDoubleResult = _parser.Parse(escapeDoubleInDouble) as Function;
            var normalEscapesResult = _parser.Parse(normalEscapes) as Function;
            var octalEscapesResult = _parser.Parse(octalEscapes) as Function;
            var unicodeEscapeResult = _parser.Parse(unicodeEscape) as Function;
            var stringsInListResult = _parser.Parse(stringsInList) as Function;

            Assert.IsNotNull(simpleSingleStrResult);
            Assert.IsNotNull(simpleDoubleStrResult);
            Assert.IsNotNull(escapeSingleInSingleResult);
            Assert.IsNotNull(escapeDoubleInDoubleResult);
            Assert.IsNotNull(normalEscapesResult);
            Assert.IsNotNull(octalEscapesResult);
            Assert.IsNotNull(unicodeEscapeResult);
            Assert.IsNotNull(stringsInListResult);

            Assert.AreEqual(1, simpleSingleStrResult.Arguments.Count);
            Assert.IsInstanceOf<StringArgument>(simpleSingleStrResult.Arguments[0]);
            Assert.AreEqual("string", ((StringArgument)simpleSingleStrResult.Arguments[0]).Value);
            Assert.AreEqual(1, simpleDoubleStrResult.Arguments.Count);
            Assert.IsInstanceOf<StringArgument>(simpleDoubleStrResult.Arguments[0]);
            Assert.AreEqual("string", ((StringArgument)simpleDoubleStrResult.Arguments[0]).Value);

            Assert.Throws<SecurityExpressionParseException>(() => _parser.Parse(dontEscapeDoubleInSingle));
            Assert.Throws<SecurityExpressionParseException>(() => _parser.Parse(dontEscapeSingleInDouble));

            Assert.IsInstanceOf<StringArgument>(escapeSingleInSingleResult.Arguments[0]);
            Assert.AreEqual("string'", ((StringArgument)escapeSingleInSingleResult.Arguments[0]).Value);

            Assert.IsInstanceOf<StringArgument>(escapeDoubleInDoubleResult.Arguments[0]);
            Assert.AreEqual("string\"", ((StringArgument)escapeDoubleInDoubleResult.Arguments[0]).Value);

            Assert.IsInstanceOf<StringArgument>(normalEscapesResult.Arguments[0]);
            Assert.AreEqual("\b\t\n\f\r\\", ((StringArgument)normalEscapesResult.Arguments[0]).Value);

            Assert.IsInstanceOf<StringArgument>(unicodeEscapeResult.Arguments[0]);
            Assert.AreEqual("\u0000\u000a\u00a1\u0a1B", ((StringArgument)unicodeEscapeResult.Arguments[0]).Value);

            var list = stringsInListResult.Arguments[0] as ListArgument;
            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.List.Count);
            Assert.IsInstanceOf<StringArgument>(list.List[0]);
            Assert.IsInstanceOf<Identifier>(list.List[1]);
            Assert.IsInstanceOf<StringArgument>(list.List[2]);
            Assert.AreEqual("string1", ((StringArgument)list.List[0]).Value);
            Assert.AreEqual("string2", ((StringArgument)list.List[2]).Value);
        }

        [Test]
        public void FunctionArgumentsTest()
        {
            var emptyArg = "func()";
            var simpleArg = "func(identifier)";
            var emptyListArg = "func([])";
            var listArg = "func([identifier1, identifier2])";
            var multiArgs = "func(identifier1, identifier2)";

            var emptyArgResult = _parser.Parse(emptyArg) as Function;
            var simpleArgResult = _parser.Parse(simpleArg) as Function;
            var emptyListArgResult = _parser.Parse(emptyListArg) as Function;
            var listArgResult = _parser.Parse(listArg) as Function;
            var multiArgsResult = _parser.Parse(multiArgs) as Function;

            Assert.IsNotNull(emptyArgResult);
            Assert.IsNotNull(simpleArgResult);
            Assert.IsNotNull(emptyListArgResult);
            Assert.IsNotNull(listArgResult);
            Assert.IsNotNull(multiArgsResult);

            // empty
            Assert.AreEqual("func", emptyArgResult.Name);
            Assert.AreEqual(0, emptyArgResult.Arguments.Count);

            // simple
            Assert.AreEqual("func", simpleArgResult.Name);
            Assert.AreEqual(1, simpleArgResult.Arguments.Count);
            Assert.IsInstanceOf<Identifier>(simpleArgResult.Arguments[0]);
            Assert.AreEqual("identifier", ((Identifier)simpleArgResult.Arguments[0]).Name);

            // empty list
            Assert.AreEqual("func", emptyListArgResult.Name);
            Assert.AreEqual(1, emptyListArgResult.Arguments.Count);
            Assert.IsInstanceOf<ListArgument>(emptyListArgResult.Arguments[0]);
            Assert.AreEqual(0, ((ListArgument)emptyListArgResult.Arguments[0]).List.Count);

            // list arg
            Assert.AreEqual("func", listArgResult.Name);
            Assert.AreEqual(1, listArgResult.Arguments.Count);
            var list = listArgResult.Arguments[0] as ListArgument;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.List.Count);
            Assert.IsInstanceOf<Identifier>(list.List[0]);
            Assert.AreEqual("identifier1", ((Identifier)list.List[0]).Name);
            Assert.IsInstanceOf<Identifier>(list.List[1]);
            Assert.AreEqual("identifier2", ((Identifier)list.List[1]).Name);

            // multi arg
            Assert.AreEqual("func", multiArgsResult.Name);
            Assert.AreEqual(2, multiArgsResult.Arguments.Count);
            var arg1 = multiArgsResult.Arguments[0] as Identifier;
            Assert.IsNotNull(arg1);
            var arg2 = multiArgsResult.Arguments[1] as Identifier;
            Assert.IsNotNull(arg2);
            Assert.AreEqual("identifier1", arg1.Name);
            Assert.AreEqual("identifier2", arg2.Name);
        }

        [Test]
        public void NotExprTest()
        {
            var singleNot = "!true";
            var multiNot = "!!true";

            var singleResult = _parser.Parse(singleNot) as Not;
            var multiResult = _parser.Parse(multiNot) as Not;

            Assert.IsNotNull(singleResult);
            Assert.IsNotNull(multiResult);
            Assert.IsInstanceOf<BooleanValue>(singleResult.Expression);
            Assert.IsInstanceOf<Not>(multiResult.Expression);
            var notnot = multiResult.Expression as Not;
            Assert.IsNotNull(notnot);
            Assert.IsInstanceOf<BooleanValue>(notnot.Expression);
        }
    }
}