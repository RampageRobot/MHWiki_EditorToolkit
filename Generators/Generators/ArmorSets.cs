using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
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
				ret.AppendLine($@"{{{{GenericNav|{set.Game}}}}}
<br>
<br>
The {set.SetName} Set is a {GetRankLink(set.Game, set.Rarity)} [[{set.Game}/Armor|Armor Set]] in [[{Weapon.GetGameFullName(set.Game, (int?)set.Rarity)}]].<br>
{{{{GenericArmorSet
|Game                               = {set.Game}
|Set Name                           = {set.SetName}
|Max Level                          = {set.Pieces.Max(x => x.MaxLevel)}
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
					ret.AppendLine($@"{{{{GenericArmorSetPiece
|Game                  = {set.Game}
|Max Level             = {piece.MaxLevel}
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
				return $"[[Armor Set List ({game})#Low Rank|Low Rank (LR)]]";
			}
			else
			{
				return game switch
				{
					"MHWI" => rarity < 5 ? $"[[Armor Set List ({game})#Low Rank|Low Rank (LR)]]" : rarity < 9 ? $"[[Armor Set List ({game})#High Rank|High Rank (HR)]]" : $"[[Armor Set List ({game})#Master Rank|Master Rank (MR)]]",
					"MHRS" => rarity < 4 ? $"[[Armor Set List ({game})#Low Rank|Low Rank (LR)]]" : rarity < 8 ? $"[[Armor Set List ({game})#High Rank|High Rank (HR)]]" : $"[[Armor Set List ({game})#Master Rank|Master Rank (MR)]]",
					_ => $"[[Armor Set List ({game})#Low Rank|Low Rank (LR)]]",
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

		private static string GetSkillsTemplates(Skill[] skills, string game)
		{
			StringBuilder sb = new();
			foreach (Skill skill in skills.OrderByDescending(x => x.Level))
			{
				sb.Append($"\r\n<div>{{{{GenericSkillLink|{game}|{skill.Name.Replace("/", "-")}|Armor|{skill.WikiIconColor}{(skill.Name.Contains('/') ? "|" + skill.Name : "")}}}}} x{skill.Level}</div>");
			}
			return sb.ToString();
		}

		private static string GetMaterialsTemplates(Material[] materials, string game)
		{
			StringBuilder sb = new();
			foreach (Material material in materials.OrderByDescending(x => x.Quantity))
			{
				sb.Append($"\r\n<div>{{{{GenericItemLink|{game}|{material.Name}|{material.Icon}|{material.Color}}}}} x{material.Quantity}</div>");
			}
			return sb.ToString();
		}

		public static string GenerateFromJson(string json)
		{
			return Generate(WebToolkitData.FromJson(json)).Result;
        }

		public static void MassGenerate(string game)
		{
			WebToolkitData[] src = [];
			if (game == "MHWI")
			{
				src = Models.Data.MHWI.Armor.GetWebToolkitData();
			}
			else
			{
				src = Models.Data.MHRS.Armor.GetWebToolkitData();
			}
			foreach (WebToolkitData data in src)
			{
				Directory.CreateDirectory($@"C:\Users\mkast\Desktop\MHWiki Generated Armor Sets\{game}");
				File.WriteAllText($@"C:\Users\mkast\Desktop\MHWiki Generated Armor Sets\{game}\{data.SetName!.Replace("\"", "")} Set.txt", Generate(data).Result);
			}
			StringBuilder setList = new();
			string gameNameFull = Weapon.GetGameFullName(src[0].Game);
			setList.Append($@"{{{{Meta
|MetaTitle     = {src[0].Game} Armor Sets
|MetaDesc      = A list of all Armor Sets from {gameNameFull}
|MetaKeywords  = {src[0].Game}, {gameNameFull}, Armor, Armor Sets
|MetaImage     = {src[0].Game}-Logo.png
}}}}
{{{{GenericNav|{src[0].Game}}}}}
<br>
<br>
The following is a list of all armor sets that appear in [[{gameNameFull}]] and their corresponding armor pieces.
__TOC__");
			string lastRank = "";
			long? lastRarity = null;
			foreach (WebToolkitData data in src.OrderBy(x => x.Rarity))
			{
				bool newRarity = false;
				if (lastRarity == null || lastRarity != data.Rarity)
				{
					newRarity = true;
					setList.AppendLine("</div>");
				}
				string rank = GetRank(data.Game, data.Rarity);
				if (rank != lastRank)
				{
					setList.AppendLine("=" + rank + "=");
				}
				if (newRarity)
				{
					setList.AppendLine($@"==Rarity {data.Rarity!.Value}==
<div style=""display:flex; flex-wrap:wrap; height: 100%; align-items:stretch; justify-content:space-around; margin-bottom:20px;"">");
				}
				setList.AppendLine($@"{{{{ArmorSetListItem
|Game               = {data.Game}
|Set Rarity         = {data.Rarity!.Value}
|Set Name           = {data.SetName}
|Male Image         = {(string.IsNullOrEmpty(data.MaleFrontImg) ? "wiki.png" : data.MaleFrontImg)}
|Female Image       = {(string.IsNullOrEmpty(data.FemaleFrontImg) ? "wiki.png" : data.FemaleFrontImg)}
|Head Piece Name    = {data.Pieces.FirstOrDefault(x => x.IconType == "Helmet")?.Name ?? "None"}
|Chest Piece Name   = {data.Pieces.FirstOrDefault(x => x.IconType == "Chestplate")?.Name ?? "None"}
|Arm Piece Name     = {data.Pieces.FirstOrDefault(x => x.IconType == "Armguards")?.Name ?? "None"}
|Waist Piece Name   = {data.Pieces.FirstOrDefault(x => x.IconType == "Waist")?.Name ?? "None"}
|Leg Piece Name     = {data.Pieces.FirstOrDefault(x => x.IconType == "Leggings")?.Name ?? "None"}
}}}}");
				lastRank = rank;
				lastRarity = data.Rarity;
			}
			setList.AppendLine("</div>");
			File.WriteAllText($@"C:\Users\mkast\Desktop\MHWiki Generated Armor Sets\{game}\_setlist.txt", setList.ToString());
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
|Arm Piece Name     = {setData.Pieces.FirstOrDefault(x => x.IconType == "Armguards")?.Name ?? "None"}
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
