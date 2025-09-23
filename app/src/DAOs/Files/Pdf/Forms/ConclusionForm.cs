using MigraDocCore.DocumentObjectModel;
using System;
using System.Collections.Generic;

namespace BallerupKommune.DAOs.Files.Pdf.Forms
{
    public class ConclusionForm : BaseForm
    {
        public string CaseNumber { get; set; } = "";
        public string HearingType { get; set; } = "";
        public string SubjectArea { get; set; } = "";
        public DateTime ConclusionDate { get; set; } = new DateTime();
        public string ConclusionBody { get; set; } = "";
        public new string Title { get; set; } = "";
        public string Summary { get; set; } = "";

        public override Document CreateDocument()
        {
            Header = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Sagsnummer: ", CaseNumber),
                new KeyValuePair<string, string>("Høringstype: ", HearingType),
                new KeyValuePair<string, string>("Fagområde: ", SubjectArea),
                new KeyValuePair<string, string>("Svardato: ", ConclusionDate.ToString("dd/MM/yyyy")),
                new KeyValuePair<string, string>("Opsummering: ", Summary)
            };
            Subject = "Høringskonklusion";
            BodyText = ConclusionBody;

            return InternalCreateDocument();
        }
    }
}