namespace BallerupKommune.DAOs.Esdh.Sbsip.DTOs
{
    public class PartDto
    {
        public int PartId { get; set; }
        public PartDtoPartType PartType { get; set; }
        public string CVRnummer { get; set; }
        public string CPRnummer { get; set; }
        public string Navn { get; set; }
    }
}