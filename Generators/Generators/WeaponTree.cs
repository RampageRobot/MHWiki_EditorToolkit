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
				string legendIconPrefix = $@"{srcData.FirstOrDefault()?.Game ?? "MHWI"}-{(WebToolkitData.GetWeaponName(string.IsNullOrEmpty(srcData.FirstOrDefault()?.Data?.FirstOrDefault()?.IconType) ? defaultIcon : srcData.FirstOrDefault()?.Data?.FirstOrDefault()?.IconType))}";
				ret.AppendLine($@"==Legend==
<div class=""threecol"">
{{| class=""wikitable center wide""
! colspan=3 | Element Legend
|-
| {{{{Element|Fire}}}} Fire
| {{{{Element|Water}}}} Water
|-
| {{{{Element|Thunder}}}} Thunder
| {{{{Element|Ice}}}} Ice
|-
| colspan=""2"" | {{{{Element|Dragon}}}} Dragon
|}}
{{| class=""wikitable center wide""
! colspan=""2"" | Status Legend
|-
| {{{{Element|Poison}}}} Poison
| {{{{Element|Paralysis}}}} Paralysis
|-
| {{{{Element|Sleep}}}} Sleep
| {{{{Element|Blast}}}} Blast
|-
| {{{{Element|Bleeding}}}} Bleeding
| {{{{Element|Stun}}}} Stun
|-
|}}
{{| class=""wikitable center wide""
! Other Legend
|-
| [[File:{legendIconPrefix} Icon Rare 1.png|class=craftmark|20x20px]] Craftable
|-
| [[File:UI-Rollback.png|Can Rollback Upgrade]] Can Rollback
|}}
</div>
{{| class=""wikitable center""
! colspan=3 | Rarity Legend
|-
| [[File:{legendIconPrefix} Icon Rare 1.png|20x20px|link=]] Rare 1
| [[File:{legendIconPrefix} Icon Rare 2.png|20x20px|link=]] Rare 2
| [[File:{legendIconPrefix} Icon Rare 3.png|20x20px|link=]] Rare 3
|-
| [[File:{legendIconPrefix} Icon Rare 4.png|20x20px|link=]] Rare 4
| [[File:{legendIconPrefix} Icon Rare 5.png|20x20px|link=]] Rare 5
| [[File:{legendIconPrefix} Icon Rare 6.png|20x20px|link=]] Rare 6
|-
| [[File:{legendIconPrefix} Icon Rare 7.png|20x20px|link=]] Rare 7
| [[File:{legendIconPrefix} Icon Rare 8.png|20x20px|link=]] Rare 8
| [[File:{legendIconPrefix} Icon Rare 9.png|20x20px|link=]] Rare 9
|-
| [[File:{legendIconPrefix} Icon Rare 10.png|20x20px|link=]] Rare 10
| [[File:{legendIconPrefix} Icon Rare 11.png|20x20px|link=]] Rare 11
| [[File:{legendIconPrefix} Icon Rare 12.png|20x20px|link=]] Rare 12
|}}");
				foreach (WebToolkitData dataArray in srcData)
				{
					int cntr = 0;
					ret.AppendLine($@"=={dataArray.PathName} Path== 
{{| class=""wikitable center wide mw-collapsible mw-collapsed""
! colspan=8 | {dataArray.PathName} Path
|-
!Name 
!Rarity!! [[File:UI-Attack Up.png|24x24px|link=]] !! [[File:UI-Blastblight.png|24x24px|link=]] !! [[File:UI-Affinity Up.png|24x24px|link=]] !! [[File:2ndGen-Whetstone Icon Yellow.png|24x24px|link=]] !! [[File:2ndGen-Decoration Icon Blue.png|24x24px|link=]] !! [[File:UI-Defense Up.png|24x24px|link=]]");
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
| style=""text-align:left"" | {prefix}{{{{GenericWeaponLink|{dataArray.Game}|{dataObj.Name}|{iconType}|{dataObj.Rarity}{(dataObj.CanForge == true ? "|true" : "")}}}}}{(dataObj.CanRollback == true ? "[[File:UI-Rollback.png|Can Rollback Upgrade]]" : "")}{(!string.IsNullOrEmpty(dataObj.PathLink) ? $" [[#{dataObj.PathLink} Path|(Go to path)]]" : "")}
| {dataObj.Rarity}
| {dataObj.Attack}
| {(string.IsNullOrEmpty(dataObj.Element) ? "-" : $"{{{{Element|{dataObj.Element}|{dataObj.ElementDamage}}}}}")}
| {(dataObj.Affinity == 0 ? "0%" : dataObj.Affinity + "%")}
| {(string.IsNullOrEmpty(sharpness) ? "-" : sharpness)}
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
			while (iterAncestor != null)
			{
				iterAncestor = dataArray.Data.FirstOrDefault(x => x.Name == iterAncestor.Parent);
				if (iterAncestor == null || iterAncestor != ancestor)
				{
					ancestors.Add(Array.IndexOf(dataNames, (iterAncestor ?? ancestor).Name));
				}
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
