using System.Collections.Generic;

namespace BallerupKommune.DAOs.Messages.EBoks.DTOs
{
    class OpretEboksDokumentDto
    {
        // Maximum of 50 characters
        public string Overskrift { get; set; }
        public AfsenderDto Afsender { get; set; }
        public List<FileDto> Fil { get; set; }
        public List<LeveranceMetodeDto> LeveranceMetode { get; set; }
        public LeverandoerDto Leverandoer { get; set; }
        public List<ModtagerDto> Modtager { get; set; }
        public List<UdvidelseDto> Udvidelse { get; set; }
    }
}