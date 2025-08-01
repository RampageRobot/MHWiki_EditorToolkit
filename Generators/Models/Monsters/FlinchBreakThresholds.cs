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
		public KinsectEssence Essence { get; set; }

		public static FlinchBreakThresholds[] GetFlinchBreakThresholds(string monsterName)
		{
			List<FlinchBreakThresholds> ret = [];
			string fileName = $@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\Parts.json";
			if (File.Exists(fileName))
			{
				Dictionary<string, dynamic[]> partData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
				foreach (dynamic dyn in partData["Flinches"])
				{
					ret.Add(new()
					{
						Name = dyn.Name,
						CanFlinch = true, //??? when is this gonna be false
						Essence = GetKinsectEssence((int)dyn.Kinsect_Color)
					});
				}
			}
			return [.. ret];
		}

		public static string Format(FlinchBreakThresholds[] thresholds)
		{
			StringBuilder sb = new();
			sb.AppendLine(@"===Parts===
{| class=""wikitable"" style=""text-align:center;width:100%;""
|- class=""sticky-row""
!Name
![[File:MHRS-Kinsect_Cutting_Icon_Rare_1.png|24x24px]]");
			foreach (FlinchBreakThresholds threshold in thresholds)
			{
				string[] colorNames = ["", "crimson", "#c0c0c0", "orange", "limegreen"];
				string[] numberBalls = ["", "❶", "❷", "❸", "❹"];
				sb.AppendLine($@"
|-
|{threshold.Name}
|<span style=""color:{colorNames[(int)threshold.Essence]};"">{numberBalls[(int)threshold.Essence]}</span>");
			}
			sb.AppendLine("|}");
			return sb.ToString();
		}

		private static KinsectEssence GetKinsectEssence(int id)
		{
			id++;
			return (KinsectEssence)id;
		}
	}

	public enum KinsectEssence
	{
		None,
		Red,
		White,
		Orange,
		Green
	}
}
