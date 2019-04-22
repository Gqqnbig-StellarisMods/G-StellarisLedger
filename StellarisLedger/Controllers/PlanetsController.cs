using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
			var fileName= Stellaris.GetMostRecentSaves(saveGamesPath, 1).FirstOrDefault();
			if (fileName == null)
				return View();

			var content = Stellaris.GetGameSaveContent(System.IO.Path.Combine(saveGamesPath, fileName));
			var analyst=new Analyst(content);
			var planetTitleses= analyst.GetCountryPlanetTiles("0");

			return View(planetTitleses.ToArray());
		}
	}
}
