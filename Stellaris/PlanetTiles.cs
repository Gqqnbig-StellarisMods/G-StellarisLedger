using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StellarisLedger
{
	public class PlanetTiles
	{
		public string Name { get; set; }
		public string Id { get; set; }

		public Dictionary<int, Tile> Tiles { get; set; }
	}


	public class Tile
	{
		//public string Id { get; set; }

		public Dictionary<string, double> Resources { get; set; } = new Dictionary<string, double>();
		//public int Minerals { get; set; }

		//public int Food { get; set; }
		//public int Energy { get; set; }
		//public int SocietyResearch { get; set; }
		//public int PhysicsResearch { get; set; }
		//public int EngineeringResearch { get; set; }

		///// <summary>
		///// 用于标识其他未识别的资源类型，比如倍燃石、外形宠物等。
		///// </summary>
		//public int Other { get; set; }

	}
}
