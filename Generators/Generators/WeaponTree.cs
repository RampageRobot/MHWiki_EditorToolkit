using MediawikiTranslator.Models.WeaponTree;
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
				StringBuilder ret = new();
				ret.AppendLine($@"{{{{WeaponTreeLegend|{srcData.FirstOrDefault()?.Game ?? "MHWI"}|{(WebToolkitData.GetWeaponName(string.IsNullOrEmpty(srcData.FirstOrDefault()?.Data?.FirstOrDefault()?.IconType) ? defaultIcon : srcData.FirstOrDefault()?.Data?.FirstOrDefault()?.IconType))}}}}}");
				foreach (WebToolkitData dataArray in srcData)
				{
					bool tableHasElderseal = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.Elderseal));
					bool tableHasRampageSlots = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.RampageSlots));
					bool tableHasRampageDecos = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.RampageDeco));
					bool tableHasArmorSkills = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.ArmorSkill));
					int cntr = 0;
					ret.AppendLine($@"=={dataArray.PathName} Path== 
{{| class=""wikitable center wide mw-collapsible mw-collapsed""
! colspan=12 | {dataArray.PathName} Path
|-
!Name 
!Rarity
!{{{{Element|Attack}}}}
!{{{{Element|Element}}}}
!{{{{Element|Affinity}}}}");
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
						ret.AppendLine(@"![[File:UI-Rampage Decoration 3.png|20x20px|center|link=Rampage Decorations]]");
					}
					if (tableHasArmorSkills)
					{
						ret.AppendLine(@"![[File:UI-Blights Negated.png|20x20px|center|Link=Armor Skills]]");
					}
					ret.AppendLine(@"![[File:2ndGen-Whetstone Icon Yellow.png|24x24px|link=Sharpness]] 
![[File:2ndGen-Decoration Icon Blue.png|24x24px|link=Decorations]] 
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
							Decoration[] objDecos = [.. Newtonsoft.Json.JsonConvert.DeserializeObject<Decoration[]>(dataObj.Decos)!.OrderBy(x => x.Level).ThenBy(x => x.IsRampage)];
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
| {dataObj.Attack}
| {(string.IsNullOrEmpty(dataObj.Element) ? "-" : $"{{{{Element|{dataObj.Element}|{dataObj.ElementDamage}}}}}")}
| {((dataObj.Affinity == 0 || dataObj.Affinity == null) ? "0%" : dataObj.Affinity + "%")}");
						if (!string.IsNullOrEmpty(dataObj.Elderseal))
						{
							ret.AppendLine($@"| {dataObj.Elderseal}");
						}
						else if (tableHasElderseal)
						{
							ret.AppendLine("| -");
						}
						if (!string.IsNullOrEmpty(dataObj.RampageSlots))
						{
							ret.AppendLine($@"| {dataObj.RampageSlots}");
						}
						else if (tableHasRampageSlots)
						{
							ret.AppendLine("| -");
						}
						if (!string.IsNullOrEmpty(dataObj.RampageDeco))
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
						ret.AppendLine($@"| {(string.IsNullOrEmpty(sharpness) ? "-" : sharpness)}
| {(string.IsNullOrEmpty(decos) ? "-" : decos)}
| {(string.IsNullOrEmpty(dataObj.Defense) ? "-" : dataObj.Defense)}");
						cntr++;
					}
					ret.AppendLine("|}");
				}
				return ret.ToString();
			});
		}

		private static string GetPrefix(WebToolkitData dataArray, Datum dataObj, int i)
		{
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
