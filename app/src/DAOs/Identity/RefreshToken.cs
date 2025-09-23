using System;

namespace BallerupKommune.DAOs.Identity
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime ExpirationDate { get; set; }

        // Many-to-one relationship with ApplicationUser
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}