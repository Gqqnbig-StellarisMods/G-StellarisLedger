using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace StellarisLedger.Controllers
{
	public class PlanetsController : Controller
	{
		private string saveGamesPath;

		public PlanetsController(IOptions<AppSettings> appSettings)
		{
			saveGamesPath = Path.GetFullPath(appSettings.Value.SaveGamesPath);
		}

		[HttpGet(@"latest/latest/Countries/me/Planets")]
		public ActionResult Index()
		{
			var fileName = Stellaris.GetMostRecentSaves(saveGamesPath, 1).FirstOrDefault();
			if (fileName != null)
				ViewData["saveGamePath"] = fileName.Replace("\\", "/");
			return View();
		}
	}
}
