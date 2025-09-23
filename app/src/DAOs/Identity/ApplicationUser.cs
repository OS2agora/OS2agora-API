using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BallerupKommune.DAOs.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
