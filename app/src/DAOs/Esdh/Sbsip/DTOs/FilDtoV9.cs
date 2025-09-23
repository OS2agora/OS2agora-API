using System;

namespace BallerupKommune.DAOs.Esdh.Sbsip.DTOs
{
    public class FilDtoV9
    {
        public Guid Id { get; set; }
        public string Filnavn { get; set; }
        public string Filendelse { get; set; }
        public bool ErPrimaerFil { get; set; }
        public string MimeType { get; set; }
        public FilDtoV9DokumentDataInfoType DokumentDataInfoType { get; set; }
    }
}