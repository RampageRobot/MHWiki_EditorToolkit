using DocumentFormat.OpenXml.Bibliography;
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
        private static dynamic[] BookInfo { get; set; } = [];

        public static TrapEffectiveness[] GetTraps(string monsterName)
        {
            Stamina stam = new(monsterName);
            List<TrapEffectiveness> ret = [];
            string fileName = $@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\Damage Attributes.json";
            if (File.Exists(fileName))
            {
				Dictionary<string, dynamic[]> trapData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
				string[] trapTypeNames = ["Pitfall Trap", "Shock Trap", "Ivy Trap Unk", "Flash Pod", "Meat", "Dung Pod", "Sonic Pod"];
                dynamic dungObjs = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\dungpod.json"))!;
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
                            Duration = (float?)Math.Round((float)thisTrapObj.Duration, 2),
                            ToleranceReduction = (float?)Math.Round((float)thisTrapObj.Duration_Decrease_Per_Use, 2),
                            MinDuration = (float?)Math.Round((float)thisTrapObj.Duration_Minimum, 2),
                            DurationExhaust = (float?)Math.Round((float)thisTrapObj.Duration * (1 + (1 - (float)(stam.Speed == null ? 1 : stam.Speed))), 2),
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
                                    retObj.Duration = (float?)Math.Round((float)thisTrapObj.Duration, 2);
                                    retObj.ToleranceReduction = (float?)Math.Round((float)thisTrapObj.Duration_Decrease_Per_Use, 2);
                                    retObj.MinDuration = (float?)Math.Round((float)thisTrapObj.Duration_Minimum, 2);
                                    retObj.DurationExhaust = (float?)Math.Round((float)thisTrapObj.Duration * (1 + (1 - (float)(stam.Speed == null ? 1 : stam.Speed))), 2);
                                    retObj.Effectiveness = 100;
									ret.Add(retObj);
									dynamic thisMRObj = trapData["Status Buildup: Dizziness MR"].First();
									TrapEffectiveness mrObj = new()
                                    {
                                        Type = thisType,
                                        Rank = Rank.Master,
										Duration = (float?)Math.Round((float)thisMRObj.Duration, 2),
										ToleranceReduction = (float?)Math.Round((float)thisMRObj.Duration_Decrease_Per_Use, 2),
										MinDuration = (float?)Math.Round((float)thisMRObj.Duration_Minimum, 2),
										DurationExhaust = (float?)Math.Round((float)thisMRObj.Duration * (1 + (1 - (float)(stam.Speed == null ? 1 : stam.Speed))), 2),
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
                                    dynamic thisDungObj = dungObjs[monsterName];
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

        public static string Format(string game, TrapEffectiveness[] traps, string monsterName)
        {
            if (traps.Any())
            {
                TrapEffectiveness pit = traps.First(x => x.Type == TrapType.Pitfall);
                TrapEffectiveness shock = traps.First(x => x.Type == TrapType.Shock);
                TrapEffectiveness vine = traps.First(x => x.Type == TrapType.Vine);
                TrapEffectiveness flash = traps.First(x => x.Type == TrapType.FlashPod);
                TrapEffectiveness dung = traps.First(x => x.Type == TrapType.DungPod);
                TrapEffectiveness scream = traps.First(x => x.Type == TrapType.SonicPod);
                if (BookInfo.Length == 0)
                {
                    BookInfo = JsonConvert.DeserializeObject<dynamic[]>(File.ReadAllText(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\bookData.json"))!;
                }
                dynamic thisBookInfo = BookInfo.First(x => x.Name == monsterName);
                Dictionary<string, List<int>> sourceDict = [];
                string mhwKey = "''モンスターハンター:ワールド 公式ガイドブック ([[Monster Hunter: World Official Guidebook]])''";
                string mhwiKey = "''モンスターハンターワールド:アイスボーン 公式ガイドブック ([[Monster Hunter World: Iceborne Official Guidebook]])''";
                if (thisBookInfo.MHWPage != "")
                {
                    if (!sourceDict.TryGetValue(mhwKey, out List<int>? value))
                    {
                        sourceDict.Add(mhwKey, [(int)thisBookInfo.MHWPage]);
                    }
                    else
                    {
                        value.Add((int)thisBookInfo.MHWPage);
                    }
                }
                if (thisBookInfo.MHWIPage1 != "" && (thisBookInfo.MHWPage == "" || (thisBookInfo.MHWPage != "" && (thisBookInfo.MHWDung != thisBookInfo.MHWIDung || thisBookInfo.MHWScreamer != thisBookInfo.MHWIScreamer || thisBookInfo.MHWMeat != thisBookInfo.MHWIMeat))))
                {
                    if (!sourceDict.TryGetValue(mhwiKey, out List<int>? value))
                    {
                        sourceDict.Add(mhwiKey, [(int)thisBookInfo.MHWIPage1]);
                    }
                    else
                    {
                        value.Add((int)thisBookInfo.MHWIPage1);
                    }
                }
                if (thisBookInfo.MHWIPage2 != "" && (thisBookInfo.MHWPage == "" || (thisBookInfo.MHWPage != "" && (thisBookInfo.MHWDung != thisBookInfo.MHWIDung || thisBookInfo.MHWScreamer != thisBookInfo.MHWIScreamer || thisBookInfo.MHWMeat != thisBookInfo.MHWIMeat))))
                {
                    if (!sourceDict.TryGetValue(mhwiKey, out List<int>? value))
                    {
                        sourceDict.Add(mhwiKey, [(int)thisBookInfo.MHWIPage2]);
                    }
                    else
                    {
                        value.Add((int)thisBookInfo.MHWIPage2);
                    }
                }
                foreach (string key in sourceDict.Keys)
                {
                    sourceDict[key] = [.. sourceDict[key].Distinct()];
                }
                string sources = "";
                int bookCnt = 1;
                foreach (KeyValuePair<string, List<int>> kvp in sourceDict)
                {
                    int pageCnt = 1;
                    sources += $@"
|Guidebook {bookCnt} = {kvp.Key}";
                    foreach (int page in kvp.Value)
                    {
                        sources += $@"
|Guidebook {bookCnt} Page {pageCnt} = {page}";
                        pageCnt++;
                    }
                    bookCnt++;
                }
                return $@"===Item Effectiveness===
{{{{ItemEffectivenessTable
|Game = {game}{sources}

|Pitfall Duration = {(pit.Duration == 0 ? "" : pit.Duration)}
|Pitfall Exhausted Duration = {(pit.DurationExhaust == 0 ? "" : pit.DurationExhaust)}
|Pitfall Tolerance Reduction = {(pit.ToleranceReduction == 0 ? "" : pit.ToleranceReduction)}
|Pitfall Min Duration = {(pit.MinDuration == 0 ? "" : pit.MinDuration)}

|Shock Duration = {(shock.Duration == 0 ? "" : shock.Duration)}
|Shock Exhausted Duration = {(shock.DurationExhaust == 0 ? "" : shock.DurationExhaust)}
|Shock Tolerance Reduction = {(shock.ToleranceReduction == 0 ? "" : shock.ToleranceReduction)}
|Shock Min Duration = {(shock.MinDuration == 0 ? "" : shock.MinDuration)}

|Ivy Duration = {(vine.Duration == 0 ? "" : vine.Duration)}
|Ivy Exhausted Duration = {(vine.DurationExhaust == 0 ? "" : vine.DurationExhaust)}

|Flash Duration = {(flash.Duration == 0 ? "" : flash.Duration)}
|Flash Note =
|Flash Tolerance Reduction = {(flash.ToleranceReduction == 0 ? "" : flash.ToleranceReduction)}
|Flash Min Duration = {(flash.MinDuration == 0 ? "" : flash.MinDuration)}

|Meat Effectiveness ={(thisBookInfo.MHWMeat == "O" || thisBookInfo.MHWIMeat == "O" ? " X" : "")}

|Dung Effectiveness = {(thisBookInfo.MHWDung != "" ? thisBookInfo.MHWDung : thisBookInfo.MHWIDung != "" ? thisBookInfo.MHWIDung : "")}

|Sonic Effectiveness ={(thisBookInfo.MHWScreamer == "O" || thisBookInfo.MHWIScreamer == "O" ? " X" : "")}
|Sonic Note ={(!string.IsNullOrEmpty(thisBookInfo.MHWIScreamer_Condition) ? thisBookInfo.MHWIScreamer_Condition : (!string.IsNullOrEmpty(thisBookInfo.MHWScreamer_Condition) ? thisBookInfo.MHWScreamer_Condition : ""))}
}}}}";
            }
            else
            {
                return "";
            }
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
