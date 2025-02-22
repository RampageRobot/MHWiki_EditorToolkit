using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
	public class FlinchBreakThresholds
	{
		public string Rank { get; set; } = "Low/High";
		public string Name { get; set; } = string.Empty;
		public bool CanFlinch { get; set; } = false;
		public string TripConditions { get; set; } = string.Empty;
		public int TripDuration { get; set; }
		public string BreakConditions { get; set; } = string.Empty;

		public static FlinchBreakThresholds[] GetFlinchBreakThresholds(string monsterName)
		{
			List<FlinchBreakThresholds> ret = [];
			string fileName = $@"C:\Users\mkast\Desktop\test monster stuff\MHWI\{monsterName}\Parts.json";
			if (File.Exists(fileName))
			{
				Dictionary<string, dynamic[]> partData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
				foreach (dynamic dyn in partData["Flinches"])
				{
					ret.Add(new()
					{
						Name = dyn.Name,
						CanFlinch = true, //??? when is this gonna be false
					});
				}
			}
			return [.. ret];
		}
	}
}
