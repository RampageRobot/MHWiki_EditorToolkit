using Newtonsoft.Json;
using System.Text;

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
			try
			{
				string fileName = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\Monster Data\{monsterName}\Parts.json";
				if (File.Exists(fileName))
				{
					Dictionary<string, dynamic[]> partData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
					foreach (dynamic dyn in partData["Flinches"])
					{
						ret.Add(new()
						{
							Name = dyn.Name,
							CanFlinch = true, //??? when is this gonna be false
							Essence = Monster.GetKinsectEssence((int)dyn.Kinsect_Color)
						});
					}
				}
			}
			catch { }
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
	}
}
