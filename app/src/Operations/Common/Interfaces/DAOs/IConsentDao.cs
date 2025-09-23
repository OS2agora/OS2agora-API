using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IConsentDao
    {
        Task<Consent> CreateAsync(Consent model, IncludeProperties includes = null);
        Task<Consent> UpdateAsync(Consent model, IncludeProperties includes = null);
    }
}