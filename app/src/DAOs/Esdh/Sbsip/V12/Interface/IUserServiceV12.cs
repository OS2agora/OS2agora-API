using System;
using System.Collections.Generic;
using System.Text;
using Agora.DAOs.Esdh.Sbsip.DTOs;
using System.Threading.Tasks;
using System.Net.Http;

namespace Agora.DAOs.Esdh.Sbsip.V12.Interface
{
    public interface IUserServiceV12
    {
        Task<BrugerDto> SearchForUser(string logonId);
    }
}
