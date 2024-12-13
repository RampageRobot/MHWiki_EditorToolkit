using ClosedXML.Excel;
using MediawikiTranslator.Models.ArmorSets;
using System.IO.Compression;
using System.Text;

namespace MediawikiTranslator.Generators
{
	public class ArmorSets
	{
		private static async Task<string> Generate(WebToolkitData set)
		{

			return await Task.Run(() =>
			{
				StringBuilder ret = new();
				ret.AppendLine($@"{{{{GenericArmorSet
|Set Name                           = {set.SetName}
|Male Image                         = {(string.IsNullOrEmpty(set.MaleFrontImg) ? "wiki.png" : set.MaleFrontImg)}
|Female Image                       = {(string.IsNullOrEmpty(set.FemaleFrontImg) ? "wiki.png" : set.FemaleFrontImg)}
|Male Image Back                    = {(string.IsNullOrEmpty(set.MaleBackImg) ? "wiki.png" : set.MaleBackImg)}
|Female Image Back                  = {(string.IsNullOrEmpty(set.FemaleBackImg) ? "wiki.png" : set.FemaleBackImg)}
|Set Rarity                         = {(set.Rarity > 0 ? set.Rarity : "")}
|Set Skill 1 Name                   = {set.SetSkill1Name}
|Set Skill 2 Name                   = {set.SetSkill2Name}
|Group Skill 1 Name                 = {set.GroupSkill1Name}
|Group Skill 2 Name                 = {set.GroupSkill2Name}
|Total Forging Cost                 = {set.Pieces.Sum(x => x.ForgingCost):n0}
|Total Skills                       = {string.Join("<br>", set.Pieces.SelectMany(x => x.Skills).GroupBy(x => new { x.Name, x.Level }).Select(x => $"{{{{{(string.IsNullOrEmpty(set.Game) ? "MHWildsItem" : set.Game + "ItemLink")}|{x.Key.Name}}}}} {x.Sum(y => y.Level)}"))}
|Total Forging Materials            = {string.Join("<br>", set.Pieces.SelectMany(x => x.Materials).GroupBy(x => new { x.Name, x.Icon, x.Color }).Select(x => $"{{{{{(string.IsNullOrEmpty(set.Game) ? "MHWildsItem" : set.Game + "ItemLink")}|{x.Key.Name}|{x.Key.Icon}|{x.Key.Color}}}}} {set.Pieces.SelectMany(y => y.Materials).Where(y => y.Name == x.Key.Name).Sum(y => y.Quantity)}"))}
|Total Decos 1                      = {set.Pieces.Sum(x => x.Decos1)}
|Total Decos 2                      = {set.Pieces.Sum(x => x.Decos2)}
|Total Decos 3                      = {set.Pieces.Sum(x => x.Decos3)}
|Total Decos 4                      = {set.Pieces.Sum(x => x.Decos4)}
|Total Defense                      = {set.Pieces.Sum(x => Convert.ToInt32(x.Defense[..x.Defense.IndexOf('-')])) + "-" + set.Pieces.Sum(x => Convert.ToInt32(x.Defense[(x.Defense.IndexOf('-') + 1)..]))}
|Total Fire Res                     = {set.Pieces.Sum(x => x.FireRes)}
|Total Water Res                    = {set.Pieces.Sum(x => x.WaterRes)}
|Total Thunder Res                  = {set.Pieces.Sum(x => x.ThunderRes)}
|Total Ice Res                      = {set.Pieces.Sum(x => x.IceRes)}
|Total Dragon Res                   = {set.Pieces.Sum(x => x.DragonRes)}
}}}}");
				foreach (Piece piece in set.Pieces)
				{
					ret.AppendLine($@"{{{{GenericArmorSetPiece
|Piece Name            = {piece.Name}
|Rarity                = {(piece.Rarity > 0 ? piece.Rarity : "")}
|Item Icon Type        = {piece.IconType}
|Male Image            = {piece.MaleImage}
|Female Image          = {piece.FemaleImage}
|Description           = {piece.Description}
|Level 1 Decos         = {piece.Decos1}
|Level 2 Decos         = {piece.Decos2}
|Level 3 Decos         = {piece.Decos3}
|Level 4 Decos         = {piece.Decos4}
|Forging Cost          = {piece.ForgingCost:n0}
|Defense               = {piece.Defense}
|Fire Res              = {piece.FireRes}
|Water Res             = {piece.WaterRes}
|Thunder Res           = {piece.ThunderRes}
|Ice Res               = {piece.IceRes}
|Dragon Res            = {piece.DragonRes}
|Skill Link Prefix     = {(string.IsNullOrEmpty(set.Game) ? "MHWilds:_Skills" : set.Game + ":_Skills")}
|Skill 1               = {(piece.Skills.Length > 0 ? piece.Skills[0].Name : "")}
|Skill 1 Level         = {(piece.Skills.Length > 0 ? piece.Skills[0].Level : "")}
|Skill 2               = {(piece.Skills.Length > 1 ? piece.Skills[1].Name : "")}
|Skill 2 Level         = {(piece.Skills.Length > 1 ? piece.Skills[1].Level : "")}
|Skill 3               = {(piece.Skills.Length > 2 ? piece.Skills[2].Name : "")}
|Skill 3 Level         = {(piece.Skills.Length > 2 ? piece.Skills[2].Level : "")}
|Skill 4               = {(piece.Skills.Length > 3 ? piece.Skills[3].Name : "")}
|Skill 4 Level         = {(piece.Skills.Length > 3 ? piece.Skills[3].Level : "")}
|Material Template     = {(string.IsNullOrEmpty(set.Game) ? "MHWildsItem" : set.Game + "ItemLink")}
|Material 1 Name       = {(piece.Materials.Length > 0 ? piece.Materials[0].Name : "")}
|Material 1 Icon       = {(piece.Materials.Length > 0 ? piece.Materials[0].Icon : "")}
|Material 1 Icon Color = {(piece.Materials.Length > 0 ? piece.Materials[0].Color : "")}
|Material 1 Quantity   = {(piece.Materials.Length > 0 ? piece.Materials[0].Quantity : "")}
|Material 2 Name       = {(piece.Materials.Length > 1 ? piece.Materials[1].Name : "")}
|Material 2 Icon       = {(piece.Materials.Length > 1 ? piece.Materials[1].Icon : "")}
|Material 2 Icon Color = {(piece.Materials.Length > 1 ? piece.Materials[1].Color : "")}
|Material 2 Quantity   = {(piece.Materials.Length > 1 ? piece.Materials[1].Quantity : "")}
|Material 3 Name       = {(piece.Materials.Length > 2 ? piece.Materials[2].Name : "")}
|Material 3 Icon       = {(piece.Materials.Length > 2 ? piece.Materials[2].Icon : "")}
|Material 3 Icon Color = {(piece.Materials.Length > 2 ? piece.Materials[2].Color : "")}
|Material 3 Quantity   = {(piece.Materials.Length > 2 ? piece.Materials[2].Quantity : "")}
|Material 4 Name       = {(piece.Materials.Length > 3 ? piece.Materials[3].Name : "")}
|Material 4 Icon       = {(piece.Materials.Length > 3 ? piece.Materials[3].Icon : "")}
|Material 4 Icon Color = {(piece.Materials.Length > 3 ? piece.Materials[3].Color : "")}
|Material 4 Quantity   = {(piece.Materials.Length > 3 ? piece.Materials[3].Quantity : "")}
|Material 5 Name       = {(piece.Materials.Length > 4 ? piece.Materials[4].Name : "")}
|Material 5 Icon       = {(piece.Materials.Length > 4 ? piece.Materials[4].Icon : "")}
|Material 5 Icon Color = {(piece.Materials.Length > 4 ? piece.Materials[4].Color : "")}
|Material 5 Quantity   = {(piece.Materials.Length > 4 ? piece.Materials[4].Quantity : "")}}}}}");
				}
				ret.AppendLine(@"}}");
				return ret.ToString();
			});
		}

		public static string GenerateFromJson(string json)
		{
			return Generate(WebToolkitData.FromJson(json)).Result;
        }

		public static string[] GenerateFromXlsx(string xlsxBase64)
		{
			DirectoryInfo workspace = Utilities.GetWorkspace();
			string xlsxPath = Path.Combine(workspace.FullName, Guid.NewGuid().ToString() + ".xlsx");
			File.WriteAllBytes(xlsxPath, Convert.FromBase64String(xlsxBase64));
			Dictionary<string, XlsxData> retData = [];
			using (XLWorkbook wb = new(xlsxPath))
			{
				IXLWorksheet sets = wb.Worksheet("Low Rank Sets");
				int cntr = 0;
				foreach (IXLRow row in sets.Rows())
				{
					if (cntr > 0)
					{
						string? name = !row.Cell(1).Value.IsBlank ? row.Cell(1).Value.GetText() : null;
						if (!string.IsNullOrEmpty(name))
						{
							retData.Add(name, new()
							{
								Name = name,
								BonusName = !row.Cell(2).Value.IsBlank ? row.Cell(2).Value.GetText() : null,
								BonusSkill1 = !row.Cell(3).Value.IsBlank ? row.Cell(3).Value.GetText() : null,
								PiecesRequired1 = row.Cell(4).Value.IsNumber ? (int)row.Cell(4).Value.GetNumber() : null,
								BonusSkill2 = !row.Cell(5).Value.IsBlank ? row.Cell(5).Value.GetText() : null,
								PiecesRequired2 = row.Cell(6).Value.IsNumber ? (int)row.Cell(6).Value.GetNumber() : null
							});
						}
					}
					cntr++;
				}
				IXLWorksheet pieces = wb.Worksheet("Low Rank Pieces");
				cntr = 0;
				foreach (IXLRow row in pieces.Rows())
				{
					if (cntr > 0)
					{
						string? name = !row.Cell(3).Value.IsBlank ? row.Cell(3).Value.GetText() : null;
						if (!string.IsNullOrEmpty(name))
						{
							string? setName = !row.Cell(1).Value.IsBlank ? row.Cell(1).Value.GetText() : null;
							if (!string.IsNullOrEmpty(setName))
							{
								retData[setName].Pieces.Add(new()
								{
									PieceType = (ArmorSetPieceType)Enum.Parse(typeof(ArmorSetPieceType), row.Cell(2).Value.GetText()),
									Name = name,
									Rarity = (int)row.Cell(4).Value.GetNumber(),
									Cost = (int)row.Cell(5).Value.GetNumber(),
									Material1 = !row.Cell(6).Value.IsBlank ? row.Cell(6).Value.GetText() : null,
									Material1Quantity = row.Cell(7).Value.IsNumber ? (int)row.Cell(7).Value.GetNumber() : null,
									Material2 = !row.Cell(8).Value.IsBlank ? row.Cell(8).Value.GetText() : null,
									Material2Quantity = row.Cell(9).Value.IsNumber ? (int)row.Cell(9).Value.GetNumber() : null,
									Material3 = !row.Cell(10).Value.IsBlank ? row.Cell(10).Value.GetText() : null,
									Material3Quantity = row.Cell(11).Value.IsNumber ? (int)row.Cell(11).Value.GetNumber() : null,
									Material4 = !row.Cell(12).Value.IsBlank ? row.Cell(12).Value.GetText() : null,
									Material4Quantity = row.Cell(13).Value.IsNumber ? (int)row.Cell(13).Value.GetNumber() : null,
									BaseDefense = row.Cell(14).Value.IsNumber ? (int)row.Cell(14).Value.GetNumber() : null,
									MaxDefense = row.Cell(15).Value.IsNumber ? (int)row.Cell(15).Value.GetNumber() : null,
									FireRes = row.Cell(16).Value.IsNumber ? (int)row.Cell(16).Value.GetNumber() : null,
									WaterRes = row.Cell(17).Value.IsNumber ? (int)row.Cell(17).Value.GetNumber() : null,
									ThunderRes = row.Cell(18).Value.IsNumber ? (int)row.Cell(18).Value.GetNumber() : null,
									IceRes = row.Cell(19).Value.IsNumber ? (int)row.Cell(19).Value.GetNumber() : null,
									DragonRes = row.Cell(20).Value.IsNumber ? (int)row.Cell(20).Value.GetNumber() : null,
									Level1Slots = row.Cell(21).Value.IsNumber ? (int)row.Cell(21).Value.GetNumber() : null,
									Level2Slots = row.Cell(22).Value.IsNumber ? (int)row.Cell(22).Value.GetNumber() : null,
									Level3Slots = row.Cell(23).Value.IsNumber ? (int)row.Cell(23).Value.GetNumber() : null,
									Level4Slots = row.Cell(24).Value.IsNumber ? (int)row.Cell(24).Value.GetNumber() : null,
									Skill1 = !row.Cell(25).Value.IsBlank ? row.Cell(25).Value.GetText() : null,
									Skill1Level = row.Cell(26).Value.IsNumber ? (int)row.Cell(26).Value.GetNumber() : null,
									Skill2 = !row.Cell(27).Value.IsBlank ? row.Cell(27).Value.GetText() : null,
									Skill2Level = row.Cell(28).Value.IsNumber ? (int)row.Cell(28).Value.GetNumber() : null,
									Description = !row.Cell(29).Value.IsBlank ? row.Cell(29).Value.GetText() : null
								});
							}
						}
					}
					cntr++;
				}
			}
            return [..retData.Select(x => Generate(x.Value.ToToolkitData()).Result)];
		}
	}

	public class XlsxData
	{
		public string? Name { get; set; } = string.Empty;
		public string? BonusName { get; set; } = string.Empty;
		public string? BonusSkill1 { get; set; } = string.Empty;
		public int? PiecesRequired1 { get; set; }
		public string? BonusSkill2 { get; set; } = string.Empty;
		public int? PiecesRequired2 { get; set; }
		public List<ArmorSetPiece> Pieces { get; set; } = [];
        public WebToolkitData ToToolkitData()
		{
			return null;
		}
	}

	public class ArmorSetPiece
	{
		public ArmorSetPieceType PieceType { get; set; }
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
