using System.Threading.Tasks;
using Agora.Models.Models.Cpr;
using Agora.Operations.Common.Interfaces.Cvr;

namespace Agora.DAOs.UserDataEnrichment.CVR
{
    public class CvrInformationServiceStub : ICvrInformationService
    {
        public Task<AddressInformation> GetAddressInformation(string cvr)
        {
            return Task.FromResult<AddressInformation>(null);
        }

        public Task<AddressInformation> GetMailAddressInformation(string cvr)
        {
            return Task.FromResult<AddressInformation>(null);
        }
    }
}