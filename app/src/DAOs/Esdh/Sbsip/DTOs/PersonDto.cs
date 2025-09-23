using System;

namespace BallerupKommune.DAOs.Esdh.Sbsip.DTOs
{
    public class PersonDto
    {
        public DateTimeOffset FoedeDato { get; set; }
        public string Initialer { get; set; }
        public string Titel { get; set; }
        public string Uddannelse { get; set; }
        public string Stilling { get; set; }
        public string Ansaettelsessted { get; set; }
        public PersonDtoKoen Koen { get; set; }
        public string CprNummer { get; set; }
        public AdresseDto Adresse { get; set; }
        public int CivilstandId { get; set; }
        public string AegtefaelleCPR { get; set; }
        public string MorCPR { get; set; }
        public string FarCPR { get; set; }
        public bool TilmeldtDigitalPost { get; set; }
        public Guid Uuid { get; set; }
        public int Id { get; set; }
        public string Navn { get; set; }
    }
}