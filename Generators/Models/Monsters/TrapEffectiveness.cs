using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
	public class TrapEffectiveness
    {
        public TrapType Type { get; set; }
        public float? Duration { get; set; }
        public float? DurationExhaust { get; set; }
        public float? ToleranceReduction { get; set; }
        public float? MinDuration { get; set; }
        public float? Effectiveness { get; set; }

        public static TrapEffectiveness[] GetTraps(string monsterName)
        {
            Stamina stam = new(monsterName);
            List<TrapEffectiveness> ret = [];
            string fileName = $@"C:\Users\mkast\Desktop\test monster stuff\MHWI\{monsterName}\Stamina.json";
            if (File.Exists(fileName))
            {
				Dictionary<string, dynamic[]> trapData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
				string[] trapTypeNames = ["Pitfall Trap", "Shock Trap", "Vine Trap", "Flash Pod", "Meat", "Dung Pod", "Sonic Pod"];
                dynamic[] traps = trapData["Status Buildup: Shock Trap/Pitfall Trap/Ivy Trap/Unk"];
                for (int i = 0; i < trapTypeNames.Length; i++)
                {
                    TrapType thisType = (TrapType)i;
                    string thisTypeName = trapTypeNames[i];
					dynamic thisTrapObj = traps.First(x => x.Name == thisTypeName);
                    if (i <= 3)
                    {
                        ret.Add(new()
                        {
                            Type = thisType,
                            Duration = Math.Round(thisTrapObj.Duration, 2),
                            ToleranceReduction = Math.Round(thisTrapObj.Duration_Decrease_Per_Use, 2),
                            MinDuration = Math.Round(thisTrapObj.Duration_Minimum, 2),
                            DurationExhaust = Math.Round(thisTrapObj.Duration * (1 + (1 - stam.Speed)), 2),
                            Effectiveness = 100
                        });
                    }
                    else
                    {
                        TrapEffectiveness retObj = new()
                        {
                            Type = thisType
                        };
                        switch (thisType)
                        {
                            case TrapType.Meat:
                                retObj.Duration = Math.Round(thisTrapObj.Duration, 2);
                                break;
                            case TrapType.DungPod:
                                break;
                            case TrapType.SonicPod:
                                break;
                        }
                    }
				}
            }
        }
    }

    public enum TrapType
    {
        Pitfall,
        Shock,
        Vine,
        FlashPod,
        Meat,
        DungPod,
        SonicPod
    }
}
