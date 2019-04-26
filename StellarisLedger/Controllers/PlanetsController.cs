using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace StellarisLedger.Controllers
{
	public class PlanetsController : Controller
	{
		private readonly IMemoryCache memoryCache;
		private string saveGamesPath;

		public PlanetsController(IOptions<AppSettings> appSettings, IMemoryCache memoryCache)
		{
			this.memoryCache = memoryCache;
			saveGamesPath = Path.GetFullPath(appSettings.Value.SaveGamesPath);
		}

		[HttpGet(@"latest/latest/Countries/me/Planets")]
		public ActionResult Index()
		{
			var fileName = Stellaris.GetMostRecentSaves(saveGamesPath, 1).FirstOrDefault();

			var model = new StellarisLedger.Models.PlanetsIndexViewModel();
			model.LatestSaveGamesPath = fileName;

			Lazy<Analyst> analyst =new Lazy<Analyst>(() =>
			{
				var content = Stellaris.GetGameSaveContent(Path.Combine(saveGamesPath, fileName));
				return new Analyst(content);
			});
			model.IsMachineEmpire = memoryCache.GetOrUpdateTag0IsMachineEmpire(false, () => analyst.Value.GetCountry("0").IsMachineEmpire, () => analyst.Value.GetInGameDate());

			return View(model);
		}
	}
}
