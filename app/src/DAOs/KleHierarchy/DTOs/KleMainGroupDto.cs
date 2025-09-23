using System.Collections.Generic;

namespace BallerupKommune.DAOs.KleHierarchy.DTOs
{
    public class KleMainGroupDto
    {
        public string Title { get; set; }
        public string Number { get; set; }

        public bool IsActive = true;

        public List<KleGroupDto> Groups { get; set; } = new List<KleGroupDto>();
    }
}