using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StellarisInGameLedgerInCSharp.Controllers
{
    //[controller]是路由占位符，被替换为控制器名称 https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#token-replacement-in-route-templates-controller-action-area
    [Route("api/")]
    public class CountriesController : Controller
    {
		private string saveGamesPath;

		public CountriesController(IOptions<AppSettings> appSettings)
		{
			saveGamesPath =Path.GetFullPath(appSettings.Value.SaveGamesPath);
		}


        //// GET api/values
        //[HttpGet("Countries")]
        //public IList<Country> Get()
        //{
        //    var mostRecentSave = GetMostRecentSaves().First();
        //    var content = GetGameSaveContent(Path.Combine(saveGamesPath, mostRecentSave));

        //    return Analysis.GetCountries(content);
        //}

        private static string GetGameSaveContent(string saveGamePath)
        {
            var zipArchive = ZipFile.Open(saveGamePath, ZipArchiveMode.Read);
            var gamestate = zipArchive.GetEntry("gamestate");
            string content;

            using (var sr = new StreamReader(gamestate.Open(), System.Text.Encoding.UTF8))
            {
                content = sr.ReadToEnd();
            }

            return content;
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

            var content = GetGameSaveContent(fileName);

	        var analyst = new Analyst(content);
            var countries= analyst.GetCountries();
	        string json = JsonConvert.SerializeObject(countries, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new SerializePopContractResolver() });

	        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
	        response.Content = new StringContent(json);
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

		    var content = GetGameSaveContent(fileName);
		    
			var analyst = new Analyst(content);
		    var country= analyst.GetCountry(analyst.PlayerTag);

		    string json = JsonConvert.SerializeObject(country, Formatting.Indented, new JsonSerializerSettings{ContractResolver=new SerializePopContractResolver()});

		    var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
		    response.Content = new StringContent(json);
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

            var saveGamesDirectory = new DirectoryInfo(saveGamesPath);
            var directories = saveGamesDirectory.GetDirectories();
            if (directories.Any() == false)
                throw new ArgumentException($"存档目录\"{saveGamesPath}\"不包含子文件夹");

            var mostRecentWriteTime = directories.Max(d => d.LastWriteTime);
            var mostRecentPlayDirectory = directories.First(d => d.LastAccessTime == mostRecentWriteTime);

            var files = mostRecentPlayDirectory.GetFiles("*.sav");
            if (files.Any() == false)
                throw new ArgumentException($"存档目录\"{mostRecentPlayDirectory.FullName}\"不包含游戏存档文件(*.sav)");

            var orderedFiles = files.OrderByDescending(f => f.LastWriteTime).ToList();
            //排除文件名日期顺序不符的

            int lastDate = int.MaxValue;
            for (int i = 0; i < orderedFiles.Count; i++)
            {
                var dateString = Path.GetFileNameWithoutExtension(orderedFiles[i].Name);
                try
                {
                    dateString = dateString.Replace(".", "");
                    int d = Convert.ToInt32(dateString);
                    if (d < lastDate)
                        lastDate = d;
                    else
                        orderedFiles.RemoveAt(i--);
                }
                catch
                {
                    orderedFiles.RemoveAt(i--);
                }
            }
            
            var result = orderedFiles.Select(f => f.FullName.Substring(saveGamesPath.Length).Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (limit == null)
                return result;
            else
                return result.Take(limit.Value);
        }


	    class SerializePopContractResolver : DefaultContractResolver
	    {
		    public static readonly SerializePopContractResolver Instance = new SerializePopContractResolver();

		    protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
		    {
			    JsonProperty property = base.CreateProperty(member, memberSerialization);

			    if (property.DeclaringType == typeof(Planet) && property.PropertyName == nameof(Planet.Pops))
			    {
				    property.PropertyType = typeof(string[]);
				    property.ItemConverter = new PopConverter();
			    }

			    return property;
		    }


		    class PopConverter : Newtonsoft.Json.JsonConverter<Pop>
		    {
			    //public PopConverter()
			    //{
				    
			    //}


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
