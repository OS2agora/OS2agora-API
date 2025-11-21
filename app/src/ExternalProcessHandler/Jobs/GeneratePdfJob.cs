using Agora.DAOs;
using Agora.DAOs.ExternalProcesses.Interfaces;
using Agora.DAOs.ExternalProcesses.Services.ExternalPdfService;
using Agora.DAOs.Files;
using Agora.ExternalProcessHandler.Interfaces;
using Agora.ExternalProcessHandler.Models;
using Agora.Models.Enums;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using Agora.Operations.Resolvers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Agora.ExternalProcessHandler.Jobs
{
    public class GeneratePdfJob : IExternalJob
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, IExternalProcessConfiguration jobConfiguration)
        {
            var pdfConfig = (GeneratePdfConfiguration)jobConfiguration;
            services.Configure<AppOptions>(options => configuration.GetSection(AppOptions.App).Bind(options));
            services.Configure<FileDrivesOptions>(options =>
                configuration.GetSection(FileDrivesOptions.FileDrives).Bind(options));

            services.AddTransient<ITextResolver, TextResolver>();
            services.AddTransient<IFileService, FileService>();

            services.AddTransient<IHostingEnvironmentPath>(provider => 
                new ExternalProcessPath
                {
                    WebRootPath = pdfConfig.WebRootPath
                });

            services.AddPdfServices();
        }

        public int Handle(IExternalProcessConfiguration jobConfiguration, IServiceProvider serviceProvider)
        {
            var pdfConfig = (GeneratePdfConfiguration)jobConfiguration;

            var pdfService = serviceProvider.GetRequiredService<IPdfService>();

            var output = GenerateFile(pdfService, pdfConfig);

            File.WriteAllBytes(pdfConfig.OutputPath, output);

            return 0;
        }

        private byte[] GenerateFile(IPdfService pdfService, GeneratePdfConfiguration input)
        {
            switch (input.ExportFormat)
            {
                case ExportFormat.FULL_PDF:
                    return pdfService.CreateFullHearingReport(input.HearingRecord);
                case ExportFormat.PDF:
                    return pdfService.CreateHearingReport(input.HearingRecord);
                case ExportFormat.EXCEL:
                case ExportFormat.RESPONSE_REPORT_EXCEL:
                case ExportFormat.USER_REPORT_EXCEL:
                case ExportFormat.NONE:
                default:
                    throw new InvalidOperationException($"Invalid ExportFormat: ExternalPdfGenerator only support generating '{ExportFormat.PDF}' and '{ExportFormat.FULL_PDF}'");
            }
        }
    }
}

