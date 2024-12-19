using MediawikiTranslator.Models.WeaponTree;
using System.Text;

namespace MediawikiTranslator.Generators
{
	public class WeaponTree
	{
		public static string ParseJson(string json, string sharpnessBase, string weaponLink, int maxSharpnessCount, string pathName, string defaultIcon)
		{
			return Generate(WebToolkitData.FromJson(json), sharpnessBase, weaponLink, maxSharpnessCount, pathName, defaultIcon).Result;
		}

		public static async Task<string> Generate(WebToolkitData[] srcData, string sharpnessBase, string weaponLink, int maxSharpnessCount, string pathName, string defaultIcon)
		{
			return await Task.Run(() =>
			{
				StringBuilder ret = new();
				ret.AppendLine($@"=={pathName} Path== 
{{| class=""wikitable center wide mw-collapsible mw-collapsed""
! colspan=8 | {pathName} Path
|-
!Name 
!Rarity!! [[File:UI-Attack Up.png|24x24px|link=]] !! [[File:UI-Blastblight.png|24x24px|link=]] !! [[File:UI-Affinity Up.png|24x24px|link=]] !! [[File:2ndGen-Whetstone Icon Yellow.png|24x24px|link=]] !! [[File:2ndGen-Decoration Icon Blue.png|24x24px|link=]] !! [[File:UI-Defense Up.png|24x24px|link=]]");
				int cntr = 0;
				foreach (WebToolkitData dataObj in srcData)
				{
					string iconType = dataObj.IconType;
					if (string.IsNullOrEmpty(iconType))
					{
						iconType = defaultIcon;
					}
					string prefix = GetPrefix(dataObj, srcData, cntr);
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
					ret.AppendLine($@"
|-
| style=""text-align:left"" | {prefix}{(dataObj.CanForge ? "'''" : "")}{{{{{weaponLink}|{dataObj.Name}|{iconType}|{dataObj.Rarity}}}}}{(dataObj.CanForge ? "'''" : "")}{(dataObj.CanRollback ? "<sup>R</sup>" : "")}
| {dataObj.Rarity}
| {dataObj.Attack}
| {(dataObj.Element == Element.Empty ? "-" : $"{{{{Element|{dataObj.Element}|{dataObj.ElementDamage}}}}}")}
| {(dataObj.Affinity == 0 ? "0%" : dataObj.Affinity + "%")}
| {(string.IsNullOrEmpty(sharpness) ? "-" : sharpness)}
| {(string.IsNullOrEmpty(decos) ? "-" : decos)}
| {(string.IsNullOrEmpty(dataObj.Defense) ? "-" : dataObj.Defense)}");
					cntr++;
				}
				ret.AppendLine("|}");
				return ret.ToString();
			});
		}

		private static string GetPrefix(WebToolkitData dataObj, WebToolkitData[] src, int i)
		{
			string[] dataNames = src.Select(x => x.Name).ToArray();
			WebToolkitData? iterAncestor = dataObj;
			List<int> ancestors = [];
			WebToolkitData ancestor = src[0];
			while (iterAncestor != null)
			{
				iterAncestor = src.FirstOrDefault(x => x.Name == iterAncestor.Parent);
				if (iterAncestor == null || iterAncestor != ancestor)
				{
					ancestors.Add(Array.IndexOf(dataNames, (iterAncestor ?? ancestor).Name));
				}
			}
			string prefix = "";
			ancestors.Reverse();
			for (var i2 = 1; i2 < ancestors.Count; i2++)
			{
				WebToolkitData thisAncestor = src[ancestors[i2]];
				int thisIndex = Array.IndexOf(dataNames, thisAncestor.Name);
				if (src.Select((x,y) => new { Item = x, Index = y })
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
				if (src.Select((x, y) => new { Item = x, Index = y }).Any(x => x.Item.Parent == dataObj.Parent && x.Item.Name != dataObj.Name && x.Index > i))
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
