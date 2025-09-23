using System;

namespace BallerupKommune.DTOs.Common
{
    public abstract class AuditableDto<T> : BaseDto<T>
    {
        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? LastModified { get; set; }

        public string LastModifiedBy { get; set; }
    }
}