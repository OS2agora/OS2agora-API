using System.Threading.Tasks;
using Agora.DAOs.Esdh.Sbsip.DTOs;
using Agora.DAOs.Esdh.Sbsip.V12.Interface;

namespace Agora.DAOs.Esdh.Sbsip.V12.Mock
{
    public class UserServiceV12Mock : IUserServiceV12
    {
        public async Task<BrugerDto> SearchForUser(string logonId)
        {
            var user = new BrugerDto { Navn = logonId };
            return user;
        }
    }
}