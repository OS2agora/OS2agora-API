using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Agora.DAOs.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
