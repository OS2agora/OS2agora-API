using MigraDocCore.DocumentObjectModel;
using System;
using System.Collections.Generic;

namespace BallerupKommune.DAOs.Files.Pdf.Forms
{
    class CommentForm : BaseForm
    {
        public string CaseNumber { get; set; } = "";
        public string HearingType { get; set; } = "";
        public string SubjectArea { get; set; } = "";
        public DateTime ResponseDate { get; set; } = new DateTime();
        public string ResponseWriterName { get; set; }
        public string ResponseBody { get; set; } = "";
        public new string Title { get; set; } = "";

        public override Document CreateDocument()
        {
            Header = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Sagsnummer: ", CaseNumber),
                new KeyValuePair<string, string>("Høringstype: ", HearingType),
                new KeyValuePair<string, string>("Fagområde: ", SubjectArea),
                new KeyValuePair<string, string>("Svardato: ", ResponseDate.ToString("dd/MM/yyyy")),
                new KeyValuePair<string, string>("Svaret af: ", ResponseWriterName)
            };
            Subject = "Høringssvar";
            BodyText = ResponseBody;

            return InternalCreateDocument();
        }
    }
}