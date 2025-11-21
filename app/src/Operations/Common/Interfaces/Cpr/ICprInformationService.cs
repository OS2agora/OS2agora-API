using Agora.Models.Models.Cpr;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.Cpr
{
    public interface ICprInformationService
    {
        public Task<AddressInformation> GetAddressInformation(string cpr);
        public Task<AddressInformation> GetMailAddressInformation(string cpr);
    }
}