using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MediawikiTranslator.Generators;
using MediawikiTranslator.Models.Data.MHWI;
using MediawikiTranslator.Models.Weapon;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MediawikiTranslator.Models.Data.MHRS
{
	public class Weapon
	{
		[JsonProperty("_Id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; } = string.Empty;

		[JsonProperty("_SortId", NullValueHandling = NullValueHandling.Ignore)]
		public long? SortId { get; set; }

		[JsonProperty("_RareType", NullValueHandling = NullValueHandling.Ignore)]
		public string RareType { get; set; } = string.Empty;

		[JsonProperty("_ModelId", NullValueHandling = NullValueHandling.Ignore)]
		public string ModelId { get; set; } = string.Empty;

		[JsonProperty("_BaseVal", NullValueHandling = NullValueHandling.Ignore)]
		public long? BaseVal { get; set; }

		[JsonProperty("_BuyVal", NullValueHandling = NullValueHandling.Ignore)]
		public long? BuyVal { get; set; }

		[JsonProperty("_Atk", NullValueHandling = NullValueHandling.Ignore)]
		public long? Atk { get; set; }

		[JsonProperty("_CriticalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? CriticalRate { get; set; }

		[JsonProperty("_DefBonus", NullValueHandling = NullValueHandling.Ignore)]
		public long? DefBonus { get; set; }

		[JsonProperty("_HyakuryuSkillIdList", NullValueHandling = NullValueHandling.Ignore)]
		public string[] HyakuryuSkillIdList { get; set; } = [];

		[JsonProperty("_SlotNumList", NullValueHandling = NullValueHandling.Ignore)]
		public long[] SlotNumList { get; set; } = [];

		[JsonProperty("_HyakuryuSlotNumList", NullValueHandling = NullValueHandling.Ignore)]
		public long[] HyakuryuSlotNumList { get; set; } = [];

		[JsonProperty("_CustomTableNo", NullValueHandling = NullValueHandling.Ignore)]
		public long? CustomTableNo { get; set; }

		[JsonProperty("_CustomCost", NullValueHandling = NullValueHandling.Ignore)]
		public long? CustomCost { get; set; }

		[JsonProperty("_MainElementType", NullValueHandling = NullValueHandling.Ignore)]
		public string MainElementType { get; set; } = string.Empty;

		[JsonProperty("_MainElementVal", NullValueHandling = NullValueHandling.Ignore)]
		public long? MainElementVal { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		[JsonProperty("_SharpnessValList", NullValueHandling = NullValueHandling.Ignore)]
		public long[] SharpnessValList { get; set; } = [];

		[JsonProperty("_TakumiValList", NullValueHandling = NullValueHandling.Ignore)]
		public long[] HandicraftValList { get; set; } = [];

		[JsonIgnore]
		public WeaponTreeDataParam? Tree { get; set; }

		[JsonIgnore]
		public WeaponCraftingDataParam? CraftingData { get; set; }

		[JsonIgnore]
		public WeaponForgingDataParam? ForgingData { get; set; }

		private static Items[] _mhrsItems { get; set; } = Items.Fetch();

		public static WebToolkitData[] GetWebToolkitData()
		{
			Weapon[] allWeapons = Fetch();
			List<WebToolkitData> ret = [];
			foreach (Weapon weapon in allWeapons)
			{
				try
				{
					WebToolkitData newData = new()
					{
						Name = weapon.Name,
						Description = weapon.Description.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "),
						Tree = weapon.Tree?.Name ?? "???",
						Game = "MHRS",
						Affinity = weapon.CriticalRate,
						Attack = weapon.Atk?.ToString() ?? "-",
						Defense = weapon.DefBonus?.ToString() ?? "-",
						Decos1 = weapon.SlotNumList[0],
						Decos2 = weapon.SlotNumList[1],
						Decos3 = weapon.SlotNumList[2],
						Decos4 = weapon.SlotNumList[3],
						Element1 = weapon.MainElementType != "None" && !string.IsNullOrEmpty(weapon.MainElementType) ? weapon.MainElementType.Replace("Posion", "Poison").Replace("Bomb", "Blast") : string.Empty,
						ElementDmg1 = weapon.MainElementType != "None" && !string.IsNullOrEmpty(weapon.MainElementType) ? weapon.MainElementVal!.Value.ToString() : string.Empty,
						Rarity = Convert.ToInt32(weapon.RareType.Substring(2)),
						RampageDecoration = (weapon.HyakuryuSlotNumList[0] > 0 ? "{{UI|MHRS|Rampage Deco|1}}" : "") + (weapon.HyakuryuSlotNumList[1] > 0 ? "{{UI|MHRS|Rampage Deco|2}}" : "") + (weapon.HyakuryuSlotNumList[2] > 0 ? "{{UI|MHRS|Rampage Deco|3}}" : ""),
						Type = weapon.TypeInterpreter()
					};
					if (!new string[] { "Bo", "LBG", "HBG" }.Contains(newData.Type))
					{
						int[] sharpness = [(int)weapon.SharpnessValList[0], (int)weapon.SharpnessValList[1], (int)weapon.SharpnessValList[2], (int)weapon.SharpnessValList[3], (int)weapon.SharpnessValList[4], (int)weapon.SharpnessValList[5], (int)weapon.SharpnessValList[6]];
						int[] handi = [(int)weapon.SharpnessValList[0], (int)weapon.SharpnessValList[1], (int)weapon.SharpnessValList[2], (int)weapon.SharpnessValList[3], (int)weapon.SharpnessValList[4], (int)weapon.SharpnessValList[5], (int)weapon.SharpnessValList[6]];
						int lastPositionWithData = 0;
						for (int i = 0; i < 7; i++)
						{
							if (sharpness[i] > 0)
							{
								lastPositionWithData = i;
							}
						}
						if (weapon.HandicraftValList[0] > 0)
						{
							handi[lastPositionWithData] += (int)weapon.HandicraftValList[0];
						}
						if (weapon.HandicraftValList[1] > 0)
						{
							handi[lastPositionWithData + 1] += (int)weapon.HandicraftValList[1];
						}
						if (weapon.HandicraftValList[2] > 0)
						{
							handi[lastPositionWithData + 2] += (int)weapon.HandicraftValList[2];
						}
						if (weapon.HandicraftValList[3] > 0)
						{
							handi[lastPositionWithData + 3] += (int)weapon.HandicraftValList[3];
						}
						SharpnessData handiSharp = new SharpnessData()
						{
							Red = handi[0],
							Orange = handi[1],
							Yellow = handi[2],
							Green = handi[3],
							Blue = handi[4],
							White = handi[5],
							Purple = handi[6]
						};
						SharpnessData sharp = new SharpnessData()
						{
							Red = sharpness[0],
							Orange = sharpness[1],
							Yellow = sharpness[2],
							Green = sharpness[3],
							Blue = sharpness[4],
							White = sharpness[5],
							Purple = sharpness[6]
						};
						newData.Sharpness = $"[[{sharp.Red},{sharp.Orange},{sharp.Yellow},{sharp.Green},{sharp.Blue},{sharp.White},{sharp.Purple}],[{handiSharp.Red},{handiSharp.Orange},{handiSharp.Yellow},{handiSharp.Green},{handiSharp.Blue},{handiSharp.White},{handiSharp.Purple}]]";
					}
					switch (weapon)
					{
						case DualBladesParam wep:
							if (wep.SubElementType != "None")
							{
								newData.Element2 = wep.SubElementType.Replace("Posion", "Poison").Replace("Bomb", "Blast");
								newData.ElementDmg2 = wep.SubElementVal!.Value.ToString();
							}
							break;
						case ChargeBladeParam wep:
							newData.CbPhialType = wep.ChargeAxeBottleType;
							break;
						case SwitchAxeParam wep:
							newData.SaPhialType = wep.SlashAxeBottleType.Replace("StrongElement", "Power Element").Replace("DownStamina", "Exhaust");
							break;
						case GunlanceParam wep:
							newData.GlShellingType = wep.GunLanceFireType;
							newData.GlShellingLevel = wep.GunLanceFireLv[2..];
							break;
						case LightBowGunParam wep:
							switch (wep.Fluctuation)
							{
								case "RightAndLeftMuch":
									newData.LbgDeviation = "LR Severe";
									break;
								case "RightMuch":
									newData.LbgDeviation = "R Severe";
									break;
								case "LeftMuch":
									newData.LbgDeviation = "L Severe";
									break;
								case "RightAndLeftLittle":
									newData.LbgDeviation = "LR Mild";
									break;
								case "RightLittle":
									newData.LbgDeviation = "R Mild";
									break;
								case "LeftLittle":
									newData.LbgDeviation = "L Mild";
									break;
								case "None":
									newData.LbgDeviation = "None";
									break;
							}
							newData.ShellTableWikitext = GetShellTableWikitext(wep);
							newData.LbgSpecialAmmoType = "Wyvernblast";
							break;
						case HeavyBowGunParam wep:
							switch (wep.Fluctuation)
							{
								case "RightAndLeftMuch":
									newData.HbgDeviation = "LR Severe";
									break;
								case "RightMuch":
									newData.HbgDeviation = "R Severe";
									break;
								case "LeftMuch":
									newData.HbgDeviation = "L Severe";
									break;
								case "RightAndLeftLittle":
									newData.HbgDeviation = "LR Mild";
									break;
								case "RightLittle":
									newData.HbgDeviation = "R Mild";
									break;
								case "LeftLittle":
									newData.HbgDeviation = "L Mild";
									break;
								case "None":
									newData.HbgDeviation = "None";
									break;
							}
							newData.ShellTableWikitext = GetShellTableWikitext(wep);
							newData.HbgSpecialAmmoType = wep.HeavyBowgunUniqueBulletType == "Snipe" ? "Wyvernsnipe" : "Wyvernheart";
							break;
						case BowParam wep:
							string[] coatings = ["Close-range", "Power", "Poison", "Para", "Sleep", "Blast", "Exhaust"];
							string coatingsForWep = "";
							for (int i = 0; i < coatings.Length; i++)
							{
								if (wep.BowBottleEquipFlagList[i])
								{
									if (coatingsForWep != "")
									{
										coatingsForWep += ", ";
									}
									coatingsForWep += coatings[i];
								}
							}
							newData.BoCoatings = coatingsForWep;
							break;
						case HuntingHornParam wep:
							newData.HhMelodies = @$"[[File:MHRS-HH Note 1.png|20x20px]][[File:MHRS-HH Note 1.png|20x20px]] {CommonMsgs.GetMsg($"Horn_UniqueParam_{wep.HornMelodyTypeList[0].Replace("Concert_", "")}_Name")}
[[File:MHRS-HH Note 2.png|20x20px]][[File:MHRS-HH Note 2.png|20x20px]] {CommonMsgs.GetMsg($"Horn_UniqueParam_{wep.HornMelodyTypeList[1].Replace("Concert_", "")}_Name")}
[[File:MHRS-HH Note 3.png|20x20px]][[File:MHRS-HH Note 3.png|20x20px]] {CommonMsgs.GetMsg($"Horn_UniqueParam_{wep.HornMelodyTypeList[2].Replace("Concert_", "")}_Name")}";
							break;
						case InsectGlaiveParam wep:
							newData.IgKinsectBonus = "Level " + wep.InsectGlaiveInsectLv;
							break;
					}
					if (weapon.Tree != null)
					{
#nullable enable
						if (!allWeapons.Any(x => x.Tree != null && x.TypeInterpreter() == weapon.TypeInterpreter() && x.Tree.NextWeaponIndexList.Select((y, z) => new Tuple<long, int>(y, z)).Any(y => y.Item1 == weapon.Tree.Index && weapon.Tree.TreeType == x.Tree.NextWeaponTypeList[y.Item2])) && weapon.CraftingData != null && weapon.CraftingData.Items.Length != 0)
						{
							Weapon? parent = allWeapons.First(x => x.Tree != null && x.TypeInterpreter() == weapon.TypeInterpreter() && x.Tree.NextWeaponIndexList.Any(y => y == weapon.Tree.Index));
							newData.PreviousName = parent!.Name;
							newData.PreviousRarity = Convert.ToInt32(parent!.RareType[2..]);
							newData.UpgradeCost = weapon.BaseVal;
							newData.UpgradeMaterials = GetMaterials(weapon!, weapon.CraftingData);
						}
#nullable disable
						if (weapon.ForgingData != null)
						{
							newData.ForgeCost = weapon.BaseVal;
							newData.ForgeMaterials = GetForgeMaterials(weapon, weapon.ForgingData);
						}
						if (weapon.Tree != null && weapon.Tree.PrevWeaponIndex > -1)
						{
							newData.Rollback = "true";
						}
						int cntr = 0;
						foreach (int index in weapon.Tree.NextWeaponIndexList.Where(x => x > -1))
						{
							Weapon child = allWeapons.First(x => x.TypeInterpreter() == weapon.TypeInterpreter() && x.Tree != null && x.Tree.Index!.Value == index && x.Tree.TreeType == weapon.Tree.NextWeaponTypeList[cntr]);
							if (string.IsNullOrEmpty(newData.Next1Name))
							{
								newData.Next1Name = child.Name;
								newData.Next1Rarity = Convert.ToInt32(child.RareType[2..]);
								newData.Next1Cost = child.BaseVal;
								newData.Next1Materials = GetMaterials(child, child.CraftingData);
							}
							else if (string.IsNullOrEmpty(newData.Next2Name))
							{
								newData.Next2Name = child.Name;
								newData.Next2Rarity = Convert.ToInt32(child.RareType[2..]);
								newData.Next2Cost = child.BaseVal;
								newData.Next2Materials = GetMaterials(child, child.CraftingData);
							}
							else if (string.IsNullOrEmpty(newData.Next3Name))
							{
								newData.Next3Name = child.Name;
								newData.Next3Rarity = Convert.ToInt32(child.RareType[2..]);
								newData.Next3Cost = child.BaseVal;
								newData.Next3Materials = GetMaterials(child, child.CraftingData);
							}
							else if (string.IsNullOrEmpty(newData.Next4Name))
							{
								newData.Next4Name = child.Name;
								newData.Next4Rarity = Convert.ToInt32(child.RareType[2..]);
								newData.Next4Cost = child.BaseVal;
								newData.Next4Materials = GetMaterials(child, child.CraftingData);
							}
							cntr++;
						}
					}
					if (!string.IsNullOrEmpty(newData.Name))
					{
						ret.Add(newData);
					}
				}
				catch (Exception exc)
				{
					Debugger.Break();
				}
			}
			return [.. ret];
		}

		private static string GetForgeMaterials(Weapon parent, WeaponForgingDataParam parentCraft)
		{
			string upgradeFromMats = "[";
			int cntr = 0;
			Tuple<string, long>[] mats = [..parentCraft.Items.Select(x => new Tuple<string,long?>(x.Id, parentCraft.ItemNum[cntr++]))
						.Where(x => !string.IsNullOrEmpty(x.Item1) && x.Item2.HasValue && x.Item2.Value > 0)
						.Select(x => new Tuple<string,long>(x.Item1, x.Item2!.Value)).OrderByDescending(x => x.Item2)];
			foreach (Tuple<string, long> matIds in mats)
			{
				Items mat = _mhrsItems.First(x => x.Id == matIds.Item1);
				upgradeFromMats += $"{{\"name\": \"{mat.Name}\", \"icon\": \"{mat.WikiIconName}\", \"color\": \"{mat.WikiIconColor}\", \"quantity\": {matIds.Item2}}},";
			}
			if (parentCraft.MaterialCategory != "None")
			{
				int catNum = Convert.ToInt32(parentCraft.MaterialCategory[(parentCraft.MaterialCategory.LastIndexOf('_') + 1)..]);
				string fullText = $"{CommonMsgs.GetMsg($"ICT_Name_{(catNum < 100 ? catNum.ToString("D2") : catNum.ToString("D3"))}")} Material(s) x{parentCraft.MaterialCategoryNum} pt{(parentCraft.MaterialCategoryNum > 1 ? "s" : "")}.";
				upgradeFromMats += $"{{\"name\": \"{fullText}\", \"icon\": \"MATERIAL_NOICON\", \"color\": \"White\", \"quantity\": 1}},";
			}
			return upgradeFromMats + "]";
		}

		private static string GetMaterials(Weapon parent, WeaponCraftingDataParam parentCraft)
		{
			string upgradeFromMats = "[";
			int cntr = 0;
			if (parentCraft.Items.Length > 0)
			{
				Tuple<string, long>[] mats = [..parentCraft.Items.Select(x => new Tuple<string,long?>(x.Id, parentCraft.ItemNum[cntr++]))
						.Where(x => !string.IsNullOrEmpty(x.Item1) && x.Item2.HasValue && x.Item2.Value > 0)
						.Select(x => new Tuple<string,long>(x.Item1, x.Item2!.Value)).OrderByDescending(x => x.Item2)]; ;
				foreach (Tuple<string, long> matIds in mats)
				{
					Items mat = _mhrsItems.First(x => x.Id == matIds.Item1);
					upgradeFromMats += $"{{\"name\": \"{mat.Name}\", \"icon\": \"{mat.WikiIconName}\", \"color\": \"{mat.WikiIconColor}\", \"quantity\": {matIds.Item2}}},";
				}
			}
			if (parentCraft.MaterialCategory != "None")
			{
				int catNum = Convert.ToInt32(parentCraft.MaterialCategory[(parentCraft.MaterialCategory.LastIndexOf('_') + 1)..]);
				string fullText = $"{CommonMsgs.GetMsg($"ICT_Name_{(catNum < 100 ? catNum.ToString("D2") : catNum.ToString("D3"))}")} Material(s) x{parentCraft.MaterialCategoryNum} pt{(parentCraft.MaterialCategoryNum > 1 ? "s" : "")}.";
				upgradeFromMats += $"{{\"name\": \"{fullText}\", \"icon\": \"MATERIAL_NOICON\", \"color\": \"White\", \"quantity\": 1}},";
			}
			return upgradeFromMats + "]";
		}

		private static string GetShellTableWikitext(Weapon wep)
		{
			string[] ammoList = ["Normal Ammo 1", "Normal Ammo 2", "Normal Ammo 3", "Pierce Ammo 1", "Pierce Ammo 2", "Pierce Ammo 3", "Spread Ammo 1", "Spread Ammo 2", "Spread Ammo 3", "Shrapnel Ammo 1", "Shrapnel Ammo 2", "Shrapnel Ammo 3", "Sticky Ammo 1", "Sticky Ammo 2", "Sticky Ammo 3", "Cluster Bomb 1", "Cluster Bomb 2", "Cluster Bomb 3", "Poison Ammo 1", "Poison Ammo 2", "Paralysis Ammo 1", "Paralysis Ammo 2", "Sleep Ammo 1", "Sleep Ammo 2", "Exhaust Ammo 1", "Exhaust Ammo 2", "Recover Ammo 1", "Recover Ammo 2", "Demon Ammo", "Armor Ammo", "Flaming Ammo", "Piercing Fire Ammo", "Water Ammo", "Piercing Water Ammo", "Freeze Ammo", "Piercing Ice Ammo", "Thunder Ammo", "Piercing Thunder Ammo", "Dragon Ammo", "Piercing Dragon Ammo", "Slicing Ammo", "Wyvern Ammo", "Tranq Ammo"];
			Dictionary<string, int[]> ammoRaws = new Dictionary<string, int[]>()
			{
				{ "Normal Ammo 1", new int[] { 8, 4 } },
				{ "Normal Ammo 2", new int[] { 9, 5 } },
				{ "Normal Ammo 3", new int[] { 9, 6 } },
				{ "Pierce Ammo 1", new int[] { 9, 6 } },
				{ "Pierce Ammo 2", new int[] { 11, 7 } },
				{ "Pierce Ammo 3", new int[] { 11, 7 } },
				{ "Spread Ammo 1", new int[] { 9, 5 } },
				{ "Spread Ammo 2", new int[] { 10, 6 } },
				{ "Spread Ammo 3", new int[] { 11, 7 } },
				{ "Shrapnel Ammo 1", new int[] { 9, 5 } },
				{ "Shrapnel Ammo 2", new int[] { 10, 6 } },
				{ "Shrapnel Ammo 3", new int[] { 11, 7 } },
				{ "Sticky Ammo 1", new int[] { 11, 7 } },
				{ "Sticky Ammo 2", new int[] { 13, 8 } },
				{ "Sticky Ammo 3", new int[] { 13, 9 } },
				{ "Cluster Bomb 1", new int[] { 13, 9 } },
				{ "Cluster Bomb 2", new int[] { 14, 10 } },
				{ "Cluster Bomb 3", new int[] { 14, 10 } },
				{ "Poison Ammo 1", new int[] { 12, 7 } },
				{ "Poison Ammo 2", new int[] { 14, 9 } },
				{ "Paralysis Ammo 1", new int[] { 12, 8 } },
				{ "Paralysis Ammo 2", new int[] { 14, 9 } },
				{ "Sleep Ammo 1", new int[] { 12, 8 } },
				{ "Sleep Ammo 2", new int[] { 14, 9 } },
				{ "Exhaust Ammo 1", new int[] { 11, 7 } },
				{ "Exhaust Ammo 2", new int[] { 12, 8 } },
				{ "Recover Ammo 1", new int[] { 10, 5 } },
				{ "Recover Ammo 2", new int[] { 12, 7 } },
				{ "Demon Ammo", new int[] { 10, 8 } },
				{ "Armor Ammo", new int[] { 10, 8 } },
				{ "Flaming Ammo", new int[] { 9, 6 } },
				{ "Piercing Fire Ammo", new int[] { 10, 7 } },
				{ "Water Ammo", new int[] { 9, 6 } },
				{ "Piercing Water Ammo", new int[] { 10, 7 } },
				{ "Freeze Ammo", new int[] { 9, 6 } },
				{ "Piercing Ice Ammo", new int[] { 10, 7 } },
				{ "Thunder Ammo", new int[] { 9, 6 } },
				{ "Piercing Thunder Ammo", new int[] { 10, 7 } },
				{ "Dragon Ammo", new int[] { 12, 9 } },
				{ "Piercing Dragon Ammo", new int[] { 14, 10 } },
				{ "Slicing Ammo", new int[] { 13, 8 } },
				{ "Wyvern Ammo", new int[] { 15, 8 } },
				{ "Tranq Ammo", new int[] { 10, 5 } }
			};
			StringBuilder ret = new();
			HeavyBowGunParam? hbg = null;
			LightBowGunParam? lbg = null;
			bool[] bulletEquipList = [];
			long[] bulletNumList = [];
			string[] bulletTypeList = [];
			string[] rapidShotList = [];
			string reloadVal = "";
			string recoilVal = "";
			if (wep.GetType() == typeof(HeavyBowGunParam))
			{
				hbg = (HeavyBowGunParam)wep;
				bulletEquipList = hbg.BulletEquipFlagList;
				bulletNumList = hbg.BulletNumList;
				bulletTypeList = hbg.BulletTypeList;
				reloadVal = hbg.Reload;
				recoilVal= hbg.Recoil;
			}
			if (wep.GetType() == typeof(LightBowGunParam))
			{
				lbg = (LightBowGunParam)wep;
				bulletEquipList = lbg.BulletEquipFlagList;
				bulletNumList = lbg.BulletNumList;
				bulletTypeList = lbg.BulletTypeList;
				rapidShotList = lbg.RapidShotList;
				reloadVal = lbg.Reload;
				recoilVal = lbg.Recoil;
			}
			for (int i = 1; i < bulletEquipList.Length; i++)
			{
				if (bulletEquipList[i] && bulletNumList[i] > 0 && ammoList.Length >= i)
				{
					Items shellItem = _mhrsItems.First(x => x.Name == ammoList[i - 1]);
					int recoilRaw = ammoRaws[shellItem.Name][0];
					int reloadRaw = ammoRaws[shellItem.Name][1];
					ret.AppendLine($"|{{{{GenericItemLink|MHRS|{shellItem.Name}|Ammo|{shellItem.WikiIconColor}}}}}");
					ret.AppendLine($"|{bulletNumList[i]}");
					int recoilValInt = Convert.ToInt32(recoilVal[(recoilVal.IndexOf('_') + 1)..]);
					string recoil = "";
					if (shellItem.Name.Contains("Wyvern"))
					{
						recoil = "Wyvern";
					}
					else if (recoilRaw < (8 + recoilValInt))
					{
						recoil = "Low";
					}
					else if (recoilRaw == (8 + recoilValInt))
					{
						recoil = "Average";
					}
					else if (recoilRaw < (11 + recoilValInt))
					{
						recoil = "High";
					}
					else
					{
						recoil = "Very High";
					}
					int reloadValInt = Convert.ToInt32(reloadVal[(reloadVal.IndexOf('_') + 1)..]);
					string reload = "";
					if (reloadValInt == 1)
					{
						if (reloadRaw < 3)
						{
							reload = "Fastest";
						}
						else if (reloadRaw < 4)
						{
							reload = "Fast";
						}
						else if (reloadRaw < 7)
						{
							reload = "Slow";
						}
						else
						{
							reload = "Slowest";
						}
					}
					else
					{
						if (reloadRaw < (3 + reloadValInt))
						{
							reload = "Fastest";
						}
						else if (reloadRaw < (5 + reloadValInt))
						{
							reload = "Fast";
						}
						else if (reloadRaw < (7 + reloadValInt))
						{
							reload = "Slow";
						}
						else
						{
							reload = "Slowest";
						}
					}
					ret.AppendLine($"|{recoil}");
					ret.AppendLine($"|{reload}");
					string icons = "";
					string wepType = wep.GetType() == typeof(HeavyBowGunParam) ? "HBG" : "LBG";
					switch (bulletTypeList[i])
					{
						case "Shoot_00":
							break;
						case "Shoot_01":
							icons = $"{{{{UI|{{{{{{Game|MHWI}}}}}}|{wepType} Moving Shot Enabled|size=24x24px|nolink=true|title=Moving Shot Enabled}}}}";
							break;
						case "Shoot_02":
							icons = $"{{{{UI|{{{{{{Game|MHWI}}}}}}|{wepType} Moving Shot Enabled|size=24x24px|nolink=true|title=Moving Shot Enabled}}}} {{{{UI|{{{{{{Game|MHWI}}}}}}|{wepType} Moving Reload Enabled|size=24x24px|nolink=true|title=Moving Reload Enabled}}}}";
							break;
						case "Shoot_03":
							icons = $"{{{{UI|{{{{{{Game|MHWI}}}}}}|{wepType} Moving Shot Enabled|size=24x24px|nolink=true|title=Moving Shot Enabled}}}} {{{{UI|{{{{{{Game|MHRS}}}}}}|{wepType} Single Fire Auto Reload|size=24x24px|nolink=true|title=Single Fire Auto Reload}}}}";
							break;
						case "Shoot_04":
							icons = $"{{{{UI|{{{{{{Game|MHWI}}}}}}|{wepType} Moving Reload Enabled|size=24x24px|nolink=true|title=Moving Reload Enabled}}}}";
							break;
						case "Shoot_05":
							icons = $"{{{{UI|{{{{{{Game|MHWI}}}}}}|{wepType} Moving Reload Enabled|size=24x24px|nolink=true|title=Moving Reload Enabled}}}} {{{{UI|{{{{{{Game|MHRS}}}}}}|{wepType} Single Fire Auto Reload|size=24x24px|nolink=true|title=Single Fire Auto Reload}}}}";
							break;
						case "Shoot_06":
							icons = $"{{{{UI|{{{{{{Game|MHRS}}}}}}|{wepType} Single Fire Auto Reload|size=24x24px|nolink=true|title=Single Fire Auto Reload}}}}";
							break;
						case "Shoot_07":
							icons = $"{{{{UI|{{{{{{Game|MHWI}}}}}}|{wepType} Moving Shot Enabled|size=24x24px|nolink=true|title=Moving Shot Enabled}}}} {{{{UI|{{{{{{Game|MHWI}}}}}}|{wepType} Moving Reload Enabled|size=24x24px|nolink=true|title=Moving Reload Enabled}}}} {{{{UI|{{{{{{Game|MHRS}}}}}}|{wepType} Single Fire Auto Reload|size=24x24px|nolink=true|title=Single Fire Auto Reload}}}}";
							break;
					}
					if (shellItem.Name.Contains("Wyvern"))
					{
						icons += $" [[File:MHRS-Ammo Cluster Icon Brown.png|24x24px|link=|Wyvern]]";
					}
					if (shellItem.Name.StartsWith("Cluster"))
					{
						icons += $" [[File:MHRS-Ammo Cluster Icon Red.png|24x24px|link=|Cluster]]";
					}
					ret.AppendLine($"|{(string.IsNullOrEmpty(icons) ? " -" : icons)}");
					ret.AppendLine("|-");
				}
			}
			return ret.ToString();
		}

		public string TypeInterpreter()
		{
			switch (this)
			{
				case BowParam _:
					return "Bo";
				case ChargeBladeParam _:
					return "CB";
				case SwitchAxeParam _:
					return "SA";
				case DualBladesParam _:
					return "DB";
				case GreatSwordParam _:
					return "GS";
				case GunlanceParam _:
					return "GL";
				case HammerParam _:
					return "Hm";
				case HeavyBowGunParam _:
					return "HBG";
				case HuntingHornParam _:
					return "HH";
				case InsectGlaiveParam _:
					return "IG";
				case LanceParam _:
					return "Ln";
				case LightBowGunParam _:
					return "LBG";
				case LongSwordParam _:
					return "LS";
				case SwordAndShieldParam _:
					return "SnS";
				default: return "???";
			}
		}

		public static Weapon[] Fetch()
		{
			WeaponCraftingDataParam[] craftingData = WeaponCraftingData.GetCraftingData();
			WeaponForgingDataParam[] forgingData = WeaponForgingData.GetForgingData();
			WeaponTreeDataParam[] treeData = WeaponTreeData.GetWeaponTreeData();
			List<Weapon> allWeapons = [];
			allWeapons.AddRange(Bow.Fetch());
			allWeapons.AddRange(ChargeBlade.Fetch());
			allWeapons.AddRange(SwitchAxe.Fetch());
			allWeapons.AddRange(DualBlades.Fetch());
			allWeapons.AddRange(GreatSword.Fetch());
			allWeapons.AddRange(Gunlance.Fetch());
			allWeapons.AddRange(Hammer.Fetch());
			allWeapons.AddRange(HeavyBowGun.Fetch());
			allWeapons.AddRange(HuntingHorn.Fetch());
			allWeapons.AddRange(InsectGlaive.Fetch());
			allWeapons.AddRange(Lance.Fetch());
			allWeapons.AddRange(LightBowGun.Fetch());
			allWeapons.AddRange(LongSword.Fetch());
			allWeapons.AddRange(SwordAndShield.Fetch());
			foreach (Weapon weapon in allWeapons)
			{
				weapon.Name = CommonMsgs.GetMsg($"{weapon.Id}_Name");
				weapon.Description = CommonMsgs.GetMsg($"{weapon.Id}_Explain");
				weapon.CraftingData = craftingData.FirstOrDefault(x => x.Id == weapon.Id);
				weapon.ForgingData = forgingData.FirstOrDefault(x => x.Id == weapon.Id);
				weapon.Tree = treeData.FirstOrDefault(x => x.WeaponId == weapon.Id);
			}
			return [.. allWeapons.Where(x => !string.IsNullOrEmpty(x.Name) && !x.Name.StartsWith("<COLOR FF0000>#Rejected#</COLOR>"))];
		}
	}
}
