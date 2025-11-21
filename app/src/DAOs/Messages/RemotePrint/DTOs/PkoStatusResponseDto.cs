namespace Agora.DAOs.Messages.RemotePrint.DTOs
{
    public class PkoStatusResponseDto
    {
        public PkoPostStatusDto PkoStatus { get; set; }
        public MailActorDto Actor { get; set; }
        public MailActionDto Action { get; set; }
    }
}