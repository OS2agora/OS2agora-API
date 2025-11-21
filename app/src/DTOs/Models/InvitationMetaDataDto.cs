namespace Agora.DTOs.Models
{
    public class InvitationMetaDataDto
    {
        public int NewInvitees { get; set; }

        public int ExistingInvitees { get; set; }

        public int DeletedInvitees { get; set; }

        public int InviteesNotDeleted { get; set; }
    }
}