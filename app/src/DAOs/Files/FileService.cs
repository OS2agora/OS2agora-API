using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces.Files;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Agora.DAOs.Files
{
    /// <summary>
    /// Hearing File drive:
    ///
    /// /Hearing-{hearingId}
    ///     /zip
    ///         /comments.zip
    ///     /comments
    ///         /comment-{commentId}
    ///             /bilag1.pdf
    ///             /bilag2.excel
    ///             /bilag3.docx
    ///     /fields
    ///         /field-{fieldId}
    ///             /bilag1.pdf
    ///             /bilag2.excel
    ///             /bilag3.docx
    ///
    /// Export File drive:
    /// /Hearing-{hearingId}
    ///     /exports
    ///         /EXCEL
    ///             /{title}.xlsx
    ///         /PDF
    ///             /{title}.pdf
    ///         /FULL_PDF
    ///             /{title}.pdf
    ///         /USER_REPORT_EXCEL
    ///             /{title}.xlsx
    ///         /RESPONSE_REPORT_EXCEL
    ///             /{title}.xlsx
    /// 
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IOptions<FileDrivesOptions> _fileDriveOptions;
        private string HearingFileDriveBasePath => _fileDriveOptions.Value.HearingFileDriveBasePath;
        private string ExportFileDriveBasePath => _fileDriveOptions.Value.ExportFileDriveBasePath;
        private readonly ILogger<FileService> _logger;

        public FileService(IOptions<FileDrivesOptions> fileDriveOptions, ILogger<FileService> logger)
        {
            _fileDriveOptions = fileDriveOptions;
            _logger = logger;
        }

        public async Task<string> SaveFieldFileToDisk(byte[] file, int hearingId, int fieldId)
        {
            var folderPath = GetSubFolderPath(HearingFileDriveBasePath, hearingId, "fields", $"field-{fieldId}");
            var fileName = Path.GetRandomFileName();
            return await SaveFileToDisk(file, folderPath, fileName);
        }

        public async Task<string> SaveCommentFileToDisk(byte[] file, int hearingId, int commentId)
        {
            var folderPath = GetSubFolderPath(HearingFileDriveBasePath, hearingId, "comments", $"comment-{commentId}");
            var fileName = Path.GetRandomFileName();
            return await SaveFileToDisk(file, folderPath, fileName);
        }

        public async Task SaveExportFileToDisk(byte[] file, int hearingId, ExportFormat format, string fileName)
        {
            var folderPath =
                GetSubFolderPath(ExportFileDriveBasePath, hearingId, "exports", format.ToString());
            await SaveFileToDisk(file, folderPath, fileName);
        }

        public string GetDirectoryPath(int hearingId, string folderName)
        {
            return GetDirectoryPath(hearingId, folderName, HearingFileDriveBasePath);
        }

        private string GetDirectoryPath(int hearingId, string folderName, string basePath)
        {
            var folderSeparator = Path.DirectorySeparatorChar;

            return Path.Combine(basePath,
                $"Hearing-{hearingId}{folderSeparator}{folderName}");
        }

        private string GetSubFolderPath(string basePath, int hearingId, string fileCategory, string subFolderName)
        {
            var folderSeparator = Path.DirectorySeparatorChar;
            var subFolderPath = $"{fileCategory}{folderSeparator}{subFolderName}";

            return GetDirectoryPath(hearingId, subFolderPath, basePath);
        }

        private async Task<string> SaveFileToDisk(byte[] file, string folderPath, string fileName)
        {
            if (file.Length > 0)
            {
                var filePath = Path.Combine(folderPath, fileName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                await using (FileStream fs = File.Create(filePath))
                {
                    fs.Write(file, 0, file.Length);
                }

                return filePath;
            }

            return null;
        }

        public Task<byte[]> GetFileFromDisk(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllBytesAsync(filePath);
            }

            _logger.LogWarning("File at path {filePath} does not exist. Returning null", filePath);

            return Task.FromResult<byte[]>(null);
        }

        public void DeleteFileFromDisk(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void DeleteDirectory(int hearingId)
        {
            var directoryPath = Path.Combine(HearingFileDriveBasePath, $"Hearing-{hearingId}");

            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        public async Task<byte[]> ZipContent(IEnumerable<Content> contentToZip)
        {
            var folderSeparator = Path.DirectorySeparatorChar;

            await using var archiveStream = new MemoryStream();
            using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
            {
                foreach (var content in contentToZip)
                {
                    var fileContent = await GetFileFromDisk(content.FilePath);
                    var zipArchiveEntry =
                        archive.CreateEntry($"{content.Comment.Number}{folderSeparator}{content.FileName}",
                            CompressionLevel.Fastest);
                    await using var zipStream = zipArchiveEntry.Open();
                    await zipStream.WriteAsync(fileContent, 0, fileContent.Length);
                }
            }

            var zipFile = archiveStream.ToArray();

            return zipFile;
        }
    }
}