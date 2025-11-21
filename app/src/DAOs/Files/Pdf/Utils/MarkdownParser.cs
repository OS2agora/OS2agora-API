using Markdig;

namespace Agora.DAOs.Files.Pdf.Utils
{
    public static class MarkdownParser
    {
        public static string ConvertMarkdownToHtml(string markdownText)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(markdownText, pipeline);
        }
    }
}
