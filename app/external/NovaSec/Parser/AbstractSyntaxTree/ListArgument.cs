using System.Collections.Generic;

namespace NovaSec.Parser.AbstractSyntaxTree
{
    public class ListArgument : IFunctionArgument
    {
        public List<IListItem> List { get; }

        public ListArgument(List<IListItem> argument)
        {
            List = argument;
        }
    }
}