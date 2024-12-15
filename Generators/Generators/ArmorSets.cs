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
|Set Rarity                         = {(set.Rarity > 0 ? set.Rarity : "1")}
|Set Skill 1 Name                   = {set.SetSkill1Name}
|Set Skill 2 Name                   = {set.SetSkill2Name}
|Group Skill 1 Name                 = {set.GroupSkill1Name}
|Group Skill 2 Name                 = {set.GroupSkill2Name}
|Total Forging Cost                 = {set.Pieces.Sum(x => x.ForgingCost):n0}
|Total Skills                       = {string.Join("<br>", set.Pieces.SelectMany(x => x.Skills).GroupBy(x => new { x.Name, x.Level }).OrderByDescending(x => x.Sum(y => y.Level)).ThenBy(x => x.Key.Name).Select(x => $"{{{{{(string.IsNullOrEmpty(set.Game) ? "MHWildsItem" : set.Game + "ItemLink")}|{x.Key.Name}}}}} {x.Sum(y => y.Level)}"))}
|Total Forging Materials            = {string.Join("<br>", set.Pieces.SelectMany(x => x.Materials).GroupBy(x => new { x.Name, x.Icon, x.Color }).Select(x => new { x.Key, Quantity = set.Pieces.SelectMany(y => y.Materials).Where(y => y.Name == x.Key.Name).Sum(y => y.Quantity) }).OrderByDescending(x => x.Quantity).ThenBy(x => x.Key.Name).Select(x => $"{{{{{(string.IsNullOrEmpty(set.Game) ? "MHWildsItem" : set.Game + "ItemLink")}|{x.Key.Name}|{x.Key.Icon}|{x.Key.Color}}}}} {x.Quantity}"))}
|Total Decos 1                      = {set.Pieces.Sum(x => x.Decos1)}
|Total Decos 2                      = {set.Pieces.Sum(x => x.Decos2)}
|Total Decos 3                      = {set.Pieces.Sum(x => x.Decos3)}
|Total Decos 4                      = {set.Pieces.Sum(x => x.Decos4)}
|Total Defense                      = {set.Pieces.Sum(x => x.Defense) + (set.Pieces.Any(x => x.MaxDefense != null) ? (" (" + set.Pieces.Sum(x => x.MaxDefense)) + ")" : "")}
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
|Defense               = {piece.Defense + "-" + piece.MaxDefense}
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
|Material 5 Quantity   = {(piece.Materials.Length > 4 ? piece.Materials[4].Quantity : "")}
}}}}");
				}
				ret.AppendLine(@"}}");
				return ret.ToString();
			});
		}

		public static string GenerateFromJson(string json)
		{
			return Generate(WebToolkitData.FromJson(json)).Result;
        }

		public static async Task<string> GenerateFromXlsx(string xlsxBase64, string game)
		{
			DirectoryInfo workspace = Utilities.GetWorkspace();
			string xlsxPath = Path.Combine(workspace.FullName, Guid.NewGuid().ToString() + ".xlsx");
			File.WriteAllBytes(xlsxPath, Convert.FromBase64String(xlsxBase64));
			List<Dictionary<string, XlsxData>> retData = [];
			string[] ranks = ["Low Rank", "High Rank", "Master Rank"];
			using (XLWorkbook wb = new(xlsxPath))
			{
				foreach (string rank in ranks)
				{
					Dictionary<string, XlsxData> rankData = [];
					IXLWorksheet sets = wb.Worksheet(rank + " Sets");
					int cntr = 0;
					foreach (IXLRow row in sets.Rows())
					{
						if (cntr > 0)
						{
							string? name = !row.Cell(1).Value.IsBlank ? row.Cell(1).Value.GetText() : null;
							if (!string.IsNullOrEmpty(name))
							{
								rankData.Add(name, new()
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
					IXLWorksheet pieces = wb.Worksheet(rank + " Pieces");
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
                                    ArmorSetPiece piece = new()
									{
										PieceType = !row.Cell(2).IsEmpty() ? (ArmorSetPieceType)Enum.Parse(typeof(ArmorSetPieceType), row.Cell(2).Value.GetText()) : null,
										Name = name,
										Rarity = row.Cell(4).Value.IsNumber ? (int)row.Cell(4).Value.GetNumber() : null,
										Cost = row.Cell(5).Value.IsNumber ? (int)row.Cell(5).Value.GetNumber() : null,
										Material1 = !row.Cell(6).IsEmpty() ? row.Cell(6).Value.GetText() : null,
										Material1Quantity = row.Cell(7).Value.IsNumber ? (int)row.Cell(7).Value.GetNumber() : null,
										Material2 = !row.Cell(8).IsEmpty() ? row.Cell(8).Value.GetText() : null,
										Material2Quantity = row.Cell(9).Value.IsNumber ? (int)row.Cell(9).Value.GetNumber() : null,
										Material3 = !row.Cell(10).IsEmpty() ? row.Cell(10).Value.GetText() : null,
										Material3Quantity = row.Cell(11).Value.IsNumber ? (int)row.Cell(11).Value.GetNumber() : null,
										Material4 = !row.Cell(12).IsEmpty() ? row.Cell(12).Value.GetText() : null,
										Material4Quantity = row.Cell(13).Value.IsNumber ? (int)row.Cell(13).Value.GetNumber() : null,
										BaseDefense = row.Cell(14).Value.IsNumber ? (int)row.Cell(14).Value.GetNumber() : null,
										MaxDefense = row.Cell(15).Value.IsNumber ? (int)row.Cell(15).Value.GetNumber() : null,
										FireRes = row.Cell(16).Value.IsNumber ? (int)row.Cell(16).Value.GetNumber() : null,
										WaterRes = row.Cell(17).Value.IsNumber ? (int)row.Cell(17).Value.GetNumber() : null,
										ThunderRes = row.Cell(18).Value.IsNumber ? (int)row.Cell(18).Value.GetNumber() : null,
										IceRes = row.Cell(19).Value.IsNumber ? (int)row.Cell(19).Value.GetNumber() : null,
										DragonRes = row.Cell(20).Value.IsNumber ? (int)row.Cell(20).Value.GetNumber() : null,
										Skill1 = !row.Cell(24).IsEmpty() ? row.Cell(24).Value.GetText() : null,
										Skill1Level = row.Cell(25).Value.IsNumber ? (int)row.Cell(25).Value.GetNumber() : null,
										Skill2 = !row.Cell(26).IsEmpty() ? row.Cell(26).Value.GetText() : null,
										Skill2Level = row.Cell(27).Value.IsNumber ? (int)row.Cell(27).Value.GetNumber() : null,
										Description = !row.Cell(28).IsEmpty() ? row.Cell(28).Value.GetText() : null
									};
									int[] jewelSlots = [row.Cell(21).Value.IsNumber ? (int)row.Cell(21).Value.GetNumber() : 0,
										row.Cell(22).Value.IsNumber ? (int)row.Cell(22).Value.GetNumber() : 0,
										row.Cell(23).Value.IsNumber ? (int)row.Cell(23).Value.GetNumber() : 0];
									piece.Level1Slots = jewelSlots.Where(x => x == 1).Sum();
									piece.Level2Slots = jewelSlots.Where(x => x == 2).Sum();
									piece.Level3Slots = jewelSlots.Where(x => x == 3).Sum();
									piece.Level4Slots = jewelSlots.Where(x => x == 4).Sum();
                                    rankData[setName].Pieces.Add(piece);
								}
							}
						}
						cntr++;
                    }
                    retData.Add(rankData);
                }
			}
			DirectoryInfo zipDirInfo = Utilities.GetWorkspace();
            string zipDir = Path.Combine(zipDirInfo.FullName, Guid.NewGuid().ToString());
			Directory.CreateDirectory(zipDir);
			for (int i = 0; i < 3; i++)
			{
				Dictionary<string, XlsxData> thisDict = retData[i];
				DirectoryInfo rankDir = Directory.CreateDirectory(Path.Combine(zipDir, ranks[i]));
				foreach (KeyValuePair<string, XlsxData> val in thisDict)
				{
					string txtPath = Path.Combine(rankDir.FullName, val.Key + ".txt");
					File.WriteAllText(txtPath, await Generate(val.Value.ToToolkitData(game)));
				}
			}
			StringBuilder setList = new();
			setList.AppendLine("__TOC__");
			string lastRarity = "0";
			foreach (WebToolkitData setData in retData.SelectMany(x => x.Values.Select(x => x.ToToolkitData(game))).OrderBy(x => x.Rarity))
			{
				if (lastRarity != (setData.Rarity?.ToString() ?? "???"))
				{
					if (lastRarity != "0")
					{
						setList.AppendLine("</div>");
					}
					setList.AppendLine(@$"=Rarity {setData.Rarity?.ToString() ?? "???"}=
<div style=""display:flex; flex-wrap:wrap; height: 100%; align-items:stretch; justify-content:space-around; margin-bottom:20px;"">");
					lastRarity = setData.Rarity?.ToString() ?? "???";
                }
				setList.AppendLine($@"{{{{ArmorSetListItem
|Game               = {game}
|Set Rarity         = {(setData.Rarity != null ? setData.Rarity : 1)}
|Set Name           = {setData.SetName}
|Male Image         = {(!string.IsNullOrEmpty(setData.MaleFrontImg) ? setData.MaleFrontImg : "Placeholder-Male-Armorset.png")}
|Female Image       = {(!string.IsNullOrEmpty(setData.FemaleFrontImg) ? setData.FemaleFrontImg : "Placeholder-Female-Armorset.png")}
|Head Piece Name    = {setData.Pieces.FirstOrDefault(x => x.IconType == "Helmet")?.Name ?? "None"}
|Chest Piece Name   = {setData.Pieces.FirstOrDefault(x => x.IconType == "Chestplate")?.Name ?? "None"}
|Arm Piece Name     = {setData.Pieces.FirstOrDefault(x => x.IconType == "Armguard")?.Name ?? "None"}
|Waist Piece Name   = {setData.Pieces.FirstOrDefault(x => x.IconType == "Waist")?.Name ?? "None"}
|Leg Piece Name     = {setData.Pieces.FirstOrDefault(x => x.IconType == "Leggings")?.Name ?? "None"}
}}}}");
			}
			File.WriteAllText(Path.Combine(zipDir, game + "_Armor_Set_List.txt"), setList.ToString());
            DirectoryInfo zipPathInfo = Utilities.GetWorkspace();
            string zipPath = Path.Combine(zipPathInfo.FullName, Guid.NewGuid() + ".zip");
			ZipFile.CreateFromDirectory(zipDir, zipPath);
			string zipBytes = Convert.ToBase64String(File.ReadAllBytes(zipPath));
			File.Delete(zipPath);
			Directory.Delete(workspace.FullName, true);
			return zipBytes;
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
        public WebToolkitData ToToolkitData(string game)
		{
			WebToolkitData retData = new()
			{
				Game = game
			};
			if (Name != null)
			{
				retData.SetName = Name;
			}
			if (BonusSkill1 != null)
			{
				retData.SetSkill1Name = BonusSkill1;
			}
			if (BonusSkill2 != null)
			{
				retData.SetSkill2Name = BonusSkill2;
			}
			List<Piece> retPieces = [];
			foreach (ArmorSetPiece piece in Pieces)
			{
				Piece retPiece = new()
				{
					Name = piece.Name
				};
				switch (piece.PieceType)
				{
					case ArmorSetPieceType.Head:
						retPiece.IconType = "Helmet";
						break;
					case ArmorSetPieceType.Chest:
                        retPiece.IconType = "Chestplate";
                        break;
					case ArmorSetPieceType.Arms:
                        retPiece.IconType = "Armguard";
                        break;
					case ArmorSetPieceType.Waist:
                        retPiece.IconType = "Waist";
                        break;
					case ArmorSetPieceType.Legs:
                        retPiece.IconType = "Leggings";
                        break;
				}
				if (piece.Rarity != null)
				{
					retPiece.Rarity = piece.Rarity;
				}
				if (piece.Cost != null)
				{
					retPiece.ForgingCost = piece.Cost;
				}
				if (piece.BaseDefense != null)
				{
					retPiece.Defense = piece.BaseDefense;
                }
                if (piece.MaxDefense != null)
                {
                    retPiece.MaxDefense = piece.BaseDefense;
                }
                if (piece.FireRes != null)
				{
					retPiece.FireRes = piece.FireRes;
                }
                if (piece.WaterRes != null)
                {
                    retPiece.WaterRes = piece.WaterRes;
                }
                if (piece.ThunderRes != null)
                {
                    retPiece.ThunderRes = piece.ThunderRes;
                }
                if (piece.IceRes != null)
                {
                    retPiece.IceRes = piece.IceRes;
                }
                if (piece.DragonRes != null)
                {
                    retPiece.DragonRes = piece.DragonRes;
                }
				if (!string.IsNullOrEmpty(piece.Description))
				{
					retPiece.Description = piece.Description;
				}
				if (piece.Level1Slots != null)
				{
					retPiece.Decos1 = piece.Level1Slots;
                }
                if (piece.Level2Slots != null)
                {
                    retPiece.Decos2 = piece.Level2Slots;
                }
                if (piece.Level3Slots != null)
                {
                    retPiece.Decos3 = piece.Level3Slots;
                }
                if (piece.Level4Slots != null)
                {
                    retPiece.Decos4 = piece.Level4Slots;
                }
                List<Material> mats = [];
				if (!string.IsNullOrEmpty(piece.Material1))
				{
					mats.Add(new()
					{
						Name = piece.Material1,
						Quantity = piece.Material1Quantity != null ? piece.Material1Quantity : 1,
						Icon = !string.IsNullOrEmpty(piece.Material1IconType) ? piece.Material1IconType : "Question Mark",
						Color = !string.IsNullOrEmpty(piece.Material1IconColor) ? piece.Material1IconColor : "White",
					});
                }
                if (!string.IsNullOrEmpty(piece.Material2))
                {
                    mats.Add(new()
                    {
                        Name = piece.Material2,
                        Quantity = piece.Material2Quantity != null ? piece.Material2Quantity : 1,
                        Icon = !string.IsNullOrEmpty(piece.Material2IconType) ? piece.Material2IconType : "Question Mark",
                        Color = !string.IsNullOrEmpty(piece.Material2IconColor) ? piece.Material2IconColor : "White",
                    });
                }
                if (!string.IsNullOrEmpty(piece.Material3))
                {
                    mats.Add(new()
                    {
                        Name = piece.Material3,
                        Quantity = piece.Material3Quantity != null ? piece.Material3Quantity : 1,
                        Icon = !string.IsNullOrEmpty(piece.Material3IconType) ? piece.Material3IconType : "Question Mark",
                        Color = !string.IsNullOrEmpty(piece.Material3IconColor) ? piece.Material3IconColor : "White",
                    });
                }
                if (!string.IsNullOrEmpty(piece.Material4))
                {
                    mats.Add(new()
                    {
                        Name = piece.Material4,
                        Quantity = piece.Material4Quantity != null ? piece.Material4Quantity : 1,
                        Icon = !string.IsNullOrEmpty(piece.Material4IconType) ? piece.Material4IconType : "Question Mark",
                        Color = !string.IsNullOrEmpty(piece.Material4IconColor) ? piece.Material4IconColor : "White",
                    });
                }
                retPiece.Materials = [..mats];
				List<Skill> skills = [];
				if (!string.IsNullOrEmpty(piece.Skill1))
				{
					skills.Add(new Skill()
					{
						Level = piece.Skill1Level,
						Name = piece.Skill1
					});
				}
				if (!string.IsNullOrEmpty(piece.Skill2))
                {
                    skills.Add(new Skill()
                    {
                        Level = piece.Skill2Level,
                        Name = piece.Skill2
                    });
                }
				retPiece.Skills = [.. skills];
				retPieces.Add(retPiece);
			}
			retData.Pieces = [.. retPieces];
			if (retData.Rarity == null && retData.Pieces.Any())
			{
				retData.Rarity = retData.Pieces[0].Rarity;
			}
			return retData;
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
