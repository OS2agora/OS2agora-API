using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;

namespace Agora.Api.Controllers
{
    public class VersionController : ApiController
	{
		[HttpGet]
		public IActionResult GetVersion()
		{
			var version = GetInformationalVersion();
			return Ok(version);
		}

        [HttpGet("HealthCheck")]
        public IActionResult HealthCheck()
        {
            var response = new Dictionary<string, string>
            {
                { "status", "Healthy" }
            };

            return Ok(response);
        }

        private string GetInformationalVersion()
		{
			return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
		}
	}
}
