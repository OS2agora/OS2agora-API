using Agora.Models.Models.Files;
using Agora.Models.Models.Multiparts;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Agora.Operations.Plugins.Plugins
{
    public class VirusScanFilesPlugin : PluginBase
    {
        private readonly IVirusScanner _virusScanner;
        private readonly ILogger<FileOperation> _fileOperationLogger;
        private readonly ILogger<File> _fileLogger;

        public VirusScanFilesPlugin(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration) : base(serviceProvider, pluginConfiguration)
        {
            _virusScanner = serviceProvider.GetService<IVirusScanner>();
            _fileOperationLogger = serviceProvider.GetService<ILogger<FileOperation>>();
            _fileLogger = serviceProvider.GetService<ILogger<File>>();
        }

        public override async Task<FileOperation> BeforeFileOperation(FileOperation fileOperation)
        {
            if (fileOperation?.File?.Content == null)
            {
                return fileOperation;
            }

            using var activity = StartVirusScannerActivity(fileOperation.File);
            var fileScannerResult = await _virusScanner.ScanFileAsync(fileOperation.File.Content);
            if (fileScannerResult == VirusScannerResult.Clean)
            {
                return fileOperation;
            }

            fileOperation.MarkedByScanner = true;
            _fileOperationLogger.LogInformation($"File with name: {fileOperation.File.Name} did not pass the virus scan. The result was: {fileScannerResult}");
            return fileOperation;
        }

        public override async Task<File> BeforeFileUpload(File file)
        {
            if (file?.Content == null)
            {
                return file;
            }

            using var activity = StartVirusScannerActivity(file);
            var fileScannerResult = await _virusScanner.ScanFileAsync(file.Content);
            if (fileScannerResult == VirusScannerResult.Clean)
            {
                return file;
            }

            file.MarkedByScanner = true;
            _fileLogger.LogInformation($"File with name: {file.Name} did not pass the virus scan. The result was: {fileScannerResult}");
            return file;
        }

        private static Activity StartVirusScannerActivity(File file, [CallerMemberName] string methodName = "")
        {
            var activity = Instrumentation.Source.StartActivity($"Virus Scanning Activity - {methodName}");
            activity?.SetTag("file.size.in.mb", file?.Content?.Length / 1000000);
            activity?.SetTag("file.name", file?.Name);
            activity?.SetTag("file.extension", file?.Extension);
            return activity;
        }
    }
}
