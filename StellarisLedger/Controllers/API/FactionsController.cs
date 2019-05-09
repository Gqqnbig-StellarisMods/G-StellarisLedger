using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Remotion.Linq.Clauses;
using StellarisLedger.Models.Api;

namespace StellarisLedger.Controllers.Api
{
	[Route("api/")]
	public class FactionsController : Controller
	{
		private readonly IMemoryCache memoryCache;
		private readonly string saveGamesPath;

		public FactionsController(IOptions<AppSettings> appSettings, IMemoryCache memoryCache)
		{
			this.memoryCache = memoryCache;
			saveGamesPath = Path.GetFullPath(appSettings.Value.SaveGamesPath);
		}

		[ResponseCache(Duration = 3600 * 24 * 30)]
		[HttpGet(@"{gameId}/{saveName}/Countries/{tag}/Factions")]
		public IEnumerable<PlanetFactions> GetPlanetFactionsList(string gameId, string saveName, string tag)
		{

			var content = Stellaris.GetGameSaveContent(Path.Combine(saveGamesPath, gameId, saveName));
			var analyst = new Analyst(content);
			var planetTilesData = analyst.GetCountryPlanetTiles(tag);

			var r = from p in planetTilesData
					select new PlanetFactions { Name = p.Name, Factions = GetFactions(analyst, p.Tiles.Values) };




			return r;
		}

		private Dictionary<string, int> GetFactions(Analyst analyst, IEnumerable<PlanetTiles.Tile> tiles)
		{
			var list = new Dictionary<string, int>();

			foreach (var t in tiles)
			{
				if (t.PopId != null)
				{
					var pop = analyst.GetPop(t.PopId.Value);
					if (pop?.FactionId != null)
					{
						var factionName = analyst.GetFactionName(pop.FactionId.Value);
						if (list.TryGetValue(factionName, out var count))
							list[factionName] = count + 1;
						else
							list.Add(factionName, 1);
					}
				}
			}

			return list;

		}
	}
}