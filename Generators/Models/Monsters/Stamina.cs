using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
	public class Stamina
	{
		public int? LRMinStamina { get; set; }
		public int? LRMaxStamina { get; set; }
        public float? LRDuration { get; set; }
		public int? HRMinStamina { get; set; }
		public int? HRMaxStamina { get; set; }
		public float? HRDuration { get; set; }
		public int? MRMinStamina { get; set; }
		public int? MRMaxStamina { get; set; }
		public float? MRDuration { get; set; }
		public float? Speed { get; set; }
		public bool FileFound { get; set; }

		public Stamina() { }

		public Stamina(string monsterName)
		{
			string fileName = $@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\Stamina.json";
			if (File.Exists(fileName))
			{
				FileFound = true;
				Dictionary<string, dynamic[]> stamData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
				foreach (KeyValuePair<string, dynamic[]> kvp in stamData.Where(x => x.Key.StartsWith("Fatigue (") && x.Value.First().Duration != null && x.Value.First().Duration > -1))
				{
					dynamic stamObj = kvp.Value.First();
					switch (kvp.Key)
					{
						case string key when key.EndsWith("(LR)"):
							LRMinStamina = stamObj.Stamina_Min;
							LRMaxStamina = stamObj.Stamina_Max;
							LRDuration = stamObj.Duration;
							break;
						case string key when key.EndsWith("(HR)"):
							HRMinStamina = stamObj.Stamina_Min;
							HRMaxStamina = stamObj.Stamina_Max;
							HRDuration = stamObj.Duration;
							break;
						case string key when key.EndsWith("(MR)"):
							MRMinStamina = stamObj.Stamina_Min;
							MRMaxStamina = stamObj.Stamina_Max;
							MRDuration = stamObj.Duration;
							break;
					}
				}
				Speed = stamData.Values.FirstOrDefault(x => x.FirstOrDefault()?.Fatigue_Speed != null)?.FirstOrDefault()?.Fatigue_Speed;
			}
		}

		public string Format()
		{
			int rankDiff = 0;
			if ((HRMaxStamina != MRMaxStamina || HRMinStamina != MRMinStamina || HRDuration != MRDuration) && HRMaxStamina == LRMaxStamina && HRMinStamina == LRMinStamina && HRDuration == LRDuration)
			{
				rankDiff = 1;
			}
			else if ((LRMaxStamina != HRMaxStamina || LRMinStamina != HRMinStamina || LRDuration != HRDuration) && (HRMaxStamina != MRMaxStamina || HRMinStamina != MRMinStamina || HRDuration != MRDuration))
			{
				rankDiff = 2;
			}
			else if (LRMaxStamina == MRMaxStamina && LRMinStamina == MRMinStamina && LRDuration == MRDuration && (HRMaxStamina != MRMaxStamina || HRMinStamina != MRMinStamina || HRDuration != MRDuration))
			{
				rankDiff = 3;
			}
			string fields = "";
			switch (rankDiff)
			{
				case 0:
					fields = $@"| Max Stamina 1 = {(LRMaxStamina > 0 ? LRMaxStamina : HRMaxStamina > 0 ? HRMaxStamina : MRMaxStamina)}
| Max Stamina 2 = 
| Max Stamina 3 =

| Duration 1 = {(LRDuration > 0 ? LRDuration : HRDuration > 0 ? HRDuration : MRDuration)}
| Duration 2 = 
| Duration 3 =

| Recovered Stamina 1 = {(LRMinStamina > 0 ? LRMinStamina : HRMinStamina > 0 ? HRMinStamina : MRMinStamina)}
| Recovered Stamina 2 = 
| Recovered Stamina 3 =";
					break;
				case 1:
					fields = $@"| Max Stamina 1 = {(LRMaxStamina > 0 ? LRMaxStamina : HRMaxStamina > 0 ? HRMaxStamina : 0)}
| Max Stamina 2 = {MRMaxStamina}
| Max Stamina 3 =

| Duration 1 = {(LRDuration > 0 ? LRDuration : HRDuration > 0 ? HRDuration : 0)}
| Duration 2 = {MRDuration}
| Duration 3 =

| Recovered Stamina 1 = {(LRMinStamina > 0 ? LRMinStamina : HRMinStamina > 0 ? HRMinStamina : 0)}
| Recovered Stamina 2 = {MRMinStamina}
| Recovered Stamina 3 =";
					break;
				case 2:
					fields = $@"| Max Stamina 1 = {LRMaxStamina}
| Max Stamina 2 = {HRMaxStamina}
| Max Stamina 3 = {MRMaxStamina}

| Duration 1 = {LRDuration}
| Duration 2 = {HRDuration}
| Duration 3 = {MRDuration}

| Recovered Stamina 1 = {LRMinStamina}
| Recovered Stamina 2 = {HRMinStamina}
| Recovered Stamina 3 = {MRMinStamina}";
					break;
				case 3:
					fields = $@"| Max Stamina 1 = {LRMaxStamina}
| Max Stamina 2 = {(HRMaxStamina > 0 ? HRMaxStamina : MRMaxStamina > 0 ? MRMaxStamina : 0)}
| Max Stamina 3 =

| Duration 1 = {LRDuration}
| Duration 2 = {(HRDuration > 0 ? HRDuration : MRDuration > 0 ? MRDuration : 0)}
| Duration 3 =

| Recovered Stamina 1 = {LRMinStamina}
| Recovered Stamina 2 = {(HRMinStamina > 0 ? HRMinStamina : MRMinStamina > 0 ? MRMinStamina : 0)}
| Recovered Stamina 3 =";
					break;
			}
			return $@"{{{{MonsterFatigueTable
| Rank Difference? = {rankDiff}

{fields}

| Fatigued Speed Multiplier = {Speed}
}}}}";
		}
	} 
}
