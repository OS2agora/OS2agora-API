namespace Agora.Operations.ApplicationOptions
{
    public class ClamAvOptions
    {
        public const string ClamAv = "ClamAv";

        public string Server { get; set; }
        public string Port { get; set; }
        public string MaxStreamSizeInMb { get; set; }
    }
}
