using Agora.DTOs.Common;

namespace Agora.DTOs.Models.Invitations
{
    public class InviteeIdentifiersDto : BaseDto<InviteeIdentifiersDto>
    {
        public string Cvr { get; set; }
        public string Cpr { get; set; }
        public string Email { get; set; }
    }
}