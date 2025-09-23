using System;
using System.Collections.Generic;
using System.Text;
using BallerupKommune.DAOs.Esdh.Sbsip.DTOs;
using System.Threading.Tasks;
using System.Net.Http;

namespace BallerupKommune.DAOs.Esdh.Sbsip.V12.Interface
{
    public interface IUserServiceV12
    {
        Task<BrugerDto> SearchForUser(string logonId);
    }
}
