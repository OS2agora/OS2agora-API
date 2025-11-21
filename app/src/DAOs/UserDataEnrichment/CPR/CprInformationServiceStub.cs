using System.Threading.Tasks;
using Agora.Models.Models.Cpr;
using Agora.Operations.Common.Interfaces.Cpr;

namespace Agora.DAOs.UserDataEnrichment.CPR;

public class CprInformationServiceStub : ICprInformationService
{
    public Task<AddressInformation> GetAddressInformation(string cpr)
    {
        return Task.FromResult<AddressInformation>(null);
    }

    public Task<AddressInformation> GetMailAddressInformation(string cpr)
    {
        return Task.FromResult<AddressInformation>(null);
    }
}