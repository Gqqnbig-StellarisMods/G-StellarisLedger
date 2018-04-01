using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;

namespace StellarisInGameLedgerInCSharp
{
    public class Country
    {
        public string Tag { get; set; }
        public string Name { get; internal set; }
        public int TechnologyCount { get; set; }
        public double MilitaryPower { get; set; }
        public int ColonyCount { get; set; }
        public int CivilianStations { get; internal set; }
        public int Population { get; internal set; }
        public double Energy { get; set; }
        public double Minerals { get; set; }
        public double Food { get; set; }
        public double Influence { get; set; }
        public double Unity { get; set; }
        public double EnergyIncome { get; set; }
        public double MineralsIncome { get; internal set; }
        public double FoodIncome { get; internal set; }
        public double InfluenceIncome { get; internal set; }
        public double UnityIncome { get; internal set; }
        public double PhysicsResearchIncome { get; internal set; }
        public double SocietyResearchIncome { get; set; }
        public double EngineeringResearchIncome { get; internal set; }
    }
}
