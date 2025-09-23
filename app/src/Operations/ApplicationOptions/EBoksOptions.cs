namespace BallerupKommune.Operations.ApplicationOptions
{
    public class EBoksOptions
    {
        public const string EBoks = "EBoks";

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public bool Disabled { get; set; }
    }
}