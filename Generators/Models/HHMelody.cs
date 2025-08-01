using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models
{
	public class HHMelody
	{
		public string Game { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Icon { get; set; } = string.Empty;
		public string[] Notes { get; set; } = [];

		public static HHMelody[] Fetch()
		{
			Dictionary<string, string> melodyIcons = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\melodyIcons.json"))!;
			dynamic[] melodyDescs = JsonConvert.DeserializeObject<dynamic[]>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\hhMelodyDescs.json"))!;
			List<HHMelody> allMelodies = [];
			List<Tuple<string, string[]>> mhwildsMelodies = Generators.Weapon.GetHHMelodies("MHWilds");
			allMelodies.AddRange(mhwildsMelodies.Select(x => {
				string thisMelodyNameTemp = x.Item1 == "HIGHFREQ" ? "Echo Wave" : x.Item1;
				HHMelody mel = new()
				{
					Game = "MHWilds",
					Description = melodyDescs.FirstOrDefault(x => x.skillName == thisMelodyNameTemp)?.skillDesc ?? "???",
					Icon = thisMelodyNameTemp == "Echo Wave" ? melodyIcons["Echo Wave (Slash)"] : melodyIcons[thisMelodyNameTemp],
					Name = thisMelodyNameTemp,
					Notes = [..x.Item2.Select(x => x.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan"))]
				};
				return mel;
			}));
			List<Tuple<string, string[]>> mhwiMelodies = Generators.Weapon.GetHHMelodies("MHWI");
			allMelodies.AddRange(mhwiMelodies.Select(x => {
				Tuple<string, string> melodyInfo = Generators.Weapon.GetWorldMelodyEffect(x.Item1);
				HHMelody mel = new()
				{
					Game = "MHWI",
					Description = melodyInfo.Item2,
					Icon = melodyIcons.TryGetValue(melodyInfo.Item1, out string? value) ? value : "???",
					Name = melodyInfo.Item1,
					Notes = x.Item2
				};
				return mel;
			}));
			return [.. allMelodies];
		}
	}
}
