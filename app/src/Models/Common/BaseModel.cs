using System.Collections.Generic;

namespace BallerupKommune.Models.Common
{
    public abstract class BaseModel
    {
        public int Id { get; set; }

        public List<string> PropertiesUpdated = new List<string>();
    }
}
