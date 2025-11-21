using System;

namespace Agora.DAOs.Esdh.Sbsip.DTOs
{
    public class FirmaDto
    {
        public bool Reklamebeskyttelse { get; set; }
        public string Navn2 { get; set; }
        public string CvrNummer { get; set; }
        public string SENummer { get; set; }
        public string Homepage { get; set; }
        public string EanNummer { get; set; }
        public BrancheDto Branche { get; set; }
        public int AntalAnsatte { get; set; }
        public AdresseDto Adresse { get; set; }
        public string Pnummer { get; set; }
        public bool ErJuridiskEnhed { get; set; }
        public bool TilmeldtDigitalPost { get; set; }
        public Guid Uuid { get; set; }
        public int Id { get; set; }
        public string Navn { get; set; }
    }
}