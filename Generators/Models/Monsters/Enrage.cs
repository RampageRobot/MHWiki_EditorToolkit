using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
	/// <summary>
	/// Template is here: <see href="https://monsterhunterwiki.org/Template:EnrageDataTable"/>
	/// </summary>
	public class Enrage
    {
        public bool FileFound { get; set; }
		public bool MRChanges { get; set; } = false;
        public int Duration { get; set; }
        public int? MRDuration { get; set; }
        public float DamageMod { get; set; }
        public float? MRDamageMod { get; set; }
        public float SpeedMod { get; set; }
        public float? MRSpeedMod { get; set; }
        public float PlayerDamageMod { get; set; }
		public float? MRPlayerDamageMod { get; set; }

        public Enrage() { }

        public Enrage(string monsterName)
        {
            string fileName = $@"C:\Users\mkast\Desktop\test monster stuff\MHWI\{monsterName}\Enrage.json";
            if (File.Exists(fileName))
            {
                FileFound = true;
                List<dynamic[]> enrageData = JsonConvert.DeserializeObject<List<dynamic[]>>(File.ReadAllText(fileName))!;
                foreach (dynamic[] objList in enrageData)
                {
                    foreach (dynamic obj in objList)
                    {
                        try
                        {
                            if (obj.Name != "MR")
							{
								DamageMod = obj.Damage_Modifier;
								SpeedMod = obj.Speed_Modifier;
								PlayerDamageMod = obj.Player_Damage_Modifier;
								Duration = Convert.ToInt32(obj.Duration);
							}
                            else
                            {
                                MRDamageMod = obj.Damage_Modifier;
                                MRSpeedMod = obj.Speed_Modifier;
                                MRPlayerDamageMod = obj.Player_Damage_Modifier;
                                MRDuration = Convert.ToInt32(obj.Duration);
                            }
                        }
                        catch { }
                    }
				}
				MRChanges = MRDuration != null && (MRDamageMod != DamageMod || MRSpeedMod != SpeedMod || MRPlayerDamageMod != PlayerDamageMod || MRDuration != Duration);
			}
        }
	}
}
