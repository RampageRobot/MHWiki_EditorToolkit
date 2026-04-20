using ClosedXML.Excel;
using MediawikiTranslator.Models.Weapon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;

namespace MediawikiTranslator.Models.Data.MHWilds
{
	public class WeaponCraft
	{
		public string Name { get; set; } = string.Empty;
		public int Rarity { get; set; }
		public string Guid { get; set; } = string.Empty;
		public Weapon? Parent { get; set; }
		public Items[] Materials { get; set; } = [];
		public int[] Qtys { get; set; } = [];
		public int Cost { get; set; }
	}

	public class Weapon : WeaponUtils
	{
		public string? Type { get; set; }
		public string? Name { get; set; }
		public int Rarity { get; set; }
		public int Level1Slots { get; set; }
		public int Level2Slots { get; set; }
		public int Level3Slots { get; set; }
		public int Level4Slots { get; set; }
		public int Attack { get; set; }
		public int? Defense { get; set; }
		public string? Element { get; set; }
		public int? ElementDamage { get; set; }
		public int Affinity { get; set; }
		public string? Description { get; set; }
		public string? Skill1 { get; set; }
		public int? Skill1Level { get; set; }
		public string? Skill2 { get; set; }
		public int? Skill2Level { get; set; }
		public string? Skill3 { get; set; }
		public int? Skill3Level { get; set; }
		public WeaponCraft ForgeMaterials { get; set; } = new WeaponCraft();
		public WeaponCraft[] Upgrades { get; set; } = [];
		public string? Sharpness { get; set; }
		public string? HHNote1 { get; set; }
		public string? HHNote2 { get; set; }
		public string? HHNote3 { get; set; }
		public string? HHSpecialMelody { get; set; }
		public string? HHEchoBubble { get; set; }
		public string? GLShellingType { get; set; }
		public string? GLShellingPower { get; set; }
		public string? SAPhialType { get; set; }
		public int? SAPhialDamage { get; set; }
		public string? CBPhialType { get; set; }
		public int? IGKinsectLevel { get; set; }
		public string? BoCoatings { get; set; }
		public string? LBGDefaultMod1 { get; set; }
		public string? LBGDefaultMod2 { get; set; }
		public string? LBGDefaultSpecialAmmoType { get; set; }
		public string? HBGDefaultMod1 { get; set; }
		public string? HBGDefaultMod2 { get; set; }
		public string? HBGDefaultSpecialAmmoType1 { get; set; }
		public string? HBGDefaultSpecialAmmoType2 { get; set; }
		public string? HBGIgnitionGauge { get; set; }
		public string? HBGStandardIgnitionType { get; set; }
		public WildsShellTable[]? ShellTable { get; set; }
		public string HHHighFreqEnum { get; set; } = string.Empty;
		public string TreeName { get; set; } = string.Empty;
		public int WeaponId { get; set; }
		public string WeaponIdStr { get; set; } = string.Empty;
		public bool CanShortcut { get; set; } = false;

		public static Weapon[] GetAllWeapons(bool onlyNames = false)
		{
			List<Weapon> src = [];
			List<Weapon> temp = [];
			try
			{
				Dictionary<string, string> wepNameDict = new() {
					{ "Bo", "Bow" },
					{ "CB", "ChargeAxe" },
					{ "GL", "GunLance" },
					{ "Hm", "Hammer" },
					{ "HBG", "HeavyBowgun" },
					{ "Ln", "Lance" },
					{ "LBG", "LightBowgun" },
					{ "GS", "LongSword" },
					{ "IG", "Rod" },
					{ "SnS", "ShortSword" },
					{ "LS", "Tachi" },
					{ "DB", "TwinSword" },
					{ "SA", "SlashAxe" },
					{ "HH", "Whistle" }
				};
				Dictionary<string, string> wepAbbrevDict = new() {
					{ "Bow", "Bo" },
					{ "ChargeAxe", "CB" },
					{ "GunLance", "GL" },
					{ "Hammer", "Hm" },
					{ "HeavyBowgun", "HBG" },
					{ "Lance", "Ln" },
					{ "LightBowgun", "LBG" },
					{ "LongSword", "GS" },
					{ "Rod", "IG" },
					{ "ShortSword", "SnS" },
					{ "Tachi", "LS" },
					{ "TwinSword", "DB" },
					{ "SlashAxe", "SA" },
					{ "Whistle", "HH" }
				};
				foreach (string wepType in new string[] { "Bow", "ChargeAxe", "GunLance", "Hammer", "HeavyBowgun", "Lance", "LightBowgun", "LongSword", "Rod", "ShortSword", "Tachi", "SlashAxe", "TwinSword", "Whistle" })
				{
					string wepFile = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Weapon\{wepType}.user.3.json";
					string wepNameFile = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\{wepType}.msg.23.json";
					string wepRecipeFile = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Weapon\{wepType}Recipe.user.3.json";
					string wepTreeFile = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Weapon\{wepType}Tree.user.3.json";
					if (!WeaponObjs.ContainsKey(wepType))
					{
						WeaponObjs.Add(wepType, [.. JsonConvert.DeserializeObject<JArray>(Utilities.ReadAllText(wepFile))!.First().Value<JObject>("app.user_data.WeaponData")!.Value<JArray>("_Values")!.Select(x => x.Value<JObject>("app.user_data.WeaponData.cData"))!]);
						WeaponNames.Add(wepType, [.. JsonConvert.DeserializeObject<JObject>(Utilities.ReadAllText(wepNameFile))!.Value<JArray>("entries")!]);
						if (!onlyNames)
						{
							WeaponRecipes.Add(wepType, [.. JsonConvert.DeserializeObject<JArray>(Utilities.ReadAllText(wepRecipeFile))!.First().Value<JObject>("app.user_data.WeaponRecipeData")!.Value<JArray>("_Values")!.Select(x => x.Value<JObject>("app.user_data.WeaponRecipeData.cData"))!]);
							WeaponTrees.Add(wepType, JsonConvert.DeserializeObject<JArray>(Utilities.ReadAllText(wepTreeFile))!.First().Value<JObject>("app.user_data.WeaponTree")!);
						}
					}
				}
				if (onlyNames)
				{
					foreach (string weaponType in wepAbbrevDict.Keys)
					{
						src.AddRange(WeaponObjs[weaponType]
							.Select(x => WeaponNames[weaponType].First(y => y.Value<string>("guid") == x.Value<string>("_Name")).Value<JArray>("content")![1].Value<string>()!)
							.Where(x => !x.Contains("<"))
							.Select(x => new Weapon() { Type = wepAbbrevDict[weaponType], Name = x }));
					}
				}
				else
				{
					Dictionary<string, string> elements = [];
					Dictionary<string, string> hbgMods = [];
					Dictionary<string, string> hbgMods1_checkLater = [];
					Dictionary<string, string> hbgMods2_checkLater = [];
					Dictionary<string, string> hbgSpecialAmmo = [];
					Dictionary<string, string> hbgSpecialAmmo_checkLater = [];
					Dictionary<string, string> lbgMods = [];
					Dictionary<string, string> lbgMods1_checkLater = [];
					Dictionary<string, string> lbgMods2_checkLater = [];
					Dictionary<string, string> lbgSpecialAmmo = [];
					Dictionary<string, string> lbgSpecialAmmo_checkLater = [];
					Dictionary<string, string> glShellTypes = [];
					Dictionary<string, string> glShellTypes_checkLater = [];
					Dictionary<string, string> glShellLevels = [];
					Dictionary<string, string> glShellLevels_checkLater = [];
					Dictionary<string, string> saPhialType = [];
					Dictionary<string, string> saPhialType_checkLater = [];
					Dictionary<string, string> cbPhialType = [];
					Dictionary<string, string> cbPhialType_checkLater = [];
					Dictionary<string, int> igLevels = [];
					Dictionary<string, string> igLevels_checkLater = [];
					Dictionary<string, string> hhSpecialMelodies = [];
					Dictionary<string, string> hhSpecialMelodies_checkLater = [];
					Dictionary<string, string> hhEchoBubbles = [];
					Dictionary<string, string> hhEchoBubbles_checkLater = [];
					glShellLevels.Add("[-170079472]LV4", "Strong");
					hbgMods.Add("[-1291939200]CP_028", "Ignition Mod");
					string[] unreleased = [];
					foreach (string weaponType in wepAbbrevDict.Keys)
					{
						JObject[] weapons = WeaponObjs[weaponType];
						foreach (JObject weaponObj in weapons)
						{
							string weaponName = WeaponNames[weaponType].First(x => x.Value<string>("guid") == weaponObj.Value<string>("_Name")).Value<JArray>("content")![1].Value<string>()!;
							if (!unreleased.Contains(weaponName) && !weaponName.StartsWith("<COLOR FF0000>#Rejected#</COLOR>"))
							{
								string? element = null;
								string elKey = weaponObj.Value<JObject>("_Attribute")!.Value<JObject>("app.WeaponDef.ATTR_Serializable")!.Value<string>("_Value")!;
								if (elKey != "[0]NONE")
								{
									if (elements.ContainsKey(elKey))
									{
										element = elements[elKey];
									}
									else if (temp.Any(x => x.Name == weaponName && !string.IsNullOrEmpty(x.Element)))
									{
										element = temp.First(x => x.Name == weaponName).Element;
										elements.Add(elKey, element!);
									}
									else
									{
										Debugger.Break();
									}
								}
								string rarityStr = weaponObj.Value<JObject>("_Rare")!.Value<JObject>("app.ItemDef.RARE_Serializable")!.Value<string>("_Value")!;
								Weapon newWeapon = new()
								{
									Type = wepAbbrevDict[weaponType],
									Name = weaponName,
									Rarity = Convert.ToInt32(rarityStr!.Substring(rarityStr!.IndexOf("RARE") + 4)) + 1,
									Level4Slots = 0,
									Attack = weaponObj.Value<int>("_Attack"),
									Defense = weaponObj.Value<int>("_Defense"),
									Element = element,
									ElementDamage = element == null ? null : weaponObj.Value<int>("_AttributeValue"),
									Affinity = weaponObj.Value<int>("_Critical"),
									Description = WeaponNames[weaponType].First(x => x.Value<string>("guid") == weaponObj.Value<string>("_Explain")).Value<JArray>("content")![1].Value<string>()!.Replace("\r\n", " ")
								};
								string[] slotLevelArr = weaponObj.Value<JArray>("_SlotLevel")!.Select(x => x.Value<JObject>("app.EquipDef.SlotLevel_Serializable")!.Value<string>("_Value")!).ToArray();
								int[] slots = [
									Convert.ToInt32(string.Join("", slotLevelArr[0].Substring(0, slotLevelArr[0].IndexOf("]")).Where(char.IsDigit))),
								Convert.ToInt32(string.Join("", slotLevelArr[1].Substring(0, slotLevelArr[1].IndexOf("]")).Where(char.IsDigit))),
								Convert.ToInt32(string.Join("", slotLevelArr[2].Substring(0, slotLevelArr[2].IndexOf("]")).Where(char.IsDigit)))
								];
								newWeapon.Level1Slots = slots.Count(x => x == 1);
								newWeapon.Level2Slots = slots.Count(x => x == 2);
								newWeapon.Level3Slots = slots.Count(x => x == 3);
								newWeapon.Level4Slots = slots.Count(x => x == 4); ;
								int weaponId = Convert.ToInt32(string.Join("", weaponObj.Value<string>($"_{weaponType}")!.Substring(0, weaponObj.Value<string>($"_{weaponType}")!.IndexOf("]")).Where(char.IsDigit)));
								newWeapon.WeaponId = weaponId;
								newWeapon.WeaponIdStr = weaponObj.Value<string>($"_{weaponType}")!;
								JObject? weaponTreeEntry = WeaponTrees[weaponType].Value<JArray>("_WeaponTreeList")!.Select(x => x.Value<JObject>("app.user_data.WeaponTree.cWeaponTree")!).FirstOrDefault(x => x.Value<int>("_WeaponID") == weaponId);
								if (weaponTreeEntry != null)
								{
									string seriesId = WeaponTrees[weaponType].Value<JArray>("_RowDataList")!.Select(x => x.Value<JObject>("app.user_data.WeaponTree.cRowData")!).First(x => x.Value<int>("_RowLevel") == weaponTreeEntry.Value<int>("_RowDataLevel")).Value<string>("_Series")!;
									seriesId = seriesId.Substring(seriesId.IndexOf("]") + 1);
									string nameId = WeaponSeriesData.First(x =>
									{
										string thisSeriesId = x.Value<JObject>("app.user_data.WeaponSeriesData.cData")!.Value<JObject>("_Series")!.Value<JObject>("app.WeaponDef.SERIES_Serializable")!.Value<string>("_Value")!;
										thisSeriesId = thisSeriesId.Substring(thisSeriesId.IndexOf("]") + 1);
										return thisSeriesId == seriesId;
									}).Value<JObject>("app.user_data.WeaponSeriesData.cData")!.Value<string>("_Name")!;
									newWeapon.TreeName = WeaponSeriesNames.First(x => x.Value<string>("guid") == nameId).Value<JArray>("content")![1].Value<string>()!;
									JObject recipe = WeaponRecipes[weaponType].First(x => x.Value<string>($"_{weaponType}") == weaponObj.Value<string>($"_{weaponType}"));
									string[] previous = weaponTreeEntry.Value<JArray>("_PreDataGuidList")!.Select(x => x.Value<string>())!.ToArray()!;
									newWeapon.ForgeMaterials = new WeaponCraft()
									{
										Guid = weaponTreeEntry.Value<string>("_Guid")!,
										Cost = weaponObj.Value<int>("_Price"),
									};
									newWeapon.CanShortcut = recipe.Value<bool>("_canShortcut");
									if (previous.Length > 0)
									{
										List<Items> mats = [];
										List<int> matQtys = [];
										int cntr2 = 0;
										int[] qtys = recipe.Value<JArray>("_ItemNum")!.Select(x => x.Value<int>()).ToArray();
										foreach (string itemId in recipe.Value<JArray>("_Item")!.Select(x => x.Value<string>()!).Where(x => x != "[0]INVALID").ToArray())
										{
											try
											{
												mats.Add(MHWildsItems.First(x => x.ItemID == itemId));
												matQtys.Add(qtys[cntr2]);
												cntr2++;
											}
											catch (Exception)
											{
												//Debugger.Break();
											}
										}
										newWeapon.ForgeMaterials.Materials = [.. mats];
										newWeapon.ForgeMaterials.Qtys = [.. matQtys];
									}
								}
								Weapon? excelWep = temp.FirstOrDefault(x => x.Name!.ToUpper().Trim() == newWeapon.Name!.ToUpper().Trim());
								int cntr = 0;
								int[] skillLevels = weaponObj.Value<JArray>("_SkillLevel")!.Select(x => x.Value<int>()).ToArray();
								foreach (JObject skillObj in weaponObj.Value<JArray>("_Skill")!.Cast<JObject>())
								{
									string skill = skillObj.Value<JObject>("app.HunterDef.Skill_Serializable")!.Value<string>("_Value")!;
									if (!string.IsNullOrEmpty(skill) && skill != "[0]NONE")
									{
										if (string.IsNullOrEmpty(newWeapon.Skill1))
										{
											newWeapon.Skill1 = GetSkillLink(skill, false);
											newWeapon.Skill1Level = skillLevels[cntr];
										}
										else if (string.IsNullOrEmpty(newWeapon.Skill2))
										{
											newWeapon.Skill2 = GetSkillLink(skill, false);
											newWeapon.Skill2Level = skillLevels[cntr];
										}
										else if (string.IsNullOrEmpty(newWeapon.Skill3))
										{
											newWeapon.Skill3 = GetSkillLink(skill, false);
											newWeapon.Skill3Level = skillLevels[cntr];
										}
									}
									cntr++;
								}
								JObject[] noteSets = JsonConvert.DeserializeObject<JArray>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Player\ActionData\Wp05\UserData\Wp05MusicSkillToneTable.user.3.json"))!.First().Value<JObject>("app.Wp05MusicSkillToneTable")!.Value<JArray>("_Datas")!.Select(x => x.Value<JObject>("app.Wp05MusicSkillToneTable.cData")!)!.ToArray();
								if (newWeapon.Type == "HH")
								{
									List<char> notesList = [];
									string noteKey = weaponObj.Value<JObject>("_Wp05UniqueType")!.Value<JObject>("app.Wp05Def.UNIQUE_TYPE_Serializable")!.Value<string>("_Value")!;
									noteKey = noteKey.Substring(noteKey.IndexOf("]") + 1);
									JObject noteSet = noteSets.First(x => x.Value<string>("_UniqueType")!.EndsWith(noteKey));
									notesList.AddRange([
										noteSet.Value<string>("_ToneColor1")!.Substring(noteSet.Value<string>("_ToneColor1")!.IndexOf("]") + 1, 1)[0],
								noteSet.Value<string>("_ToneColor2")!.Substring(noteSet.Value<string>("_ToneColor2")!.IndexOf("]") + 1, 1)[0],
								noteSet.Value<string>("_ToneColor3")!.Substring(noteSet.Value<string>("_ToneColor3")!.IndexOf("]") + 1, 1)[0],
							]);
									newWeapon.HHNote1 = GetHHNote(notesList[0]);
									newWeapon.HHNote2 = GetHHNote(notesList[1]);
									newWeapon.HHNote3 = GetHHNote(notesList[2]);
									string specialMelodyKey = weaponObj.Value<JObject>("_Wp07ShellType")!.Value<JObject>("app.Wp07Def.SHELL_TYPE_Serializable")!.Value<string>("_Value")!;
									if (hhSpecialMelodies.ContainsKey(specialMelodyKey))
									{
										newWeapon.HHSpecialMelody = hhSpecialMelodies[specialMelodyKey];
									}
									else if (excelWep != null)
									{
										string specialMelody = excelWep.HHSpecialMelody!;
										hhSpecialMelodies.Add(specialMelodyKey, specialMelody);
										newWeapon.HHSpecialMelody = specialMelody;
									}
									else
									{
										hhSpecialMelodies_checkLater.Add(newWeapon.Name, specialMelodyKey);
									}
									string echoBubbleKey = weaponObj.Value<JObject>("_Wp07ShellType")!.Value<JObject>("app.Wp07Def.SHELL_TYPE_Serializable")!.Value<string>("_Value")!;
									if (hhEchoBubbles.ContainsKey(echoBubbleKey))
									{
										newWeapon.HHEchoBubble = hhEchoBubbles[echoBubbleKey];
									}
									else if (excelWep != null)
									{
										string echoBubble = excelWep.HHEchoBubble!;
										hhEchoBubbles.Add(echoBubbleKey, echoBubble);
										newWeapon.HHEchoBubble = echoBubble;
									}
									else
									{
										hhEchoBubbles_checkLater.Add(newWeapon.Name, echoBubbleKey);
									}
								}
								else if (newWeapon.Type == "GL")
								{
									string shellTypeKey = weaponObj.Value<JObject>("_Wp07ShellType")!.Value<JObject>("app.Wp07Def.SHELL_TYPE_Serializable")!.Value<string>("_Value")!;
									if (glShellTypes.ContainsKey(shellTypeKey))
									{
										newWeapon.GLShellingType = glShellTypes[shellTypeKey];
									}
									else if (excelWep != null)
									{
										string shellType = excelWep.GLShellingType!;
										glShellTypes.Add(shellTypeKey, shellType);
										newWeapon.GLShellingType = shellType;
									}
									else
									{
										glShellTypes_checkLater.Add(newWeapon.Name, shellTypeKey);
									}
									string shellLevelKey = weaponObj.Value<JObject>("_Wp07ShellLv")!.Value<JObject>("app.Wp07ShellLevel.SHELL_LV_Serializable")!.Value<string>("_Value")!;
									if (glShellLevels.ContainsKey(shellLevelKey))
									{
										newWeapon.GLShellingPower = glShellLevels[shellLevelKey];
									}
									else if (excelWep != null)
									{
										string shellType = excelWep.GLShellingPower!;
										glShellLevels.Add(shellLevelKey, shellType);
										newWeapon.GLShellingPower = shellType;
									}
									else
									{
										glShellLevels_checkLater.Add(newWeapon.Name, shellLevelKey);
									}

								}
								else if (newWeapon.Type == "SA")
								{
									string phialTypeKey = weaponObj.Value<string>("_Wp08BinType")!;
									if (saPhialType.ContainsKey(phialTypeKey))
									{
										newWeapon.SAPhialType = saPhialType[phialTypeKey];
									}
									else if (excelWep != null)
									{
										string phialType = excelWep.SAPhialType!;
										saPhialType.Add(phialTypeKey, phialType);
										newWeapon.SAPhialType = phialType;
									}
									else
									{
										saPhialType_checkLater.Add(newWeapon.Name, phialTypeKey);
									}
									newWeapon.SAPhialDamage = weaponObj.Value<int>("_Wp08BinValue");
								}
								else if (newWeapon.Type == "CB")
								{
									string phialTypeKey = weaponObj.Value<string>("_Wp09BinType")!;
									if (cbPhialType.ContainsKey(phialTypeKey))
									{
										newWeapon.CBPhialType = cbPhialType[phialTypeKey];
									}
									else if (excelWep != null)
									{
										string phialType = excelWep.CBPhialType!;
										cbPhialType.Add(phialTypeKey, phialType);
										newWeapon.CBPhialType = phialType;
									}
									else
									{
										cbPhialType_checkLater.Add(newWeapon.Name, phialTypeKey);
									}
								}
								else if (newWeapon.Type == "IG")
								{
									string igLevelKey = weaponObj.Value<JObject>("_RodInsectLv")!.Value<JObject>("app.WeaponDef.ROD_INSECT_LV_Serializable")!.Value<string>("_Value")!;
									if (igLevels.ContainsKey(igLevelKey))
									{
										newWeapon.IGKinsectLevel = igLevels[igLevelKey];
									}
									else if (excelWep != null)
									{
										int igLevel = excelWep.IGKinsectLevel!.Value;
										igLevels.Add(igLevelKey, igLevel);
										newWeapon.IGKinsectLevel = igLevel;
									}
									else
									{
										igLevels_checkLater.Add(newWeapon.Name, igLevelKey);
									}
								}
								else if (newWeapon.Type == "Bo")
								{
									string[] order = ["Close-range", "Power", "Pierce", "Poison", "Paralysis", "Sleep", "Blast", "Exhaust"];
									newWeapon.BoCoatings = "";
									bool[] coatingVals = weaponObj.Value<JArray>("_isLoadingBin")!.Select(x => x.Value<bool>()).ToArray();
									for (int i = 0; i < 8; i++)
									{
										if (coatingVals[i])
										{
											if (!string.IsNullOrEmpty(newWeapon.BoCoatings))
											{
												newWeapon.BoCoatings += ", ";
											}
											newWeapon.BoCoatings += order[i];
										}
									}
								}
								if (!GunnerWeapons.Contains(weaponType))
								{
									if (newWeapon.Name == null)
									{
										Debugger.Break();
									}
									string wepName = wepNameDict[newWeapon.Type];
									string thisWepNameGuid = "";
									try
									{
										thisWepNameGuid = WeaponNames[wepName].First(x => x.Value<JArray>("content")![1].Value<string>() == newWeapon.Name).Value<string>("guid")!;
									}
									catch (Exception)
									{
										Debugger.Break();
									}
									JObject wepObj = WeaponObjs[wepName].First(x => x.Value<string>("_Name") == thisWepNameGuid);
									int[] sharpness = [.. wepObj.Value<JArray>("_SharpnessValList")!.Select(x => x.Value<int>())];
									int[] handiValList = [.. wepObj.Value<JArray>("_TakumiValList")!.Select(x => x.Value<int>())];
									int[] handi = [0, 0, 0, 0, 0, 0, 0];
									int lastPositionWithData = 0;
									for (int i = 0; i < 7; i++)
									{
										if (sharpness[i] > 0)
										{
											lastPositionWithData = i;
										}
									}
									if (handiValList[0] > 0)
									{
										handi[lastPositionWithData] += (int)handiValList[0];
									}
									if (handiValList[1] > 0)
									{
										handi[lastPositionWithData + 1] += (int)handiValList[1];
									}
									if (handiValList[2] > 0)
									{
										handi[lastPositionWithData + 2] += (int)handiValList[2];
									}
									if (handiValList[3] > 0)
									{
										handi[lastPositionWithData + 3] += (int)handiValList[3];
									}
									newWeapon.Sharpness = $"[{JsonConvert.SerializeObject(sharpness)},{JsonConvert.SerializeObject(handi)}]";
								}
								if (newWeapon.Type == "LBG")
								{
									string mod1Key = weaponObj.Value<JArray>("_CustomizePattern")![0]!.Value<JObject>("app.CustomizePatternID.ID_Serializable")!.Value<string>("_Value")!;
									string mod2Key = weaponObj.Value<JArray>("_CustomizePattern")![1]!.Value<JObject>("app.CustomizePatternID.ID_Serializable")!.Value<string>("_Value")!;
									if (lbgMods.ContainsKey(mod1Key))
									{
										newWeapon.LBGDefaultMod1 = lbgMods[mod1Key];
									}
									else if (excelWep != null)
									{
										string mod = excelWep.LBGDefaultMod1!;
										lbgMods.Add(mod1Key, mod);
										newWeapon.LBGDefaultMod1 = mod;
									}
									else
									{
										lbgMods1_checkLater.Add(newWeapon.Name!, mod1Key);
									}
									if (lbgMods.ContainsKey(mod2Key))
									{
										newWeapon.LBGDefaultMod2 = lbgMods[mod2Key];
									}
									else if (excelWep != null)
									{
										string mod = excelWep.LBGDefaultMod2!;
										lbgMods.Add(mod2Key, mod);
										newWeapon.LBGDefaultMod2 = mod;
									}
									else
									{
										lbgMods2_checkLater.Add(newWeapon.Name!, mod2Key);
									}
									string specialAmmoKey = weaponObj.Value<JObject>("_Wp13SpecialAmmo")!.Value<JObject>("app.Wp13Def.SPECIAL_AMMO_TYPE_Serializable")!.Value<string>("_Value")!;
									if (lbgSpecialAmmo.ContainsKey(specialAmmoKey))
									{
										newWeapon.LBGDefaultSpecialAmmoType = lbgSpecialAmmo[specialAmmoKey];
									}
									else if (excelWep != null)
									{
										string mod = excelWep.LBGDefaultSpecialAmmoType!;
										lbgSpecialAmmo.Add(specialAmmoKey, mod);
										newWeapon.LBGDefaultSpecialAmmoType = mod;
									}
									else
									{
										lbgSpecialAmmo_checkLater.Add(newWeapon.Name!, specialAmmoKey);
									}
									string[] shellLevels = weaponObj.Value<JArray>("_ShellLv")!.Select(x => x.Value<string>()!).ToArray();
									int[] shellCaps = weaponObj.Value<JArray>("_ShellNum")!.Select(x => x.Value<int>()!).ToArray();
									bool[] shellRapids = weaponObj.Value<JArray>("_IsRappid")!.Select(x => x.Value<bool>()!).ToArray();
									newWeapon.ShellTable = [..AmmoTypes.Select((x, y) => new WildsShellTable()
							{
								Name = AmmoTypes[y],
								Capacity = shellCaps[y],
								Level = shellLevels[y].EndsWith('2') ? 3 : shellLevels[y].EndsWith('1') ? 2 : 1,
								IsRapid = shellRapids[y]
							})];
								}
								else if (newWeapon.Type == "HBG")
								{
									string mod1Key = weaponObj.Value<JArray>("_CustomizePattern")![0]!.Value<JObject>("app.CustomizePatternID.ID_Serializable")!.Value<string>("_Value")!;
									string mod2Key = weaponObj.Value<JArray>("_CustomizePattern")![1]!.Value<JObject>("app.CustomizePatternID.ID_Serializable")!.Value<string>("_Value")!;
									if (hbgMods.ContainsKey(mod1Key))
									{
										newWeapon.HBGDefaultMod1 = hbgMods[mod1Key];
									}
									else if (excelWep != null)
									{
										string mod = excelWep.HBGDefaultMod1!;
										hbgMods.Add(mod1Key, mod);
										newWeapon.HBGDefaultMod1 = mod;
									}
									else
									{
										hbgMods1_checkLater.Add(newWeapon.Name!, mod1Key);
									}
									if (hbgMods.ContainsKey(mod2Key))
									{
										newWeapon.HBGDefaultMod2 = hbgMods[mod2Key];
									}
									else if (excelWep != null)
									{
										string mod = excelWep.HBGDefaultMod2!;
										hbgMods.Add(mod2Key, mod);
										newWeapon.HBGDefaultMod2 = mod;
									}
									else
									{
										hbgMods2_checkLater.Add(newWeapon.Name!, mod2Key);
									}
									newWeapon.HBGDefaultSpecialAmmoType1 = weaponObj.Value<JObject>("_EnergyShellTypeNormal")!.Value<JObject>("app.BowgunEnergyShellType.NORMAL_Serializable")!.Value<string>("_Value") == "[1372910720]ESN_000" ? "Wyvernheart Ignition" : "Wyvernpiercer Ignition";
									newWeapon.HBGDefaultSpecialAmmoType2 = weaponObj.Value<JObject>("_EnergyShellTypePower")!.Value<JObject>("app.BowgunEnergyShellType.POWER_Serializable")!.Value<string>("_Value") == "[62024620]ESP_000" ? "Wyverncounter Ignition" : "Wyvernblast Ignition";
									string ignitionGauge = weaponObj.Value<JObject>("_EnergyEfficiency")!.Value<JObject>("app.EnergyEfficiencyType.TYPE_Serializable")!.Value<string>("_Value")!;
									newWeapon.HBGIgnitionGauge = ignitionGauge.EndsWith("EET_001") ? "2" : "1";
									string ignitionType = weaponObj.Value<JObject>("_AmmoStrength")!.Value<JObject>("app.AmmoStrengthType.TYPE_Serializable")!.Value<string>("_Value")!;
									newWeapon.HBGStandardIgnitionType = ignitionGauge.EndsWith("AST_001") ? "Standard Mode ST II" : "Standard/Ignition Base Type";
									string[] shellLevels = weaponObj.Value<JArray>("_ShellLv")!.Select(x => x.Value<string>()!).ToArray();
									int[] shellCaps = weaponObj.Value<JArray>("_ShellNum")!.Select(x => x.Value<int>()!).ToArray();
									bool[] shellRapids = weaponObj.Value<JArray>("_IsRappid")!.Select(x => x.Value<bool>()!).ToArray();
									newWeapon.ShellTable = [..AmmoTypes.Select((x, y) => new WildsShellTable()
							{
								Name = AmmoTypes[y],
								Capacity = shellCaps[y],
								Level = shellLevels[y].EndsWith('2') ? 3 : shellLevels[y].EndsWith('1') ? 2 : 1,
								IsRapid = shellRapids[y]
							})];
								}
								src.Add(newWeapon);
							}
						}
					}
					foreach (Weapon weapon in src)
					{
						string weaponType = wepNameDict[weapon.Type!];
						JObject? weaponTreeEntry = WeaponTrees[weaponType].Value<JArray>("_WeaponTreeList")!.Select(x => x.Value<JObject>("app.user_data.WeaponTree.cWeaponTree")!).FirstOrDefault(x => x.Value<int>("_WeaponID") == weapon.WeaponId);
						if (weaponTreeEntry != null)
						{
							string seriesId = WeaponTrees[weaponType].Value<JArray>("_RowDataList")!.Select(x => x.Value<JObject>("app.user_data.WeaponTree.cRowData")!).First(x => x.Value<int>("_RowLevel") == weaponTreeEntry.Value<int>("_RowDataLevel")).Value<string>("_Series")!;
							JObject recipe = WeaponRecipes[weaponType].First(x => x.Value<string>($"_{weaponType}") == weapon.WeaponIdStr);
							if (weapon.ForgeMaterials != null && weaponTreeEntry.Value<JArray>("_PreDataGuidList")!.Count > 0)
							{
								try
								{
									weapon.ForgeMaterials.Parent = src.First(x => x.ForgeMaterials != null && x.ForgeMaterials.Guid == weaponTreeEntry.Value<JArray>("_PreDataGuidList")!.First().Value<string>());
								}
								catch (Exception)
								{
									Debugger.Break();
								}
							}
							List<WeaponCraft> upgrades = [];
							foreach (string upgradeId in weaponTreeEntry.Value<JArray>("_NextDataGuidList")!.Select(x => x.Value<string>()!).ToArray())
							{
								try
								{
									Weapon child = src.First(x => x.ForgeMaterials.Guid == upgradeId);
									upgrades.Add(new()
									{
										Name = child.Name!,
										Rarity = child.Rarity,
										Guid = child.ForgeMaterials.Guid,
										Cost = child.ForgeMaterials.Cost,
										Materials = child.ForgeMaterials.Materials,
										Qtys = child.ForgeMaterials.Qtys
									});
								}
								catch (Exception)
								{
									Debugger.Break();
								}
							}
							weapon.Upgrades = [.. upgrades];
						}
						if (weapon.Type == "HH")
						{
							if (hhSpecialMelodies_checkLater.ContainsKey(weapon.Name!))
							{
								string specialMelodyKey = hhSpecialMelodies_checkLater[weapon.Name!];
								if (hhSpecialMelodies.ContainsKey(specialMelodyKey))
								{
									weapon.HHSpecialMelody = hhSpecialMelodies[specialMelodyKey];
								}
								else
								{
									Debugger.Break();
								}
							}
							if (hhEchoBubbles_checkLater.ContainsKey(weapon.Name!))
							{
								string echoBubbleKey = hhEchoBubbles_checkLater[weapon.Name!];
								if (hhEchoBubbles.ContainsKey(echoBubbleKey))
								{
									weapon.HHEchoBubble = hhSpecialMelodies[echoBubbleKey];
								}
								else
								{
									Debugger.Break();
								}
							}
						}
						else if (weapon.Type == "GL")
						{
							if (glShellTypes_checkLater.ContainsKey(weapon.Name!))
							{
								string shellTypeKey = glShellTypes_checkLater[weapon.Name!];
								if (glShellTypes.ContainsKey(shellTypeKey))
								{
									weapon.GLShellingType = glShellTypes[shellTypeKey];
								}
								else
								{
									Debugger.Break();
								}
							}
							if (glShellLevels_checkLater.ContainsKey(weapon.Name!))
							{
								string shellLevelKey = glShellLevels_checkLater[weapon.Name!];
								if (glShellLevels.ContainsKey(shellLevelKey))
								{
									weapon.GLShellingPower = glShellLevels[shellLevelKey];
								}
								else
								{
									Debugger.Break();
								}
							}
						}
						else if (weapon.Type == "SA")
						{
							if (saPhialType_checkLater.ContainsKey(weapon.Name!))
							{
								string phialTypeKey = saPhialType_checkLater[weapon.Name!];
								if (saPhialType.ContainsKey(phialTypeKey))
								{
									weapon.SAPhialType = saPhialType[phialTypeKey];
								}
								else
								{
									Debugger.Break();
								}
							}
						}
						else if (weapon.Type == "CB")
						{
							if (cbPhialType_checkLater.ContainsKey(weapon.Name!))
							{
								string Key = cbPhialType_checkLater[weapon.Name!];
								if (cbPhialType.ContainsKey(Key))
								{
									weapon.CBPhialType = cbPhialType[Key];
								}
								else
								{
									Debugger.Break();
								}
							}
						}
						else if (weapon.Type == "IG")
						{
							if (igLevels_checkLater.ContainsKey(weapon.Name!))
							{
								string Key = igLevels_checkLater[weapon.Name!];
								if (igLevels.ContainsKey(Key))
								{
									weapon.IGKinsectLevel = igLevels[Key];
								}
								else
								{
									Debugger.Break();
								}
							}
						}
						if (weapon.Type == "LBG")
						{
							if (lbgMods1_checkLater.ContainsKey(weapon.Name!))
							{
								string Key = lbgMods1_checkLater[weapon.Name!];
								if (lbgMods.ContainsKey(Key))
								{
									weapon.LBGDefaultMod1 = lbgMods[Key];
								}
								else
								{
									Debugger.Break();
								}
							}
							if (lbgMods2_checkLater.ContainsKey(weapon.Name!))
							{
								string Key = lbgMods2_checkLater[weapon.Name!];
								if (lbgMods.ContainsKey(Key))
								{
									weapon.LBGDefaultMod2 = lbgMods[Key];
								}
								else
								{
									Debugger.Break();
								}
							}
							if (lbgSpecialAmmo_checkLater.ContainsKey(weapon.Name!))
							{
								string Key = lbgSpecialAmmo_checkLater[weapon.Name!];
								if (lbgSpecialAmmo.ContainsKey(Key))
								{
									weapon.LBGDefaultSpecialAmmoType = lbgSpecialAmmo[Key];
								}
								else
								{
									Debugger.Break();
								}
							}
						}
						else if (weapon.Type == "HBG")
						{
							if (hbgMods1_checkLater.ContainsKey(weapon.Name!))
							{
								string Key = hbgMods1_checkLater[weapon.Name!];
								if (hbgMods.ContainsKey(Key))
								{
									weapon.HBGDefaultMod1 = hbgMods[Key];
								}
								else
								{
									Debugger.Break();
								}
							}
							if (hbgMods2_checkLater.ContainsKey(weapon.Name!))
							{
								string Key = hbgMods2_checkLater[weapon.Name!];
								if (hbgMods.ContainsKey(Key))
								{
									weapon.HBGDefaultMod2 = hbgMods[Key];
								}
								else
								{
									Debugger.Break();
								}
							}
						}
					}
					string[] artianNames = ["Artian Blade I", "Artian Blade II", "Varianza", "Artian Saber I", "Artian Saber II", "Dimensius", "Artian Sword I", "Artian Sword II", "Verdoloto", "Artian Edges I", "Artian Edges II", "Tiltkreise", "Artian Hammer I", "Artian Hammer II", "Moteurvankel", "Artian Sounder I", "Artian Sounder II", "Omiltika", "Artian Lance I", "Artian Lance II", "Skyscraper", "Artian Cannon I", "Artian Cannon II", "Argenesis", "Artian Saw I", "Artian Saw II", "Mundus Altus", "Artian Defender I", "Artian Defender II", "Chrono Gear", "Artian Glaive I", "Artian Glaive II", "Diprielcha", "Artian Blaster I", "Artian Blaster II", "Animilater", "Artian Sheller I", "Artian Sheller II", "Greifen", "Artian Sight I", "Artian Sight II", "Angelbein"];
					foreach (Weapon weapon in src.Where(x => artianNames.Contains(x.Name) || (x.ForgeMaterials?.Parent == null && x.Upgrades.Length == 0)))
					{
						weapon.TreeName = "Artian";
						switch (weapon.Rarity)
						{
							case 6:
							case 7:
								weapon.Level1Slots = 0;
								weapon.Level2Slots = 3;
								weapon.Level3Slots = 0;
								weapon.Level4Slots = 0;
								break;
							case 8:
								weapon.Level1Slots = 0;
								weapon.Level2Slots = 0;
								weapon.Level3Slots = 3;
								weapon.Level4Slots = 0;
								break;
						}
					}
				}
				return [.. src];
			}
			catch (Exception)
            {
				Debugger.Break();
				return [];
			}
		}

		private static string GetSkillLink(string skillName, bool isOldMethod = true)
		{
			try
			{
				if (isOldMethod)
				{
					dynamic skill = SkillDict.First(x => x.name.ToString().ToUpper().Trim() == skillName.ToUpper().Trim());
					return $"{{{{MHWildsSkillLink|{skill.name.ToString()}|{skill._SkillIconType.ToString()}}}}}";
				}
				else
				{
					Skill skill = Skills.First(x => x.SkillId == skillName);
					return $"{{{{MHWildsSkillLink|{skill.SkillName}|{skill.SkillIconType}}}}}";
				}
			}
			catch (Exception)
			{
				Debugger.Break();
				return "";
			}
		}

		public static WebToolkitData[] GetWebToolkitData(bool onlyNames = false)
		{
			Dictionary<string, dynamic> weaponTreeData = [];
			Dictionary<string, dynamic[]> weaponData = [];
			Dictionary<string, dynamic[]> weaponNames = [];
			Weapon[] src = GetAllWeapons(onlyNames);
			return [.. src.Select(x => onlyNames ? new WebToolkitData() { Type = x.Type, Name = x.Name } : Parse(x, weaponTreeData, weaponData, weaponNames, src))];
		}

		private static WebToolkitData Parse(Weapon weapon, Dictionary<string, dynamic> weaponTreeData, Dictionary<string, dynamic[]> weaponData, Dictionary<string, dynamic[]> weaponNames, Weapon[] src)
		{
			WebToolkitData? data = null;
			try
			{
				string weaponType = GetWeaponName(weapon.Type!);
				if (!weaponTreeData.ContainsKey(weaponType))
				{
					weaponTreeData.Add(weaponType, JsonConvert.DeserializeObject<dynamic>(Utilities.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Weapon\{GetWeaponInternalName(weapon.Type!)}Tree.user.3.json"))![0]["app.user_data.WeaponTree"]);
					weaponData.Add(weaponType, ((JArray)JsonConvert.DeserializeObject<dynamic[]>(Utilities.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Weapon\{GetWeaponInternalName(weapon.Type!)}.user.3.json"))![0]["app.user_data.WeaponData"]._Values).ToObject<dynamic[]>()!);
					weaponNames.Add(weaponType, ((JArray)JsonConvert.DeserializeObject<dynamic>(Utilities.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\{GetWeaponInternalName(weapon.Type!)}.msg.23.json"))!.entries).ToObject<dynamic[]>()!);
				}
				string weaponGuid = weaponNames[weaponType].First(x => x.content[1].ToString() == weapon.Name).guid.ToString();
				dynamic wepData = weaponData[weaponType].First(x => x["app.user_data.WeaponData.cData"]._Name.ToString() == weaponGuid);
				string armorSkills = "";
				if (!string.IsNullOrEmpty(weapon.Skill1))
				{
					if (armorSkills != "")
					{
						armorSkills += ", ";
					}
					armorSkills += weapon.Skill1 + " lv. " + weapon.Skill1Level;
				}
				if (!string.IsNullOrEmpty(weapon.Skill2))
				{
					if (armorSkills != "")
					{
						armorSkills += ", ";
					}
					armorSkills += weapon.Skill2 + " lv. " + weapon.Skill2Level;
				}
				if (!string.IsNullOrEmpty(weapon.Skill3))
				{
					if (armorSkills != "")
					{
						armorSkills += ", ";
					}
					armorSkills += weapon.Skill3 + " lv. " + weapon.Skill3Level;
				}
				string treeName = weapon.TreeName == "???" ? "Artian" : weapon.TreeName;
				bool canForge = weapon.CanShortcut || (weapon.ForgeMaterials.Parent == null && treeName != "Artian");
				data = new()
				{
					Affinity = weapon.Affinity,
					ArmorSkills = armorSkills,
					Attack = weapon.Attack.ToString(),
					HbgSpecialAmmoType1 = weapon.HBGDefaultSpecialAmmoType1,
					HbgSpecialAmmoType2 = weapon.HBGDefaultSpecialAmmoType2,
					HbgDefaultMod1 = weapon.HBGDefaultMod1,
					HbgDefaultMod2 = weapon.HBGDefaultMod2,
					HbgIgnitionGauge = weapon.HBGIgnitionGauge,
					HbgStandardIgnitionType = weapon.HBGStandardIgnitionType,
					LbgDefaultMod1 = weapon.LBGDefaultMod1,
					LbgDefaultMod2 = weapon.LBGDefaultMod2,
					LbgSpecialAmmoType = weapon.LBGDefaultSpecialAmmoType,
					BoCoatings = weapon.BoCoatings,
					CbPhialType = weapon.CBPhialType,
					GlShellingType = weapon.GLShellingType,
					GlShellingLevel = weapon.GLShellingPower,
					Decos1 = weapon.Level1Slots,
					Decos2 = weapon.Level2Slots,
					Decos3 = weapon.Level3Slots,
					Decos4 = weapon.Level4Slots,
					Defense = weapon.Defense == null ? "-" : weapon.Defense!.ToString()!,
					Description = weapon.Description,
					Element1 = weapon.Element!,
					ElementDmg1 = weapon.ElementDamage.ToString()!,
					ForgeCost = canForge ? weapon.ForgeMaterials?.Cost * (weapon.CanShortcut ? 2 : 1) ?? null : null,
					ForgeMaterials = canForge ? weapon.ForgeMaterials != null ? GetMaterials(weapon.ForgeMaterials, weapon.CanShortcut ? 2 : 1) : "" : "",
					UpgradeCost = weapon.ForgeMaterials?.Cost ?? null,
					UpgradeMaterials = weapon.ForgeMaterials != null ? GetMaterials(weapon.ForgeMaterials) : null,
					Game = "MHWilds",
					HhEchoBubble = weapon.HHEchoBubble,
					HhNote1 = weapon.HHNote1,
					HhNote2 = weapon.HHNote2,
					HhNote3 = weapon.HHNote3,
					HhSpecialMelody = weapon.HHSpecialMelody,
					IgKinsectBonus = "Lv. " + weapon.IGKinsectLevel,
					Name = weapon.Name,
					Rarity = weapon.Rarity,
					SaPhialDamage = weapon.SAPhialDamage!.ToString(),
					SaPhialType = weapon.SAPhialType,
					Sharpness = weapon.Sharpness,
					ShellTableWikitext = GetShellTableWikitext(weapon),
					Type = weapon.Type,
					Tree = treeName
				};
				data.Attack = ((int)wepData["app.user_data.WeaponData.cData"]._Attack).ToString();
				data.Description = weaponNames[weaponType].First(x => x.guid == wepData["app.user_data.WeaponData.cData"]._Explain).content[1].ToString().Replace("\r\n", " ");
				if (data.Type == "HH")
				{
					data.HighFreqEnum = (string)wepData["app.user_data.WeaponData.cData"]["_Wp05MusicSkillHighFreqType"]["app.Wp05Def.WP05_MUSIC_SKILL_HIGH_FREQ_TYPE_Serializable"]._Value;
				}
				if (!string.IsNullOrEmpty(data.Element1))
				{
					data.ElementDmg1 = ((int)wepData["app.user_data.WeaponData.cData"]._AttributeValue).ToString();
				}
				if (weapon.ForgeMaterials != null)
				{
					data.PreviousName = weapon.ForgeMaterials.Parent?.Name;
					data.PreviousRarity = weapon.ForgeMaterials.Parent?.Rarity ?? 1;
				}
				foreach (WeaponCraft upgrade in weapon.Upgrades)
				{
					if (string.IsNullOrEmpty(data.Next1Name))
					{
						data.Next1Name = upgrade.Name;
						data.Next1Rarity = upgrade.Rarity;
						data.Next1Cost = upgrade.Cost;
						data.Next1Materials = GetMaterials(upgrade);
					}
					else if (string.IsNullOrEmpty(data.Next2Name))
					{
						data.Next2Name = upgrade.Name;
						data.Next2Rarity = upgrade.Rarity;
						data.Next2Cost = upgrade.Cost;
						data.Next2Materials = GetMaterials(upgrade);
					}
					else if (string.IsNullOrEmpty(data.Next3Name))
					{
						data.Next3Name = upgrade.Name;
						data.Next3Rarity = upgrade.Rarity;
						data.Next3Cost = upgrade.Cost;
						data.Next3Materials = GetMaterials(upgrade);
					}
					else if (string.IsNullOrEmpty(data.Next4Name))
					{
						data.Next4Name = upgrade.Name;
						data.Next4Rarity = upgrade.Rarity;
						data.Next4Cost = upgrade.Cost;
						data.Next4Materials = GetMaterials(upgrade);
					}
				}
			}
			catch (Exception)
			{
				Debugger.Break();
			}
			return data!;
		}

		private static string GetHHNote(char key)
		{
			return new Dictionary<char, string>()
			{
				{ 'P', "Purple" },
				{ 'R', "Red" },
				{ 'Y', "Yellow" },
				{ 'W', "White" },
				{ 'B', "Blue" },
				{ 'G', "Green" },
				{ 'O', "Orange" },
				{ 'C', "Sky" },
				{ 'S', "Sky" },
			}[key];
		}

		private static string GetWeaponType(string key)
		{
			return new Dictionary<string, string>()
			{
				{ "Charge Blade", "CB" },
				{ "Dual Blades", "DB" },
				{ "Great Sword", "GS" },
				{ "Gunlance", "GL" },
				{ "Hammer", "Hm" },
				{ "Hunting Horn", "HH" },
				{ "Insect Glaive", "IG" },
				{ "Lance", "Ln" },
				{ "Long Sword", "LS" },
				{ "Switch Axe", "SA" },
				{ "Sword & Shield", "SnS" },
				{ "Bow", "Bo" },
				{ "Heavy Bowgun", "HBG" },
				{ "Light Bowgun", "LBG" }
			}[key];
		}

		public static string GetWeaponName(string key)
		{
			return new Dictionary<string, string>()
			{
				{ "CB", "Charge Blade" },
				{ "DB", "Dual Blades" },
				{ "GS", "Great Sword" },
				{ "GL", "Gunlance" },
				{ "Hm", "Hammer" },
				{ "HH", "Hunting Horn" },
				{ "IG", "Insect Glaive" },
				{ "Ln", "Lance" },
				{ "LS", "Long Sword" },
				{ "SA", "Switch Axe" },
				{ "SnS", "Sword and Shield" },
				{ "Bo", "Bow" },
				{ "HBG", "Heavy Bowgun" },
				{ "LBG", "Light Bowgun" }
			}[key];
		}

		private static string GetWeaponInternalName(string key)
		{
			return new Dictionary<string, string>()
			{
				{ "CB", "ChargeAxe" },
				{ "DB", "TwinSword" },
				{ "GS", "LongSword" },
				{ "GL", "GunLance" },
				{ "Hm", "Hammer" },
				{ "HH", "Whistle" },
				{ "IG", "Rod" },
				{ "Ln", "Lance" },
				{ "LS", "Tachi" },
				{ "SA", "SlashAxe" },
				{ "SnS", "ShortSword" },
				{ "Bo", "Bow" },
				{ "HBG", "HeavyBowgun" },
				{ "LBG", "LightBowgun" }
			}[key];
		}

		private static string GetShellTableWikitext(Weapon weapon)
		{
			if (weapon.ShellTable == null)
			{
				return string.Empty;
			}
			StringBuilder ret = new();
			foreach (WildsShellTable shell in weapon.ShellTable!)
			{
				try
				{
					Items shellItem = MHWildsItems.First(x => x.Name!.ToUpper().Trim() == shell.Name!.ToUpper().Trim());
					ret.AppendLine("<tr>");
					ret.AppendLine($"<td>{{{{IconPickerUniversalAlt|MHWilds|{shellItem.Icon}|{shellItem.Name}|Color={Generators.Weapon.GetColorString(shellItem.IconColor)}}}}}</td>");
					ret.AppendLine($"<td class=\"ammo-level ammo-{shellItem.Name.Replace(" Bomb", "").Replace(" Ammo", "").ToLower()}\"> {(shell.Capacity <= 0 ? " -" : shell.Level)}</td>");
					ret.AppendLine($"<td class=\"ammo-capacity ammo-{shellItem.Name.Replace(" Bomb", "").Replace(" Ammo", "").ToLower()}\" > {(shell.Capacity <= 0 ? " -" : shell.Capacity)}</td>");
					if (weapon.Type == "LBG")
					{
						ret.AppendLine($"<td>{(shell.IsRapid && shell.Capacity > 0 ? $" {{{{UI|MHRS|{weapon.Type} Rapid Fire|size=24x24px|nolink=true|title=Rapid Fire}}}}" : " -")}</td>");
					}
					ret.AppendLine("</tr>");
				}
				catch (Exception)
				{
					Debugger.Break();
				}
			}
			return ret.ToString();
		}

		private static string GetMaterials(WeaponCraft craft, int matMult = 1)
		{
			string upgradeFromMats = "[";
			int cnt = 0;
			foreach (Items mat in craft.Materials)
			{
				int qty = craft.Qtys[cnt] * matMult;
				try
				{
					if (upgradeFromMats != "[")
					{
						upgradeFromMats += ", ";
					}
					upgradeFromMats += $"{{\"name\": \"{mat.Name}\", \"icon\": \"{mat.Icon}\", \"color\": \"{mat.IconColor}\", \"quantity\": {qty}}}";
				}
				catch (Exception)
				{
					Debugger.Break();
				}
				cnt++;
			}
			return upgradeFromMats + "]";
		}
	}

	public class WeaponUtils
	{
		public static readonly string[] AmmoTypes = ["Normal Ammo", "Pierce Ammo", "Spread Ammo", "Sticky Ammo", "Cluster Bomb", "Slicing Ammo", "Wyvern Ammo", "Flaming Ammo", "Water Ammo", "Thunder Ammo", "Freeze Ammo", "Dragon Ammo", "Poison Ammo", "Paralysis Ammo", "Sleep Ammo", "Demon Ammo", "Armor Ammo", "Recover Ammo", "Exhaust Ammo", "Tranq Ammo"];
		public static readonly string[] GunnerWeapons = ["Bow", "LightBowgun", "HeavyBowgun"];
		public static readonly dynamic[] HeavyBowgunNames = ((JArray)JsonConvert.DeserializeObject<dynamic>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\HeavyBowgun.msg.23.json"))!.entries!).ToObject<dynamic[]>()!;
		public static readonly dynamic[] HeavyBowguns = ((JArray)JsonConvert.DeserializeObject<JArray>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Weapon\HeavyBowgun.user.3.json"))![0].ToObject<dynamic>()!["app.user_data.WeaponData"]._Values).ToObject<dynamic[]>()!;
		public static readonly dynamic[] LightBowgunNames = ((JArray)JsonConvert.DeserializeObject<dynamic>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\LightBowgun.msg.23.json"))!.entries!).ToObject<dynamic[]>()!;
		public static readonly dynamic[] LightBowguns = ((JArray)JsonConvert.DeserializeObject<JArray>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Weapon\LightBowgun.user.3.json"))![0].ToObject<dynamic>()!["app.user_data.WeaponData"]._Values).ToObject<dynamic[]>()!;
		public static readonly Items[] MHWildsItems = Items.Fetch();
		public static readonly Dictionary<string, string>[] SeriesNameIdPairs = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\seriesNamePairs.json"))!;
		public static readonly dynamic[] SeriesNames = ((JArray)JsonConvert.DeserializeObject<dynamic>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\WeaponSeries.msg.23.json"))!.entries!).ToObject<dynamic[]>()!;
		public static readonly dynamic SharpnessVals = JsonConvert.DeserializeObject<dynamic>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\weaponSharpness.json"))!;
		public static readonly dynamic[] SkillDict = JsonConvert.DeserializeObject<dynamic[]>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\skillDict.json"))!;
		public static readonly Skill[] Skills = Skill.GetSkills();
		public static Dictionary<string, JArray> WeaponNames = new();
		public static Dictionary<string, JObject[]> WeaponObjs = new();
		public static Dictionary<string, JObject> WeaponTrees = new();
		public static Dictionary<string, JObject[]> WeaponRecipes = new();
		public static JArray WeaponSeriesNames = JsonConvert.DeserializeObject<JObject>(Utilities.ReadAllText("D:\\MH_Data Repo\\MH_Data\\Parsed Files\\MHWilds\\dtlnor rips\\MHWs-in-json-main\\natives\\STM\\GameDesign\\Text\\Excel_Equip\\WeaponSeries.msg.23.json"))!.Value<JArray>("entries")!;
		public static JArray WeaponSeriesData = JsonConvert.DeserializeObject<JArray>(Utilities.ReadAllText("D:\\MH_Data Repo\\MH_Data\\Parsed Files\\MHWilds\\dtlnor rips\\MHWs-in-json-main\\natives\\STM\\GameDesign\\Common\\Equip\\WeaponSeriesData.user.3.json"))!.First()!.Value<JObject>("app.user_data.WeaponSeriesData")!.Value<JArray>("_Values")!;
	}

	public class WildsShellTable
	{
		public string Name { get; set; } = string.Empty;
		public int Level { get; set; }
		public int Capacity { get; set; }
		public bool IsRapid { get; set; }
	}
}