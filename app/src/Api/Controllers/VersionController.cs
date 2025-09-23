using BallerupKommune.Api.Models.JsonApi;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace BallerupKommune.Api.Controllers
{
	public class VersionController : ApiController
	{
		[HttpGet]
		public IActionResult GetVersion()
		{
			var version = GetInformationalVersion();
			return Ok(version);
		}

		private string GetInformationalVersion()
		{
			return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
		}
	}
}
