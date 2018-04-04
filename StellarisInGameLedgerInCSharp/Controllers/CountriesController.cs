using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO.Compression;

namespace StellarisInGameLedgerInCSharp.Controllers
{
    //[controller]是路由占位符，被替换为控制器名称 https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#token-replacement-in-route-templates-controller-action-area
    [Route("api/")]
    public class CountriesController : Controller
    {
        private static string saveGamesPath = @"D:\Documents\Paradox Interactive\Stellaris\save games";

        // GET api/values
        [HttpGet("Countries")]
        public IList<Country> Get()
        {
            var mostRecentSave = GetMostRecentSave();
            var content = GetGameSaveContent(mostRecentSave);

            return Analysis.GetCountries(content);
        }

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

        // GET api/values/5
        [HttpGet("{gameId}/{saveName}/Countries")]
        public IList<Country> Get(string gameId, string saveName)
        {
            var fileName = System.IO.Path.Combine(saveGamesPath, gameId, saveName);
            if (System.IO.File.Exists(fileName) == false)
            {
                Response.StatusCode = 404;
                return null;
            }

            var content = GetGameSaveContent(fileName);


            throw new NotImplementedException();
        }



        private static string GetMostRecentSave()
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

            mostRecentWriteTime = files.Max(f => f.LastWriteTime);
            var mostRecentSave = files.First(f => f.LastWriteTime == mostRecentWriteTime);
            return mostRecentSave.FullName;
        }

        //private static string[] GetPreviousSaves(string fileName, DateTime writeTime)
        //{

        //}

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
