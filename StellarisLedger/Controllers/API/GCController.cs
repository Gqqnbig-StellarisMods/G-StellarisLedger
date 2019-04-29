using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace StellarisLedger.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/gc/")]
    public class GCController : Controller
    {
		[HttpGet("run")]
		public void Run()
		{
			var logger = ApplicationLogging.CreateLogger<GCController>();
			logger.LogInformation("GC.Collect()");
			GC.Collect();
		}
    }
}