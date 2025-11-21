using Agora.DAOs.ExternalProcesses.Enums;
using Agora.DAOs.ExternalProcesses.Interfaces;
using Agora.Models.Enums;
using Agora.Models.Models.Records;

namespace Agora.DAOs.ExternalProcesses.Services.ExternalPdfService
{
    public class GeneratePdfConfiguration : IExternalProcessConfiguration
    {
        public ExternalProcessJobs Job { get; set; } = ExternalProcessJobs.GENERATE_PDF;
        public HearingRecord HearingRecord { get; set; }
        public string WebRootPath { get; set; }
        public ExportFormat ExportFormat { get; set; }
        public string OutputPath { get; set; }
    }
}