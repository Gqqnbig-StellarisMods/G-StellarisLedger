using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Remotion.Linq.Clauses;
using StellarisLedger.Models.Api;

namespace StellarisLedger.Controllers.Api
{
	[Route("api/")]
	public class FactionsController : Controller
	{
		private readonly ILogger logger = ApplicationLogging.CreateLogger<FactionsController>();
		private readonly IMemoryCache memoryCache;
		private readonly string saveGamesPath;

		public FactionsController(IOptions<AppSettings> appSettings, IMemoryCache memoryCache)
		{
			this.memoryCache = memoryCache;
			saveGamesPath = Path.GetFullPath(appSettings.Value.SaveGamesPath);
		}

		[ResponseCache(Duration = 3600 * 24 * 30)]
		[HttpGet(@"{gameId}/{saveName}/Countries/{tag}/Factions")]
		public List<PlanetFactions> GetPlanetFactionsList(string gameId, string saveName, string tag)
		{
			if (tag.Equals("me", StringComparison.OrdinalIgnoreCase))
				tag = "0";

			if (memoryCache.TryGetValue(saveName + tag + "PlanetFactions", out List<PlanetFactions> r))
			{
				logger.LogDebug("Received planet factions data from cache for " + saveName);
				return r;
			}
			else
			{
				var content = Stellaris.GetGameSaveContent(Path.Combine(saveGamesPath, gameId, saveName));
				var analyst = new Analyst(content);
				if (memoryCache.TryGetValue(saveName + tag + "PlanetTiles", out List<PlanetTiles> planetTilesData))
				{
					logger.LogDebug("Received planet surface data from cache for " + saveName);
				}
				else
				{
					planetTilesData = analyst.GetCountryPlanetTiles(tag);
					memoryCache.Set(saveName + tag + "PlanetTiles", planetTilesData, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(30) });
				}

				r = (from p in planetTilesData
					select new PlanetFactions { Name = p.Name, Factions = GetFactions(analyst, p.Tiles.Values) }).ToList();

				memoryCache.Set(saveName + tag + "PlanetFactions", r, new MemoryCacheEntryOptions {SlidingExpiration = TimeSpan.FromMinutes(30)});
				return r;
			}
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