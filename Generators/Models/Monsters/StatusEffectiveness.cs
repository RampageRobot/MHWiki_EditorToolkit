using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
	public class StatusEffectiveness
    {
        public StatusType Type { get; set; }
        public float Threshold { get; set; }
        public float Increase { get; set; }
        public float Duration { get; set; }
        public float Decay { get; set; }
        public float DecayDuration { get; set; }
        public float Damage { get; set; }

        public static StatusEffectiveness[] GetStatuses(string monsterName)
        {
            List<StatusEffectiveness> ret = [];
            string fileName = $@"C:\Users\mkast\Desktop\test monster stuff\MHWI\{monsterName}\Damage Attributes.json";
            if (File.Exists(fileName))
			{
				Dictionary<string, dynamic[]> effectData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
				string[] typeNames = ["Poison", "Paralysis", "Sleep", "Blastblight", "Stun", "Exhaustion", "Mount", "Elderseal"];
                StatusType[] types = Enum.GetValues<StatusType>();
				for (int i = 0; i < types.Length; i++)
                {
                    StatusType type = types[i];
                    string typeName = typeNames[i];
                    switch (type)
                    {
                        case StatusType.Poison:
                            {
                                dynamic effDyn = effectData["Status Buildup: Poison"].First();
                                dynamic damageAttr = effectData["Monster Damage Attributes (2)"].First();
                                ret.Add(new()
                                {
                                    Type = type,
                                    Duration = Math.Round(effDyn.Duration, 2),
                                    Decay = effDyn.Drain_Value,
                                    DecayDuration = Math.Round(effDyn.Drain_Time, 2),
                                    Damage = damageAttr.Poison_Damage * damageAttr.Poison_Interval * effDyn.Duration,
                                    Increase = effDyn.Buildup,
                                    Threshold = effDyn.Max_Cap
                                });
                            }
                            break;
                        case StatusType.Sleep:
                        case StatusType.Stun:
                        case StatusType.Paralysis:
							{
								dynamic effDyn = effectData["Status Buildup: Sleep/Paralysis/Stun/Exhaustion"].First(x => x.Name == typeName);
								ret.Add(new()
								{
									Type = type,
									Duration = Math.Round(effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = Math.Round(effDyn.Drain_Time, 2),
									Damage = 0,
									Increase = effDyn.Buildup,
									Threshold = effDyn.Max_Cap
								});
							}
                            break;
						case StatusType.Exhaust:
							{
								dynamic effDyn = effectData["Status Buildup: Sleep/Paralysis/Stun/Exhaustion"].First(x => x.Name == typeName);
                                dynamic exhaustDam = effectData["Monster Damage Attributes (3)"].First();
								ret.Add(new()
								{
									Type = type,
									Duration = Math.Round(effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = Math.Round(effDyn.Drain_Time, 2),
									Damage = exhaustDam.Exhaustion_Damage,
									Increase = effDyn.Buildup,
									Threshold = effDyn.Max_Cap
								});
							}
							break;
                        case StatusType.Blast:
                            {
                                dynamic effDyn = effectData["Status Buildup: Mount/Blastblight"].First(x => x.Name == typeName);
								dynamic blastDam = effectData["Monster Damage Attributes (5)"].First();
								ret.Add(new()
								{
									Type = type,
									Duration = Math.Round(effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = Math.Round(effDyn.Drain_Time, 2),
									Damage = blastDam.Blastblight_Damage,
									Increase = effDyn.Buildup,
									Threshold = effDyn.Max_Cap
								});
							}
                            break;
                        case StatusType.Mount:
							{
								dynamic effDyn = effectData["Status Buildup: Mount/Blastblight"].First(x => x.Name == typeName);
								ret.Add(new()
								{
									Type = type,
									Duration = Math.Round(effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = Math.Round(effDyn.Drain_Time, 2),
									Damage = 0,
									Increase = effDyn.Buildup,
									Threshold = effDyn.Max_Cap
								});
							}
                            break;
                        case StatusType.Elderseal:
							{
								dynamic effDyn = effectData["Status Buildup: Dragonseal"].First(x => x.Name == typeName);
								ret.Add(new()
								{
									Type = type,
									Duration = Math.Round(effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = Math.Round(effDyn.Drain_Time, 2),
									Damage = 0,
									Increase = effDyn.Buildup,
									Threshold = effDyn.Max_Cap
								});
							}
                            break;
					}
                }
            }
            return [.. ret];
        }
    }

    public enum StatusType
    {
        Poison,
        Paralysis,
        Sleep,
        Blast,
        Stun,
        Exhaust,
        Mount,
        Elderseal
    }
}
