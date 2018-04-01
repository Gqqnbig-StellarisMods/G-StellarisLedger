using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace StellarisInGameLedgerInCSharp.Controllers
{
    //[controller]是路由占位符，被替换为控制器名称 https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#token-replacement-in-route-templates-controller-action-area
    [Route("api/[controller]")]
    public class CountriesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IList<Country> Get()
        {
            string content;
            using (var sr = new StreamReader(@"D:\Documents\Paradox Interactive\Stellaris\save games\2_333029590\2259.05.18\gamestate", System.Text.Encoding.UTF8))
            {
                content = sr.ReadToEnd();
            }

            //这里的大括号匹配并不强壮，而且与Antlr语法重复。但是Antlr语法分析比较慢，这里做一个优化，预先提取出有效部分。
            var t1 = GetScopeBodyAsync(GetMatchedScope(content, "country"));
            var t2 = GetScopeBodyAsync(GetMatchedScope(content, "planet"));

            Task.WaitAll(t1, t2);
            IParseTree countriesData = t1.Result;
            IParseTree planetsData = t2.Result;


            Stopwatch sw = new Stopwatch();
            List<Country> countries = new List<Country>();
            for (int i = 0; i < countriesData.ChildCount; i++)
            {
                var country = new Country();

                var countryData = countriesData.GetChild(i);

                country.Tag = countryData.GetChild(0).GetText();
                var rightValue = countryData.GetChild(2);

                if (!(rightValue is ParadoxParser.ScopeContext))
                    continue;

                if (new[] { "fallen_empire", "default" }.Contains(GetValue(rightValue.GetChild(1), "type").GetChild(0).GetText().Trim('"')) == false)
                    continue;

                country.Name = GetValue(rightValue.GetChild(1), "name").GetChild(0).GetText().Trim('"');

                country.TechnologyCount = GetTechnologyCount(rightValue);

                country.MilitaryPower = Convert.ToDouble(GetValue(rightValue.GetChild(1), "military_power").GetChild(0).GetText());


                country.CivilianStations = GetValue(rightValue.GetChild(1), "controlled_planets").ChildCount - 2;

                var owned_planets = GetValue(rightValue.GetChild(1), "owned_planets");
                country.ColonyCount = owned_planets.ChildCount - 2;

                country.Population = GetPopulation(owned_planets, planetsData);

                var modules = GetValue(rightValue.GetChild(1), "modules").GetChild(1);
                var standard_economy_module = GetValue(modules, "standard_economy_module").GetChild(1);
                var resources = GetValue(standard_economy_module, "resources").GetChild(1);
                var energy = GetValue(resources, "energy");
                if (energy is ParadoxParser.ScopeContext)
                    country.Energy = Convert.ToDouble(energy.GetChild(1).GetText());
                else
                    country.Energy = Convert.ToDouble(energy.GetText());

                var minerals = GetValue(resources, "minerals");
                if (minerals is ParadoxParser.ScopeContext)
                    country.Minerals = Convert.ToDouble(minerals.GetChild(1).GetText());
                else
                    country.Minerals = Convert.ToDouble(minerals.GetText());

                var food = GetValue(resources, "food");
                if (food is ParadoxParser.ScopeContext)
                    country.Food = Convert.ToDouble(food.GetChild(1).GetText());
                else
                    country.Food = Convert.ToDouble(food.GetText());


                var influence = GetValue(resources, "influence");
                if (influence is ParadoxParser.ScopeContext)
                    country.Influence = Convert.ToDouble(influence.GetChild(1).GetText());
                else
                    country.Influence = Convert.ToDouble(influence.GetText());


                var unity = GetValue(resources, "unity");
                if (unity is ParadoxParser.ScopeContext)
                    country.Unity = Convert.ToDouble(unity.GetChild(1).GetText());
                else
                    country.Unity = Convert.ToDouble(unity.GetText());

                // ReSharper disable InconsistentNaming
                var last_month = GetValue(standard_economy_module, "last_month").GetChild(1);
                var energyIncome = GetValue(last_month, "energy");
                country.EnergyIncome = Convert.ToDouble(energyIncome.GetChild(1).GetText());

                var mineralsIncome = GetValue(last_month, "minerals");
                country.MineralsIncome = Convert.ToDouble(mineralsIncome.GetChild(1).GetText());

                var foodIncome = GetValue(last_month, "food");
                country.FoodIncome = Convert.ToDouble(foodIncome.GetChild(1).GetText());


                var influenceIncome = GetValue(last_month, "influence");
                country.InfluenceIncome = Convert.ToDouble(influenceIncome.GetChild(1).GetText());


                var physics_research = GetValue(last_month, "physics_research");
                country.PhysicsResearchIncome = Convert.ToDouble(physics_research.GetChild(1).GetText());

                var society_research = GetValue(last_month, "society_research");
                country.SocietyResearchIncome = Convert.ToDouble(society_research.GetChild(1).GetText());

                var engineering_research = GetValue(last_month, "engineering_research");
                country.EngineeringResearchIncome = Convert.ToDouble(engineering_research.GetChild(1).GetText());




                // ReSharper restore InconsistentNaming
                countries.Add(country);
            }

            sw.Stop();
            Console.WriteLine($"生成数据用时{sw.ElapsedMilliseconds}ms");

            return countries;
        }

        private static int GetPopulation(IParseTree owned_planets, IParseTree planetsData)
        {
            var population = 0;
            for (int j = 1; j < owned_planets.ChildCount - 1; j++)
            {
                var planetId = owned_planets.GetChild(j).GetText();
                var planetData = GetValue(planetsData, planetId).GetChild(1);
                var popData = GetValue(planetData, "pop");
                if (popData != null)
                    population += popData.ChildCount - 2;
            }

            return population;
        }

        private static int GetTechnologyCount(IParseTree rightValue)
        {
            var tech_status = GetValue(rightValue.GetChild(1), "tech_status").GetChild(1);
            int levels = 0;
            for (int j = 0; j < tech_status.ChildCount; j++)
            {
                if (tech_status.GetChild(j).GetChild(0).GetText() == "level")
                    levels += Convert.ToInt32(tech_status.GetChild(j).GetChild(2).GetText().Trim('"'));
            }

            return levels;
        }

        private static string GetMatchedScope(string content, string scopeName)
        {
            string lineEndingSymbol = content.DetectLineEnding();

            int bracketBalance = 0;
            var start = content.IndexOf(scopeName + "={");
            int i = start;
            for (; i < content.Length; i++)
            {
                if (content[i] == '{')
                    bracketBalance++;
                else if (content[i] == '}')
                {
                    bracketBalance--;
                    if (bracketBalance == 0)
                        return content.Substring(start, i - start + 1);
                }
                else if (content[i] == '"')
                {
                    i = content.IndexOf('"', i + 1);
                    if (i == -1)
                        break;
                }
                else if (content[i] == '#')
                {
                    i = content.IndexOf(lineEndingSymbol, i + 1);
                    if (i == -1)
                        break;
                }
            }

            return null;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        public static async Task<IParseTree> GetScopeBodyAsync(string input)
        {
            //ICharStream cstream = CharStreams.fromStream(stream);


            var d = await Task.Run(() =>
             {
                 ICharStream cstream = CharStreams.fromstring(input);
                 ITokenSource lexer = new ParadoxLexer(cstream);
                 ITokenStream tokens = new CommonTokenStream(lexer);
                 var parser = new ParadoxParser(tokens);

                 var data = parser.paradox().children;
                 return data[0].GetChild(2).GetChild(1);
             });
            return d;
        }

        /// <summary>
        /// 如果找不到则返回null。
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IParseTree GetValue(IParseTree tree, string key)
        {
            for (int i = 0; i < tree.ChildCount; i++)
            {
                if (tree.GetChild(i).GetChild(0).GetText() == key)
                    return tree.GetChild(i).GetChild(2);
            }
            return null;
        }

    }
}
