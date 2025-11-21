namespace Agora.Operations.ApplicationOptions.OperationsOptions
{
    public class HearingOperationsOptions
    {
        public const string Hearings = "Hearings";
        
        public CreateHearingOptions CreateHearing { get; set; }
        public ExportHearingOptions ExportHearing { get; set; }

        public class CreateHearingOptions
        {
            public bool DisableAutoApproveComments { get; set; } = false;
        }

        public class ExportHearingOptions
        {
            public bool UseExternalPdfService { get; set; } = true;
        }
    }
}
