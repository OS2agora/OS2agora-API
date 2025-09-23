using BallerupKommune.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFieldFileToDisk(byte[] file, int hearingId, int fieldId);
        Task<string> SaveCommentFileToDisk(byte[] file, int hearingId, int commentId);
        void DeleteFileFromDisk(string path);
        Task<byte[]> GetFileFromDisk(string filePath);
        void DeleteDirectory(int hearingId);
        Task<byte[]> ZipContent(IEnumerable<Content> contentToZip);
    }
}