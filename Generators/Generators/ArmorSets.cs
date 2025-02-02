using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using MediawikiTranslator.Models.ArmorSets;
using System.IO.Compression;
using System.Text;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace MediawikiTranslator.Generators
{
    public class ArmorSets
	{
		private static async Task<string> Generate(WebToolkitData set, WebToolkitData[] src)
		{
			return await Task.Run(() =>
			{
				StringBuilder ret = new();
				string setGenderMark = set.OnlyForGender != null && src.Any(x => x.SetName == set.SetName && x.OnlyForGender != set.OnlyForGender) ? " (" + set.OnlyForGender + ")" : "";
				ret.AppendLine($@"{{{{GenericNav|{set.Game}}}}}
<br>
<br>
The {set.SetName}{setGenderMark} Set is a {GetRankLink(set.Game, set.Rarity)} armor set in [[{Weapon.GetGameFullName(set.Game, (int?)set.Rarity)}]].<br>
{{{{GenericArmorSet
|Game                               = {set.Game}
|Set Name                           = {set.SetName}{setGenderMark}
|Max Level                          = {set.Pieces.Max(x => x.MaxLevel)}
|Male Image                         = {(string.IsNullOrEmpty(set.MaleFrontImg) ? "wiki.png" : set.MaleFrontImg)}
|Female Image                       = {(string.IsNullOrEmpty(set.FemaleFrontImg) ? "wiki.png" : set.FemaleFrontImg)}
|Set Rarity                         = {(set.Rarity > 0 ? set.Rarity : "1")}
|Set Skill 1                        = {(set.SetSkill1 != null ? GetSkillsTemplates([set.SetSkill1], set.Game, true) : "None")}
|Set Skill 2                        = {(set.SetSkill2 != null ? GetSkillsTemplates([set.SetSkill2], set.Game, true) : "")}
|Group Skill 1                      = {(set.GroupSkill1 != null ? GetSkillsTemplates([set.GroupSkill1], set.Game, true) : "")}
|Group Skill 2                      = {(set.GroupSkill2 != null ? GetSkillsTemplates([set.GroupSkill2], set.Game, true) : "")}
|Total Forging Cost                 = {set.Pieces.Sum(x => x.ForgingCost):n0}
|Total Skills                       = {GetSetSkillsTemplates([..set.Pieces.SelectMany(x => x.Skills)], set.Game)}
|Total Forging Materials            = {GetSetMaterialsTemplates([..set.Pieces.SelectMany(x => x.Materials)], set.Game)}
|Total Decos 1                      = {set.Pieces.Sum(x => x.Decos1)}
|Total Decos 2                      = {set.Pieces.Sum(x => x.Decos2)}
|Total Decos 3                      = {set.Pieces.Sum(x => x.Decos3)}
|Total Decos 4                      = {set.Pieces.Sum(x => x.Decos4)}
|Total Defense                      = {set.Pieces.Sum(x => x.Defense) + (set.Pieces.Any(x => x.MaxDefense != null) ? " → " + set.Pieces.Sum(x => x.MaxDefense) : "")}
|Total Fire Res                     = {set.Pieces.Sum(x => x.FireRes)}
|Total Water Res                    = {set.Pieces.Sum(x => x.WaterRes)}
|Total Thunder Res                  = {set.Pieces.Sum(x => x.ThunderRes)}
|Total Ice Res                      = {set.Pieces.Sum(x => x.IceRes)}
|Total Dragon Res                   = {set.Pieces.Sum(x => x.DragonRes)}
}}}}");
				foreach (Piece piece in set.Pieces)
				{
					//TODO: If you ever get set piece renders, add these commented lines to the template.	
//|Male Image            = {piece.MaleImage}
//|Female Image          = {piece.FemaleImage}
					ret.AppendLine($@"{{{{GenericArmorSetPiece
|Game                  = {set.Game}
|Max Level             = {piece.MaxLevel}
|Piece Name            = {piece.Name}
|Rarity                = {(piece.Rarity > 0 ? piece.Rarity : "")}
|Item Icon Type        = {piece.IconType}
|Description           = {piece.Description}
|Level 1 Decos         = {piece.Decos1}
|Level 2 Decos         = {piece.Decos2}
|Level 3 Decos         = {piece.Decos3}
|Level 4 Decos         = {piece.Decos4}
|Forging Cost          = {piece.ForgingCost:n0}
|Defense               = {piece.Defense + " → " + piece.MaxDefense}
|Fire Res              = {piece.FireRes}
|Water Res             = {piece.WaterRes}
|Thunder Res           = {piece.ThunderRes}
|Ice Res               = {piece.IceRes}
|Dragon Res            = {piece.DragonRes}
|Skills                = {GetSkillsTemplates(piece.Skills, set.Game)}
|Materials             = {GetMaterialsTemplates(piece.Materials, set.Game)}
}}}}");
				}
				return ret.ToString();
			});
		}

		private static string GetRankLink(string game, long? rarity)
		{
			if (rarity == null)
			{
				return $"[[{game}/Armor#Low Rank|Low Rank (LR)]]";
			}
			else
			{
				return game switch
				{
					"MHWI" => rarity < 5 ? $"[[{game}/Armor#Low Rank|Low Rank (LR)]]" : rarity < 9 ? $"[[{game}/Armor#High Rank|High Rank (HR)]]" : $"[[{game}/Armor#Master Rank|Master Rank (MR)]]",
					"MHRS" => rarity < 4 ? $"[[{game}/Armor#Low Rank|Low Rank (LR)]]" : rarity < 8 ? $"[[{game}/Armor#High Rank|High Rank (HR)]]" : $"[[{game}/Armor#Master Rank|Master Rank (MR)]]",
					_ => $"[[{game}/Armor#Low Rank|Low Rank (LR)]]",
				};
			}
		}

		public static string GetRank(string game, long? rarity)
		{
			if (rarity == null)
			{
				return "Low Rank";
			}
			else
			{
				return game switch
				{
					"MHWI" => rarity < 5 ? "Low Rank" : rarity < 9 ? "High Rank" : "Master Rank",
					"MHRS" => rarity < 4 ? "Low Rank" : rarity < 8 ? "High Rank" : "Master Rank",
					_ => "Low Rank",
				};
			}
		}

		private static string GetSetSkillsTemplates(Skill[] skills, string game)
		{
			return GetSkillsTemplates([..skills.GroupBy(x => new { x.Name, x.WikiIconColor })
				.Select(x => new Skill() { 
					Name = x.Key.Name, 
					Level = x.Sum(y => y.Level),
					WikiIconColor = x.Key.WikiIconColor
				})], game);
		}

		private static string GetSetMaterialsTemplates(Material[] materials, string game)
		{
			return GetMaterialsTemplates([.. materials.GroupBy(x => new { x.Name, x.Color, x.Icon })
				.Select(x => new Material() { 
					Name = x.Key.Name, 
					Color = x.Key.Color,
					Icon = x.Key.Icon,
					Quantity = x.Sum(y => y.Quantity) 
				})], game);
		}

		private static string GetSkillsTemplates(Skill[] skills, string game, bool isSet = false)
		{
			StringBuilder sb = new();
			foreach (Skill skill in skills.OrderByDescending(x => x.Level))
			{
				sb.Append($"\r\n<div>{{{{GenericSkillLink|{game}|{skill.Name.Replace("/", "-")}|Armor|{skill.WikiIconColor}{(skill.Name.Contains('/') ? "|" + skill.Name : "")}}}}}{(!isSet ? " x" + skill.Level : "")}</div>");
			}
			return sb.ToString();
		}

		private static string GetMaterialsTemplates(Material[] materials, string game)
		{
			StringBuilder sb = new();
			foreach (Material material in materials.OrderByDescending(x => x.Quantity))
			{
				if (material.Icon == "MATERIAL_NOICON")
				{
					sb.Append($"\r\n<div>{{{{GenericMaterialLink|{game}|{material.Name}}}}} {material.Quantity} {(material.Quantity == 1 ? "pt" : "pts")}.</div>");
				}
				else
				{
					sb.Append($"\r\n<div>{{{{GenericItemLink|{game}|{material.Name}|{material.Icon}|{material.Color}}}}} x{material.Quantity}</div>");
				}
			}
			return sb.ToString();
		}

		public static string GenerateFromJson(string json)
		{
			return Generate(WebToolkitData.FromJson(json), []).Result;
        }

		public static Dictionary<WebToolkitData, string> MassGenerate(string game)
		{
			Dictionary<WebToolkitData, string> ret = [];
			WebToolkitData[] src = [];
			if (game == "MHWI")
			{
				src = Models.Data.MHWI.Armor.GetWebToolkitData();
			}
			else
			{
				src = Models.Data.MHRS.Armor.GetWebToolkitData();
			}
			foreach (WebToolkitData data in src.Where(x => x.Pieces.Length > 0))
			{
				string setGenderMark = data.OnlyForGender != null && src.Any(x => x.SetName == data.SetName && x.OnlyForGender != data.OnlyForGender) ? " (" + data.OnlyForGender + ")" : "";
				ret.Add(data, Generate(data, src).Result);
				//File.WriteAllText($@"C:\Users\mkast\Desktop\MHWiki Generated Armor Sets\{game}\{data.SetName!.Replace("\"", "")}{setGenderMark} Set.txt", Generate(data, src).Result);
			}
			return ret;
			//File.WriteAllText($@"C:\Users\mkast\Desktop\MHWiki Generated Armor Sets\{game}\_setlist.txt", setList.ToString());
		}
	}

	public class ArmorSetPiece
	{
		public ArmorSetPieceType? PieceType { get; set; }
		public string Name { get; set; } = string.Empty;
		public int? Rarity { get; set; }
		public int? Cost { get; set; }
		public string? Material1 { get; set; }
		public int? Material1Quantity { get; set; }
		public string? Material1IconType { get; set; }
		public string? Material1IconColor { get; set; }
		public string? Material2 { get; set; } = string.Empty;
		public int? Material2Quantity { get; set; }
		public string? Material2IconType { get; set; }
		public string? Material2IconColor { get; set; }
		public string? Material3 { get; set; } = string.Empty;
		public int? Material3Quantity { get; set; }
		public string? Material3IconType { get; set; }
		public string? Material3IconColor { get; set; }
		public string? Material4 { get; set; }
		public int? Material4Quantity { get; set; }
		public string? Material4IconType { get; set; }
		public string? Material4IconColor { get; set; }
		public int? BaseDefense { get; set; }
		public int? MaxDefense { get; set; }
		public int? FireRes { get; set; }
		public int? WaterRes { get; set; }
		public int? ThunderRes { get; set; }
		public int? IceRes { get; set; }
		public int? DragonRes { get; set; }
		public int? Level1Slots { get; set; }
		public int? Level2Slots { get; set; }
		public int? Level3Slots { get; set; }
		public int? Level4Slots { get; set; }
		public string? Skill1 { get; set; } = string.Empty;
		public int? Skill1Level { get; set; }
		public string? Skill2 { get; set; } = string.Empty;
		public int? Skill2Level { get; set; }
		public string? Description { get; set; } = string.Empty;
	}

	public enum ArmorSetPieceType
	{
		Head,
		Chest,
		Arms,
		Waist,
		Legs
	}
}
