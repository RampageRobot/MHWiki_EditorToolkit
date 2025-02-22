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
        public Rank Rank { get; set; } = Rank.All;
        public float? Duration { get; set; }
        public float? DurationExhaust { get; set; }
        public float? ToleranceReduction { get; set; }
        public float? MinDuration { get; set; }
        public float? Effectiveness { get; set; }

        public static TrapEffectiveness[] GetTraps(string monsterName)
        {
            Stamina stam = new(monsterName);
            List<TrapEffectiveness> ret = [];
            string fileName = $@"C:\Users\mkast\Desktop\test monster stuff\MHWI\{monsterName}\Damage Attributes.json";
            if (File.Exists(fileName))
            {
				Dictionary<string, dynamic[]> trapData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
				string[] trapTypeNames = ["Pitfall Trap", "Shock Trap", "Vine Trap", "Flash Pod", "Meat", "Dung Pod", "Sonic Pod"];
                dynamic[] traps = trapData["Status Buildup: Shock Trap/Pitfall Trap/Ivy Trap/Unk"];
                for (int i = 0; i < trapTypeNames.Length; i++)
                {
                    TrapType thisType = (TrapType)i;
                    string thisTypeName = trapTypeNames[i];
                    if (i <= 2)
					{
						dynamic thisTrapObj = traps.First(x => x.Name == thisTypeName);
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
                            case TrapType.FlashPod:
                                {
                                    retObj.Rank = Rank.LowHigh;
                                    dynamic thisTrapObj = trapData["Status Buildup: Dizziness LR/HR"].First();
                                    retObj.Duration = Math.Round(thisTrapObj.Duration, 2);
                                    retObj.ToleranceReduction = Math.Round(thisTrapObj.Duration_Decrease_Per_Use, 2);
                                    retObj.MinDuration = Math.Round(thisTrapObj.Duration_Minimum, 2);
                                    retObj.DurationExhaust = Math.Round(thisTrapObj.Duration * (1 + (1 - stam.Speed)), 2);
                                    retObj.Effectiveness = 100;
									ret.Add(retObj);
									dynamic thisMRObj = trapData["Status Buildup: Dizziness MR"].First();
									TrapEffectiveness mrObj = new TrapEffectiveness()
                                    {
                                        Type = thisType,
                                        Rank = Rank.Master,
										Duration = Math.Round(thisMRObj.Duration, 2),
										ToleranceReduction = Math.Round(thisMRObj.Duration_Decrease_Per_Use, 2),
										MinDuration = Math.Round(thisMRObj.Duration_Minimum, 2),
										DurationExhaust = Math.Round(thisMRObj.Duration * (1 + (1 - stam.Speed)), 2),
										Effectiveness = 100
									};
									ret.Add(retObj);
									ret.Add(mrObj);
								}
                                break;
                            case TrapType.Meat:
								ret.Add(retObj);
								break;
                            case TrapType.DungPod:
                                {
                                    dynamic thisTrapObj = trapData["Status Buildup: Dung"].First();
                                    retObj.Effectiveness = thisTrapObj.Base == 1 ? 100 : 0;
                                    retObj.Duration = Math.Round(thisTrapObj.Duration, 2);
									ret.Add(retObj);
								}
								break;
                            case TrapType.SonicPod:
								ret.Add(retObj);
								break;
                        }
                    }
				}
            }
            return [.. ret];
        }
    }

    public enum Rank
    {
        All,
        LowHigh,
        Master
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
