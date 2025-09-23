using System.Collections.Generic;

namespace BallerupKommune.DAOs.Esdh.Sbsip.DTOs
{
    public class CreateSagFromSkabelonDto
    {
        public string SagsTitel { get; set; }
        public BaseUuidDto Ansaettelsessted { get; set; }
        public string AnsaettelsesStedCode { get; set; }
        public int SagsBehandlerID { get; set; }
        public int FagomraadeID { get; set; }
        public int StyringsReolID { get; set; }
        public ICollection<PartDto> Parts { get; set; }
        public PartDto PrimaryPart { get; set; }
        public string EmneplanNummer { get; set; }
        public string Facet { get; set; }
        public ICollection<int> SagsRoller { get; set; }
        public int SkabelonId { get; set; }
    }
}