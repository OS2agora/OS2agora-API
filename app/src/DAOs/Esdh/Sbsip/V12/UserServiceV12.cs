using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BallerupKommune.DAOs.Esdh.Sbsip.DTOs;
using BallerupKommune.DAOs.Esdh.Sbsip.V12.Interface;
using BallerupKommune.Operations.ApplicationOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BallerupKommune.DAOs.Esdh.Sbsip.V12
{
    public class UserServiceV12 : BaseService, IUserServiceV12
    {
        private readonly ILogger<UserServiceV12> _logger;

        public UserServiceV12(HttpClient httpClient, IOptions<SbsipOptions> sbsipOptions, TokenService sbsipTokenService, ILogger<BaseService> logger, ILogger<UserServiceV12> logger1) : base(httpClient, sbsipOptions, sbsipTokenService, logger)
        {
            _logger = logger1;
        }

        public async Task<BrugerDto> SearchForUser(string logonId)
        {
            var searchUserDto = new SearchBrugerDto
            {
                LogonId = logonId
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(searchUserDto), Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.PostAsync("bruger/search", postContent);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Failed to lookup user. LogonId=<{LogonId}>,  ErrorMessage=<{ErrorMessage}>.",
                    logonId, exception.Message);
                throw;
            }

            var result = await response.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<List<BrugerDto>>(result);

            var user = users.FirstOrDefault(u => string.Equals(u.LogonId, logonId, StringComparison.InvariantCultureIgnoreCase));

            return user;
        }
    }
}