using BallerupKommune.Models.Models;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Utility;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Files
{
    /// <summary>
    /// Fil drev:
    ///
    /// /Hearing-{hearingId}
    ///     /zip
    ///         /comments.zip
    ///     /comments
    ///         /{commentId}
    ///             /bilag1.pdf
    ///             /bilag2.excel
    ///             /bilag3.docx
    ///     /fields
    ///         /{fieldId}
    ///             /bilag1.pdf
    ///             /bilag2.excel
    ///             /bilag3.docx
    ///    
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IOptions<FileDriveOptions> _fileDriveOptions;
        private string BasePathFromConfiguration => _fileDriveOptions.Value.BasePath;

        public FileService(IOptions<FileDriveOptions> fileDriveOptions)
        {
            _fileDriveOptions = fileDriveOptions;
        }

        public async Task<string> SaveFieldFileToDisk(byte[] file, int hearingId, int fieldId)
        {
            return await SaveFileToDisk(file, hearingId, fieldId, "fields", "field");
        }

        public async Task<string> SaveCommentFileToDisk(byte[] file, int hearingId, int commentId)
        {
            return await SaveFileToDisk(file, hearingId, commentId, "comments", "comment");
        }

        private async Task<string> SaveFileToDisk(byte[] file, int hearingId, int identifier, string folderName,
            string fileName)
        {
            if (file.Length > 0)
            {
                var folderSeparator = Path.DirectorySeparatorChar;

                var fieldPath = Path.Combine(BasePathFromConfiguration,
                    $"Hearing-{hearingId}{folderSeparator}{folderName}{folderSeparator}{fileName}-{identifier}");
                var filePath = Path.Combine(fieldPath, Path.GetRandomFileName());

                if (!Directory.Exists(fieldPath))
                {
                    Directory.CreateDirectory(fieldPath);
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
            var directoryPath = Path.Combine(BasePathFromConfiguration, $"Hearing-{hearingId}");

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