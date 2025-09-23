using System.Collections.Generic;

namespace BallerupKommune.DAOs.KleHierarchy.DTOs
{
    public class KleGroupDto
    {
        public string Name { get; set; }
        public string Number { get; set; }

        public bool IsActive = true;

        public List<KleTopicDto> Topics { get; set; } = new List<KleTopicDto>();
    }
}