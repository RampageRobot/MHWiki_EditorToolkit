using MediawikiTranslator.Models.Monsters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;

namespace MediawikiTranslator.Models.Data.MHRS
{
	public class HitZoneValues
	{
		private static DataList[] _bossData = MonsterListBossData.Fetch().SnowDataMonsterListMonsterListBossData!.DataList!;
		private static JObject _hunterNoteMenuMsg = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\natives\stm\message\hunternote\hn_hunternote_menu.msg.539100710.json"))!.Value<JObject>("msgs")!;

		public static void Temp()
		{
			Dictionary<string, DamageEffectiveness[]> hzvs = [];
			foreach (string dir in Directory.EnumerateDirectories(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\unpacked\re_chunk_000\natives\STM\enemy").Where(x => new DirectoryInfo(x).Name.StartsWith("em") && !new DirectoryInfo(x).Name.StartsWith("em131") && !new DirectoryInfo(x).Name.StartsWith("ems")))
			{
				foreach (string subdir in Directory.EnumerateDirectories(dir).Where(x => new DirectoryInfo(x).Name! != "child" && new DirectoryInfo(x).Name! != "common"))
				{
					string id = new DirectoryInfo(dir).Name + "_" + new DirectoryInfo(subdir).Name;
					hzvs.Add(id, GetWebToolkitData(id));
				}
			}
			Dictionary<string, MonsterData> srcDict = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
			StringBuilder sb = new();
			sb.AppendLine("Monster ID\tMonster Name\tPart Name\tIs Phase?");
			foreach (KeyValuePair<string, DamageEffectiveness[]> val in hzvs.Where(x => x.Value.Any(y => y.IsPhase)))
			{
				List<DamageEffectiveness> alreadyDone = [];
				string monsterName = srcDict.First(x => x.Value.LargeMonster && x.Value.GameAppearances.Any(y => (y.GameAcronym == "MHRS" || y.GameAcronym == "MHRise") && y.InternalID == val.Key)).Key;
				foreach (DamageEffectiveness eff in val.Value.Where(x => (!x.IsPhase && val.Value!.Any(y => y.PartName == x.PartName && y.IsPhase)) || x.IsPhase))
				{
					if (!eff.IsPhase || (eff.IsPhase && !alreadyDone.Any(y =>
						y.IceEffect == eff.IceEffect &&
						y.WaterEffect == eff.WaterEffect &&
						y.FireEffect == eff.FireEffect &&
						y.ThunderEffect == eff.ThunderEffect &&
						y.DragonEffect == eff.DragonEffect &&
						y.BluntEffect == eff.BluntEffect &&
						y.BulletEffect == eff.BulletEffect &&
						y.SeverEffect == eff.SeverEffect &&
						y.StunEffect == eff.StunEffect &&
						y.PartName == eff.PartName)))
					{
						sb.AppendLine($"{val.Key}\t{monsterName}\t{eff.PartName}\t{(eff.IsPhase ? "Yes" : "No")}");
						alreadyDone.Add(eff);
					}
				}
			}
		}
		public static Monsters.DamageEffectiveness[] GetWebToolkitData(string monsterId)
		{
			List<Monsters.DamageEffectiveness> ret = [];
			MeatData meat = MeatData.Fetch(monsterId);
			try
			{
				DataList bossEntry = _bossData.First(x => x.EmType == monsterId.Replace("em", "EmType"));
				int cntr = 0;
				string[] charArray = [.. "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Select(x => x.ToString())];
				foreach (MeatContainer partGroup in meat.SnowEnemyEnemyMeatData!.MeatContainer!)
				{
					int phaseCntr = 0;
					foreach (MeatGroupInfo part in partGroup.MeatGroupInfo!)
					{
						try
						{
							if (part.Slash + part.Strike + part.Shell + part.Fire + part.Water + part.Ice + part.Elect + part.Dragon + part.Piyo > 0)
							{
								string partName = "";
								if (!bossEntry.PartTableData!.Any(x => x.EmPart == charArray[cntr]) && monsterId == "em062_00")
								{
									partName = "Bottom Half";
								}
								else if (monsterId == "em086_05")
								{
									if (!bossEntry.PartTableData!.Any(x => x.EmPart == charArray[cntr]))
									{
										partName = "Chest";
									}
									else
									{
										string partId = bossEntry.PartTableData!.First(x => x.EmPart == charArray[cntr]).Part!;
										if (partId == "Parts71")
										{
											partName = "Shell";
										}
										else
										{
											partName = _hunterNoteMenuMsg.Values().First(x => x.Value<string>("name") == "HN_Hunternote_ML_Tab_02_" + partId)!.Value<JObject>("content")!.Value<string>("English")!;
										}
										if (partName == "Chest")
										{
											partName = "Chest (While Inhaling)";
										}
									}
								}
								else if (monsterId == "em071_00" || monsterId == "em071_05")
								{
									string partId = bossEntry.PartTableData!.First(x => x.EmPart == charArray[cntr]).Part!;
									if (partId == "Parts72")
									{
										partName = "Antenna";
									}
									else
									{
										partName = _hunterNoteMenuMsg.Values().First(x => x.Value<string>("name") == "HN_Hunternote_ML_Tab_02_" + partId)!.Value<JObject>("content")!.Value<string>("English")!;
									}
								}
								else if (monsterId == "em089_00" && cntr > 7)
								{
									switch (cntr)
									{
										case 8:
											partName = "Arm, Tail, and Tail Tip Hellfire";
											break;
										case 9:
											partName = "Hellfire Gas (on back)";
											break;
										case 10:
											partName = "Face Hellfire";
											break;
									}
								}
								else if (monsterId == "em089_05" && cntr == 2)
								{
									partName = "Torso";
								}
								else if (monsterId == "em094_00" || monsterId == "em094_01")
								{
									switch (cntr)
									{
										case 0:
										case 1:
											{
												string partId = bossEntry.PartTableData!.First(x => x.EmPart == charArray[cntr]).Part!;
												partName = _hunterNoteMenuMsg.Values().First(x => x.Value<string>("name") == "HN_Hunternote_ML_Tab_02_" + partId)!.Value<JObject>("content")!.Value<string>("English")!;
											}
											break;
										case 2:
											partName = "???";
											break;
										case 3:
										case 4:
										case 5:
											{
												string partId = bossEntry.PartTableData!.First(x => x.EmPart == charArray[cntr]).Part!;
												partName = _hunterNoteMenuMsg.Values().First(x => x.Value<string>("name") == "HN_Hunternote_ML_Tab_02_" + partId)!.Value<JObject>("content")!.Value<string>("English")!;
												if (cntr == 4 && monsterId == "em094_01")
												{
													partName = "Back Legs (Thread)";
												}
											}
											break;
										case 6:
											partName = "Abdomen (Cocooning)";
											break;
										case 7:
											partName = "Leg";
											break;
										case 8:
											partName = "Claw (Broken)";
											break;
										case 9:
											partName = "Front Legs (Thread)";
											break;
									}
								}
								else if (monsterId == "em095_00" && cntr == 7)
								{
									partName = "Mud Balls (Tail)";
								}
								else if (monsterId == "em095_01" && cntr == 7)
								{
									partName = "Magma Balls (Tail)";
								}
								else if (monsterId == "em107_00" && cntr > 3)
								{
									partName = cntr == 4 ? "Rock" : "Jar";
								}
								else if (monsterId == "em020_00" && cntr == 6)
								{
									partName = "Weakness? (弱点) area under shell";
								}
								else if ((monsterId == "em097_00" && cntr == 6) || (monsterId == "em134_00" && cntr == 6))
								{
									partName = "??? (no internal name or target area)";
								}
								else
								{
									string partId = bossEntry.PartTableData!.First(x => x.EmPart == charArray[cntr]).Part!;
									if (partId == "Parts71")
									{
										partName = "Shell";
									}
									else if (partId == "Parts72")
									{
										partName = "Tactile Part [Organ]";
									}
									else
									{
										partName = _hunterNoteMenuMsg.Values().First(x => x.Value<string>("name") == "HN_Hunternote_ML_Tab_02_" + partId)!.Value<JObject>("content")!.Value<string>("English")!;
									}
								}
								ret.Add(new()
								{
									IsPhase = partGroup.MeatGroupInfo!.Length > 1 && phaseCntr > 0,
									PartName = partName,
									BluntEffect = (int)part.Strike!.Value!,
									SeverEffect = (int)part.Slash!.Value!,
									BulletEffect = (int)part.Shell!.Value!,
									FireEffect = (int)part.Fire!.Value!,
									WaterEffect = (int)part.Water!.Value!,
									IceEffect = (int)part.Ice!.Value!,
									ThunderEffect = (int)part.Elect!.Value!,
									DragonEffect = (int)part.Dragon!.Value!,
									StunEffect = (int)part.Piyo!.Value!
								});
							}
							phaseCntr++;
						}
						catch (Exception e)
						{
							Debugger.Break();
							return [];
						}
					}
					cntr++;
				}
				return [.. ret];
			}
			catch (Exception e)
			{
				Debugger.Break();
				return [];
			}
		}
	}

	public partial class MeatData
	{
		[JsonProperty("snow.enemy.EnemyMeatData", NullValueHandling = NullValueHandling.Ignore)]
		public SnowEnemyEnemyMeatData? SnowEnemyEnemyMeatData { get; set; }

		public static MeatData Fetch(string monsterId)
		{
			string[] idParts = monsterId.Split('_');
			string species = idParts[0];
			string sub = idParts[1];
			return JsonConvert.DeserializeObject<MeatData>(File.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\natives\stm\enemy\{species}\{sub}\user_data\{species}_{sub}_meat_data.user.2.json"))!;
		}
	}

	public partial class SnowEnemyEnemyMeatData
	{
		[JsonProperty("_MeatContainer", NullValueHandling = NullValueHandling.Ignore)]
		public MeatContainer[]? MeatContainer { get; set; }
	}

	public partial class MeatContainer
	{
		[JsonProperty("_MeatGroupInfo", NullValueHandling = NullValueHandling.Ignore)]
		public MeatGroupInfo[]? MeatGroupInfo { get; set; }
	}

	public partial class MeatGroupInfo
	{
		[JsonProperty("_Slash", NullValueHandling = NullValueHandling.Ignore)]
		public long? Slash { get; set; }

		[JsonProperty("_Strike", NullValueHandling = NullValueHandling.Ignore)]
		public long? Strike { get; set; }

		[JsonProperty("_Shell", NullValueHandling = NullValueHandling.Ignore)]
		public long? Shell { get; set; }

		[JsonProperty("_Fire", NullValueHandling = NullValueHandling.Ignore)]
		public long? Fire { get; set; }

		[JsonProperty("_Water", NullValueHandling = NullValueHandling.Ignore)]
		public long? Water { get; set; }

		[JsonProperty("_Ice", NullValueHandling = NullValueHandling.Ignore)]
		public long? Ice { get; set; }

		[JsonProperty("_Elect", NullValueHandling = NullValueHandling.Ignore)]
		public long? Elect { get; set; }

		[JsonProperty("_Dragon", NullValueHandling = NullValueHandling.Ignore)]
		public long? Dragon { get; set; }

		[JsonProperty("_Piyo", NullValueHandling = NullValueHandling.Ignore)]
		public long? Piyo { get; set; }
	}
}
