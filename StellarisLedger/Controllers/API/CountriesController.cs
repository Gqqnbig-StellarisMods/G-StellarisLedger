using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace StellarisLedger.Controllers.Api
{
    //[controller]是路由占位符，被替换为控制器名称 https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#token-replacement-in-route-templates-controller-action-area
    [Route("api/")]
    public class CountriesController : Controller
    {
		private readonly string saveGamesPath;
		private readonly JsonSerializerSettings serializerSettings;


		public CountriesController(IOptions<AppSettings> appSettings, IOptions<MvcJsonOptions> jsonOptions)
		{
			saveGamesPath = Path.GetFullPath(appSettings.Value.SaveGamesPath);
			serializerSettings = jsonOptions.Value.SerializerSettings;
		}



        [ResponseCache(Duration = 3600 * 24 * 30)]
        [HttpGet(@"{gameId:regex(^[[\d_-]]+$)}/{saveName:regex(^[[\d.]]+\.sav$)}/Countries")]
        public HttpResponseMessage GetCountries(string gameId, string saveName)
        {
            var fileName = Path.Combine(saveGamesPath, gameId, saveName);
            if (System.IO.File.Exists(fileName) == false)
            {
                Response.StatusCode = 404;
                return null;
            }

			var content = Stellaris.GetGameSaveContent(fileName);

			var analyst = new Analyst(content);
			var countries = analyst.GetCountries();
			GC.Collect();

			string json = JsonConvert.SerializeObject(countries, new JsonSerializerSettings { ContractResolver = new SerializePopContractResolver(),
																							  Formatting = serializerSettings.Formatting });

			var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
#pragma warning disable DF0022 // Marks undisposed objects assinged to a property, originated in an object creation.
			response.Content = new StringContent(json);
#pragma warning restore DF0022 // Marks undisposed objects assinged to a property, originated in an object creation.
			return response;
		}

	    [ResponseCache(Duration = 3600 * 24 * 30)]
	    [HttpGet(@"{gameId:regex(^[[\d_-]]+$)}/{saveName:regex(^[[\d.]]+\.sav$)}/Countries/Player")]
	    public HttpResponseMessage GetPlayerCountry(string gameId, string saveName)
	    {
			var fileName = Path.Combine(saveGamesPath, gameId, saveName);
		    if (System.IO.File.Exists(fileName) == false)
		    {
			    Response.StatusCode = 404;
			    return null;
		    }

			var content = Stellaris.GetGameSaveContent(fileName);

			var analyst = new Analyst(content);
		    var country= analyst.GetCountry(analyst.PlayerTag);

			GC.Collect();
			string json = JsonConvert.SerializeObject(country, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new SerializePopContractResolver() });

			var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
#pragma warning disable DF0022 // Marks undisposed objects assinged to a property, originated in an object creation.
			response.Content = new StringContent(json);
#pragma warning restore DF0022 // Marks undisposed objects assinged to a property, originated in an object creation.
			return response;
		}

        /// <summary>
        /// 获取最近的limit个游戏存档
        /// </summary>
        /// <param name="limit"></param>
        /// <returns>第0个是最近的</returns>
        [HttpGet("RecentSaves")]
        public IEnumerable<string> GetMostRecentSaves([FromQuery]int? limit = null)
		{
			return Stellaris.GetMostRecentSaves(saveGamesPath,limit);
		}


	    class SerializePopContractResolver : CamelCasePropertyNamesContractResolver
		{
		    public static readonly SerializePopContractResolver Instance = new SerializePopContractResolver();

		    protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
		    {
			    JsonProperty property = base.CreateProperty(member, memberSerialization);

			    if (property.DeclaringType == typeof(Planet) && property.PropertyName.Equals(nameof(Planet.Pops),StringComparison.OrdinalIgnoreCase))
			    {
				    property.PropertyType = typeof(string[]);
				    property.ItemConverter = new PopConverter();
			    }

			    return property;
		    }


		    class PopConverter : Newtonsoft.Json.JsonConverter<Pop>
		    {
			    public override void WriteJson(JsonWriter writer, Pop value, JsonSerializer serializer)
			    {
				    writer.WriteValue(value.Faction);
			    }

			    public override Pop ReadJson(JsonReader reader, Type objectType, Pop existingValue, bool hasExistingValue, JsonSerializer serializer)
			    {
				    throw new NotImplementedException();
			    }
		    }

		}
	}

}
