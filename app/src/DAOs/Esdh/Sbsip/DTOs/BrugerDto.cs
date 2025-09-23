using System;

namespace BallerupKommune.DAOs.Esdh.Sbsip.DTOs
{
    public class BrugerDto
    {
        public string LogonId { get; set; }
        public Guid Uuid { get; set; }
        public int Id { get; set; }
        public string Navn { get; set; }
    }
}