using DocumentFormat.OpenXml.Vml;
using MediawikiTranslator.Models.Data.MHWI;
using MediawikiTranslator.Models.WeaponTree;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using System.Text.Json;

namespace MediawikiTranslator.Generators
{
    public class WeaponTree
	{
		public static string ParseJson(string json, string sharpnessBase, int maxSharpnessCount, string pathName, string defaultIcon)
		{
			return Generate(WebToolkitData.FromJson(json), sharpnessBase, maxSharpnessCount, pathName, defaultIcon).Result;
		}

		public static string ParseCsv(string csvFile, bool duplicateSharpness, string delimiter = ",")
		{
			return JsonSerializer.Serialize(ParseWeaponsFromCsv(csvFile, duplicateSharpness, delimiter));
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
				ret.AppendLine($@"{{{{#css:
  @media screen and (max-width:600px)
  {{ 
    .hide-on-mobile {{
      display:none;
    }}
    table.wikitable.mobile-sm tr td span.divot {{
      font-size:smaller!important;
    }}
    table.wikitable.mobile-sm td {{
      padding-left:0px!important;
      padding-right:0px!important;
    }}
  }}
}}}}
{{{{WeaponTreeLegend|{srcData.FirstOrDefault()?.Game ?? "MHWI"}|{(WebToolkitData.GetWeaponName(string.IsNullOrEmpty(srcData.FirstOrDefault()?.Data?.FirstOrDefault()?.IconType) ? defaultIcon : srcData.FirstOrDefault()?.Data?.FirstOrDefault()?.IconType))}}}}}");
				foreach (WebToolkitData dataArray in srcData)
				{
					bool tableHasElderseal = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.Elderseal));
					bool tableHasRampageSlots = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.RampageSlots) && x.RampageSlots != "0");
					bool tableHasRampageDecos = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.RampageDeco) && x.RampageDeco != "0");
					bool tableHasArmorSkills = dataArray.Data.Any(x => !string.IsNullOrEmpty(x.ArmorSkill));
					int cntr = 0;
					string sourceTree = srcData.FirstOrDefault(x => x.Data.Any(y => y.PathLink == dataArray.PathName))?.PathName ?? "";
					string sourceWeapon = srcData.Select(x => x.Data.FirstOrDefault(y => y.PathLink == dataArray.PathName))?.FirstOrDefault()?.Parent ?? "";
					ret.AppendLine($@"<br>
{{| class=""wikitable center wide mw-collapsible mw-collapsed mobile-sm""
! colspan=12 | <h4 style=""margin:0px;"">{dataArray.PathName} Tree</h4>{(!string.IsNullOrEmpty(sourceTree) ? $" [[#{sourceWeapon.Replace(" ", "_")}_{sourceTree.Replace(" ", "_")}|(Return to {sourceWeapon})]]" : "")}
|-
!Name 
!class=""hide-on-mobile""|Rarity
!class=""hide-on-mobile""|{{{{UI|MHWI|Attack}}}}
!class=""hide-on-mobile""|{{{{UI|MHWI|Element}}}}
!class=""hide-on-mobile""|{{{{UI|MHWI|Affinity}}}}");
					string tableIconType = dataArray.Data.Length > 0 ? dataArray.Data[0].IconType : "GS";
					if (string.IsNullOrEmpty(tableIconType))
					{
						tableIconType = defaultIcon;
					}
					switch (tableIconType)
					{
						case "Bo":
							ret.AppendLine("!class=\"hide-on-mobile\"|{{UI|MHWI|Bow Coatings}}");
							break;
						case "CB":
							ret.AppendLine("!class=\"hide-on-mobile\"|{{UI|MHWI|CB Phial Type}}");
							break;
						case "SA":
							ret.AppendLine("!class=\"hide-on-mobile\"|{{UI|MHWI|SA Phial Type}}");
							break;
						case "GL":
							ret.AppendLine("!class=\"hide-on-mobile\"|{{UI|MHWI|GL Shelling Type}}");
							break;
						case "HH":
							ret.AppendLine("!class=\"hide-on-mobile\"|{{UI|MHWI|HH Menu Notes}}");
							break;
						case "IG":
							ret.AppendLine("!class=\"hide-on-mobile\"|{{UI|MHWI|IG Kinsect Bonus}}");
							break;
						case "HBG":
							ret.AppendLine("!class=\"hide-on-mobile\"|{{UI|MHWI|HBG Special Ammo}}");
							if (dataArray.Game != "MHWI")
							{
								ret.AppendLine("!class=\"hide-on-mobile\"|Reload / Recoil");
							}
							break;
						case "LBG":
							ret.AppendLine("!{{UI|MHWI|LBG Special Ammo}}");
							if (dataArray.Game != "MHWI")
							{
								ret.AppendLine("!class=\"hide-on-mobile\"|Reload / Recoil");
							}
							break;
						default: break;
					}
					if (tableHasElderseal)
					{
						ret.AppendLine(@"!class=""hide-on-mobile""|{{UI|MHWI|Elderseal}}");
					}
					if (tableHasRampageSlots)
					{
						ret.AppendLine(@"!class=""hide-on-mobile""|Rmpg. Slots");
					}
					if (tableHasRampageDecos)
					{
						ret.AppendLine(@"!class=""hide-on-mobile""|[[File:UI-Rampage Decoration 3.png|20x20px|center|link=]]");
					}
					if (tableHasArmorSkills)
					{
						ret.AppendLine(@"!class=""hide-on-mobile""|[[File:UI-Blights Negated.png|20x20px|center|Link=]]");
					}
					if (!new string[] { "Bo", "HBG", "LBG" }.Contains(tableIconType))
					{
						ret.AppendLine(@"!class=""hide-on-mobile""|[[File:2ndGen-Whetstone Icon Yellow.png|24x24px|link=]]");
					}
					else
					{
						ret.AppendLine("!");
					}
					ret.AppendLine(@"!class=""hide-on-mobile""|[[File:2ndGen-Decoration Icon Blue.png|24x24px|link=]] 
!class=""hide-on-mobile""|{{UI|MHWI|Defense}}");
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
						int trueRaw = Convert.ToInt32(Math.Round(Convert.ToInt32(dataObj.Attack) / Weapon.GetWeaponBloat(iconType, dataArray.Game)));
						ret.AppendLine($@"|-
| style=""text-align:left"" id=""{dataObj.Name.Replace(" ", "_")}_{dataArray.PathName.Replace(" ", "_")}"" | {prefix}{{{{GenericWeaponLink|{dataArray.Game}|{dataObj.Name}|{iconType}|{dataObj.Rarity}{(dataObj.CanForge == true ? "|true" : "")}{(dataObj.CanRollback == true ? (dataObj.CanForge != true ? "||true" : "|true") : "")}}}}}{(!string.IsNullOrEmpty(dataObj.PathLink) ? $" [[#{dataObj.PathLink} Tree|(Go to tree)]]" : "")}
| class=""hide-on-mobile""|{dataObj.Rarity}
| class=""hide-on-mobile""|{dataObj.Attack + (trueRaw == Convert.ToInt32(dataObj.Attack) ? "" : $" ({trueRaw})")}
| class=""hide-on-mobile""|{(string.IsNullOrEmpty(dataObj.Element) && dataObj.Element != "0" ? "-" : $"{{{{UI|UI|{dataObj.Element}|text={dataObj.ElementDamage}}}}}")}
| class=""hide-on-mobile""|{((dataObj.Affinity == 0 || dataObj.Affinity == null) ? "0%" : dataObj.Affinity + "%")}");
						switch (iconType)
						{
							case "Bo":
								ret.AppendLine($"|class=\"hide-on-mobile\"|{string.Join(", ", dataObj.BoCoatings.Split(',').Select(x => x.Trim()))}");
								break;
							case "CB":
								ret.AppendLine($"|class=\"hide-on-mobile\"|{dataObj.CBPhialType}");
								break;
							case "SA":
								ret.AppendLine($"|class=\"hide-on-mobile\"|{dataObj.SAPhialType}");
								break;
							case "GL":
								ret.AppendLine($"|class=\"hide-on-mobile\"|{dataObj.GLShellingType}");
								break;
							case "HH":
								ret.AppendLine($"|class=\"hide-on-mobile\"|{{{{UI|MHWI|HH Note|1 {dataObj.HHNote1}}}}}{{{{UI|MHWI|HH Note|2 {dataObj.HHNote2}}}}}{{{{UI|MHWI|HH Note|3 {dataObj.HHNote3}}}}}");
								break;
							case "IG":
								ret.AppendLine($"|class=\"hide-on-mobile\"|{dataObj.IGKinsectBonus}");
								break;
							case "HBG":
								ret.AppendLine($"|class=\"hide-on-mobile\"|{dataObj.HBGSpecialAmmoType}");
								if (dataArray.Game != "MHWI")
								{
									ret.AppendLine($"|class=\"hide-on-mobile\"|{dataObj.HBGReloadRecoil}");
								}
								else
								{
									ret.AppendLine("|class=\"hide-on-mobile\"| -");
								}
								break;
							case "LBG":
								ret.AppendLine($"|class=\"hide-on-mobile\"|{dataObj.LBGSpecialAmmoType}");
								if (dataArray.Game != "MHWI")
								{
									ret.AppendLine($"|class=\"hide-on-mobile\"|{dataObj.LBGReloadRecoil}");
								}
								else
								{
									ret.AppendLine("|class=\"hide-on-mobile\"| -");
								}
								break;
							default: break;
						}
						if (!string.IsNullOrEmpty(dataObj.Elderseal))
						{
							ret.AppendLine($@"|class=""hide-on-mobile""| {dataObj.Elderseal}");
						}
						else if (tableHasElderseal)
						{
							ret.AppendLine("|class=\"hide-on-mobile\"| -");
						}
						if (!string.IsNullOrEmpty(dataObj.RampageSlots) && dataObj.RampageSlots != "0")
						{
							ret.AppendLine($@"|class=""hide-on-mobile""| {dataObj.RampageSlots}");
						}
						else if (tableHasRampageSlots)
						{
							ret.AppendLine("|class=\"hide-on-mobile\"| -");
						}
						if (!string.IsNullOrEmpty(dataObj.RampageDeco) && dataObj.RampageDeco != "0")
						{
							ret.AppendLine($@"|class=""hide-on-mobile""| [[File:UI-Rampage Decoration {dataObj.RampageDeco}.png|20x20px|center]]");
						}
						else if (tableHasRampageDecos)
						{
							ret.AppendLine("|class=\"hide-on-mobile\"| -");
						}
						if (!string.IsNullOrEmpty(dataObj.ArmorSkill))
						{
							ret.AppendLine($@"|class=""hide-on-mobile""| {dataObj.ArmorSkill}{(!string.IsNullOrEmpty(dataObj.ArmorSkill2) ? ", " + dataObj.ArmorSkill2 : "")}");
						}
						else if (tableHasArmorSkills)
						{
							ret.AppendLine("|class=\"hide-on-mobile\"| -");
						}
						if (!new string[] {"Bo", "HBG", "LBG"}.Contains(iconType))
						{
							ret.AppendLine($@"|class=""hide-on-mobile""| {(string.IsNullOrEmpty(sharpness) ? "-" : sharpness)}");
						}
						ret.AppendLine($@"|class=""hide-on-mobile""| {(string.IsNullOrEmpty(decos) ? "-" : decos)}
|class=""hide-on-mobile""| {(string.IsNullOrEmpty(dataObj.Defense) && dataObj.Defense != "0" ? "-" : dataObj.Defense)}");
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
	
		private static List<WeaponCsv> ParseWeaponsFromCsv(string csvFile, bool duplicateSharpness, string delimiter = ",")
		{
			var weapons = new List<WeaponCsv>();
			using (var parser = new TextFieldParser(GenerateStreamFromString(csvFile)))
			{
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(delimiter);
				while (!parser.EndOfData)
				{
					//Processing row
					string[]? fields = parser.ReadFields();
					if(fields?.Length == 15 || fields?.Length == 22 || fields?.Length == 29) // Very manual thing that should be modified in case we want more row/make it more generic
                    {
                        var weapon = ParseWeaponFromLine(fields);

						if(fields.Length > 15)
                        {
							weapon.Sharpness1 = ParseSharpness1FromLine(fields);

							
							if (fields.Length == 29)
                            {
								weapon.Sharpness2 = ParseSharpness2FromLine(fields);
                            }
                            else if (duplicateSharpness)
                            {
                                weapon.Sharpness2 = weapon.Sharpness1;
                            }
                            else
							{
								weapon.Sharpness2 = new SharpnessData();
							}
                        }
                        weapons.Add(weapon);
                    }
				}
			}

            return weapons;
        }
		
		// Very straightforward but also sensible
		private static WeaponCsv ParseWeaponFromLine(string[] lineFields)
		{
			return new WeaponCsv()
			{
				Name = lineFields[0],
				Parent = lineFields[1],
				Rarity = GetIntFieldOrEmpty(lineFields[2]),
				Attack = GetIntFieldOrEmpty(lineFields[3]),
				Affinity = GetIntFieldOrEmpty(lineFields[4]),
				Defense = GetIntFieldOrEmpty(lineFields[5]),
				ElementHidden = GetBoolFieldOrEmpty(lineFields[6]),
				Element1 = lineFields[7],
				Element1Attack = GetIntFieldOrEmpty(lineFields[8]),
				Element2 = lineFields[9],
				Element2Attack = GetIntFieldOrEmpty(lineFields[10]),
				ElderSeal = lineFields[11],
				DecoSlot1 = GetIntFieldOrEmpty(lineFields[12]),
				DecoSlot2 = GetIntFieldOrEmpty(lineFields[13]),
				DecoSlot3 = GetIntFieldOrEmpty(lineFields[14]),
			};
        }

		private static SharpnessData ParseSharpness1FromLine(string[] fields)
		{
			return new SharpnessData()
			{
				Red = GetIntFieldOrEmpty(fields[15]),
				Orange = GetIntFieldOrEmpty(fields[16]),
				Yellow = GetIntFieldOrEmpty(fields[17]),
				Green = GetIntFieldOrEmpty(fields[18]),
				Blue = GetIntFieldOrEmpty(fields[19]),
				White = GetIntFieldOrEmpty(fields[20]),
				Purple = GetIntFieldOrEmpty(fields[21]),
			};
		}

        private static SharpnessData ParseSharpness2FromLine(string[] fields)
        {
            return new SharpnessData()
            {
                Red = GetIntFieldOrEmpty(fields[22]),
                Orange = GetIntFieldOrEmpty(fields[23]),
                Yellow = GetIntFieldOrEmpty(fields[24]),
                Green = GetIntFieldOrEmpty(fields[25]),
                Blue = GetIntFieldOrEmpty(fields[26]),
                White = GetIntFieldOrEmpty(fields[27]),
                Purple = GetIntFieldOrEmpty(fields[28]),
            };
        }

        private static int GetIntFieldOrEmpty(string field)
		{
			return !string.IsNullOrWhiteSpace(field) ? int.Parse(field) : 0;
        }

        private static bool? GetBoolFieldOrEmpty(string field)
        {
            return !string.IsNullOrWhiteSpace(field) ? bool.Parse(field) : null;
        }

        private static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

		private class WeaponCsv
		{
			public string Name { get; set; }
            public string Parent { get; set; }
			public int Rarity { get; set; }
            public int Attack { get; set; }
            public int Affinity { get; set; }
            public int Defense { get; set; }
            public bool? ElementHidden { get; set; }
            public string Element1 { get; set; }
            public int Element1Attack { get; set; }
            public string Element2 { get; set; }
            public int Element2Attack { get; set; }
            public string ElderSeal { get; set; }
            public int DecoSlot1 { get; set; }
            public int DecoSlot2 { get; set; }
            public int DecoSlot3 { get; set; }

			public SharpnessData Sharpness1 { get; set; }
            public SharpnessData Sharpness2 { get; set; }

        }
    }
}
