namespace Agora.Models.Models.Invitations
{
    public class InvitationMetaData
    {
        public int NewInvitees { get; set; }

        public int ExistingInvitees { get; set; }

        public int DeletedInvitees { get; set; }

        public int InviteesNotDeleted { get; set; }
    }
}