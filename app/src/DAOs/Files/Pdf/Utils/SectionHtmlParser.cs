using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using MigraDoc.DocumentObjectModel;
using Agora.DAOs.Files.Pdf.Constants;

namespace Agora.DAOs.Files.Pdf.Utils
{
    public static class SectionHtmlParser
    {
        public static void ParseHtmlToSection(Section section, string html)
        {
            var parser = new HtmlParser();
            var htmlDocument = parser.ParseDocument(html);

            foreach (var element in htmlDocument.Body.Children)
            {
                ProcessHtmlElement(section, element);
            }
        }

        private static void ProcessHtmlElement(Section section, IElement element)
        {
            switch (element.TagName.ToLower())
            {
                case "p":
                    AddParagraph(section, element);
                    break;
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    AddHeading(section, element);
                    break;
                case "ul":
                    AddUnorderedList(section, element);
                    break;
                case "ol":
                    AddOrderedList(section, element);
                    break;
                default:
                    AddParagraph(section, element);
                    break;
            }
        }

        private static void AddParagraph(Section section, IElement element)
        {
            var paragraph = section.AddParagraph();
            foreach (var child in element.ChildNodes)
            {
                ProcessHtmlNode(paragraph, child);
            }
        }

        private static void AddHeading(Section section, IElement element)
        {
            var paragraph = section.AddParagraph();
            switch (element.TagName.ToLower())
            {
                case "h1": paragraph.Style = StyleNames.Heading1; break;
                case "h2": paragraph.Style = StyleNames.Heading2; break;
                case "h3": paragraph.Style = StyleNames.Heading3; break;
                case "h4": paragraph.Style = StyleNames.Heading4; break;
                case "h5": paragraph.Style = StyleNames.Heading5; break;
                case "h6": paragraph.Style = StyleNames.Heading6; break;
            }

            foreach (var child in element.ChildNodes)
            {
                ProcessHtmlNode(paragraph, child);
            }
        }

        private static void AddUnorderedList(Section section, IElement ulElement, string marker = "- ")
        {
            int index = 1;
            foreach (var li in ulElement.Children)
            {
                if (li.TagName.ToLower() == "li")
                {
                    var paragraph = section.AddParagraph(marker, CustomStyles.UnorderedList);
                    if (index == 1)
                    {
                        paragraph.Format.SpaceBefore = "7pt";
                    }
                    else if (index == ulElement.Children.Length)
                    {
                        paragraph.Format.SpaceAfter = "7pt";
                    }

                    foreach (var child in li.ChildNodes)
                    {
                        ProcessHtmlNode(paragraph, child);
                    }
                    index++;
                }
            }
        }

        private static void AddOrderedList(Section section, IElement olElement)
        {
            int index = 1;
            foreach (var li in olElement.Children)
            {
                if (li.TagName.ToLower() == "li")
                {
                    var paragraph = section.AddParagraph($"{index}. ", CustomStyles.OrderedList);

                    if (index == 1)
                    {
                        paragraph.Format.SpaceBefore = "10pt";
                    } 
                    else if (index == olElement.Children.Length)
                    {
                        paragraph.Format.SpaceAfter = "10pt";
                    }

                    foreach (var child in li.ChildNodes)
                    {
                        ProcessHtmlNode(paragraph, child);
                    }
                    index++;
                }
            }
        }

        private static void ProcessHtmlNode(Paragraph paragraph, INode node)
        {
            switch (node.NodeType)
            {
                case NodeType.Text:
                    paragraph.AddText(node.TextContent);
                    break;
                case NodeType.Element:
                    var element = node as IElement;
                    if (element != null)
                    {
                        switch (element.TagName.ToLower())
                        {
                            case "strong":
                                AddNestedFormattedText(paragraph, element, TextFormat.Bold);
                                break;
                            case "em":
                                AddNestedFormattedText(paragraph, element, TextFormat.Italic);
                                break;
                            case "u":
                                AddNestedFormattedText(paragraph, element, TextFormat.Underline);
                                break;
                            case "a":
                                var link = paragraph.AddHyperlink(element.GetAttribute("href") ?? string.Empty, HyperlinkType.Web);
                                link.AddFormattedText(element.TextContent);
                                break;
                            default:
                                foreach (var child in element.ChildNodes)
                                {
                                    ProcessHtmlNode(paragraph, child);
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        private static void AddNestedFormattedText(Paragraph paragraph, IElement element, TextFormat parentFormat)
        {
            var formattedText = paragraph.AddFormattedText(string.Empty, parentFormat);

            foreach (var child in element.ChildNodes)
            {
                if (child.NodeType == NodeType.Text)
                {
                    formattedText.AddText(child.TextContent);
                }
                else if (child.NodeType == NodeType.Element)
                {
                    var childElement = child as IElement;
                    if (childElement != null)
                    {
                        switch (childElement.TagName.ToLower())
                        {
                            case "em":
                                var italicText = formattedText.AddFormattedText(childElement.TextContent, TextFormat.Italic);
                                italicText.Bold = parentFormat == TextFormat.Bold; // Bevar bold fra parent
                                break;
                            case "strong":
                                var boldText = formattedText.AddFormattedText(childElement.TextContent, TextFormat.Bold);
                                boldText.Italic = parentFormat == TextFormat.Italic; // Bevar italic fra parent
                                break;
                            default:
                                AddNestedFormattedText(paragraph, childElement, parentFormat);
                                break;
                        }
                    }
                }
            }
        }
    }
}
