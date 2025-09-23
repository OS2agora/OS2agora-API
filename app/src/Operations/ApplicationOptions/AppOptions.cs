namespace BallerupKommune.Operations.ApplicationOptions
{
    public class AppOptions
    {
        public const string App = "App";

        public bool UseDevelopmentDatabase { get; set; }
        public bool DisableDatabaseEncryption { get; set; }
        public bool DisableBackgroundJobs { get; set; }
        public string PathToReadInternalHearing { get; set; }
        public string PathToReadPublicHearing { get; set; }
    }
}