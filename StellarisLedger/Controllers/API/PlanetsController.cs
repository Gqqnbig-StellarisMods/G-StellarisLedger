using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StellarisLedger.Controllers.Api;

namespace StellarisLedger.Controllers.API
{
	[Route("api/")]
	public class PlanetsController
	{
		private string saveGamesPath;
		private readonly JsonSerializerSettings serializerSettings;

		public PlanetsController(IOptions<AppSettings> appSettings, IOptions<MvcJsonOptions> jsonOptions)
		{
			saveGamesPath = Path.GetFullPath(appSettings.Value.SaveGamesPath);
			serializerSettings = jsonOptions.Value.SerializerSettings;
		}

		[ResponseCache(Duration = 3600 * 24 * 30)]
		[HttpGet(@"{gameId}/{saveName}/Countries/{tag}/Planets")]
		public HttpResponseMessage GetPlanetTilesData(string gameId, string saveName, string tag)
		{
			if (tag.Equals("me",StringComparison.OrdinalIgnoreCase))
				tag = "0";

			var content = Stellaris.GetGameSaveContent(Path.Combine(saveGamesPath,gameId,saveName));
			List<PlanetTiles> planetTilesData;
			using (var analyst = new Analyst(content))
			{
				planetTilesData = analyst.GetCountryPlanetTiles(tag);
			}
			string json = JsonConvert.SerializeObject(planetTilesData, new JsonSerializerSettings { Formatting = serializerSettings.Formatting});

			var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
#pragma warning disable DF0022 // Marks undisposed objects assinged to a property, originated in an object creation.
			response.Content = new StringContent(json);
#pragma warning restore DF0022 // Marks undisposed objects assinged to a property, originated in an object creation.
			return response;
		}

		[ResponseCache(Duration = 3600 * 24 * 30)]
		[HttpGet(@"{gameId}/{saveName}/Planets/{planetId}")]
		public HttpResponseMessage GetPlanetTile(string gameId, string saveName, string planetId)
		{
			var content = Stellaris.GetGameSaveContent(Path.Combine(saveGamesPath, gameId, saveName));
			PlanetTiles planetTiles;
			using (var analyst = new Analyst(content))
			{
				planetTiles = analyst.GetPlanetTitles(planetId);
			}
			string json = JsonConvert.SerializeObject(planetTiles, new JsonSerializerSettings { Formatting = serializerSettings.Formatting });

			var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
#pragma warning disable DF0022 // Marks undisposed objects assinged to a property, originated in an object creation.
			response.Content = new StringContent(json);
#pragma warning restore DF0022 // Marks undisposed objects assinged to a property, originated in an object creation.
			return response;
		}
	}
}
