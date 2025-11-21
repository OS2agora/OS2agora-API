namespace Agora.Operations.Common.TextResolverKeys
{
    public class GroupKey : TextResolverBaseKey
    {
        private GroupKey(string value) : base(value) { }

        public static GroupKey General => new GroupKey("General");
        public static GroupKey AddedAsReviewer => new GroupKey("AddedAsReviewer");
        public static GroupKey FileGeneration => new GroupKey("FileGeneration");
        public static GroupKey PdfGeneration => new GroupKey("PdfGeneration");
    }
}