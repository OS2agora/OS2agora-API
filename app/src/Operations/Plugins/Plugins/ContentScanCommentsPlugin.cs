using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Files;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Plugins.Plugins
{
    public class ContentScanCommentsPlugin : PluginBase
    {
        private readonly ICommentDao _commentDao;
        private readonly IContentDao _contentDao;
        private readonly IDataScanner _dataScanner;
        private readonly IFileService _fileService;

        public ContentScanCommentsPlugin(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration) : base(serviceProvider, pluginConfiguration)
        {
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
