using System;

namespace BallerupKommune.Models.Common
{
    public abstract class AuditableModel : BaseModel
    {
        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? LastModified { get; set; }

        public string LastModifiedBy { get; set; }
    }
}