using Microsoft.AspNetCore.Mvc;

namespace StellarisLedger.Controllers
{
	[Route("Factions/")]
	public class FactionsController : Controller
    {
		[HttpGet]
        // GET: Factions
        public ActionResult Index()
        {
            return View();
        }
    }
}