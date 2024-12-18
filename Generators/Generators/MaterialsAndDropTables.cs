using MediawikiTranslator.Models.MaterialsAndDropTables;
using System.Text;
using System.Xml.Linq;

namespace MediawikiTranslator.Generators
{
	public class MaterialsAndDropTables
	{
		public static string ParseJson(string json, string game, bool escapeTabbers)
		{
			return Generate(WebToolkitData.FromJson(json), game, escapeTabbers).Result;
		}

		public static async Task<string> Generate(WebToolkitData[] srcData, string game, bool escapeTabbers = false)
		{
			return await Task.Run(() =>
			{
				if (escapeTabbers)
				{
					return GenerateDataEscape(srcData, game);
				}
				else
				{
					return GenerateDataUnescape(srcData, game);
				}
			});
		}

		public static string GenerateDataUnescape(WebToolkitData[] srcData, string game)
		{
			string[] numWords = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"];
			StringBuilder ret = new();
			ret.AppendLine(@"== Materials ==
<div>
{| class=""wikitable mw-collapsible mw-collapsed""
!colspan=""3"" style=""width:800px""| <big>" + srcData[0].Monster + @" Materials</big>
|-
!style=""width:400px;text-align:left"" | Item
!style=""width:65px;text-align:left"" | Rarity
!style=""width:200px;text-align:left"" | Price");
			Item[] items = [..srcData
					.SelectMany(x => x.Tables.SelectMany(x => x.Items))
					.Where(x => x.Include)
					.GroupBy(x => new { x.ItemName, x.Icon, x.IconColor, x.Description, x.Rarity, x.Price })
					.Select(x => x.First())
					.OrderBy(x => x.Rarity)
					.ThenBy(x => x.Price)
					.ThenBy(x => x.ItemName)
					.Distinct()];
			foreach (Item item in items)
			{
				if (item.ItemName != "undefined" && item.IconColor != "undefined" && item.Icon != "undefined" &&
					!string.IsNullOrEmpty(item.ItemName) && !string.IsNullOrEmpty(item.Icon) && !string.IsNullOrEmpty(item.IconColor))
				{
					ret.AppendLine($@"|-
|{{{{{(game == "MHWilds" ? "MHWildsItem" : game + "ItemLink")}|{item.ItemName}|{item.Icon}|{item.IconColor}}}}}
|Rarity {item.Rarity}
|{item.Price}
|-
| colspan=""3"" |{item.Description}
");
				}
			}
			ret.AppendLine(@"|}
=== Drop Rates ===
<tabber>
");
			foreach (WebToolkitData data in srcData)
			{
				ret.AppendLine($@"|-| {data.Rank} Rank = 
<div class=""{numWords[data.Tables.Length - 1] + (data.Tables.Length == 3 ? "cen" : "col")}"">
");
				foreach (Table table in data.Tables)
				{
					ret.AppendLine($@"<div>
{{| class=""wikitable itemtable mw-collapsible""
!colspan=""2"" | <big>{table.Header}</big>
|-
!Item
!Chance
");
					foreach (string category in table.Items.Select(x => x.Category).Distinct())
					{
						ret.AppendLine($@"|-
! colspan=""2"" |{category}
");
						foreach (Item item in table.Items.Where(x => x.Category == category && !string.IsNullOrEmpty(x.ItemName) && !string.IsNullOrEmpty(x.Icon) && !string.IsNullOrEmpty(x.IconColor)))
						{
							ret.AppendLine($@"|-
|{{{{{(game == "MHWilds" ? "MHWildsItem" : game + "ItemLink")}|{item.ItemName}|{item.Icon}|{item.IconColor}}}}}{(item.Quantity > 1 ? " x" + item.Quantity : "")}
|{(item.Chance != null ? item.Chance + "%" : "")}");
						}
					}
					ret.AppendLine(@"|}
</div>");
				}
			}
			ret.AppendLine("</tabber>");
			return ret.ToString();
		}

		public static string GenerateDataEscape(WebToolkitData[] srcData, string game)
		{
			string[] numWords = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"];
			StringBuilder ret = new();
			ret.AppendLine(@"== Materials ==
<div>
{| class=""wikitable mw-collapsible mw-collapsed""
!colspan=""3"" style=""width:800px""| <big>" + srcData[0].Monster + @" Materials</big>
|-
!style=""width:400px;text-align:left"" | Item
!style=""width:65px;text-align:left"" | Rarity
!style=""width:200px;text-align:left"" | Price");
			Item[] items = [..srcData
					.SelectMany(x => x.Tables.SelectMany(x => x.Items))
					.Where(x => x.Include)
					.GroupBy(x => new { x.ItemName, x.Icon, x.IconColor, x.Description, x.Rarity, x.Price })
					.Select(x => x.First())
					.OrderBy(x => x.Rarity)
					.ThenBy(x => x.Price)
					.ThenBy(x => x.ItemName)
					.Distinct()];
			foreach (Item item in items)
			{
				if (item.ItemName != "undefined" && item.IconColor != "undefined" && item.Icon != "undefined" &&
					!string.IsNullOrEmpty(item.ItemName) && !string.IsNullOrEmpty(item.Icon) && !string.IsNullOrEmpty(item.IconColor))
				{
					ret.AppendLine($@"|-
|{{{{{(game == "MHWilds" ? "MHWildsItem" : game + "ItemLink")}|{item.ItemName}|{item.Icon}|{item.IconColor}}}}}
|Rarity {item.Rarity}
|{item.Price}
|-
| colspan=""3"" |{item.Description}
");
				}
			}
			ret.AppendLine(@"|}
=== Drop Rates ===
{{#tag:tabber|
");
			foreach (WebToolkitData data in srcData)
			{
				ret.AppendLine($@"{{{{!}}}}-{{{{!}}}} {data.Rank} Rank = 
<div class=""{numWords[data.Tables.Length - 1] + (data.Tables.Length == 3 ? "cen" : "col")}"">
");
				foreach (Table table in data.Tables)
				{
					ret.AppendLine($@"<div>
{{{{{{!}}}} class=""wikitable itemtable mw-collapsible""
!colspan=""2"" {{{{!}}}} <big>{table.Header}</big>
{{{{!}}}}-
!Item
!Chance
");
					foreach (string category in table.Items.Select(x => x.Category).Distinct())
					{
						ret.AppendLine($@"{{{{!}}}}-
! colspan=""2"" {{{{!}}}}{category}
");
						foreach (Item item in table.Items.Where(x => x.Category == category && !string.IsNullOrEmpty(x.ItemName) && !string.IsNullOrEmpty(x.Icon) && !string.IsNullOrEmpty(x.IconColor)))
						{
							ret.AppendLine($@"{{{{!}}}}-
{{{{!}}}}{{{{{(game == "MHWilds" ? "MHWildsItem" : game + "ItemLink")}|{item.ItemName}|{item.Icon}|{item.IconColor}}}}}{(item.Quantity > 1 ? " x" + item.Quantity : "")}
{{{{!}}}}{(item.Chance != null ? item.Chance + "%" : "")}");
						}
					}
					ret.AppendLine(@"{{!}}}
</div>");
				}
			}
			ret.AppendLine("}}");
			return ret.ToString();
		}
	}
}
