namespace BallerupKommune.Operations.Common.Constants
{
    public static class Esdh
    {
        public static string HearingFileName = "Høringstekst.pdf";
        public static string HearingFileDescription = "Dette dokument indeholder høringsteksten. Dokumentet er autogenereret";
        public static string HearingFileContentType = "application/pdf";

        public static string ConclusionFileName = "Konklusion.pdf";
        public static string ConclusionFileDescription = "Dette dokument indeholder konklusionen. Dokumentet er autogenereret";
        public static string ConclusionFileContentType = "application/pdf";

        public static string HearingAppendixDescription = "Dette dokument er et bilag til høringen";

        public static string CommentFileContentType = "application/pdf";

        public static string CommentAppendixTitle(int commentNumber, string fileName)
        {
            return $"Høringssvar - {commentNumber} Bilag - {fileName}";
        }

        public static string CommentAppendixDescription(int commentNumber)
        {
            return $"Dette dokument er et bilag til høringssvar: {commentNumber}";
        }

        public static string CommentFileName(int commentNumber)
        {
            return $"Høringsvar - {commentNumber}.pdf";
        }

        public static string CommentFileDescription(int commentNumber, string commentWriterName)
        {
            return $"Dette dokument indeholder høringssvar: {commentNumber} - af {commentWriterName}";
        }

        public static string HearingAppendixTitle(string fileName)
        {
            return $"Høringsbilag - {fileName}";
        }
    }
}