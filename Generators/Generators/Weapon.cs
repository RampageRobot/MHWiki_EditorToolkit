using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MediawikiTranslator.Models.Data.MHRS;
using MediawikiTranslator.Models.Data.MHWI;
using MediawikiTranslator.Models.Monsters;
using MediawikiTranslator.Models.Weapon;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Path = System.IO.Path;

namespace MediawikiTranslator.Generators
{
	public class Weapon
	{
		private static Dictionary<Tuple<string, string>, string[]> WeaponRenderFileNames { get; set; } = [];
		private static Models.Data.MHWI.Items[] _mhwiItems = Models.Data.MHWI.Items.Fetch();
		private static dynamic[] MelodyDescs = JsonConvert.DeserializeObject<dynamic[]>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\hhMelodyDescs.json"))!;
		private static dynamic[] HighFreqNames = JsonConvert.DeserializeObject<dynamic[]>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\highFreqNames.json"))!;
		private static async Task<string> Generate(WebToolkitData weapon, WebToolkitData[] src, Monster[]? gameMonsters = null)
		{
			return await Task.Run(() =>
			{
				Tuple<string, string, string> weaponNames = GetWeaponTypeFullName(weapon.Type!);
				Tuple<string, string> renderKey = new(weapon.Game!, weaponNames.Item1);
				if (!WeaponRenderFileNames.TryGetValue(renderKey, out string[]? value))
				{
					value = Utilities.GetWeaponRenders(renderKey.Item1, renderKey.Item2).Result;
					WeaponRenderFileNames.TryAdd(renderKey, value);
				}
				string? match = null;
				if (weapon.Game != "MHWilds")
				{
					match = value.FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.StartsWith($"File:{weapon.Game!}-{SanitizeRoman(weapon.Name!)}"));
					if (match == null)
					{
						WebToolkitData? parent = src.FirstOrDefault(x => x.Name == weapon.PreviousName);
						while (parent != null && match == null)
						{
							match = value.FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.StartsWith($"File:{parent.Game!}-{SanitizeRoman(parent.Name!)}"));
							if (match == null)
							{
								parent = src.FirstOrDefault(x => x.Name == parent.PreviousName);
							}
						}
					}
					if (match != null && match.StartsWith("File:"))
					{
						match = match[5..];
					}
				}
				else
				{
					DirectoryInfo weaponRenders = new($@"D:\MH_Data Repo\MH_Data\Guest\Image Editor Files\SOURCE\MHWilds Images\MHWilds Renders\MHWilds Weapon Renders\Base Game\{weaponNames.Item1}");
					Dictionary<string, string> files = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(weaponRenders.FullName, "dict.json")))!;
					files.TryGetValue(weapon.Name!, out match);
				}
				string tree = weapon.Tree!.Replace("Unavailable", (weapon.Name!.Contains("Safi's") ? "Safi Tree" : "Kulve Taroth Tree"));
				string ktMessage = "";
				if (tree.Contains("Kulve Taroth"))
				{
					WebToolkitData? otherWeapon = src.FirstOrDefault(x => x.Name == weapon.Name && x.Type == weapon.Type && x.Rarity != weapon.Rarity);
					if (otherWeapon != null)
					{
						if (GetRank(weapon.Game!, weapon.Rarity!.Value) == "MR")
						{
							ktMessage = $"<br>This page contains this weapon's stats at '''Level 5'''. To see its '''Level 1 (High Rank)''' version, go to {{{{GenericWeaponLink|{weapon.Game}|{weapon.Name}|{weapon.Type}|{otherWeapon.Rarity}}}}}. To see stat changes at other levels, as well as more information on the Kulve Taroth weapon upgrading system, go to [[Kulve Taroth Weapons (MWHI)]].";
						}
						else
						{

							ktMessage = $"<br>This page contains this weapon's stats at '''Level 1'''. To see its '''Level 5 (Master Rank)''' version, go to {{{{GenericWeaponLink|{weapon.Game}|{weapon.Name} (MR)|{weapon.Type}|{otherWeapon.Rarity}}}}}. To see stat changes at other levels, as well as more information on the Kulve Taroth weapon upgrading system, go to [[Kulve Taroth Weapons (MWHI)]].";
						}
					}
				}
				StringBuilder ret = new();
				int elemBloat = !new string[] { "MHR", "MHRS", "MHGU", "MHP3" }.Contains(weapon.Game) ? 10 : 1;
				int bloat = Convert.ToInt32(Math.Round(GetWeaponBloat(weapon.Type!, weapon.Game!) * Convert.ToInt32(weapon.Attack)));
				if (weapon.Tree == "Artian" && weapon.Game == "MHWilds")
				{
					ret.AppendLine($@"{{{{GenericNav|{weapon.Game}}}}}
<br>
<br>
The {weapon.Name} is {(weapon.Type == "IG" ? "an" : "a")} [[{weaponNames.Item1} ({weapon.Game})|{weaponNames.Item2}]] in [[{GetGameFullName(weapon.Game!)}]]. To view the upgrade relationships between {weaponNames.Item3}, see the [[{weapon.Game}/{weaponNames.Item1} Tree|{weapon.Game} {weaponNames.Item1} Weapon Tree]].{ktMessage}");
					ret.AppendLine($@"{{{{ArtianWeapon
|Game                    = {weapon.Game}
|Weapon Name             = {weapon.Name}
|Render File             = {match ?? "wiki.png"}
|Weapon Type             = {weapon.Type}
|Tree                    = {tree}
|Rarity                  = {weapon.Rarity}
|Description             = {weapon.Description}
|Attack                  = {(bloat == Convert.ToInt32(weapon.Attack) ? weapon.Attack : bloat + " (" + weapon.Attack + ")")}
|Affinity                = {weapon.Affinity}" +
					(weapon.Decos1 != null && weapon.Decos1!.Value > 0 ? $"\r\n|Level 1 Decos           = {weapon.Decos1.Value}" : "") +
					(weapon.Decos2 != null && weapon.Decos2!.Value > 0 ? $"\r\n|Level 2 Decos           = {weapon.Decos2.Value}" : "") +
					(weapon.Decos3 != null && weapon.Decos3!.Value > 0 ? $"\r\n|Level 3 Decos           = {weapon.Decos3.Value}" : "") +
					(weapon.Decos4 != null && weapon.Decos4!.Value > 0 ? $"\r\n|Level 4 Decos           = {weapon.Decos4.Value}" : "") +
					(!string.IsNullOrEmpty(weapon.Element1) && weapon.ElementDmg1 != null ? "\r\n|Elemental Damage        =" + (weapon.ElementDmg1.Contains("(") ? "(" : "") + (Convert.ToInt32(weapon.ElementDmg1.Replace("(", "").Replace(")", "")) * elemBloat) + (weapon.ElementDmg1.Contains("(") ? ")" : "") : "") +
					(!string.IsNullOrEmpty(weapon.Element1) && weapon.ElementDmg1 != null ? "\r\n|Elemental Damage Type   =" + weapon.Element1.Replace("Paralyze", "Paralysis").Replace("Explosion", "Blast").Replace("(", "").Replace(")", "") : "") +
					(!string.IsNullOrEmpty(weapon.Element2) && weapon.ElementDmg2 != null ? "\r\n|Elemental Damage 2      =" + (weapon.ElementDmg2.Contains("(") ? "(" : "") + (Convert.ToInt32(weapon.ElementDmg2.Replace("(", "").Replace(")", "")) * elemBloat) + (weapon.ElementDmg2.Contains("(") ? ")" : "") : "") +
					(!string.IsNullOrEmpty(weapon.Element2) && weapon.ElementDmg2 != null ? "\r\n|Elemental Damage Type 2 =" + weapon.Element2.Replace("Paralyze", "Paralysis").Replace("Explosion", "Blast").Replace("(", "").Replace(")", "") : "") +
					(!string.IsNullOrEmpty(weapon.Sharpness) ? "\r\n" + GetSharpnessTemplates(weapon.Sharpness, weapon.Game) : "") +
					(!string.IsNullOrEmpty(weapon.HhNote1) ? $"\r\n|HH Note 1               = {weapon.HhNote1.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")}" : "") +
					(!string.IsNullOrEmpty(weapon.HhNote2) ? $"\r\n|HH Note 2               = {weapon.HhNote2.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")}" : "") +
					(!string.IsNullOrEmpty(weapon.HhNote3) ? $"\r\n|HH Note 3               = {weapon.HhNote3.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")}" : "") +
					(!string.IsNullOrEmpty(weapon.GlShellingType) ? (weapon.Game == "MHWilds" ? $"\r\n|GL Shelling Type        = {weapon.GlShellingType}" : $"\r\n|GL Shelling Type        = {weapon.GlShellingType} Lv {weapon.GlShellingLevel}") : "") +
					(!string.IsNullOrEmpty(weapon.GlShellingLevel) && weapon.Game == "MHWilds" ? $"\r\n|GL Shelling Power       = {weapon.GlShellingLevel}" : "") +
					(!string.IsNullOrEmpty(weapon.SaPhialType) ? $"\r\n|SA Phial Type           = {weapon.SaPhialType}" : "") +
					(!string.IsNullOrEmpty(weapon.SaPhialDamage) && weapon.Game == "MHWilds" ? $"\r\n|SA Phial Damage         = {weapon.SaPhialDamage}" : "") +
					(!string.IsNullOrEmpty(weapon.CbPhialType) ? $"\r\n|CB Phial Type           = {weapon.CbPhialType}" : "") +
					(!string.IsNullOrEmpty(weapon.IgKinsectBonus) ? $"\r\n|IG Kinsect Bonus        = {weapon.IgKinsectBonus}" : "") +
					(!string.IsNullOrEmpty(weapon.BoCoatings) ? $"\r\n|Bo Coatings             = {weapon.BoCoatings}" : "") +
					(!string.IsNullOrEmpty(weapon.HhMelodies) ? $"\r\n|HH Melodies             = {weapon.HhMelodies}" : "") +
					(!string.IsNullOrEmpty(weapon.HhSpecialMelody) && weapon.Game == "MHWilds" ? $"\r\n|HH Special Melody       = {weapon.HhSpecialMelody}" : "") +
					(!string.IsNullOrEmpty(weapon.HhEchoBubble) && weapon.Game == "MHWilds" ? $"\r\n|HH Echo Bubble          = {weapon.HhEchoBubble}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgSpecialAmmoType) ? $"\r\n|HBG Special Ammo Type   = {weapon.HbgSpecialAmmoType}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgSpecialAmmoType1) && weapon.Game == "MHWilds" ? $"\r\n|HBG Special Ammo Type 1 = {weapon.HbgSpecialAmmoType1}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgSpecialAmmoType2) && weapon.Game == "MHWilds" ? $"\r\n|HBG Special Ammo Type 2 = {weapon.HbgSpecialAmmoType2}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgDefaultMod1) && weapon.Game == "MHWilds" ? $"\r\n|HBG Default Mod 1       = {weapon.HbgDefaultMod1}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgDefaultMod2) && weapon.Game == "MHWilds" ? $"\r\n|HBG Default Mod 2       = {weapon.HbgDefaultMod2}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgIgnitionGauge) && weapon.Game == "MHWilds" ? $"\r\n|HBG Ignition Gauge      = {weapon.HbgIgnitionGauge}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgStandardIgnitionType) && weapon.Game == "MHWilds" ? $"\r\n|HBG Ignition Type       = {weapon.HbgStandardIgnitionType}" : "") +
					(!string.IsNullOrEmpty(weapon.LbgSpecialAmmoType) ? $"\r\n|LBG Special Ammo Type   = {weapon.LbgSpecialAmmoType}" : "") +
					(!string.IsNullOrEmpty(weapon.LbgDefaultMod1) && weapon.Game == "MHWilds" ? $"\r\n|LBG Default Mod 1       = {weapon.LbgDefaultMod1}" : "") +
					(!string.IsNullOrEmpty(weapon.LbgDefaultMod2) && weapon.Game == "MHWilds" ? $"\r\n|LBG Default Mod 2       = {weapon.LbgDefaultMod2}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgDeviation) ? $"\r\n|HBG Deviation        = {weapon.HbgDeviation}" : "") +
					(!string.IsNullOrEmpty(weapon.LbgDeviation) ? $"\r\n|LBG Deviation        = {weapon.LbgDeviation}" : "") +
					(!string.IsNullOrEmpty(weapon.Elderseal) && weapon.Elderseal != "None" ? $"\r\n|Elderseal               = {weapon.Elderseal}" : "") +
					(!string.IsNullOrEmpty(weapon.ArmorSkills) ? $"\r\n|Armor Skills            = {weapon.ArmorSkills}" : "") +
					(!string.IsNullOrEmpty(weapon.Rollback) ? $"\r\n|Can Rollback            = {weapon.Rollback}" : ""));
				}
				else
				{
					ret.AppendLine($@"{{{{GenericNav|{weapon.Game}}}}}
<br>
<br>
The {weapon.Name} is {(weapon.Type == "IG" ? "an" : "a")} [[{weaponNames.Item1} ({weapon.Game})|{weaponNames.Item2}]] in [[{GetGameFullName(weapon.Game!)}]]. To view the upgrade relationships between {weaponNames.Item3}, see the [[{weapon.Game}/{weaponNames.Item1} Tree|{weapon.Game} {weaponNames.Item1} Weapon Tree]].{ktMessage}");
					ret.AppendLine($@"{{{{GenericWeapon
|Game                    = {weapon.Game}
|Weapon Name             = {weapon.Name}
|Render File             = {match ?? "wiki.png"}
|Weapon Type             = {weapon.Type}
|Tree                    = {tree}
|Rarity                  = {weapon.Rarity}
|Description             = {weapon.Description}
|Attack                  = {(bloat == Convert.ToInt32(weapon.Attack) ? weapon.Attack : bloat + " (" + weapon.Attack + ")")}
|Affinity                = {weapon.Affinity}" +
					(weapon.Decos1 != null && weapon.Decos1!.Value > 0 ? $"\r\n|Level 1 Decos           = {weapon.Decos1.Value}" : "") +
					(weapon.Decos2 != null && weapon.Decos2!.Value > 0 ? $"\r\n|Level 2 Decos           = {weapon.Decos2.Value}" : "") +
					(weapon.Decos3 != null && weapon.Decos3!.Value > 0 ? $"\r\n|Level 3 Decos           = {weapon.Decos3.Value}" : "") +
					(weapon.Decos4 != null && weapon.Decos4!.Value > 0 ? $"\r\n|Level 4 Decos           = {weapon.Decos4.Value}" : "") +
					(!string.IsNullOrEmpty(weapon.Element1) && weapon.ElementDmg1 != null ? "\r\n|Elemental Damage        =" + (weapon.ElementDmg1.Contains("(") ? "(" : "") + (Convert.ToInt32(weapon.ElementDmg1.Replace("(", "").Replace(")", "")) * elemBloat) + (weapon.ElementDmg1.Contains("(") ? ")" : "") : "") +
					(!string.IsNullOrEmpty(weapon.Element1) && weapon.ElementDmg1 != null ? "\r\n|Elemental Damage Type   =" + weapon.Element1.Replace("Paralyze", "Paralysis").Replace("Explosion", "Blast").Replace("(", "").Replace(")", "") : "") +
					(!string.IsNullOrEmpty(weapon.Element2) && weapon.ElementDmg2 != null ? "\r\n|Elemental Damage 2      =" + (weapon.ElementDmg2.Contains("(") ? "(" : "") + (Convert.ToInt32(weapon.ElementDmg2.Replace("(", "").Replace(")", "")) * elemBloat) + (weapon.ElementDmg2.Contains("(") ? ")" : "") : "") +
					(!string.IsNullOrEmpty(weapon.Element2) && weapon.ElementDmg2 != null ? "\r\n|Elemental Damage Type 2 =" + weapon.Element2.Replace("Paralyze", "Paralysis").Replace("Explosion", "Blast").Replace("(", "").Replace(")", "") : "") +
					(!string.IsNullOrEmpty(weapon.Sharpness) ? "\r\n" + GetSharpnessTemplates(weapon.Sharpness, weapon.Game) : "") +
					(!string.IsNullOrEmpty(weapon.HhNote1) ? $"\r\n|HH Note 1               = {weapon.HhNote1.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")}" : "") +
					(!string.IsNullOrEmpty(weapon.HhNote2) ? $"\r\n|HH Note 2               = {weapon.HhNote2.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")}" : "") +
					(!string.IsNullOrEmpty(weapon.HhNote3) ? $"\r\n|HH Note 3               = {weapon.HhNote3.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")}" : "") +
					(!string.IsNullOrEmpty(weapon.GlShellingType) ? (weapon.Game == "MHWilds" ? $"\r\n|GL Shelling Type        = {weapon.GlShellingType}" : $"\r\n|GL Shelling Type        = {weapon.GlShellingType} Lv {weapon.GlShellingLevel}") : "") +
					(!string.IsNullOrEmpty(weapon.GlShellingLevel) && weapon.Game == "MHWilds" ? $"\r\n|GL Shelling Power       = {weapon.GlShellingLevel}" : "") +
					(!string.IsNullOrEmpty(weapon.SaPhialType) ? $"\r\n|SA Phial Type           = {weapon.SaPhialType}" : "") +
					(!string.IsNullOrEmpty(weapon.SaPhialDamage) && weapon.Game == "MHWilds" ? $"\r\n|SA Phial Damage         = {weapon.SaPhialDamage}" : "") +
					(!string.IsNullOrEmpty(weapon.CbPhialType) ? $"\r\n|CB Phial Type           = {weapon.CbPhialType}" : "") +
					(!string.IsNullOrEmpty(weapon.IgKinsectBonus) ? $"\r\n|IG Kinsect Bonus        = {weapon.IgKinsectBonus}" : "") +
					(!string.IsNullOrEmpty(weapon.BoCoatings) ? $"\r\n|Bo Coatings             = {weapon.BoCoatings}" : "") +
					(!string.IsNullOrEmpty(weapon.HhMelodies) ? $"\r\n|HH Melodies             = {weapon.HhMelodies}" : "") +
					(!string.IsNullOrEmpty(weapon.HhSpecialMelody) && weapon.Game == "MHWilds" ? $"\r\n|HH Special Melody       = {weapon.HhSpecialMelody}" : "") +
					(!string.IsNullOrEmpty(weapon.HhEchoBubble) && weapon.Game == "MHWilds" ? $"\r\n|HH Echo Bubble          = {weapon.HhEchoBubble}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgSpecialAmmoType) ? $"\r\n|HBG Special Ammo Type   = {weapon.HbgSpecialAmmoType}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgSpecialAmmoType1) && weapon.Game == "MHWilds" ? $"\r\n|HBG Special Ammo Type 1 = {weapon.HbgSpecialAmmoType1}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgSpecialAmmoType2) && weapon.Game == "MHWilds" ? $"\r\n|HBG Special Ammo Type 2 = {weapon.HbgSpecialAmmoType2}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgDefaultMod1) && weapon.Game == "MHWilds" ? $"\r\n|HBG Default Mod 1       = {weapon.HbgDefaultMod1}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgDefaultMod2) && weapon.Game == "MHWilds" ? $"\r\n|HBG Default Mod 2       = {weapon.HbgDefaultMod2}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgIgnitionGauge) && weapon.Game == "MHWilds" ? $"\r\n|HBG Ignition Gauge      = {weapon.HbgIgnitionGauge}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgStandardIgnitionType) && weapon.Game == "MHWilds" ? $"\r\n|HBG Ignition Type       = {weapon.HbgStandardIgnitionType}" : "") +
					(!string.IsNullOrEmpty(weapon.LbgSpecialAmmoType) ? $"\r\n|LBG Special Ammo Type   = {weapon.LbgSpecialAmmoType}" : "") +
					(!string.IsNullOrEmpty(weapon.LbgDefaultMod1) && weapon.Game == "MHWilds" ? $"\r\n|LBG Default Mod 1       = {weapon.LbgDefaultMod1}" : "") +
					(!string.IsNullOrEmpty(weapon.LbgDefaultMod2) && weapon.Game == "MHWilds" ? $"\r\n|LBG Default Mod 2       = {weapon.LbgDefaultMod2}" : "") +
					(!string.IsNullOrEmpty(weapon.HbgDeviation) ? $"\r\n|HBG Deviation        = {weapon.HbgDeviation}" : "") +
					(!string.IsNullOrEmpty(weapon.LbgDeviation) ? $"\r\n|LBG Deviation        = {weapon.LbgDeviation}" : "") +
					(!string.IsNullOrEmpty(weapon.Elderseal) && weapon.Elderseal != "None" ? $"\r\n|Elderseal               = {weapon.Elderseal}" : "") +
					(!string.IsNullOrEmpty(weapon.ArmorSkills) ? $"\r\n|Armor Skills            = {weapon.ArmorSkills}" : "") +
					(!string.IsNullOrEmpty(weapon.RampageSkillSlots) ? $"\r\n|Rampage Slots           = {weapon.RampageSkillSlots}" : "") +
					(!string.IsNullOrEmpty(weapon.RampageDecoration) ? $"\r\n|Rampage Decoration      = {weapon.RampageDecoration}" : "") +
					(!string.IsNullOrEmpty(weapon.Rollback) ? $"\r\n|Can Rollback            = {weapon.Rollback}" : ""));
				}
				string gameString = "";
				string rankString = "";
				string rankAbbrev = GetRank(weapon.Game!, weapon.Rarity);
				switch (rankAbbrev)
				{
					case "LR":
						rankString = "Low Rank (LR)";
						break;
					case "HR":
						rankString = "High Rank (HR)";
						break;
					case "MR":
						rankString = "Master Rank (MR)";
						break;
				}
				switch (weapon.Game)
				{
					case "MHW":
					case "MHWI":
						gameString = "Monster Hunter: World (MHW) and Monster Hunter World: Iceborne (MHWI)";
						break;
					case "MHR":
					case "MHRS":
						gameString = "Monster Hunter: Rise (MHR) and Monster Hunter Rise: Sunbreak (MHRS)";
						break;
					default:
						gameString = $"{{{{Game|{weapon.Game}}}}} ({weapon.Game})";
						break;
				}
				string monsterName = "";
				if (gameMonsters != null)
				{
					monsterName = " " + gameMonsters.FirstOrDefault(x => x.Equipment != null && x.Equipment.Weapons.Any(y => y.Name == weapon.Name))?.Name ?? "";
				}
				ret.AppendLine($@"|MetaDescription         = The {weapon.Name} is {(weapon.Type == "IG" ? "an" : "a")} {rankString}{monsterName} {weaponNames.Item1} ({weapon.Type}) weapon{(!string.IsNullOrEmpty(weapon.Tree) && weapon.Tree != "???" ? " in the " + weapon.Tree : "")} from {gameString}.");
				if (weapon.Tree != "Artian" || weapon.Game != "MHWilds")
				{
					if (!string.IsNullOrEmpty(weapon.ForgeMaterials.Trim()))
					{
						ret.AppendLine($@"|Forge Cost              = {weapon.ForgeCost:N0}
|Forge Materials         = {GetMaterialsTemplates(weapon.ForgeMaterials, weapon.Game)}");
					}
					if (!string.IsNullOrEmpty(weapon.PreviousName))
					{
						ret.AppendLine($@"|Upgrade Cost            = {weapon.UpgradeCost:N0}
|Upgrade Materials       = {GetMaterialsTemplates(weapon.UpgradeMaterials, weapon.Game)}
|Previous Name           = {weapon.PreviousName + (src.Count(x => x.Name == weapon.PreviousName) > 1 && GetRank(weapon.Game!, weapon.PreviousRarity + 1) == "MR" ? " (MR)" : "")}
|Previous Type           = {weapon.Type}
|Previous Rarity         = {weapon.PreviousRarity}");
					}
					if (!string.IsNullOrEmpty(weapon.Next1Name))
					{
						ret.AppendLine($@"|Next 1 Name             = {weapon.Next1Name + (src.Count(x => x.Name == weapon.Next1Name) > 1 && GetRank(weapon.Game!, weapon.Next1Rarity + 1) == "MR" ? " (MR)" : "")}
|Next 1 Type             = {weapon.Type}
|Next 1 Rarity           = {weapon.Next1Rarity}
|Next 1 Cost             = {weapon.Next1Cost:N0}
|Next 1 Materials        = {GetMaterialsTemplates(weapon.Next1Materials, weapon.Game)}");
					}
					if (!string.IsNullOrEmpty(weapon.Next2Name))
					{
						ret.AppendLine($@"|Next 2 Name             = {weapon.Next2Name + (src.Count(x => x.Name == weapon.Next2Name) > 1 && GetRank(weapon.Game!, weapon.Next2Rarity + 1) == "MR" ? " (MR)" : "")}
|Next 2 Type             = {weapon.Type}
|Next 2 Rarity           = {weapon.Next2Rarity}
|Next 2 Cost             = {weapon.Next2Cost:N0}
|Next 2 Materials        = {GetMaterialsTemplates(weapon.Next2Materials, weapon.Game)}");
					}
					if (!string.IsNullOrEmpty(weapon.Next3Name))
					{
						ret.AppendLine($@"|Next 3 Name             = {weapon.Next3Name + (src.Count(x => x.Name == weapon.Next3Name) > 1 && GetRank(weapon.Game!, weapon.Next3Rarity + 1) == "MR" ? " (MR)" : "")}
|Next 3 Type             = {weapon.Type}
|Next 3 Rarity           = {weapon.Next3Rarity}
|Next 3 Cost             = {weapon.Next3Cost:N0}
|Next 3 Materials        = {GetMaterialsTemplates(weapon.Next3Materials, weapon.Game)}");
					}
					if (!string.IsNullOrEmpty(weapon.Next4Name))
					{
						ret.AppendLine($@"|Next 4 Name             = {weapon.Next4Name + (src.Count(x => x.Name == weapon.Next4Name) > 1 && GetRank(weapon.Game!, weapon.Next4Rarity + 1) == "MR" ? " (MR)" : "")}
|Next 4 Type             = {weapon.Type}
|Next 4 Rarity           = {weapon.Next4Rarity}
|Next 4 Cost             = {weapon.Next4Cost:N0}
|Next 4 Materials        = {GetMaterialsTemplates(weapon.Next4Materials, weapon.Game)}");
					}
				}
				ret.AppendLine(@"}}");
				if (weapon.Type == "HH" && weapon.Game != "MHRS")
				{
					List<Tuple<string, string[]>> melodies = GetHHMelodies(weapon.Game!);
					Dictionary<string, string> melodyIcons = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\melodyIcons.json"))!;
					ret.AppendLine(@"==Melodies==
{| class=""wikitable"" id=""tblMelodies"" style=""margin-left:auto; margin-right:auto; text-align:center;""
! style=""min-width:100px;""|Sequence !! Melody !! Effect
|-");
					if (weapon.Game != "MHWilds" || weapon.Tree != "Artian")
					{
						string[] validNotes = [.. new string[] { weapon.HhNote1!, weapon.HhNote2!, weapon.HhNote3! }.Where(x => x != "Disabled")];
						foreach (Tuple<string, string[]> melody in melodies.Where(x => !x.Item2.Where(y => y != "Echo" && y != "Disabled").Any(y => !validNotes.Contains(y.Replace("_", " ")))))
						{
							string melodyNotes = "";
							foreach (string note in melody.Item2.Where(x => x != "Disabled"))
							{
								if (note == "Echo" && weapon.Game != "MHWilds")
								{
									melodyNotes += "{{UI|" + weapon.Game + "|HH Echo|nolink=true}}";
								}
								else
								{
									string noteFile = "1";
									if (note.Replace("_", " ") == weapon.HhNote2)
									{
										noteFile = "2";
									}
									else if (note.Replace("_", " ") == weapon.HhNote3)
									{
										noteFile = "3";
									}
									melodyNotes += $"{{{{UI|{weapon.Game}|HH Note|{noteFile} {note.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")}|nolink=true}}}}";
								}
							}
							string melodyName = melody.Item1;
							string melodyInfoDesc = "???";
							if (weapon.Game == "MHWilds")
							{
								if (melody.Item1 == "HIGHFREQ")
								{
									melodyName = HighFreqNames.First(x => x.enumName == weapon.HighFreqEnum).skillName;
								}
							}
							if (weapon.Game == "MHWI")
							{
								Tuple<string, string> melodyInfo = GetWorldMelodyEffect(melody.Item1);
								melodyInfoDesc = melodyInfo.Item2;
							}
							else if (weapon.Game == "MHWilds")
							{
								melodyName = $"{melodyIcons[melodyName]} {melodyName}";
								string thisMelodyNameTemp = melody.Item1 == "HIGHFREQ" ? "Echo Wave" : melody.Item1;
								melodyInfoDesc = MelodyDescs.FirstOrDefault(x => x.skillName == thisMelodyNameTemp)?.skillDesc ?? "???";
							}
							ret.AppendLine(@$"|{melodyNotes} || {melodyName} || {melodyInfoDesc}
|-");
						}
					}
					ret.AppendLine(" |}");
				}
				if (!string.IsNullOrEmpty(weapon.ShellTableWikitext) && weapon.Type != "Bo")
				{
					if (weapon.Game != "MHWilds")
					{
						ret.AppendLine($@"==Ammunition==
{{{{AmmoTableLegend|{weapon.Game}}}}}
<br>
{{| class=""wikitable"" style=""text-align:center; width:100%; margin:0;""
!Ammo
!Capacity
!Recoil
!Reload
!Shot Type
|-
{weapon.ShellTableWikitext}");
						ret.AppendLine(@"
|}
{{UserHelpBox|'''Capacity:''' The number of rounds of an ammo that a bowgun can fire before it has to reload.<br>
'''Recoil:''' How much the hunter is pushed back when firing a shot. Impacts fire rate.<br>
'''Reload:''' How quickly the hunter can reload the bowgun.<br>
'''Shot Type:''' Certain ammo types on some bowguns have special properties when fired, such as Auto Reload or Wyvern, which impacts how the hunter uses the bowgun when firing and/or reloading.}}");
					}
					else
					{
						ret.AppendLine($@"==Ammunition==
{{| class=""wikitable ammo-table"" style=""text-align:center; width:100%; margin:0;""
!Ammo
!Level
!Capacity");
						if (weapon.Type == "LBG")
						{
							ret.AppendLine("!Rapid Fire?");
						}
						ret.AppendLine($@"|-
{weapon.ShellTableWikitext}");
						ret.AppendLine(@"
|}
{{UserHelpBox|'''Level:''' The power level of the ammunition. Affects damage and other secondary effects.<br>
'''Capacity:''' The number of rounds of an ammo that a bowgun can fire before it has to reload.<br>}}");
					}
				}
				ret.AppendLine(@$"[[Category:{weaponNames.Item1}]]
[[Category:{weapon.Game} Weapons]]
[[Category:{weapon.Game} {weaponNames.Item1}]]");
				if (weapon.Element1 != null && weapon.Element1 != "None" && !string.IsNullOrEmpty(weapon.Element1))
				{
					ret.AppendLine($"[[Category:{weapon.Element1} Weapons]]");
					ret.AppendLine($"[[Category:{weapon.Game} {weapon.Element1} Weapons]]");
					ret.AppendLine($"[[Category:{weapon.Game} {weapon.Element1} {weaponNames.Item1}]]");
				}
				if (weapon.Element2 != null && weapon.Element2 != "None" && !string.IsNullOrEmpty(weapon.Element2))
				{
					ret.AppendLine($"[[Category:{weapon.Element2} Weapons]]");
					ret.AppendLine($"[[Category:{weapon.Game} {weapon.Element2} Weapons]]");
					ret.AppendLine($"[[Category:{weapon.Game} {weapon.Element2} {weaponNames.Item1}]]");
				}
				if (match == null)
				{
					ret.AppendLine($"[[Category:{weapon.Game!} Weapons Without Renders]]");
				}
				if (weapon.Tree == "Artian")
				{
					ret.AppendLine($"[[Category:Artian Weapons]]");
				}
				return ret.ToString().Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
			});
		}

		public static double GetWeaponBloat(string type, string game)
		{
			if (!new string[] { "MHR", "MHRS", "MHGU", "MHP3" }.Contains(game))
			{
				return new Dictionary<string, double>()
				{
					{ "GS", 4.8f },
					{ "GL", 2.3f },
					{ "LS", 3.3f },
					{ "SA", 3.5f },
					{ "SnS", 1.4f },
					{ "CB", 3.6f },
					{ "DB", 1.4f },
					{ "IG", 3.1f },
					{ "Hm", 5.2f },
					{ "LBG", 1.3f },
					{ "HH", 4.2f },
					{ "HBG", 1.5f },
					{ "Ln", 2.3f },
					{ "Bo", 1.2f },
				}[type];
			}
			else
			{
				return 1.0f;
			}
		}

		public static string SanitizeRoman(string input)
		{
			string[] romans = ["I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"];
			if (romans.Any(x => input.EndsWith(" " + x)))
			{
				return input[..input.LastIndexOf(' ')];
			}
			else
			{
				return input;
			}
		}

		public static Tuple<string, string> GetWorldMelodyEffect(string melody)
		{
			return new Dictionary<string, Tuple<string, string>>()
			{
				{ "Self-improvement", new Tuple<string, string>("Self-Improvement", "Increases your movement speed for a period of time. Perform an encore to prolong the duration of the boost, raise your attack power or briefly make your attacks less likely to deflect.") },
				{ "Attack Up(S)", new Tuple<string, string>("Attack Up (S)", "Slightly increases the non-elemental power of your weapon for a period of time.") },
				{ "Attack Up(L)", new Tuple<string, string>("Attack Up (L)", "Increases the non-elemental power of your weapon for a period of time.") },
				{ "Health Boost(S)", new Tuple<string, string>("Health Boost (S)", "Slightly increases your maximum health for a period of time.") },
				{ "Health Boost(L)", new Tuple<string, string>("Health Boost (L)", "Increases your maximum health for a period of time.") },
				{ "Stamina Use Reduced(S)", new Tuple<string, string>("Stamina Use Reduced (S)", "Slightly reduces your stamina consumption.") },
				{ "Stamina Use Reduced(L)", new Tuple<string, string>("Stamina Use Reduced (L)", "Reduces your stamina consumption.") },
				{ "Wind Pressure Negated", new Tuple<string, string>("Wind Pressure Negation", "Allows you to resist wind pressure from certain monsters or attacks for a period of time.") },
				{ "All Wind Pressure Negated", new Tuple<string, string>("Wind Pressure Negation", "Allows you to resist wind pressure from certain monsters or attacks for a period of time.") },
				{ "Defense Up(S)", new Tuple<string, string>("Defense Up (S)", "Slightly increases your defense for a period of time.") },
				{ "Defense Up(L)", new Tuple<string, string>("Defense Up (L)", "Increases your defense for a period of time.") },
				{ "Tool Use Drain Reduced(S)", new Tuple<string, string>("Tool Use Drain Reduced (S)", "Slightly decreases the reductions on effect duration on your specialized tools for a period of time.") },
				{ "Tool Use Drain Reduced(L)", new Tuple<string, string>("Tool Use Drain Reduced (L)", "Decreases the reductions on effect duration on your specialized tools for a period of time.") },
				{ "Health Rec. (S)", new Tuple<string, string>("Health Rec. (S)", "Recovers a small amount of health.") },
				{ "Health Rec. (L)", new Tuple<string, string>("Health Rec. (L)", "Recovers a large amount of health.") },
				{ "Health Rec. (S) + Antidote", new Tuple<string, string>("Health Rec. (S) + Antidote", "Recovers a small amount of health and cures the poison ailment.") },
				{ "Health Rec. (M) + Antidote", new Tuple<string, string>("Health Rec. (M) + Antidote", "Recovers a moderate amount of health and cures the poison ailment.") },
				{ "Recovery Speed(S)", new Tuple<string, string>("Recovery Speed (S)", "Slightly increases the speed at which you recover health for a period of time.") },
				{ "Recovery Speed(L)", new Tuple<string, string>("Recovery Speed (L)", "Increases the speed at which you recover health for a period of time.") },
				{ "Earplugs(S)", new Tuple<string, string>("Earplugs (S)", "Protects you against small monster roars for a period of time.") },
				{ "Earplugs(L)", new Tuple<string, string>("Earplugs (L)", "Protects you against small and large monster roars for a period of time.") },
				{ "Divine Protection", new Tuple<string, string>("Divine Protection", "For a period of time, the damage you take has a chance of being reduced.") },
				{ "Scoutfly Power Up", new Tuple<string, string>("Scoutfly Power Up", "Improves scoutfly tracking for a period of time.") },
				{ "Envir.Damage Negated", new Tuple<string, string>("Environmental Damage Negation", "Protects you against some forms of environmental damage and increases resistance to heat and cold.") },
				{ "Stun Negated", new Tuple<string, string>("Stun Negation", "Makes it harder for you to get stunned for a period of time.") },
				{ "Paralysis Negated", new Tuple<string, string>("Paralysis Negation", "Makes it harder for you to become paralyzed for a period of time.") },
				{ "Tremors Negated", new Tuple<string, string>("Tremor Resistance", "Allows you to resist tremors caused by monsters or certain attacks for a period of time.") },
				{ "Muck Res", new Tuple<string, string>("Muck/Water/Deep Snow Res", "Protects you against mud for a period of time. Allows you to be more mobile in water or deep snow.") },
				{ "Fire Res Boost(S)", new Tuple<string, string>("Fire Resistance Up (S)", "Slightly increases fire resistance and protects you from fireblight for a period of time.") },
				{ "Fire Res Boost(L)", new Tuple<string, string>("Fire Resistance Up (L)", "Increases fire resistance and protects you from fireblight for a period of time.") },
				{ "Water Res Boost(S)", new Tuple<string, string>("Water Resistance Up (S)", "Slightly increases water resistance and protects you from waterblight for a period of time.") },
				{ "Water Res Boost(L)", new Tuple<string, string>("Water Resistance Up (L)", "Increases water resistance and protects you from waterblight for a period of time.") },
				{ "Thunder Res Boost(S)", new Tuple<string, string>("Thunder Resistance Up (S)", "Slightly increases thunder resistance and protects you from thunderblight for a period of time.") },
				{ "Thunder Res Boost(L)", new Tuple<string, string>("Thunder Resistance Up (L)", "Increases thunder resistance and protects you from thunderblight for a period of time.") },
				{ "Ice Res Boost(S)", new Tuple<string, string>("Ice Resistance Up (S)", "Slightly increases ice resistance and protects you from iceblight for a period of time.") },
				{ "Ice Res Boost(L)", new Tuple<string, string>("Ice Resistance Up (L)", "Increases ice resistance and protects you from iceblight for a period of time.") },
				{ "Dragon Res Boost(S)", new Tuple<string, string>("Dragon Resistance Up (S)", "Slightly increases dragon resistance and protects you from dragonblight for a period of time.") },
				{ "Dragon Res Boost(L)", new Tuple<string, string>("Dragon Resistance Up (L)", "Increases dragon resistance and protects you from dragonblight for a period of time.") },
				{ "Elemental Attack Boost", new Tuple<string, string>("Elemental Attack Up", "Increases the elemental power of your weapon for a period of time.") },
				{ "Blight Negated", new Tuple<string, string>("Blight Negation", "Protects you against all blights for a period of time.") },
				{ "Sonic Waves", new Tuple<string, string>("Sonic Waves", "Unleashes a gigantic wave of sound around you.") },
				{ "All Melody Effects Extended", new Tuple<string, string>("All Melody Effects Extended", "Extends the duration of all active melody effects.") },
				{ "Knockbacks Negated", new Tuple<string, string>("Knockback Prevention", "Protects you against certain attacks for a period of time so you won't be knocked back.") },
				{ "All Ailments Negated", new Tuple<string, string>("Ailment Negation", "Protects you against all status ailments for a period of time.") },
				{ "Blight Res Up", new Tuple<string, string>("All Resistances Up", "Increases all elemental resistances for a period of time.") },
				{ "Affinity Up/Health Rec. (S)", new Tuple<string, string>("Increases your critical damage and recovers a small amount of health.", "") },
				{ "Earplugs(S) / Wind Pressure Negated", new Tuple<string, string>("Earplugs (S) / Wind Pressure Negated", "Protects you against small monster roars and allows you to resist wind pressure from certain monsters or attacks for a period of time.") },
				{ "Abnormal Status Atk.Increased", new Tuple<string, string>("Status Attack Up", "Increases the potency of status ailment attacks dealt to monsters for a period of time.") },
				{ "Health Recovery(M)", new Tuple<string, string>("Health Recovery(M)", "Recovers a moderate amount of health.") },
				{ "Echo Impact", new Tuple<string, string>("Impact Echo Wave", "A melody effect that uses the echo mark. Sends out an echo wave capable of damaging and stunning a monster.") },
				{ "Echo Dragon", new Tuple<string, string>("Echo Wave \"Dragon\"", "A melody effect that uses the echo mark. Sends out an echo wave capable of dealing dragon element damage to a monster.") },
				{ "Max Stamina Up + Recovery", new Tuple<string, string>("Max Stamina Up + Recovery", "A melody effect that uses the echo mark. Touch the echo bubble to increase your maximum stamina for a period of time.") },
				{ "Extend Health Recovery", new Tuple<string, string>("Extended Health Recovery", "A melody effect that uses the echo mark. Touch the echo bubble to gradually recover health for a period of time.") },
				{ "Speed Boost + Evade Window", new Tuple<string, string>("Speed Boost + Evade Window Up", "A melody effect that uses the echo mark. Touch the echo bubble to increase your evade window for a period of time. Any other players using hunting horn will also get a speed boost.") },
				{ "Elemental Effectiveness Up", new Tuple<string, string>("Elemental Effectiveness Up", "A melody effect that uses the echo mark. Touch the echo bubble to increase elemental resistance and elemental attack damage. If a melody that affects elemental resistance or attack is already active, this will boost its effectiveness.") }
			}[melody];
		}

		public static Tuple<string, string, string> GetWeaponTypeFullName(string weaponType)
		{
			return new Dictionary<string, Tuple<string, string, string>>()
			{
				{ "CB", new Tuple<string,string,string>("Charge Blade", "Charge Blade", "Charge Blades") },
				{ "DB", new Tuple<string,string,string>("Dual Blades", "set of Dual Blades", "sets of Dual Blades") },
				{ "GS", new Tuple<string,string,string>("Great Sword", "Great Sword", "Great Swords") },
				{ "GL", new Tuple<string,string,string>("Gunlance", "Gunlance", "Gunlances") },
				{ "Hm", new Tuple<string,string,string>("Hammer", "Hammer", "Hammers") },
				{ "HH", new Tuple<string,string,string>("Hunting Horn", "Hunting Horn", "Hunting Horns") },
				{ "IG", new Tuple<string,string,string>("Insect Glaive", "Insect Glaive", "Insect Glaives") },
				{ "Ln", new Tuple<string,string,string>("Lance", "Lance", "Lances") },
				{ "LS", new Tuple<string,string,string>("Long Sword", "Long Sword", "Long Swords") },
				{ "SA", new Tuple<string,string,string>("Switch Axe", "Switch Axe", "Switch Axes") },
				{ "SnS", new Tuple<string,string,string>("Sword and Shield", "Sword and Shield pair", "Sword and Shield pairs") },
				{ "Bo", new Tuple<string,string,string>("Bow", "Bow", "Bows") },
				{ "HBG", new Tuple<string,string,string>("Heavy Bowgun", "Heavy Bowgun", "Heavy Bowguns") },
				{ "LBG", new Tuple<string,string,string>("Light Bowgun", "Light Bowgun", "Light Bowguns") }
			}[weaponType];
		}

		public static List<Tuple<string, string[]>> GetHHMelodies(string game)
		{
			if (game == "MHWilds")
			{
				return
				[
					new("Self-Improvement", ["Purple", "Purple"]),
					new("Self-Improvement", ["White", "White"]),
					new("Attack Up (S)", ["White", "Red", "White"]),
					new("Attack Up (S)", ["Red", "Yellow", "Purple"]),
					new("Attack Up (L)", ["Purple", "Red", "Blue", "Purple"]),
					new("Attack Up (L)", ["Purple", "Red", "Sky", "Purple"]),
					new("Attack Up (L)", ["Purple", "Red", "Green", "Purple"]),
					new("Attack Up (L)", ["Purple", "Orange", "Orange", "Red"]),
					new("Health Recovery (S)", ["Purple", "Green", "Purple"]),
					new("Health Recovery (S)", ["White", "Green", "White"]),
					new("Health Recovery (M)", ["Green", "White", "Sky", "Green"]),
					new("Health Recovery (L)", ["Green", "Green", "Purple", "Sky"]),
					new("Health Rec. (S) + Antidote", ["Green", "Blue", "White", "Blue"]),
					new("Health Rec. (M) + Antidote", ["Green", "Blue", "Purple", "Blue"]),
					new("Recovery Speed (S)", ["Green", "Green", "Yellow"]),
					new("Recovery Speed (L)", ["Green", "Green", "Red", "Purple"]),
					new("Stamina Use Reduced (S)", ["White", "Yellow", "Blue"]),
					new("Stamina Use Reduced (S)", ["White", "Sky", "Blue"]),
					new("Stamina Use Reduced (S)", ["White", "Green", "Blue"]),
					new("Stamina Use Reduced (L)", ["Purple", "Sky", "Blue", "Sky"]),
					new("Stamina Use Reduced (L)", ["Purple", "Yellow", "Blue"]),
					new("Stamina Use Reduced (L)", ["Purple", "Green", "Blue", "Green"]),
					new("Stamina Use Reduced (L)", ["Purple", "Orange", "Purple", "Blue"]),
					new("Elem Attack Boost", ["Yellow", "Sky", "Yellow", "Sky"]),
					new("Elem Attack Boost", ["Purple", "Green", "Yellow", "Green"]),
					new("Elem Attack Boost", ["Purple", "Orange", "Yellow", "Orange"]),
					new("Status Attack Up", ["Sky", "Orange", "Orange", "Purple"]),
					new("Defense Up (S)", ["White", "Blue", "Blue"]),
					new("Defense Up (L)", ["Purple", "Blue", "Blue", "Purple"]),
					new("Divine Protection", ["Purple", "Orange", "Purple", "Sky"]),
					new("Divine Protection", ["Green", "Yellow", "Yellow", "Purple"]),
					new("Fire Res (S)", ["Yellow", "Red", "White"]),
					new("Fire Res (L)", ["Purple", "Red", "Yellow"]),
					new("Water Res (S)", ["Yellow", "Sky", "White"]),
					new("Water Res (L)", ["Yellow", "Sky", "Purple"]),
					new("Thunder Res (S)", ["Yellow", "White", "Green"]),
					new("Thunder Res (L)", ["Yellow", "Purple", "Green"]),
					new("Ice Res (S)", ["Yellow", "Blue", "White"]),
					new("Ice Res (L)", ["Yellow", "Blue", "Purple"]),
					new("Dragon Res (S)", ["White", "Yellow", "Sky"]),
					new("Dragon Res (L)", ["Purple", "Yellow", "Sky"]),
					new("Earplugs (S)", ["Sky", "Sky", "Green", "White"]),
					new("Earplugs (S)", ["Sky", "Sky", "Red", "White"]),
					new("Earplugs (S)", ["Sky", "Sky", "Red", "Purple"]),
					new("Stun Negated", ["Sky", "Blue", "Purple"]),
					new("Earplugs (L)", ["Sky", "Sky", "Green", "Purple"]),
					new("Earplugs (L)", ["Orange", "Orange", "Green", "Purple"]),
					new("Tremors Negated", ["Sky", "Sky", "Yellow"]),
					new("Paralysis Negated", ["Sky", "Yellow", "White"]),
					new("Paralysis Negated", ["Sky", "Yellow", "Purple"]),
					new("Wind Pressure Negated", ["Blue", "Blue", "Red"]),
					new("Wind Pressure Negated", ["Blue", "Blue", "Sky"]),
					new("Wind Pressure Negated", ["Blue", "Blue", "Green"]),
					new("All Wind Pressure Negated", ["Blue", "Blue", "Yellow", "Purple"]),
					new("All Wind Pressure Negated", ["Blue", "Blue", "Orange"]),
					new("Aquatic/Oilsilt Mobility", ["Sky", "Red", "Sky"]),
					new("Envir. Damage Negated", ["Red", "Red", "Sky"]),
					new("Knockback Negated", ["Red", "Orange", "Red", "Purple"]),
					new("All Ailments Negated", ["Sky", "Purple", "Orange", "Orange"]),
					new("Blight Negated", ["Orange", "Yellow", "Yellow", "Orange"]),
					new("Sonic Waves", ["Yellow", "Yellow", "Yellow"]),
					new("Extend All Melodies", ["Orange", "Red", "Orange"]),
					new("Attack/Defense Up (S)", ["Purple", "Blue", "Orange", "Orange"]),
					new("Affinity Up/Health Recovery (S)", ["Green", "Orange", "Purple", "Orange"]),
					new("Sonic Barrier", ["Red", "Blue", "White"]),
					new("Sonic Barrier", ["Blue", "White", "Sky"]),
					new("Sonic Barrier", ["Blue", "Purple", "Red"]),
					new("Sonic Barrier", ["Blue", "Purple", "Sky"]),
					new("Restore Sharpness", ["Purple", "Yellow", "Orange", "Yellow"]),
					new("Restore Sharpness", ["Red", "Orange", "Purple"]),
					new("HIGHFREQ", ["White", "Red", "Red"]),
					new("HIGHFREQ", ["White", "Blue", "Sky"]),
					new("HIGHFREQ", ["White", "Blue", "Yellow"]),
					new("HIGHFREQ", ["White", "Blue", "Green"]),
					new("HIGHFREQ", ["Green", "Sky", "White"]),
					new("HIGHFREQ", ["Purple", "Blue", "Red"]),
					new("HIGHFREQ", ["Purple", "Sky", "Red"]),
					new("HIGHFREQ", ["Purple", "Yellow", "Red"]),
					new("HIGHFREQ", ["Purple", "Green", "Red"]),
					new("HIGHFREQ", ["Purple", "Orange", "Red"]),
					new("HIGHFREQ", ["Purple", "Blue", "Sky"]),
					new("HIGHFREQ", ["Purple", "Blue", "Yellow"]),
					new("HIGHFREQ", ["Purple", "Blue", "Green"]),
					new("HIGHFREQ", ["Orange", "Blue", "Orange"]),
					new("HIGHFREQ", ["Green", "Sky", "Purple"]),
					new("HIGHFREQ", ["White", "Yellow", "Green"]),
					new("HIGHFREQ", ["Orange", "Sky", "Orange"]),
					new("HIGHFREQ", ["Purple", "Yellow", "Green"]),
					new("HIGHFREQ", ["Purple", "Orange", "Green"]),
					new("HIGHFREQ", ["Orange", "Yellow", "Purple"]),
					new("Offset Melody", ["White", "Red", "Sky", "Sky"]),
					new("Offset Melody", ["Yellow", "Red", "Yellow", "White"]),
					new("Offset Melody", ["Yellow", "Red", "Yellow", "Purple"]),
					new("Offset Melody", ["Blue", "Sky", "White", "Sky"]),
					new("Offset Melody", ["Yellow", "Green", "Yellow", "White"]),
					new("Offset Melody", ["Purple", "Red", "Sky", "Sky"]),
					new("Offset Melody", ["Blue", "Sky", "Purple", "Sky"]),
					new("Offset Melody", ["Orange", "Blue", "Purple", "Orange"]),
					new("Offset Melody", ["Purple", "Sky", "Sky", "Orange"]),
					new("Offset Melody", ["Yellow", "Green", "Yellow", "Purple"]),
					new("Resounding Melody", ["White", "Red", "Blue", "Blue"]),
					new("Resounding Melody", ["Yellow", "Purple", "Orange", "Orange"]),
					new("Resounding Melody", ["Yellow", "Blue", "Yellow", "White"]),
					new("Resounding Melody", ["Purple", "Red", "Blue", "Blue"]),
					new("Resounding Melody", ["Purple", "Red", "Orange", "Orange"]),
					new("Resounding Melody", ["Yellow", "Blue", "Yellow", "Purple"]),
					new("Melody of Life", ["Green", "Blue", "Green", "White"]),
					new("Melody of Life", ["Sky", "Green", "Green", "White"]),
					new("Melody of Life", ["Yellow", "Sky", "Sky", "White"]),
					new("Melody of Life", ["Red", "Green", "Purple", "Green"]),
					new("Melody of Life", ["Sky", "Green", "Green", "Purple"]),
					new("Melody of Life", ["Yellow", "Sky", "Sky", "Purple"]),
					new("Melody of Life", ["Purple", "Green", "Green", "Orange"]),
					new("Melody of Life", ["Green", "Blue", "Green", "Purple"])
				];
			}
			else if (game == "MHWI")
			{
				return
				[
					new("Self-improvement", ["Purple", "Purple", "Disabled", "Disabled"]),
					new("Self-improvement", ["White", "White", "Disabled", "Disabled"]),
					new("Attack Up(S)", ["White", "Red", "Red", "Disabled"]),
					new("Attack Up(S)", ["Purple", "Red", "Yellow", "Disabled"]),
					new("Attack Up(S)", ["Yellow", "Purple", "Red", "Disabled"]),
					new("Attack Up(S)", ["Red", "Yellow", "Purple", "Disabled"]),
					new("Attack Up(L)", ["Purple", "Red", "Dark_Blue", "Purple"]),
					new("Attack Up(L)", ["Purple", "Red", "Green", "Purple"]),
					new("Attack Up(L)", ["Purple", "Red", "Light_Blue", "Purple"]),
					new("Health Boost(S)", ["Red", "Dark_Blue", "White", "Disabled"]),
					new("Health Boost(L)", ["Red", "Dark_Blue", "Red", "Purple"]),
					new("Stamina Use Reduced(S)", ["White", "Light_Blue", "Dark_Blue", "Disabled"]),
					new("Stamina Use Reduced(S)", ["White", "Yellow", "Dark_Blue", "Disabled"]),
					new("Stamina Use Reduced(S)", ["White", "Green", "Dark_Blue", "Disabled"]),
					new("Stamina Use Reduced(L)", ["Purple", "Light_Blue", "Dark_Blue", "Light_Blue"]),
					new("Stamina Use Reduced(L)", ["Purple", "Yellow", "Dark_Blue", "Disabled"]),
					new("Stamina Use Reduced(L)", ["Purple", "Green", "Dark_Blue", "Green"]),
					new("Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Red", "Disabled"]),
					new("Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Green", "Disabled"]),
					new("Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Light_Blue", "Disabled"]),
					new("All Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Yellow", "Purple"]),
					new("Defense Up(S)", ["White", "Dark_Blue", "Dark_Blue", "Disabled"]),
					new("Defense Up(L)", ["Purple", "Dark_Blue", "Dark_Blue", "Purple"]),
					new("Tool Use Drain Reduced(S)", ["White", "Dark_Blue", "Light_Blue", "Disabled"]),
					new("Tool Use Drain Reduced(L)", ["Purple", "Dark_Blue", "Light_Blue", "Disabled"]),
					new("Health Rec. (S)", ["White", "Green", "White", "Disabled"]),
					new("Health Rec. (S)", ["Purple", "Green", "Purple", "Disabled"]),
					new("Health Rec. (L)", ["Green", "Green", "Purple", "Light_Blue"]),
					new("Health Rec. (S) + Antidote", ["Green", "Dark_Blue", "White", "Dark_Blue"]),
					new("Health Rec. (M) + Antidote", ["Green", "Dark_Blue", "Purple", "Dark_Blue"]),
					new("Health Recovery(M)", ["Green", "White", "Light_Blue", "Green"]),
					new("Recovery Speed(S)", ["Green", "Green", "Red", "White"]),
					new("Recovery Speed(S)", ["Green", "Green", "Yellow", "Disabled"]),
					new("Recovery Speed(L)", ["Green", "Green", "Red", "Purple"]),
					new("Divine Protection", ["Green", "Yellow", "Purple", "Yellow"]),
					new("Scoutfly Power Up", ["Light_Blue", "Light_Blue", "Light_Blue", "Disabled"]),
					new("Muck Res", ["Light_Blue", "Red", "Light_Blue", "Disabled"]),
					new("Envir.Damage Negated", ["Red", "Red", "Light_Blue", "Disabled"]),
					new("Earplugs(S)", ["Light_Blue", "Light_Blue", "Red", "White"]),
					new("Earplugs(S)", ["Light_Blue", "Light_Blue", "Red", "Purple"]),
					new("Earplugs(S)", ["Light_Blue", "Light_Blue", "Green", "White"]),
					new("Earplugs(L)", ["Light_Blue", "Light_Blue", "Green", "Purple"]),
					new("Stun Negated", ["Light_Blue", "Dark_Blue", "Purple", "Disabled"]),
					new("Paralysis Negated", ["Light_Blue", "Yellow", "White", "Disabled"]),
					new("Paralysis Negated", ["Light_Blue", "Yellow", "Purple", "Disabled"]),
					new("Tremors Negated", ["Light_Blue", "Light_Blue", "Yellow", "Disabled"]),
					new("Fire Res Boost(S)", ["Yellow", "Red", "White", "Disabled"]),
					new("Fire Res Boost(L)", ["Yellow", "Red", "Purple", "Disabled"]),
					new("Water Res Boost(S)", ["Yellow", "Light_Blue", "White", "Disabled"]),
					new("Water Res Boost(L)", ["Yellow", "Light_Blue", "Purple", "Disabled"]),
					new("Thunder Res Boost(S)", ["Yellow", "Green", "White", "Disabled"]),
					new("Thunder Res Boost(L)", ["Yellow", "Green", "Purple", "Disabled"]),
					new("Ice Res Boost(S)", ["Yellow", "Dark_Blue", "White", "Disabled"]),
					new("Ice Res Boost(L)", ["Yellow", "Dark_Blue", "Purple", "Disabled"]),
					new("Dragon Res Boost(S)", ["White", "Yellow", "Light_Blue", "Disabled"]),
					new("Dragon Res Boost(L)", ["Purple", "Yellow", "Light_Blue", "Disabled"]),
					new("Elemental Attack Boost", ["Purple", "Green", "Yellow", "Green"]),
					new("Elemental Attack Boost", ["Yellow", "Light_Blue", "Yellow", "Light_Blue"]),
					new("Sonic Waves", ["Yellow", "Yellow", "Yellow", "Disabled"]),
					new("All Melody Effects Extended", ["Orange", "Red", "Orange", "Disabled"]),
					new("Knockbacks Negated", ["Red", "Orange", "Red", "Purple"]),
					new("All Ailments Negated", ["Orange", "Dark_Blue", "Purple", "Dark_Blue"]),
					new("All Ailments Negated", ["Purple", "Dark_Blue", "Orange", "Orange"]),
					new("All Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Orange", "Disabled"]),
					new("Stamina Use Reduced(L)", ["Purple", "Orange", "Dark_Blue", "Orange"]),
					new("Affinity Up/Health Rec. (S)", ["Green", "Orange", "Purple", "Orange"]),
					new("Earplugs(L)", ["Orange", "Orange", "Green", "Purple"]),
					new("Abnormal Status Atk.Increased", ["Light_Blue", "Orange", "Orange", "Purple"]),
					new("All Ailments Negated", ["Light_Blue", "Purple", "Orange", "Orange"]),
					new("Divine Protection", ["Purple", "Orange", "Purple", "Light_Blue"]),
					new("Blight Res Up", ["Yellow", "Orange", "Purple", "Disabled"]),
					new("Elemental Attack Boost", ["Purple", "Orange", "Yellow", "Orange"]),
					new("Blight Negated", ["Orange", "Yellow", "Yellow", "Orange"]),
					new("Earplugs(S) / Wind Pressure Negated", ["Purple", "Yellow", "Orange", "Yellow"]),
					new("Attack Up(L)", ["Purple", "Orange", "Orange", "Red"]),
					new("Echo Impact", ["Echo", "Red", "Disabled", "Disabled"]),
					new("Echo Dragon", ["Echo", "Orange", "Disabled", "Disabled"]),
					new("Max Stamina Up + Recovery", ["Echo", "Dark_Blue", "Disabled", "Disabled"]),
					new("Extend Health Recovery", ["Echo", "Green", "Disabled", "Disabled"]),
					new("Speed Boost + Evade Window", ["Echo", "Light_Blue", "Disabled", "Disabled"]),
					new("Elemental Effectiveness Up", ["Echo", "Yellow", "Disabled", "Disabled"]),
				];
			}
			else
			{
				return [];
			}
		}

		public static string GetGameFullName(string game, int? rarity = null)
		{
			return new Dictionary<string, string>()
			{
				{ "MH1", "Monster Hunter" },
				{ "MHG", "Monster Hunter G" },
				{ "MHF1", "Monster Hunter Freedom" },
				{ "MH2", "Monster Hunter 2" },
				{ "MHF2", "Monster Hunter Freedom 2" },
				{ "MHFU", "Monster Hunter Freedom Unite" },
				{ "MH3", "Monster Hunter 3" },
				{ "MHP3", "Monster Hunter Portable 3rd" },
				{ "MH3U", "Monster Hunter 3 Ultimate" },
				{ "MH4", "Monster Hunter 4" },
				{ "MH4U", "Monster Hunter 4 Ultimate" },
				{ "MHGen", "Monster Hunter Generations" },
				{ "MHGU", "Monster Hunter Generations Ultimate" },
				{ "MHWorld", "Monster Hunter: World" },
				{ "MHWI", rarity == null || rarity >= 9 ? "Monster Hunter World: Iceborne" : "Monster Hunter World" },
				{ "MHRise", "Monster Hunter Rise" },
				{ "MHRS", rarity == null || rarity >= 8 ? "Monster Hunter Rise: Sunbreak" : "Monster Hunter Rise" },
				{ "MHWilds", "Monster Hunter Wilds" },
				{ "MHNow", "Monster Hunter Now" },
				{ "MHOutlanders", "Monster Hunter Outlanders" },
				{ "MHST1", "Monster Hunter Stories" },
				{ "MHST2", "Monster Hunter Stories 2: Wings of Ruin" },
				{ "MHFrontier", "Monster Hunter Frontier" },
				{ "MHOnline", "Monster Hunter Online" },
				{ "MHExplore", "Monster Hunter Explore" },
				{ "MHRiders", "Monster Hunter Riders" },
				{ "MHGii", "Monster Hunter G (Wii)" },
				{ "MHP1", "Monster Hunter Portable (PSP)" },
				{ "MHP2", "Monster Hunter Portable 2nd (PSP)" },
				{ "MHP2G", "Monster Hunter Portable 2nd G (PSP)" },
				{ "MH3G", "Monster Hunter 3 G (Wii U, 3DS)" },
				{ "MH4G", "Monster Hunter 4G (3DS)" },
				{ "MHX", "Monster Hunter X (3DS)" },
				{ "MHXX ", "Monster Hunter XX (3DS, Nintendo Switch)" },
			}[game];
		}

		public static string GenerateFromJson(string json)
		{
			WebToolkitData data = WebToolkitData.FromJson(json);
			return Generate(data, [data]).Result;
		}

		public static string SingleGenerate(string game, WebToolkitData data, WebToolkitData[] src)
		{
			string wepName = GetWeaponTypeFullName(data.Type!).Item1;
			return Generate(data, src).Result;
		}

		public static Dictionary<WebToolkitData, string> MassGenerate(string game, bool genfiles = true)
		{
			Dictionary<WebToolkitData, string> ret = [];
			Monster[]? monsterList = null;
			WebToolkitData[] src = [];
			if (game == "MHWI")
			{
				WebToolkitData[] bmData = [.. BlademasterData.GetToolkitData().Where(x => x.Name != "HARDUMMY")];
				WebToolkitData[] gData = [.. GunnerData.GetToolkitData().Where(x => x.Name != "HARDUMMY")];
				src = new WebToolkitData[bmData.Length + gData.Length];
				bmData.CopyTo(src, 0);
				gData.CopyTo(src, bmData.Length);
				monsterList = [.. Directory.EnumerateDirectories(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\\MHWI").Select(x => new Monster(new DirectoryInfo(x).Name, "MHWI"))];
				foreach (WebToolkitData data in src)
				{
					data.MonsterName = monsterList.FirstOrDefault(x => x.Equipment.Weapons.Any(y => y.Name == data.Name))?.Name;
				}
			}
			else if (game == "MHWilds")
			{
				src = Models.Data.MHWilds.Weapon.GetWebToolkitData();
				foreach (WebToolkitData data in src)
				{
					string monsterName = data.Tree!.Replace(" Tree", "");
					if (monsterName.Contains("G."))
					{
						if (monsterName == "G. Ebony")
						{
							monsterName = "Guardian Ebony Odogaron";
						}
						else if (monsterName == "G. Fulgur")
						{
							monsterName = "Guardian Fulgur Anjanath";
						}
						else
						{
							monsterName = monsterName.Replace("G. ", "Guardian");
						}
					}
					data.MonsterName = monsterName;
				}
			}
			else
			{
				string[] allMonsters = ["Aknosom", "Almudron", "Amatsu", "Anjanath", "Apex Arzuros", "Apex Diablos", "Apex Mizutsune", "Apex Rathalos", "Apex Rathian", "Apex Zinogre", "Arzuros", "Astalos", "Aurora Somnacanth", "Barioth", "Barroth", "Basarios", "Bazelgeuse", "Bishaten", "Blood Orange Bishaten", "Chameleos", "Chaotic Gore Magala", "Crimson Glow Valstrax", "Daimyo Hermitaur", "Diablos", "Espinas", "Flaming Espinas", "Furious Rajang", "Gaismagorm", "Garangolm", "Gold Rathian", "Gore Magala", "Goss Harag", "Great Baggi", "Great Izuchi", "Great Wroggi", "Jyuratodus", "Khezu", "Kulu-Ya-Ku", "Kushala Daora", "Lagombi", "Lucent Nargacuga", "Lunagaron", "Magma Almudron", "Magnamalo", "Malzeno", "Mizutsune", "Nargacuga", "Narwa the Allmother", "Pukei-Pukei", "Pyre Rakna-Kadaki", "Rajang", "Rakna-Kadaki", "Rathalos", "Rathian", "Risen Chameleos", "Risen Crimson Glow Valstrax", "Risen Kushala Daora", "Risen Shagaru Magala", "Risen Teostra", "Royal Ludroth", "Scorned Magnamalo", "Seething Bazelgeuse", "Seregios", "Shagaru Magala", "Shogun Ceanataur", "Silver Rathalos", "Somnacanth", "Teostra", "Tetranadon", "Thunder Serpent Narwa", "Tigrex", "Tobi-Kadachi", "Velkhana", "Violet Mizutsune", "Volvidon", "Wind Serpent Ibushi", "Zinogre", "Altaroth", "Anteka", "Baggi", "Bnahabra", "Boggi", "Bombadgy", "Bullfango", "Ceanataur", "Delex", "Felyne", "Gajau", "Gargwa", "Gowngoat", "Hermitaur", "Hornetaur", "Izuchi", "Jaggi", "Jaggia", "Jagras", "Kelbi", "Kestodon", "Ludroth", "Melynx", "Popo", "Pyrantula", "Rachnoid", "Remobra", "Rhenoplos", "Slagtoth", "Uroktor", "Velociprey", "Vespoid", "Wroggi", "Zamite"];
				src = Models.Data.MHRS.Weapon.GetWebToolkitData();
				foreach (WebToolkitData data in src)
				{
					string monsterName = data.Tree!.Replace(" Tree", "");
					if (!string.IsNullOrEmpty(monsterName) && monsterName != "???")
					{
						switch (monsterName)
						{
							case "Bone":
							case "Bone 2":
							case "Aelucanth":
							case "Azure Star":
							case "Canyne":
							case "Chaos":
							case "Death Stench":
							case "Defender Weapon":
							case "Dragon":
							case "Edel":
							case "Great Sword":
							case "Guild":
							case "Guild 2":
							case "Hand-Me-Down":
							case "Ice":
							case "Jelly":
							case "Kamura":
							case "Long Sword":
							case "Magia":
							case "Makluva":
							case "Melahoa":
							case "Mosgharl":
							case "Ore":
							case "Ore 2":
							case "Paralysis":
							case "Poison":
							case "Rampage":
							case "Shell-Studded":
							case "Skalda":
							case "Smithy":
							case "Speartuna":
							case "Thunder":
							case "Vaik":
							case "Water":
								monsterName = "";
								break;
							case "Bnahabra (Dragon)":
							case "Bnahabra (Fire)":
							case "Bnahabra (Ice)":
							case "Bnahabra (Paralysis)":
							case "Bnahabra (Thunder)":
								monsterName = "Bnahabra";
								break;
							case "Bone Scythe":
								monsterName = "Tobi-Kadachi";
								break;
							case "Bone Scythe 2":
								break;
							case "Daimyo & Shogun":
							case "Daimyo":
								monsterName = "Daimyo Hermitaur";
								break;
							case "Fire":
								monsterName = "Rathalos";
								break;
							case "Ibushi":
								monsterName = "Wind Serpent Ibushi";
								break;
							case "Magmadron":
								monsterName = "Magma Almudron";
								break;
							case "Narwa":
								monsterName = "Thunder Serpent Narwa";
								break;
							case "Orangaten":
								monsterName = "Blood Orange Bishaten";
								break;
							case "Primordial":
								monsterName = "Primordial Malzeno";
								break;
							case "Shogun":
								monsterName = "Shogun Ceanataur";
								break;
							case "Pyre Rakna":
								monsterName = "Pyre Rakna-Kadaki";
								break;
							case "Seething Bazel":
								monsterName = "Seething Bazelgeuse";
								break;
							case "Scorned Magna":
								monsterName = "Scorned Magnamalo";
								break;
							case "Valstrax":
								monsterName = "Crimson Glow Valstrax";
								break;
							case "Baggi":
							case "Izuchi":
							case "Wroggi":
								monsterName = "Great " + monsterName;
								break;
							case "Jaggi & Jaggia":
								monsterName = "Great Jaggi";
								break;
							default:
								if (!allMonsters.Contains(monsterName))
								{
									monsterName = "";
								}
								break;
						}
					}
					data.MonsterName = monsterName;
				}
			}
			if (genfiles)
			{
				Directory.CreateDirectory($@"{System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath")}\MHWiki Generated Pages\{game}");
				foreach (string dir in Directory.EnumerateDirectories($@"{System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath")}\MHWiki Generated Pages\{game}"))
				{
					Directory.Delete(dir, true);
				}
			}
			foreach (WebToolkitData data in src)
			{
				string wepName = GetWeaponTypeFullName(data.Type!).Item1;
				string res = Generate(data, src).Result;
				ret.Add(data, res);
				if (genfiles)
				{
					Directory.CreateDirectory($@"{System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath")}\MHWiki Generated Pages\{game}\{wepName}\");
					string path = $@"{System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath")}\MHWiki Generated Pages\{game}\{wepName}\{data.Name!.Replace("\"", "")}.txt";
					if (File.Exists(path))
					{
						path = $@"{System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath")}\MHWiki Generated Pages\{game}\{wepName}\{data.Name!.Replace("\"", "")} ({GetRank(data.Game!, data.Rarity)}).txt";
					}
					File.WriteAllText(path, res);
				}
			}
			return ret;
		}

		public static string GetRank(string game, long? rarity)
		{
			if (rarity == null)
			{
				return "LR";
			}
			else
			{
				return game switch
				{
					"MHWI" => rarity < 5 ? "LR" : rarity < 9 ? "HR" : "MR",
					"MHRS" => rarity < 4 ? "LR" : rarity < 8 ? "HR" : "MR",
					"MHWilds" => rarity < 5 ? "LR" : rarity < 9 ? "HR" : "MR",
					_ => "LR",
				};
			}
		}

		private static string GetSharpnessTemplates(string? input, string? game)
		{
			if (input == null)
			{
				return "";
			}
			string[][] data = JsonConvert.DeserializeObject<string[][]>(input!)!;
			string ret = $"|Sharpness               = {{{{SharpnessBar|{data[0][0]}|{data[0][1]}|{data[0][2]}|{data[0][3]}|{data[0][4]}|{data[0][5]}|{data[0][6]}|Game={game}";
			if (data.Length > 1)
			{
				char[] handiChars = ['R', 'O', 'Y', 'G', 'B', 'W', 'P'];
				for (int i = 0; i < 7; i++)
				{
					int regHits = Convert.ToInt32(data[0][i]);
					int handiHits = Convert.ToInt32(data[1][i]);
					ret += $"|H{handiChars[i]}={handiHits}";
				}
			}
			ret += "}}";
			return ret;
		}

		public static string GetMaterialsTemplates(string? input, string? game)
		{
			if (input == null)
			{
				return "";
			}
			StringBuilder ret = new();
			string[] sparkleItems = ["Bird Wyvern Gem", "Beast Gem", "Wyvern Gem", "Uth Duna Plate", "Uth Duna Watergem", "Rey Dau Plate", "Rey Dau Boltgem", "Nu Udra Cerebrospinal Fluid", "Nu Udra Flamegem", "Guardian Rathalos Plate", "Guardian Rathalos Ruby", "Guardian Ebony Plate", "Guardian Ebony Gem", "Xu Wu Cerebrospinal Fluid", "Xu Wu Umbragem", "Rathian Ruby", "Guardian Fulgur Gem", "Rathalos Ruby", "Jin Dahaad Icegem", "Gore Magala Nyctgem", "Arkveld Gem", "Mizutsune Water Orb", "Faux Whitegleam Orb", "Rey Dau Certificate γ", "Sharp Kunafa Cheese", "Delishroom", "Turbid Shrimp", "Airy Egg", "Specialty Sild Garlic", "Queensbloom Pollen", "Nightflower Pollen", "Wyvernsprout"];
			Dictionary<string, string>[] data = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(input!)!;
			int cntr = 1;
			foreach (Dictionary<string, string> dataObj in data)
			{
				string prefix = "";
				string suffix = "";
				if (cntr > 1)
				{
					prefix = "-->";
				}
				if (cntr < data.Length)
				{
					suffix = "<br><!--";
				}
				string sparkle = "";
				if (sparkleItems.Contains(dataObj["name"]))
				{
					sparkle = "|Sparkle=y";
				}
				if (dataObj["icon"] == "MATERIAL_NOICON")
				{
					ret.AppendLine($@"{prefix}{{{{GenericItemLink|{game}|{dataObj["name"]}}}}} {dataObj["quantity"]} {(dataObj["quantity"] == "1" ? "pt" : "pts")}.{suffix}");
				}
				else
				{
					ret.AppendLine($@"{prefix}{{{{IconPickerUniversalAlt|{game}|{dataObj["icon"]}|{dataObj["name"]}|color={GetColorString(dataObj["color"])}{sparkle}}}}} x{dataObj["quantity"]}{suffix}");
				}
				cntr++;
			}
			return ret.ToString();
		}

		public static string GetColorString(string src)
		{
			return src.Replace("DarkBlue", "Dark Blue").Replace("LightBlue", "Light Blue").Replace("DarkPurple", "Dark Purple").Replace("LightGreen", "Light Green");
		}
	}
}
