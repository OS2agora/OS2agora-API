using System;

namespace BallerupKommune.DTOs.Models
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public DateTime AccessTokenExpirationDate { get; set; }
    }
}