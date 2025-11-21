using Agora.Models.Enums;
using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.ExternalProcesses.Services;
using Agora.Operations.Common.Interfaces.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Agora.DAOs.ExternalProcesses.Services.ExternalPdfService
{
    public class ExternalPdfService : ExternalProcessBaseService<ExternalPdfService>, IExternalPdfService
    {
        private readonly string _webRootPath;

        public ExternalPdfService(ILogger<ExternalPdfService> logger, IFileService fileService, IWebHostEnvironment webHostEnvironment) : base(logger, fileService)
        {
            _webRootPath = webHostEnvironment.WebRootPath;
        }

        public async Task<byte[]> GenerateReportAsync(ExportFormat format, HearingRecord hearingRecord)
        {
            var tags = new Dictionary<string, string>
            {
                { "ExportFormat", format.ToString() },
                { "HearingId", hearingRecord.BaseData.Id.ToString() }
            };
            using var activity = StartServiceActivity(nameof(GenerateReportAsync), tags);

            var tempFolder = _fileService.GetDirectoryPath(hearingRecord.BaseData.Id, "ExternalPdfServiceDir");
            var outputFilePath = Path.Combine(tempFolder, $"{Guid.NewGuid()}.pdf");
            var pdfJobConfiguration = new GeneratePdfConfiguration
            {
                HearingRecord = hearingRecord,
                WebRootPath = _webRootPath,
                ExportFormat = format,
                OutputPath = outputFilePath
            };

            try
            {
                await ExecuteExternalProcess(tempFolder, pdfJobConfiguration);

                var result = await File.ReadAllBytesAsync(outputFilePath);
                return result;
            }
            finally
            {
                CleanUp(outputFilePath);
            }
        }

        protected void CleanUp(string outputFilePath)
        {
            if (File.Exists(outputFilePath)) File.Delete(outputFilePath);
            base.CleanUp();
        }
    }
}

