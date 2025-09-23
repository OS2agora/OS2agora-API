using System.Threading.Tasks;
using BallerupKommune.DAOs.Esdh.Sbsip.DTOs;
using BallerupKommune.DAOs.Esdh.Sbsip.V12.Interface;

namespace BallerupKommune.DAOs.Esdh.Sbsip.V12.Mock
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