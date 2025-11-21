using System.Threading.Tasks;
using Agora.Models.Enums;
using Agora.Models.Models.Records;

namespace Agora.Operations.Common.Interfaces.ExternalProcesses.Services
{
    public interface IExternalPdfService
    {
        public Task<byte[]> GenerateReportAsync(ExportFormat format, HearingRecord hearingRecord);
    }
}