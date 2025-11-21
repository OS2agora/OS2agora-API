using System;

namespace Agora.DTOs.Models
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public DateTimeOffset AccessTokenExpirationDate { get; set; }
        public DateTimeOffset RefreshTokenExpirationDate { get; set; }
    }
}