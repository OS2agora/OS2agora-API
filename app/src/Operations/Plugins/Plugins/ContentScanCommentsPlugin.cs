using BallerupKommune.Models.Models.Multiparts;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using BallerupKommune.Models.Models.Files;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Enums;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Plugins.Plugins
{
    public class ContentScanCommentsPlugin : PluginBase
    {
        private readonly ILogger<FileOperation> _fileOperationLogger;
        private readonly ILogger<File> _fileLogger;
        private readonly ICommentDao _commentDao;
        private readonly IContentDao _contentDao;
        private readonly IDataScanner _dataScanner;
        private readonly IFileService _fileService;

        public ContentScanCommentsPlugin(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration) : base(serviceProvider, pluginConfiguration)
        {
            _fileOperationLogger = serviceProvider.GetService<ILogger<FileOperation>>();
            _fileLogger = serviceProvider.GetService<ILogger<File>>();
            _commentDao = serviceProvider.GetService<ICommentDao>();
            _contentDao = serviceProvider.GetService<IContentDao>();
            _dataScanner = serviceProvider.GetService<IDataScanner>();
            _fileService = serviceProvider.GetService<IFileService>();
        }

        public override async Task AfterCommentTextContentCreate(int commentId, int contentId)
        {
            var comment = await _commentDao.GetAsync(commentId);
            var content = await _contentDao.GetAsync(contentId);

            var textContent = content.TextContent;

            var dataScanResult = await _dataScanner.ScanTextContentAsync(textContent);

            await UpdateCommentWithDataScannerResult(comment, dataScanResult);
        }

        public override async Task AfterCommentTextContentUpdate(int commentId, int contentId)
        {
            var comment = await _commentDao.GetAsync(commentId);
            var content = await _contentDao.GetAsync(contentId);

            var textContent = content.TextContent;

            var dataScanResult = await _dataScanner.ScanTextContentAsync(textContent);

            await UpdateCommentWithDataScannerResult(comment, dataScanResult);
        }

        public override async Task AfterCommentFileContentCreate(int commentId, int contentId)
        {
            var comment = await _commentDao.GetAsync(commentId);
            var content = await _contentDao.GetAsync(contentId);

            var file = await _fileService.GetFileFromDisk(content.FilePath);

            var dataScanResult = await _dataScanner.ScanFileContentAsync(file, content.FileContentType, content.FileName);

            await UpdateCommentWithDataScannerResult(comment, dataScanResult);
        }

        private async Task UpdateCommentWithDataScannerResult(Comment comment, DataScannerResult dataScannerResult)
        {
            comment.ContainsSensitiveInformation = dataScannerResult != DataScannerResult.Clean;
            comment.PropertiesUpdated = new List<string> { nameof(Comment.ContainsSensitiveInformation) };
            await _commentDao.UpdateAsync(comment);
        }
    }
}
