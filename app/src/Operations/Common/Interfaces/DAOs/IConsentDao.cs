using Agora.Models.Common;
using Agora.Models.Models;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IConsentDao
    {
        Task<Consent> CreateAsync(Consent model, IncludeProperties includes = null);
        Task<Consent> UpdateAsync(Consent model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}