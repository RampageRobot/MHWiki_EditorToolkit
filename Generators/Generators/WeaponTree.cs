using DocumentFormat.OpenXml.Wordprocessing;
using MediawikiTranslator.Models.DamageTable.PartsData;
using MediawikiTranslator.Models.WeaponTree;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace MediawikiTranslator.Generators
{
	public class WeaponTree
	{
		public static string ParseJson(string json, string sharpnessBase, int maxSharpnessCount, string pathName, string defaultIcon)
		{
			return Generate(WebToolkitData.FromJson(json), sharpnessBase, maxSharpnessCount, pathName, defaultIcon).Result;
		}

		public static async Task<string> Generate(WebToolkitData[] srcData, string sharpnessBase, int maxSharpnessCount, string pathName, string defaultIcon)
		{
			return await Task.Run(() =>
			{
				if (srcData.Length == 0 || !srcData.Any(x => x.Data.Any(y => !string.IsNullOrEmpty(y.Name))))
				{
					return "";
				}
				StringBuilder ret = new();
				ret.AppendLine($@"{{{{WeaponTreeLegend|{srcData.FirstOrDefault()?.Game ?? "MHWI"}|{(WebToolkitData.GetWeaponName(string.IsNullOrEmpty(srcData.FirstOrDefault()?.Data?.FirstOrDefault()?.IconType) ? defaultIcon : srcData.FirstOrDefault()?.Data?.FirstOrDefault()?.IconType))}}}}}");
				foreach (WebToolkitData dataArray in srcData)
				{
					bool tableHasElderseal = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.Elderseal));
					bool tableHasRampageSlots = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.RampageSlots) && x.RampageSlots != "0");
					bool tableHasRampageDecos = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.RampageDeco) && x.RampageDeco != "0");
					bool tableHasArmorSkills = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.ArmorSkill));
					int cntr = 0;
					ret.AppendLine($@"<br>
{{| class=""wikitable center wide mw-collapsible mw-collapsed""
! colspan=12 | <h4 style=""margin:0px;"">{dataArray.PathName} Path</h4>
|-
!Name 
!Rarity
!{{{{Element|Attack}}}}
!{{{{Element|Element}}}}
!{{{{Element|Affinity}}}}");
					string tableIconType = dataArray.Data.Length > 0 ? dataArray.Data[0].IconType : "GS";
					if (string.IsNullOrEmpty(tableIconType))
					{
						tableIconType = defaultIcon;
					}
					switch (tableIconType)
					{
						case "Bo":
							ret.AppendLine("!{{UI|MHWI|Bow Coatings}}");
							break;
						case "CB":
							ret.AppendLine("!{{UI|MHWI|CB Phial Type}}");
							break;
						case "SA":
							ret.AppendLine("!{{UI|MHWI|SA Phial Type}}");
							break;
						case "GL":
							ret.AppendLine("!{{UI|MHWI|GL Shelling Type}}");
							break;
						case "HH":
							ret.AppendLine("!{{UI|MHWI|HH Menu Notes}}");
							break;
						case "IG":
							ret.AppendLine("!{{UI|MHWI|IG Kinsect Bonus}}");
							break;
						case "HBG":
							ret.AppendLine("!{{UI|MHWI|HBG Special Ammo}}");
							if (dataArray.Game != "MHWI")
							{
								ret.AppendLine("!Reload / Recoil");
							}
							break;
						case "LBG":
							ret.AppendLine("!{{UI|MHWI|LBG Special Ammo}}");
							if (dataArray.Game != "MHWI")
							{
								ret.AppendLine("!Reload / Recoil");
							}
							break;
						default: break;
					}
					if (tableHasElderseal)
					{
						ret.AppendLine(@"!{{Element|Elderseal}}");
					}
					if (tableHasRampageSlots)
					{
						ret.AppendLine(@"!Rmpg. Slots");
					}
					if (tableHasRampageDecos)
					{
						ret.AppendLine(@"![[File:UI-Rampage Decoration 3.png|20x20px|center|link=]]");
					}
					if (tableHasArmorSkills)
					{
						ret.AppendLine(@"![[File:UI-Blights Negated.png|20x20px|center|Link=]]");
					}
					if (!new string[] { "Bo", "HBG", "LBG" }.Contains(tableIconType))
					{
						ret.AppendLine(@"![[File:2ndGen-Whetstone Icon Yellow.png|24x24px|link=]]");
					}
					else
					{
						ret.AppendLine("!");
					}
					ret.AppendLine(@"![[File:2ndGen-Decoration Icon Blue.png|24x24px|link=]] 
!{{Element|Defense}}");
					foreach (Datum dataObj in dataArray.Data)
					{
						string iconType = dataObj.IconType;
						if (string.IsNullOrEmpty(iconType))
						{
							iconType = defaultIcon;
						}
						string prefix = GetPrefix(dataArray, dataObj, cntr);
						string sharpness = "";
						if (!string.IsNullOrEmpty(dataObj.Sharpness))
						{
							string[][] objSharpness = Newtonsoft.Json.JsonConvert.DeserializeObject<string[][]>(dataObj.Sharpness)!;
							for (var i2 = 0; i2 < objSharpness.Length; i2++)
							{
								var barObj = objSharpness[i2];
								sharpness += "{{" + sharpnessBase;
								var cnt = maxSharpnessCount > -1 ? maxSharpnessCount : barObj.Length;
								for (var i3 = 0; i3 < cnt; i3++)
								{
									sharpness += "|" + (barObj[i3] != "" ? barObj[i3] : 0);
								}
								sharpness += "}}";
								if (i2 != objSharpness.Length - 1)
								{
									sharpness += "\n";
								}
							}
						}
						string decos = "";
						if (!string.IsNullOrEmpty(dataObj.Decos))
						{
							Decoration[] objDecos = [.. Newtonsoft.Json.JsonConvert.DeserializeObject<Decoration[]>(dataObj.Decos)!.OrderBy(x => !x.IsRampage).ThenBy(x => x.Level)];
							foreach (Decoration deco in objDecos)
							{
								for (int i = 0; i < deco.Qty; i++)
								{
									decos += $"{{{{{(deco.IsRampage ? "RampageDeco" : "5thDeco")}|{deco.Level}}}}}";
								}
							}
						}
						ret.AppendLine($@"|-
| style=""text-align:left"" | {prefix}{{{{GenericWeaponLink|{dataArray.Game}|{dataObj.Name}|{iconType}|{dataObj.Rarity}{(dataObj.CanForge == true ? "|true" : "")}{(dataObj.CanRollback == true ? (dataObj.CanForge != true ? "||true" : "|true") : "")}}}}}{(!string.IsNullOrEmpty(dataObj.PathLink) ? $" [[#{dataObj.PathLink} Path|(Go to path)]]" : "")}
| {dataObj.Rarity}
| {dataObj.Attack + " (" + Convert.ToInt32(Math.Round(Convert.ToInt32(dataObj.Attack) / Weapon.GetWeaponBloat(iconType, dataArray.Game))) + ")"}
| {(string.IsNullOrEmpty(dataObj.Element) && dataObj.Element != "0" ? "-" : $"{{{{Element|{dataObj.Element}|{dataObj.ElementDamage}}}}}")}
| {((dataObj.Affinity == 0 || dataObj.Affinity == null) ? "0%" : dataObj.Affinity + "%")}");
						switch (iconType)
						{
							case "Bo":
								ret.AppendLine($"|{string.Join(", ", dataObj.BoCoatings.Split(',').Select(x => x.Trim()))}");
								break;
							case "CB":
								ret.AppendLine($"|{dataObj.CBPhialType}");
								break;
							case "SA":
								ret.AppendLine($"|{dataObj.SAPhialType}");
								break;
							case "GL":
								ret.AppendLine($"|{dataObj.GLShellingType}");
								break;
							case "HH":
								ret.AppendLine($"|{{{{UI|MHWI|HH Note|1 {dataObj.HHNote1}}}}}{{{{UI|MHWI|HH Note|2 {dataObj.HHNote2}}}}}{{{{UI|MHWI|HH Note|3 {dataObj.HHNote3}}}}}");
								break;
							case "IG":
								ret.AppendLine($"|{dataObj.IGKinsectBonus}");
								break;
							case "HBG":
								ret.AppendLine($"|{dataObj.HBGSpecialAmmoType}");
								if (dataArray.Game != "MHWI")
								{
									ret.AppendLine($"|{dataObj.HBGReloadRecoil}");
								}
								else
								{
									ret.AppendLine("|");
								}
								break;
							case "LBG":
								ret.AppendLine($"|{dataObj.LBGSpecialAmmoType}");
								if (dataArray.Game != "MHWI")
								{
									ret.AppendLine($"|{dataObj.LBGReloadRecoil}");
								}
								else
								{
									ret.AppendLine("|");
								}
								break;
							default: break;
						}
						if (!string.IsNullOrEmpty(dataObj.Elderseal))
						{
							ret.AppendLine($@"| {dataObj.Elderseal}");
						}
						else if (tableHasElderseal)
						{
							ret.AppendLine("| -");
						}
						if (!string.IsNullOrEmpty(dataObj.RampageSlots) && dataObj.RampageSlots != "0")
						{
							ret.AppendLine($@"| {dataObj.RampageSlots}");
						}
						else if (tableHasRampageSlots)
						{
							ret.AppendLine("| -");
						}
						if (!string.IsNullOrEmpty(dataObj.RampageDeco) && dataObj.RampageDeco != "0")
						{
							ret.AppendLine($@"| [[File:UI-Rampage Decoration {dataObj.RampageDeco}.png|20x20px|center]]");
						}
						else if (tableHasRampageDecos)
						{
							ret.AppendLine("| -");
						}
						if (!string.IsNullOrEmpty(dataObj.ArmorSkill))
						{
							ret.AppendLine($@"| {dataObj.ArmorSkill}{(!string.IsNullOrEmpty(dataObj.ArmorSkill2) ? ", " + dataObj.ArmorSkill2 : "")}");
						}
						else if (tableHasArmorSkills)
						{
							ret.AppendLine("| -");
						}
						if (!new string[] {"Bo", "HBG", "LBG"}.Contains(iconType))
						{
							ret.AppendLine($@"| {(string.IsNullOrEmpty(sharpness) ? "-" : sharpness)}");
						}
						ret.AppendLine($@"| {(string.IsNullOrEmpty(decos) ? "-" : decos)}
| {(string.IsNullOrEmpty(dataObj.Defense) && dataObj.Defense != "0" ? "-" : dataObj.Defense)}");
						cntr++;
					}
					ret.AppendLine("|}");
				}
				return ret.ToString();
			});
		}

		private static string GetPrefix(WebToolkitData dataArray, Datum dataObj, int i)
		{
			if (dataArray.Data.Length == 0 || !dataArray.Data.Any(x => !string.IsNullOrEmpty(x.Name)))
			{
				return "";
			}
			string[] dataNames = dataArray.Data.Select(x => x.Name).ToArray();
			Datum? iterAncestor = dataObj;
			List<int> ancestors = [];
			Datum ancestor = dataArray.Data[0];
			while (iterAncestor != null && ancestors.Count < 500)
			{
				iterAncestor = dataArray.Data.FirstOrDefault(x => x.Name == iterAncestor.Parent);
				if (iterAncestor == null || iterAncestor != ancestor)
				{
					ancestors.Add(Array.IndexOf(dataNames, (iterAncestor ?? ancestor).Name));
				}
			}
			if (ancestors.Count >= 500)
			{
				throw new Exception("Ancestor overload at " + dataArray.Data[0].Name + " line somewhere near " + iterAncestor!.Name + ". This usually means that the weapons are referencing each other in the \"Upgraded From\" section in a never-ending loop.");
			}
			string prefix = "";
			ancestors.Reverse();
			for (var i2 = 1; i2 < ancestors.Count; i2++)
			{
				Datum thisAncestor = dataArray.Data[ancestors[i2]];
				int thisIndex = Array.IndexOf(dataNames, thisAncestor.Name);
				if (dataArray.Data.Select((x,y) => new { Item = x, Index = y })
					.Any(x => x.Item.Name != thisAncestor.Name && x.Item.Parent == thisAncestor.Parent && x.Index > thisIndex))
				{
					prefix += "{{I|I}}";
				}
				else
				{
					prefix += "{{I|S}}";
				}
			}
			if (i > 0)
			{
				if (dataArray.Data.Select((x, y) => new { Item = x, Index = y }).Any(x => x.Item.Parent == dataObj.Parent && x.Item.Name != dataObj.Name && x.Index > i))
				{
					prefix += "{{I|B}}";
				}
				else
				{
					prefix += "{{I|L}}";
				}
			}
			return prefix;
		}
	}
}
