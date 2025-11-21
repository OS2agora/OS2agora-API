using Agora.Models.Models.Cpr;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.Cvr
{
    public interface ICvrInformationService
    {
        public Task<AddressInformation> GetAddressInformation(string cvr);
        public Task<AddressInformation> GetMailAddressInformation(string cvr);
    }
}