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
			string fileName = $@"C:\Users\mkast\Desktop\test monster stuff\MHWI\{monsterName}\Stamina.json";
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
	} 
}
