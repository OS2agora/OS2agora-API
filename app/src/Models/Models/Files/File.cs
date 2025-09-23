namespace BallerupKommune.Models.Models.Files
{
    public class File
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }

        public bool MarkedByScanner { get; set; }

        public byte[] Content { get; set; }
    }
}