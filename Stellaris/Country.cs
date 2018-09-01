using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


namespace StellarisLedger
{
    public class Country
    {
        public string Tag { get; set; }
        public string Name { get; internal set; }
        public int TechnologyCount { get; set; }
        public double MilitaryPower { get; set; }
        public List<Planet> Colonies { get; set; }
        public int CivilianStations { get; internal set; }
	    public int Population => Colonies.Sum(p => p.Pops.Count);
        public double Energy { get; set; }
        public double Minerals { get; set; }
        public double Food { get; set; }
        public double Influence { get; set; }

        public double EnergyIncome { get; set; }
        public double MineralsIncome { get; internal set; }
        public double? FoodIncome { get; internal set; }
        public double InfluenceIncome { get; internal set; }
        public double UnityIncome { get; internal set; }

        /// <summary>
        /// 实际的凝聚力惩罚包括已树立的传统数、人口、星球数等等
        /// </summary>
        public double UnityIncomeWithPenalty => UnityIncome / (1 + (Colonies.Count - 1) * 0.25) / (1 + Traditions.Count(t => t.EndsWith("_finish")) * 0.05);

        public int TraditionCount => Traditions.Count;

        [JsonIgnore] public List<string> Traditions { get; internal set; } = new List<string>();


        public double PhysicsResearchIncome { get; internal set; }
        public double SocietyResearchIncome { get; set; }
        public double EngineeringResearchIncome { get; internal set; }

        public double PhysicsResearchIncomeWithPenalty => PhysicsResearchIncome / GetPenalty();
        public double SocietyResearchIncomeWithPenalty => SocietyResearchIncome / GetPenalty();
        public double EngineeringResearchIncomeWithPenalty => EngineeringResearchIncome / GetPenalty();


        private double GetPenalty()
        {
            return 1 + (Colonies.Count - 1) * 0.1 + Math.Max(0, Population - 10) * 0.01;
        }
    }


	public class Planet
	{
		[JsonIgnore]
		public string Id { get; set; }

		public string Name { get; set; }
		public List<Pop> Pops { get; set; }
	}

	public class Pop
	{
		public string Id { get; set; }
		public string Faction { get; set; }
	}
}
