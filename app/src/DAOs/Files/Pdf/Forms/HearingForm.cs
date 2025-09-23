using MigraDocCore.DocumentObjectModel;
using System;
using System.Collections.Generic;

namespace BallerupKommune.DAOs.Files.Pdf.Forms
{
    public class HearingForm : BaseForm
    {
        public string CaseNumber { get; set; } = "";
        public string HearingType { get; set; } = "";
        public string SubjectArea { get; set; } = "";
        public DateTime StartDate { get; set; } = new DateTime();
        public DateTime EndDate { get; set; } = new DateTime();
        public string Summary { get; set; } = "";
        public string Description { get; set; } = "";
        public new string Title { get; set; } = "";

        public override Document CreateDocument()
        {
            Header = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Sagsnummer: ", CaseNumber),
                new KeyValuePair<string, string>("Høringstype: ", HearingType),
                new KeyValuePair<string, string>("Fagområde: ", SubjectArea),
                new KeyValuePair<string, string>("Startdato: ", StartDate.ToString("dd/MM/yyyy")),
                new KeyValuePair<string, string>("Slutdato: ", EndDate.ToString("dd/MM/yyyy")),
                new KeyValuePair<string, string>("Opsummering: ", Summary),
            };
            Subject = "Høringsjournal";
            BodyText = Description;

            return InternalCreateDocument();
        }
    }
}