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
		public string Threshold { get; set; } = string.Empty;
        public float Increase { get; set; }
        public float Duration { get; set; }
        public float Decay { get; set; }
        public float DecayDuration { get; set; }
        public float Damage { get; set; }

        public static StatusEffectiveness[] GetStatuses(string monsterName)
        {
            List<StatusEffectiveness> ret = [];
            string fileName = $@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\Damage Attributes.json";
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
                                dynamic damageAttr = effectData.First(x => x.Value.First().Poison_Damage != null).Value.First();
								ret.Add(new()
                                {
                                    Type = type,
                                    Duration = (float)Math.Round((float)effDyn.Duration, 2),
                                    Decay = effDyn.Drain_Value,
                                    DecayDuration = (float)Math.Round((float)effDyn.Drain_Time, 2),
                                    Damage = damageAttr.Poison_Damage * damageAttr.Poison_Interval * effDyn.Duration,
                                    Increase = effDyn.Buildup,
                                    Threshold = $"{effDyn.Base}→{effDyn.Max_Cap}"
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
									Duration = (float)Math.Round((float)effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = (float)Math.Round((float)effDyn.Drain_Time, 2),
									Damage = 0,
									Increase = effDyn.Buildup,
									Threshold = $"{effDyn.Base}→{effDyn.Max_Cap}"
								});
							}
                            break;
						case StatusType.Exhaust:
							{
								dynamic effDyn = effectData["Status Buildup: Sleep/Paralysis/Stun/Exhaustion"].First(x => x.Name == typeName);
                                dynamic exhaustDam = effectData.First(x => x.Value.First().Exhaustion_Damage != null).Value.First();
								ret.Add(new()
								{
									Type = type,
									Duration = (float)Math.Round((float)effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = (float)Math.Round((float)effDyn.Drain_Time, 2),
									Damage = exhaustDam.Exhaustion_Damage,
									Increase = effDyn.Buildup,
									Threshold = $"{effDyn.Base}→{effDyn.Max_Cap}"
								});
							}
							break;
                        case StatusType.Blast:
                            {
                                dynamic effDyn = effectData["Status Buildup: Mount/Blastblight"].First(x => x.Name == typeName);
								dynamic blastDam = effectData.First(x => x.Value.First().Blastblight_Damage != null).Value.First();
								ret.Add(new()
								{
									Type = type,
									Duration = (float)Math.Round((float)effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = (float)Math.Round((float)effDyn.Drain_Time, 2),
									Damage = blastDam.Blastblight_Damage,
									Increase = effDyn.Buildup,
									Threshold = $"{effDyn.Base}→{effDyn.Max_Cap}"
								});
							}
                            break;
                        case StatusType.Mount:
							{
								dynamic effDyn = effectData["Status Buildup: Mount/Blastblight"].First(x => x.Name == typeName);
								ret.Add(new()
								{
									Type = type,
									Duration = (float)Math.Round((float)effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = (float)Math.Round((float)effDyn.Drain_Time, 2),
									Damage = 0,
									Increase = effDyn.Buildup,
									Threshold = $"{effDyn.Base}→{effDyn.Max_Cap}"
								});
							}
                            break;
                        case StatusType.Elderseal:
							{
								dynamic effDyn = effectData["Status Buildup: Dragonseal"].First(x => x.Name == "Dragonseal");
								ret.Add(new()
								{
									Type = type,
									Duration = (float)Math.Round((float)effDyn.Duration, 2),
									Decay = effDyn.Drain_Value,
									DecayDuration = (float)Math.Round((float)effDyn.Drain_Time, 2),
									Damage = 0,
									Increase = effDyn.Buildup,
									Threshold = $"{effDyn.Base}→{effDyn.Max_Cap}"
								});
							}
                            break;
					}
                }
            }
            return [.. ret];
        }

		public static string Format(StatusEffectiveness[] statuses, string name)
		{
			if (statuses.Any())
			{
				dynamic[] statusStars = JsonConvert.DeserializeObject<dynamic[]>(File.ReadAllText(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\statusStars.json"))!;
				StatusEffectiveness poison = statuses.First(x => x.Type == StatusType.Poison);
				dynamic? thisStar = statusStars.FirstOrDefault(x => x.Name == name);
				thisStar ??= new
				{
					Name = name,
					Poison = 0,
					Sleep = 0,
					Paralysis = 0,
					Blast = 0,
					Stun = 0
				};
				StatusEffectiveness para = statuses.First(x => x.Type == StatusType.Paralysis);
				StatusEffectiveness sleep = statuses.First(x => x.Type == StatusType.Sleep);
				StatusEffectiveness blast = statuses.First(x => x.Type == StatusType.Blast);
				StatusEffectiveness stun = statuses.First(x => x.Type == StatusType.Stun);
				StatusEffectiveness exhaust = statuses.First(x => x.Type == StatusType.Exhaust);
				StatusEffectiveness mount = statuses.First(x => x.Type == StatusType.Mount);
				StatusEffectiveness elder = statuses.First(x => x.Type == StatusType.Elderseal);
				return $@"===Status Effectiveness===
{{{{MHWIStatusData
|Poison ★ number = {(thisStar.Poison == 0 ? "" : new string('★', (int)thisStar.Poison))}
|Poison Threshold = {(thisStar.Poison == 0 ? "" : poison.Threshold)}
|Poison Increase = {(thisStar.Poison == 0 ? "" : poison.Increase)}
|Poison Duration = {(thisStar.Poison == 0 ? "" : poison.Duration)}s
|Poison Decay = {(thisStar.Poison == 0 ? "" : poison.Decay)}/{(thisStar.Poison == 0 ? "" : poison.DecayDuration)}s
|Poison Damage = {(thisStar.Poison == 0 ? "" : poison.Damage)}
|Poison Damage in Master Rank=

|Paralysis ★ number = {(thisStar.Paralysis == 0 ? "" : new string('★', (int)thisStar.Paralysis))}
|Paralysis Threshold = {(thisStar.Paralysis == 0 ? "" : para.Threshold)}
|Paralysis Increase = {(thisStar.Paralysis == 0 ? "" : para.Increase)}
|Paralysis Duration = {(thisStar.Paralysis == 0 ? "" : para.Duration)}s
|Paralysis Decay = {(thisStar.Paralysis == 0 ? "" : para.Decay)}/{(thisStar.Paralysis == 0 ? "" : para.DecayDuration)}s

|Sleep ★ number = {(thisStar.Sleep == 0 ? "" : new string('★', (int)thisStar.Sleep))}
|Sleep Threshold = {(thisStar.Sleep == 0 ? "" : sleep.Threshold)}
|Sleep Increase = {(thisStar.Sleep == 0 ? "" : sleep.Increase)}
|Sleep Duration = {(thisStar.Sleep == 0 ? "" : sleep.Duration)}s
|Sleep Decay = {(thisStar.Sleep == 0 ? "" : sleep.Decay)}/{(thisStar.Sleep == 0 ? "" : sleep.DecayDuration)}s

|Blast ★ number = {(thisStar.Blast == 0 ? "" : new string('★', (int)thisStar.Blast))}
|Blast Threshold = {(thisStar.Blast == 0 ? "" : blast.Threshold)}
|Blast Increase = {(thisStar.Blast == 0 ? "" : blast.Increase)}
|Blast Damage = {(thisStar.Blast == 0 ? "" : blast.Damage)}
|Blast Damage in Master Rank =

|Stun ★ number = {(thisStar.Stun == 0 ? "" : new string('★', (int)thisStar.Stun))}
|Stun Threshold = {(thisStar.Stun == 0 ? "" : stun.Threshold)}
|Stun Increase = {(thisStar.Stun == 0 ? "" : stun.Increase)}
|Stun Duration = {(thisStar.Stun == 0 ? "" : stun.Duration)}s
|Stun Decay = {(thisStar.Stun == 0 ? "" : stun.Decay)}/{(thisStar.Stun == 0 ? "" : stun.DecayDuration)}s{(exhaust.Threshold == "0→0" ? "" : "\r\n\r\n|Exhaust Threshold = " + exhaust.Threshold)}{(exhaust.Threshold == "0→0" ? "" : "\r\n|Exhaust Increase = " + exhaust.Increase)}{(exhaust.Threshold == "0→0" ? "" : "\r\n|Exhaust Decay = " + exhaust.Decay + "/" + exhaust.DecayDuration + "s")}{(exhaust.Threshold == "0→0" ? "" : "\r\n|Exhaust Damage = " + exhaust.Damage)}

|Mount Threshold = {mount.Threshold}
|Mount Increase = {mount.Increase}{(elder.Threshold == "0→0" ? "" : "\r\n\r\n|Elderseal Threshold =" + elder.Threshold)} {(elder.Threshold == "0→0" ? "" : "\r\n|Elderseal Increase =" + elder.Increase)}{(elder.Threshold == "0→0" ? "" : $"\r\n|Elderseal Decay ={elder.Decay}/{elder.DecayDuration}s")}
}}}}";
			}
			else
			{
				return "";
			}
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
