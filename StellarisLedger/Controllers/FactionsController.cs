using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace StellarisLedger.Controllers
{
	[Route("Factions/")]
	public class FactionsController : Controller
    {
		private string saveGamesPath;

		public FactionsController(IOptions<AppSettings> appSettings)
		{
			saveGamesPath = Path.GetFullPath(appSettings.Value.SaveGamesPath);
		}

		[HttpGet]
        // GET: Factions
        public ActionResult Index()
		{
			var fileName = Stellaris.GetMostRecentSaves(saveGamesPath, 1).FirstOrDefault();
			ViewData["LatestSaveGamesPath"] = fileName;

			return View();
        }
    }
}