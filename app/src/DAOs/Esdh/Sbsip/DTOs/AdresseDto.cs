namespace BallerupKommune.DAOs.Esdh.Sbsip.DTOs
{
    public class AdresseDto
    {
        public int Id { get; set; }
        public bool ErUdlandsadresse { get; set; }
        public string Adresse1 { get; set; }
        public string Adresse2 { get; set; }
        public string Adresse3 { get; set; }
        public string Adresse4 { get; set; }
        public string Adresse5 { get; set; }
        public int PostNummer { get; set; }
        public string PostDistrikt { get; set; }
        public string Bynavn { get; set; }
        public string HusNummer { get; set; }
        public string Etage { get; set; }
        public string DoerBetegnelse { get; set; }
        public string BygningsNummer { get; set; }
        public string Postboks { get; set; }
        public string LandeKode { get; set; }
        public bool ErBeskyttet { get; set; }
        public string WorkEmailAdresse { get; set; }
        public string WorkFaxNummer { get; set; }
        public string WorkLokalNummer { get; set; }
        public string WorkMobilNummer { get; set; }
        public string WorkTelefonNummer { get; set; }
        public string PrivateEmailAdresse { get; set; }
        public string PrivateFaxNummer { get; set; }
        public string PrivateLokalNummer { get; set; }
        public string PrivateMobilNummer { get; set; }
        public string PrivateTelefonNummer { get; set; }
    }
}