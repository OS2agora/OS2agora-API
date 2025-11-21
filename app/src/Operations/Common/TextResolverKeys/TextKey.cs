namespace Agora.Operations.Common.TextResolverKeys
{
    public class TextKey : TextResolverBaseKey
    {
        private TextKey(string value) : base(value) { }

        // General text keys
        public static TextKey MunicipalityName => new TextKey("MunicipalityName");
        public static TextKey Name => new TextKey("Name");

        // Added as Reviewer text keys
        public static TextKey Reviewer = new TextKey("Reviewer");

        // FileGeneration text keys
        public static TextKey TitlePrefix => new TextKey("TitlePrefix");

        // PdfGeneration text keys
        public static TextKey FullHearingReportSubject => new TextKey("FullHearingReportSubject");
        public static TextKey HearingReportSubject => new TextKey("HearingReportSubject");
        public static TextKey CaseNumber => new TextKey("CaseNumber");
        public static TextKey DocumentHeader => new TextKey("DocumentHeader");
        public static TextKey DeclinedResponseText => new TextKey("DeclinedResponseText");
        public static TextKey LogoPath => new TextKey("LogoPath");
    }
}