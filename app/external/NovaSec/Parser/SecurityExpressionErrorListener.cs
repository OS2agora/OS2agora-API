using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using System.Collections.Generic;

namespace NovaSec.Parser
{
    internal class SecurityExpressionErrorListener : BaseErrorListener
    {
        public bool ErrorOccured { get; private set; }
        public List<string> ErrorMessages { get; private set; }

        public SecurityExpressionErrorListener()
        {
            ErrorMessages = new List<string>();
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            ErrorOccured = true;
            ErrorMessages.Add(msg);
        }

        public override void ReportAmbiguity(Antlr4.Runtime.Parser recognizer, DFA dfa, int startIndex, int stopIndex, bool exact, BitSet ambigAlts,
            ATNConfigSet configs)
        {
            ErrorOccured = true;
            ErrorMessages.Add($"Parse ambiguity found between index {startIndex} and {stopIndex}");
        }
    }
}
