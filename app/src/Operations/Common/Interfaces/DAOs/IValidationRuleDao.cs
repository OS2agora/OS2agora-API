using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IValidationRuleDao
    {
        Task<List<ValidationRule>> GetAllAsync(IncludeProperties includes = null);
    }
}
