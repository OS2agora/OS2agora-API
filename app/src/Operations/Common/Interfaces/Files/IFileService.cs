using Agora.Models.Enums;
using Agora.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.Files
{
    public interface IFileService
    {
        Task<string> SaveFieldFileToDisk(byte[] file, int hearingId, int fieldId);
        Task<string> SaveCommentFileToDisk(byte[] file, int hearingId, int commentId);
        Task SaveExportFileToDisk(byte[] file, int hearingId, ExportFormat format, string fileName);
        string GetDirectoryPath(int hearingId, string folderName);
        void DeleteFileFromDisk(string path);
        Task<byte[]> GetFileFromDisk(string filePath);
        void DeleteDirectory(int hearingId);
        Task<byte[]> ZipContent(IEnumerable<Content> contentToZip);
    }
}