using BallerupKommune.Models.Models.Multiparts;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using BallerupKommune.Models.Models.Files;
using BallerupKommune.Operations.Common.Enums;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Plugins.Plugins
{
    public class VirusScanFilesPlugin : PluginBase
    {
        private readonly IVirusScanner _virusScanner;
        private readonly ILogger<FileOperation> _fileOperationLogger;
        private readonly ILogger<File> _fileLogger;
        private readonly ICommentDao _commentDao;
        private readonly IContentDao _contentDao;
        private readonly IFileService _fileService;

        public VirusScanFilesPlugin(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration) : base(serviceProvider, pluginConfiguration)
        {
            _virusScanner = serviceProvider.GetService<IVirusScanner>();
            _fileOperationLogger = serviceProvider.GetService<ILogger<FileOperation>>();
            _fileLogger = serviceProvider.GetService<ILogger<File>>();
            _commentDao = serviceProvider.GetService<ICommentDao>();
            _contentDao = serviceProvider.GetService<IContentDao>();
            _fileService = serviceProvider.GetService<IFileService>();
        }


        public override async Task<FileOperation> BeforeFileOperation(FileOperation fileOperation)
        {
            if (fileOperation?.File?.Content == null)
            {
                return fileOperation;
            }

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

            var fileScannerResult = await _virusScanner.ScanFileAsync(file.Content);
            if (fileScannerResult == VirusScannerResult.Clean)
            {
                return file;
            }

            file.MarkedByScanner = true;
            _fileLogger.LogInformation($"File with name: {file.Name} did not pass the virus scan. The result was: {fileScannerResult}");
            return file;
        }
    }
}
