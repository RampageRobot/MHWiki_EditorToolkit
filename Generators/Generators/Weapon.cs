using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MediawikiTranslator.Models.Data.MHRS;
using MediawikiTranslator.Models.Data.MHWI;
using MediawikiTranslator.Models.Weapon;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace MediawikiTranslator.Generators
{
	public class Weapon
	{
		private static Dictionary<Tuple<string, string>, string[]> WeaponRenderFileNames { get; set; } = [];
		private static Models.Data.MHWI.Items[] _mhwiItems = Models.Data.MHWI.Items.Fetch();
		private static async Task<string> Generate(WebToolkitData weapon, WebToolkitData[] src)
		{
			return await Task.Run(() =>
			{
				Tuple<string, string, string> weaponNames = GetWeaponTypeFullName(weapon.Type!);
				Tuple<string, string> renderKey = new(weapon.Game!, weaponNames.Item1);
				if (!WeaponRenderFileNames.TryGetValue(renderKey, out string[]? value))
				{
					value = Utilities.GetWeaponRenders(renderKey.Item1, renderKey.Item2).Result;
					WeaponRenderFileNames.Add(renderKey, value);
				}
				string? match = value.FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.StartsWith($"File:{weapon.Game!}-{SanitizeRoman(weapon.Name!)}"));
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
				int bloat = Convert.ToInt32(Math.Round(GetWeaponBloat(weapon.Type!, weapon.Game!) * Convert.ToInt32(weapon.Attack)));
				ret.AppendLine($@"{{{{GenericNav|{weapon.Game}}}}}
<br>
<br>
The {weapon.Name} is {(weapon.Type == "IG" ? "an" : "a")} [[{weaponNames.Item1} ({weapon.Game})|{weaponNames.Item2}]] in [[{GetGameFullName(weapon.Game!)}]]. To view the upgrade relationships between {weaponNames.Item3}, see the  [[{weapon.Game}/{weaponNames.Item1} Tree|{weapon.Game} {weaponNames.Item1} Weapon Tree]].{ktMessage}");
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
(!string.IsNullOrEmpty(weapon.Element1) && weapon.ElementDmg1 != null ? "\r\n|Elemental Damage        =" + (weapon.ElementDmg1.Contains("(") ? "(" : "") + (Convert.ToInt32(weapon.ElementDmg1.Replace("(", "").Replace(")", "")) * 10) + (weapon.ElementDmg1.Contains("(") ? ")" : "") : "") +
(!string.IsNullOrEmpty(weapon.Element1) && weapon.ElementDmg1 != null ? "\r\n|Elemental Damage Type   =" + weapon.Element1.Replace("Paralyze", "Paralysis").Replace("Explosion", "Blast").Replace("(", "").Replace(")", "") : "") +
(!string.IsNullOrEmpty(weapon.Element2) && weapon.ElementDmg2 != null ? "\r\n|Elemental Damage 2      =" + (weapon.ElementDmg2.Contains("(") ? "(" : "") + (Convert.ToInt32(weapon.ElementDmg2.Replace("(", "").Replace(")", "")) * 10) + (weapon.ElementDmg2.Contains("(") ? ")" : "") : "") +
(!string.IsNullOrEmpty(weapon.Element2) && weapon.ElementDmg2 != null ? "\r\n|Elemental Damage Type 2 =" + weapon.Element2.Replace("Paralyze", "Paralysis").Replace("Explosion", "Blast").Replace("(", "").Replace(")", "") : "") +
(!string.IsNullOrEmpty(weapon.Sharpness) ? "\r\n" + GetSharpnessTemplates(weapon.Sharpness) : "") +
(!string.IsNullOrEmpty(weapon.HhNote1) ? $"\r\n|HH Note 1               = {weapon.HhNote1.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan")}" : "") +
(!string.IsNullOrEmpty(weapon.HhNote2) ? $"\r\n|HH Note 2               = {weapon.HhNote2.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan")}" : "") +
(!string.IsNullOrEmpty(weapon.HhNote3) ? $"\r\n|HH Note 3               = {weapon.HhNote3.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan")}" : "") +
(!string.IsNullOrEmpty(weapon.GlShellingType) ? $"\r\n|GL Shelling Type        = {weapon.GlShellingType} Lv {weapon.GlShellingLevel}" : "") +
(!string.IsNullOrEmpty(weapon.SaPhialType) ? $"\r\n|SA Phial Type           = {weapon.SaPhialType}" : "") +
(!string.IsNullOrEmpty(weapon.CbPhialType) ? $"\r\n|CB Phial Type           = {weapon.CbPhialType}" : "") +
(!string.IsNullOrEmpty(weapon.IgKinsectBonus) ? $"\r\n|IG Kinsect Bonus        = {weapon.IgKinsectBonus}" : "") +
(!string.IsNullOrEmpty(weapon.BoCoatings) ? $"\r\n|Bo Coatings             = {weapon.BoCoatings}" : "") +
(!string.IsNullOrEmpty(weapon.HhMelodies) ? $"\r\n|HH Melodies             = {weapon.HhMelodies}" : "") +
(!string.IsNullOrEmpty(weapon.HbgSpecialAmmoType) ? $"\r\n|HBG Special Ammo Type   = {weapon.HbgSpecialAmmoType}" : "") +
(!string.IsNullOrEmpty(weapon.LbgSpecialAmmoType) ? $"\r\n|LBG Special Ammo Type   = {weapon.LbgSpecialAmmoType}" : "") +
(!string.IsNullOrEmpty(weapon.HbgDeviation) ? $"\r\n|HBG Deviation        = {weapon.HbgDeviation}" : "") +
(!string.IsNullOrEmpty(weapon.Elderseal) && weapon.Elderseal != "None" ? $"\r\n|Elderseal               = {weapon.Elderseal}" : "") +
(!string.IsNullOrEmpty(weapon.ArmorSkills) ? $"\r\n|Armor Skills            = {weapon.ArmorSkills}" : "") +
(!string.IsNullOrEmpty(weapon.RampageSkillSlots) ? $"\r\n|Rampage Slots           = {weapon.RampageSkillSlots}" : "") +
(!string.IsNullOrEmpty(weapon.RampageDecoration) ? $"\r\n|Rampage Decoration      = {weapon.RampageDecoration}" : "") +
(!string.IsNullOrEmpty(weapon.Rollback) ? $"\r\n|Can Rollback            = {weapon.Rollback}" : ""));
				if (!string.IsNullOrEmpty(weapon.ForgeMaterials))
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
				ret.AppendLine(@"}}");
				if (weapon.Type == "HH" && weapon.Game != "MHRS")
				{
					List<Tuple<string, string[]>> melodies = GetHHMelodies();
					ret.AppendLine(@"==Melodies==
{| class=""wikitable"" style=""margin-left:auto; margin-right:auto; text-align:center;""
! style=""min-width:100px;""|Sequence !! Melody !! Effect
|-");
					string[] validNotes = [..new string[] { weapon.HhNote1!, weapon.HhNote2!, weapon.HhNote3! }.Where(x => x != "Disabled")];
					foreach (Tuple<string, string[]> melody in melodies.Where(x => !x.Item2.Where(y => y != "Echo" && y != "Disabled").Any(y => !validNotes.Contains(y.Replace("_", " ")))))
					{
						string melodyNotes = "";
						foreach (string note in melody.Item2.Where(x => x != "Disabled"))
						{
							if (note == "Echo")
							{
								melodyNotes += "{{UI|{{{Game|MHWI}}}|HH Echo|nolink=true}}";
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
								melodyNotes += $"{{{{UI|{{{{{{Game|MHWI}}}}}}|HH Note|{noteFile} {note.Replace("_", " ").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan")}|nolink=true}}}}";
							}
						}
						Tuple<string, string> melodyInfo = GetMelodyEffect(melody.Item1);
						ret.AppendLine(@$"|{melodyNotes} || {melodyInfo.Item1} || {melodyInfo.Item2}
|-");
					}
					ret.AppendLine("|}");
				}
				if (!string.IsNullOrEmpty(weapon.ShellTableWikitext) && weapon.Type != "Bo")
				{
					ret.AppendLine($@"==Ammunition==
{{{{AmmoTableLegend}}}}
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
				ret.AppendLine(@$"[[Category:{weaponNames.Item1}]]
[[Category:{weapon.Game} Weapons]]
[[Category:{weapon.Game} {weaponNames.Item1}]]");
				if (weapon.Element1 != null && weapon.Element1 != "None")
				{
					ret.AppendLine($"[[Category:{weapon.Element1} Weapons]]");
				}
				if (weapon.Element2 != null && weapon.Element2 != "None")
				{
					ret.AppendLine($"[[Category:{weapon.Element2} Weapons]]");
				}
				if (match == null)
				{
					ret.AppendLine($"[[Category:{weapon.Game!} Weapons Without Renders]]");
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

		private static Tuple<string, string> GetMelodyEffect(string melody)
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
				{ "Affinity Up and Health Rec. (S)", new Tuple<string, string>("Increases your critical damage and recovers a small amount of health.", "") },
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

		private static List<Tuple<string, string[]>> GetHHMelodies()
		{
			return new List<Tuple<string, string[]>>()
			{
				new Tuple<string, string[]>("Self-improvement", ["Purple", "Purple", "Disabled", "Disabled"]),
				new Tuple<string, string[]>("Self-improvement", ["White", "White", "Disabled", "Disabled"]),
				new Tuple<string, string[]>("Attack Up(S)", ["White", "Red", "Red", "Disabled"]),
				new Tuple<string, string[]>("Attack Up(S)", ["Purple", "Red", "Yellow", "Disabled"]),
				new Tuple<string, string[]>("Attack Up(S)", ["Yellow", "Purple", "Red", "Disabled"]),
				new Tuple<string, string[]>("Attack Up(S)", ["Red", "Yellow", "Purple", "Disabled"]),
				new Tuple<string, string[]>("Attack Up(L)", ["Purple", "Red", "Dark_Blue", "Purple"]),
				new Tuple<string, string[]>("Attack Up(L)", ["Purple", "Red", "Green", "Purple"]),
				new Tuple<string, string[]>("Attack Up(L)", ["Purple", "Red", "Light_Blue", "Purple"]),
				new Tuple<string, string[]>("Health Boost(S)", ["Red", "Dark_Blue", "White", "Disabled"]),
				new Tuple<string, string[]>("Health Boost(L)", ["Red", "Dark_Blue", "Red", "Purple"]),
				new Tuple<string, string[]>("Stamina Use Reduced(S)", ["White", "Light_Blue", "Dark_Blue", "Disabled"]),
				new Tuple<string, string[]>("Stamina Use Reduced(S)", ["White", "Yellow", "Dark_Blue", "Disabled"]),
				new Tuple<string, string[]>("Stamina Use Reduced(S)", ["White", "Green", "Dark_Blue", "Disabled"]),
				new Tuple<string, string[]>("Stamina Use Reduced(L)", ["Purple", "Light_Blue", "Dark_Blue", "Light_Blue"]),
				new Tuple<string, string[]>("Stamina Use Reduced(L)", ["Purple", "Yellow", "Dark_Blue", "Disabled"]),
				new Tuple<string, string[]>("Stamina Use Reduced(L)", ["Purple", "Green", "Dark_Blue", "Green"]),
				new Tuple<string, string[]>("Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Red", "Disabled"]),
				new Tuple<string, string[]>("Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Green", "Disabled"]),
				new Tuple<string, string[]>("Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Light_Blue", "Disabled"]),
				new Tuple<string, string[]>("All Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Yellow", "Purple"]),
				new Tuple<string, string[]>("Defense Up(S)", ["White", "Dark_Blue", "Dark_Blue", "Disabled"]),
				new Tuple<string, string[]>("Defense Up(L)", ["Purple", "Dark_Blue", "Dark_Blue", "Purple"]),
				new Tuple<string, string[]>("Tool Use Drain Reduced(S)", ["White", "Dark_Blue", "Light_Blue", "Disabled"]),
				new Tuple<string, string[]>("Tool Use Drain Reduced(L)", ["Purple", "Dark_Blue", "Light_Blue", "Disabled"]),
				new Tuple<string, string[]>("Health Rec. (S)", ["White", "Green", "White", "Disabled"]),
				new Tuple<string, string[]>("Health Rec. (S)", ["Purple", "Green", "Purple", "Disabled"]),
				new Tuple<string, string[]>("Health Rec. (L)", ["Green", "Green", "Purple", "Light_Blue"]),
				new Tuple<string, string[]>("Health Rec. (S) + Antidote", ["Green", "Dark_Blue", "White", "Dark_Blue"]),
				new Tuple<string, string[]>("Health Rec. (M) + Antidote", ["Green", "Dark_Blue", "Purple", "Dark_Blue"]),
				new Tuple<string, string[]>("Health Recovery(M)", ["Green", "White", "Light_Blue", "Green"]),
				new Tuple<string, string[]>("Recovery Speed(S)", ["Green", "Green", "Red", "White"]),
				new Tuple<string, string[]>("Recovery Speed(S)", ["Green", "Green", "Yellow", "Disabled"]),
				new Tuple<string, string[]>("Recovery Speed(L)", ["Green", "Green", "Red", "Purple"]),
				new Tuple<string, string[]>("Divine Protection", ["Green", "Yellow", "Purple", "Yellow"]),
				new Tuple<string, string[]>("Scoutfly Power Up", ["Light_Blue", "Light_Blue", "Light_Blue", "Disabled"]),
				new Tuple<string, string[]>("Muck Res", ["Light_Blue", "Red", "Light_Blue", "Disabled"]),
				new Tuple<string, string[]>("Envir.Damage Negated", ["Red", "Red", "Light_Blue", "Disabled"]),
				new Tuple<string, string[]>("Earplugs(S)", ["Light_Blue", "Light_Blue", "Red", "White"]),
				new Tuple<string, string[]>("Earplugs(S)", ["Light_Blue", "Light_Blue", "Red", "Purple"]),
				new Tuple<string, string[]>("Earplugs(S)", ["Light_Blue", "Light_Blue", "Green", "White"]),
				new Tuple<string, string[]>("Earplugs(L)", ["Light_Blue", "Light_Blue", "Green", "Purple"]),
				new Tuple<string, string[]>("Stun Negated", ["Light_Blue", "Dark_Blue", "Purple", "Disabled"]),
				new Tuple<string, string[]>("Paralysis Negated", ["Light_Blue", "Yellow", "White", "Disabled"]),
				new Tuple<string, string[]>("Paralysis Negated", ["Light_Blue", "Yellow", "Purple", "Disabled"]),
				new Tuple<string, string[]>("Tremors Negated", ["Light_Blue", "Light_Blue", "Yellow", "Disabled"]),
				new Tuple<string, string[]>("Fire Res Boost(S)", ["Yellow", "Red", "White", "Disabled"]),
				new Tuple<string, string[]>("Fire Res Boost(L)", ["Yellow", "Red", "Purple", "Disabled"]),
				new Tuple<string, string[]>("Water Res Boost(S)", ["Yellow", "Light_Blue", "White", "Disabled"]),
				new Tuple<string, string[]>("Water Res Boost(L)", ["Yellow", "Light_Blue", "Purple", "Disabled"]),
				new Tuple<string, string[]>("Thunder Res Boost(S)", ["Yellow", "Green", "White", "Disabled"]),
				new Tuple<string, string[]>("Thunder Res Boost(L)", ["Yellow", "Green", "Purple", "Disabled"]),
				new Tuple<string, string[]>("Ice Res Boost(S)", ["Yellow", "Dark_Blue", "White", "Disabled"]),
				new Tuple<string, string[]>("Ice Res Boost(L)", ["Yellow", "Dark_Blue", "Purple", "Disabled"]),
				new Tuple<string, string[]>("Dragon Res Boost(S)", ["White", "Yellow", "Light_Blue", "Disabled"]),
				new Tuple<string, string[]>("Dragon Res Boost(L)", ["Purple", "Yellow", "Light_Blue", "Disabled"]),
				new Tuple<string, string[]>("Elemental Attack Boost", ["Purple", "Green", "Yellow", "Green"]),
				new Tuple<string, string[]>("Elemental Attack Boost", ["Yellow", "Light_Blue", "Yellow", "Light_Blue"]),
				new Tuple<string, string[]>("Sonic Waves", ["Yellow", "Yellow", "Yellow", "Disabled"]),
				new Tuple<string, string[]>("All Melody Effects Extended", ["Orange", "Red", "Orange", "Disabled"]),
				new Tuple<string, string[]>("Knockbacks Negated", ["Red", "Orange", "Red", "Purple"]),
				new Tuple<string, string[]>("All Ailments Negated", ["Orange", "Dark_Blue", "Purple", "Dark_Blue"]),
				new Tuple<string, string[]>("All Ailments Negated", ["Purple", "Dark_Blue", "Orange", "Orange"]),
				new Tuple<string, string[]>("All Wind Pressure Negated", ["Dark_Blue", "Dark_Blue", "Orange", "Disabled"]),
				new Tuple<string, string[]>("Stamina Use Reduced(L)", ["Purple", "Orange", "Dark_Blue", "Orange"]),
				new Tuple<string, string[]>("Affinity Up and Health Rec. (S)", ["Green", "Orange", "Purple", "Orange"]),
				new Tuple<string, string[]>("Earplugs(L)", ["Orange", "Orange", "Green", "Purple"]),
				new Tuple<string, string[]>("Abnormal Status Atk.Increased", ["Light_Blue", "Orange", "Orange", "Purple"]),
				new Tuple<string, string[]>("All Ailments Negated", ["Light_Blue", "Purple", "Orange", "Orange"]),
				new Tuple<string, string[]>("Divine Protection", ["Purple", "Orange", "Purple", "Light_Blue"]),
				new Tuple<string, string[]>("Blight Res Up", ["Yellow", "Orange", "Purple", "Disabled"]),
				new Tuple<string, string[]>("Elemental Attack Boost", ["Purple", "Orange", "Yellow", "Orange"]),
				new Tuple<string, string[]>("Blight Negated", ["Orange", "Yellow", "Yellow", "Orange"]),
				new Tuple<string, string[]>("Earplugs(S) / Wind Pressure Negated", ["Purple", "Yellow", "Orange", "Yellow"]),
				new Tuple<string, string[]>("Attack Up(L)", ["Purple", "Orange", "Orange", "Red"]),
				new Tuple<string, string[]>("Echo Impact", ["Echo", "Red", "Disabled", "Disabled"]),
				new Tuple<string, string[]>("Echo Dragon", ["Echo", "Orange", "Disabled", "Disabled"]),
				new Tuple<string, string[]>("Max Stamina Up + Recovery", ["Echo", "Dark_Blue", "Disabled", "Disabled"]),
				new Tuple<string, string[]>("Extend Health Recovery", ["Echo", "Green", "Disabled", "Disabled"]),
				new Tuple<string, string[]>("Speed Boost + Evade Window", ["Echo", "Light_Blue", "Disabled", "Disabled"]),
				new Tuple<string, string[]>("Elemental Effectiveness Up", ["Echo", "Yellow", "Disabled", "Disabled"]),
			};

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
			WebToolkitData[] src = [];
			if (game == "MHWI")
			{
				WebToolkitData[] bmData = [..BlademasterData.GetToolkitData().Where(x => x.Name != "HARDUMMY")];
				WebToolkitData[] gData = [.. GunnerData.GetToolkitData().Where(x => x.Name != "HARDUMMY")];
				src = new WebToolkitData[bmData.Length + gData.Length];
				bmData.CopyTo(src, 0);
				gData.CopyTo(src, bmData.Length);
			}
			else
			{
				src = Models.Data.MHRS.Weapon.GetWebToolkitData();
			}
			if (genfiles)
			{
				Directory.CreateDirectory($@"C:\Users\mkast\Desktop\MHWiki Generated Pages\{game}");
				foreach (string dir in Directory.EnumerateDirectories($@"C:\Users\mkast\Desktop\MHWiki Generated Pages\{game}"))
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
					Directory.CreateDirectory($@"C:\Users\mkast\Desktop\MHWiki Generated Pages\{game}\{wepName}\");
					string path = $@"C:\Users\mkast\Desktop\MHWiki Generated Pages\{game}\{wepName}\{data.Name!.Replace("\"", "")}.txt";
					if (File.Exists(path))
					{
						path = $@"C:\Users\mkast\Desktop\MHWiki Generated Pages\{game}\{wepName}\{data.Name!.Replace("\"", "")} ({GetRank(data.Game!, data.Rarity)}).txt";
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
					_ => "LR",
				};
			}
		}

		private static string GetSharpnessTemplates(string? input)
		{
			if (input == null)
			{
				return "";
			}
			string[][] data = JsonConvert.DeserializeObject<string[][]>(input!)!;
			string ret = $"|Sharpness               = {{{{MHWISharpnessBase|{data[0][0]}|{data[0][1]}|{data[0][2]}|{data[0][3]}|{data[0][4]}|{data[0][5]}|{data[0][6]}}}}}";
			if (data.Length > 1)
			{
				ret += $"\r\n|Sharpness Handi+        = {{{{MHWISharpnessBase|{data[1][0]}|{data[1][1]}|{data[1][2]}|{data[1][3]}|{data[1][4]}|{data[1][5]}|{data[1][6]}}}}}";
			}
			return ret;
		}

		public static string GetMaterialsTemplates(string? input, string? game)
		{
			if (input == null)
			{
				return "";
			}
			StringBuilder ret = new();
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
				if (dataObj["icon"] == "MATERIAL_NOICON")
				{
					ret.AppendLine($@"{prefix}{{{{GenericMaterialLink|{game}|{dataObj["name"]}}}}} {dataObj["quantity"]} {(dataObj["quantity"] == "1" ? "pt" : "pts")}.{suffix}");
				}
				else
				{
					ret.AppendLine($@"{prefix}{{{{GenericItemLink|{game}|{dataObj["name"]}|{dataObj["icon"]}|{GetColorString(dataObj["color"])}}}}} x{dataObj["quantity"]}{suffix}");
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
