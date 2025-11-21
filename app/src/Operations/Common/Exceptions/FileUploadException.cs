using System;
using System.Collections.Generic;
using System.Linq;

namespace Agora.Operations.Common.Exceptions
{
    public class FileUploadException : Exception
    {
        public string[] FileNames { get; }

        public FileUploadException() : base("Error encountered when uploading files") { }

        public FileUploadException(IEnumerable<string> fileNames)
        {
            FileNames = fileNames.ToArray();
        }

        public FileUploadException(string fileName)
        {
            FileNames = new[] { fileName };
        }
    }
}