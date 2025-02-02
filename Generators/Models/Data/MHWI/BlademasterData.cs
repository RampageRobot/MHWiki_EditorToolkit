﻿// <auto-generated />
using System.Globalization;
using System.Text;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using MediawikiTranslator.Generators;
using MediawikiTranslator.Models.Data.MHRS;
using MediawikiTranslator.Models.Weapon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MediawikiTranslator.Models.Data.MHWI
{

	public partial class BlademasterData
	{
		[JsonProperty("WeaponType", NullValueHandling = NullValueHandling.Ignore)]
		public string WeaponType { get; set; }

		[JsonProperty("Name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		[JsonProperty("Index", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Index { get; set; }

		[JsonProperty("Id", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Id { get; set; }

		[JsonProperty("Unk1", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Unk1 { get; set; }

		[JsonProperty("BaseModelId", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? BaseModelId { get; set; }

		[JsonProperty("Part1Id", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Part1Id { get; set; }

		[JsonProperty("Part2Id", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Part2Id { get; set; }

		[JsonProperty("Color", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Color { get; set; }

		[JsonProperty("IsFixedUpgrade", NullValueHandling = NullValueHandling.Ignore)]
		public string IsFixedUpgrade { get; set; }

		[JsonProperty("Cost", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Cost { get; set; }

		[JsonProperty("Rarity", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Rarity { get; set; }

		[JsonProperty("SharpnessId", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? SharpnessId { get; set; }

		[JsonProperty("SharpnessAmount", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? SharpnessAmount { get; set; }

		[JsonProperty("Damage", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Damage { get; set; }

		[JsonProperty("Defense", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Defense { get; set; }

		[JsonProperty("Affinity", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Affinity { get; set; }

		[JsonProperty("Element", NullValueHandling = NullValueHandling.Ignore)]
		public string Element { get; set; }

		[JsonProperty("ElementDamage", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? ElementDamage { get; set; }

		[JsonProperty("HiddenElement", NullValueHandling = NullValueHandling.Ignore)]
		public string HiddenElement { get; set; }

		[JsonProperty("HiddenElementDamage", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? HiddenElementDamage { get; set; }

		[JsonProperty("Elderseal", NullValueHandling = NullValueHandling.Ignore)]
		public string Elderseal { get; set; }

		[JsonProperty("SlotCount", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? SlotCount { get; set; }

		[JsonProperty("Slot1Size", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Slot1Size { get; set; }

		[JsonProperty("Slot2Size", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Slot2Size { get; set; }

		[JsonProperty("Slot3Size", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Slot3Size { get; set; }

		[JsonProperty("SpecialAbility1Id", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? SpecialAbility1Id { get; set; }

		[JsonProperty("SpecialAbility2Id", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? SpecialAbility2Id { get; set; }

		[JsonProperty("Unk2", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Unk2 { get; set; }

		[JsonProperty("Unk3", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Unk3 { get; set; }

		[JsonProperty("Unk4", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Unk4 { get; set; }

		[JsonProperty("TreeId", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? TreeId { get; set; }

		[JsonProperty("TreePosition", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? TreePosition { get; set; }

		[JsonProperty("SkillId", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? SkillId { get; set; }

		[JsonProperty("SkillName", NullValueHandling = NullValueHandling.Ignore)]
		public string SkillName { get; set; }

		[JsonProperty("Unk5", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(BlademasterDataParseStringConverter))]
		public long? Unk5 { get; set; }

		[JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
		public string Description { get; set; }

		[JsonIgnore]
		public SharpnessData Sharpness { get; set; }

		public static WebToolkitData[] GetToolkitData()
		{
			List<WebToolkitData> ret = new List<WebToolkitData>();
			WeaponCraftingData[] craftingData = WeaponCraftingData.GetCraftingData();
			WeaponForgingData[] forgingData = WeaponForgingData.GetForgingData();
			Items[] allItems = Items.Fetch();
			BlademasterData[] data = [..GetBlademasterData().Where(x => x.Name != "Unavailable")];
			foreach (BlademasterData obj in data)
			{
				WebToolkitData newObj = new WebToolkitData()
				{
					Type = GetWeaponType(obj.WeaponType),
					Affinity = obj.Affinity,
					Attack = obj.Damage!.Value.ToString(),
					Defense = obj.Defense!.Value.ToString(),
					Description = obj.Description,
					Name = obj.Name,
					Elderseal = obj.Elderseal,
					Element1 = obj.HiddenElement != "None" ? "(" + obj.HiddenElement + ")" : obj.Element == "None" ? null : obj.Element,
					ElementDmg1 = obj.HiddenElement != "None" ? "(" + obj.HiddenElementDamage!.Value.ToString() + ")" : obj.ElementDamage == null ? null : obj.ElementDamage!.Value.ToString(),
					Game = "MHWI",
					Rarity = obj.Rarity + 1,
					Tree = GetWeaponTree(obj.TreeId!.Value)
				};
				int maxSharp = Convert.ToInt32((50 * obj.SharpnessAmount!.Value) + 150);
				SharpnessData newSharp = new SharpnessData()
				{
					Red = obj.Sharpness.Red > maxSharp ? maxSharp : obj.Sharpness.Red,
					Orange = obj.Sharpness.Orange > maxSharp ? maxSharp : obj.Sharpness.Orange,
					Yellow = obj.Sharpness.Yellow > maxSharp ? maxSharp : obj.Sharpness.Yellow,
					Green = obj.Sharpness.Green > maxSharp ? maxSharp : obj.Sharpness.Green,
					Blue = obj.Sharpness.Blue > maxSharp ? maxSharp : obj.Sharpness.Blue,
					White = obj.Sharpness.White > maxSharp ? maxSharp : obj.Sharpness.White,
					Purple = obj.Sharpness.Purple > maxSharp ? maxSharp : obj.Sharpness.Purple
				};
				SharpnessData handi = new SharpnessData()
				{
					Red = newSharp.Red,
					Orange = newSharp.Orange,
					Yellow = newSharp.Yellow,
					Green = newSharp.Green,
					Blue = newSharp.Blue,
					White = newSharp.White,
					Purple = newSharp.Purple
				};
				long[] limits = new long[] { obj.Sharpness.Red!.Value, obj.Sharpness.Orange!.Value, obj.Sharpness.Yellow!.Value, obj.Sharpness.Green!.Value, obj.Sharpness.Blue!.Value, obj.Sharpness.White!.Value, obj.Sharpness.Purple!.Value };
				long[] vals = new long[] { handi.Red!.Value, handi.Orange!.Value, handi.Yellow!.Value, handi.Green!.Value, handi.Blue!.Value, handi.White!.Value, handi.Purple!.Value };
				long leftovers = 50;
				long runningTotal = 0;
				for (int cntr = 0; cntr < vals.Length; cntr++)
				{
					if (leftovers > 0)
					{
						long thisVal = vals[cntr];
						long nextVal = 0;
						if (cntr + 1 < vals.Length)
						{
							nextVal = vals[cntr + 1];
						}
						long limit = limits[cntr];
						if (nextVal + leftovers > maxSharp && thisVal < limit)
						{
							if (thisVal + leftovers > limit)
							{
								vals[cntr] = limit;
								leftovers -= (limit - thisVal);
							}
							else
							{
								thisVal += leftovers;
								leftovers = 0;
								if (thisVal > 400)
								{
									thisVal = 400;
								}
								vals[cntr] = thisVal;
							}
						}
						runningTotal = vals[cntr];
					}
					else
					{
						vals[cntr] = limits[cntr] == 0 ? 0 : runningTotal;
					}
				}
				handi = new SharpnessData()
				{
					Red = vals[0],
					Orange = vals[1],
					Yellow = vals[2],
					Green = vals[3],
					Blue = vals[4],
					White = vals[5],
					Purple = vals[6]
				};
				obj.Sharpness = newSharp;
				//Comment the below out when switching to new sharpness template
				handi = new SharpnessData()
				{
					Red = handi.Red,
					Orange = handi.Orange - handi.Red,
					Yellow = handi.Yellow - handi.Orange,
					Green = handi.Green - handi.Yellow,
					Blue = handi.Blue - handi.Green,
					White = handi.White - handi.Blue < 0 ? 0 : handi.White - handi.Blue,
					Purple = handi.Purple - handi.White < 0 ? 0 : handi.Purple - handi.White
				};
				obj.Sharpness = new SharpnessData()
				{
					Red = obj.Sharpness.Red,
					Orange = obj.Sharpness.Orange - obj.Sharpness.Red,
					Yellow = obj.Sharpness.Yellow - obj.Sharpness.Orange,
					Green = obj.Sharpness.Green - obj.Sharpness.Yellow,
					Blue = obj.Sharpness.Blue - obj.Sharpness.Green,
					White = obj.Sharpness.White - obj.Sharpness.Blue < 0 ? 0 : obj.Sharpness.White - obj.Sharpness.Blue,
					Purple = obj.Sharpness.Purple - obj.Sharpness.White < 0 ? 0 : obj.Sharpness.Purple - obj.Sharpness.White
				};
				newObj.Sharpness = $"[[{obj.Sharpness.Red},{obj.Sharpness.Orange},{obj.Sharpness.Yellow},{obj.Sharpness.Green},{obj.Sharpness.Blue},{obj.Sharpness.White},{obj.Sharpness.Purple}],[{handi.Red},{handi.Orange},{handi.Yellow},{handi.Green},{handi.Blue},{handi.White},{handi.Purple}]]";
#nullable enable
				WeaponCraftingData? thisCraft = craftingData.FirstOrDefault(x => x.EquipmentId!.Value == obj.Index!.Value && x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace(" ", "_"));
				if (obj.TreePosition > 0 && thisCraft != null && thisCraft.Mat1Id > 0)
				{
					WeaponCraftingData? parentCraft = craftingData.FirstOrDefault(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace(" ", "_") && (x.ChildIndex1!.Value == thisCraft.Index!.Value || x.ChildIndex2!.Value == thisCraft.Index!.Value || x.ChildIndex3!.Value == thisCraft.Index!.Value || x.ChildIndex4!.Value == thisCraft.Index!.Value));
					BlademasterData? parent = data.FirstOrDefault(x => parentCraft != null && x.WeaponType == obj.WeaponType && x.Name == parentCraft.EquipmentName);
					if (parent == null || parentCraft == null)
					{
						WeaponForgingData forge = forgingData.First(x => x.EquipmentIndex!.Value == obj.Index!.Value && x.EquipmentType == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace(" ", "_"));
						parent = data.FirstOrDefault(x => x.WeaponType == obj.WeaponType && x.Name == forge.EquipmentName);
					}
					newObj.PreviousName = parent!.Name;
					newObj.PreviousRarity = parent!.Rarity;
					newObj.UpgradeCost = obj.Cost;
					newObj.UpgradeMaterials = GetMaterials(parent!, thisCraft, allItems);
				}
#nullable disable
				if (forgingData.Any(x => x.EquipmentIndex!.Value == obj.Index!.Value && x.EquipmentType == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace(" ", "_")))
				{
					WeaponForgingData forge = forgingData.First(x => x.EquipmentIndex!.Value == obj.Index!.Value && x.EquipmentType == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace(" ", "_"));
					newObj.ForgeCost = obj.Cost;
					newObj.ForgeMaterials = GetForgeMaterials(obj, forge, allItems);
				}
				if (obj.IsFixedUpgrade == "TRUE")
				{
					newObj.Rollback = "true";
				}
				if (thisCraft != null)
				{
					if (thisCraft.ChildIndex1 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex1!.Value);
						BlademasterData child = data.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						newObj.Next1Name = child.Name;
						newObj.Next1Rarity = child.Rarity;
						newObj.Next1Cost = obj.Cost;
						newObj.Next1Materials = GetMaterials(child, childCraft, allItems);
					}
					if (thisCraft.ChildIndex2 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex2!.Value);
						BlademasterData child = data.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						newObj.Next2Name = child.Name;
						newObj.Next2Rarity = child.Rarity;
						newObj.Next2Cost = obj.Cost;
						newObj.Next2Materials = GetMaterials(child, childCraft, allItems);
					}
					if (thisCraft.ChildIndex3 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex3!.Value);
						BlademasterData child = data.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						newObj.Next3Name = child.Name;
						newObj.Next3Rarity = child.Rarity;
						newObj.Next3Cost = obj.Cost;
						newObj.Next3Materials = GetMaterials(child, childCraft, allItems);
					}
				}
				newObj.Decos1 = (obj.Slot1Size == 1 ? 1 : 0) + (obj.Slot2Size == 1 ? 1 : 0) + (obj.Slot3Size == 1 ? 1 : 0);
				newObj.Decos2 = (obj.Slot1Size == 2 ? 1 : 0) + (obj.Slot2Size == 2 ? 1 : 0) + (obj.Slot3Size == 2 ? 1 : 0);
				newObj.Decos3 = (obj.Slot1Size == 3 ? 1 : 0) + (obj.Slot2Size == 3 ? 1 : 0) + (obj.Slot3Size == 3 ? 1 : 0);
				newObj.Decos4 = (obj.Slot1Size == 4 ? 1 : 0) + (obj.Slot2Size == 4 ? 1 : 0) + (obj.Slot3Size == 4 ? 1 : 0);
				if (newObj.Type == "HH")
				{
					string[] notes = GetHHNotes(obj.SpecialAbility1Id!.Value);
					newObj.HhNote1 = notes[0].Replace("_", " ");
					newObj.HhNote2 = notes[1].Replace("_", " ");
					newObj.HhNote3 = notes[2].Replace("_", " ");
				}
				else if (newObj.Type == "GL")
				{
					Tuple<string, int> shellType = GetGLShellingType(obj.SpecialAbility1Id!.Value);
					newObj.GlShellingType = shellType.Item1.Replace("_", " ");
					newObj.GlShellingLevel = shellType.Item2.ToString();
				}
				else if (newObj.Type == "SA")
				{
					Tuple<string, int> shellType = GetSAPhialType(obj.SpecialAbility1Id!.Value);
					newObj.SaPhialType = shellType.Item1.Replace("_", " ");
				}
				else if (newObj.Type == "CB")
				{
					newObj.CbPhialType = obj.SpecialAbility1Id!.Value == 0 ? "Impact Phial" : "Power Element Phial";
				}
				else if (newObj.Type == "IG")
				{
					switch (obj.SpecialAbility1Id!.Value)
					{
						case 0:
							newObj.IgKinsectBonus = "Sever Boost";
							break;
						case 1:
							newObj.IgKinsectBonus = "Blunt Boost";
							break;
						case 2:
							newObj.IgKinsectBonus = "Element Boost";
							break;
						case 3:
							newObj.IgKinsectBonus = "Speed Boost";
							break;
						case 4:
							newObj.IgKinsectBonus = "Stamina Boost";
							break;
						case 5:
							newObj.IgKinsectBonus = "Health Boost";
							break;
						case 6:
							newObj.IgKinsectBonus = "Spirit & Strength Boost ";
							break;
						case 7:
							newObj.IgKinsectBonus = "Stamina Upgrade & Healing";
							break;
					}
				}
				else if (newObj.Type == "DB")
				{
					newObj = GetDBElements(obj, newObj);
				}
				if (!ret.Any(x => x.Name == newObj.Name && x.Rarity == newObj.Rarity))
				{
					ret.Add(newObj);
				}
			}
			return [.. ret];
		}

		private static string GetForgeMaterials(BlademasterData parent, WeaponForgingData parentCraft, Items[] allItems)
		{
			string upgradeFromMats = "[";
			Tuple<long, long>[] mats = [..new Tuple<long?,long?>[] {
							new Tuple<long?,long?>(parentCraft.Mat1Id, parentCraft.Mat1Count),
							new Tuple<long?,long?>(parentCraft.Mat2Id, parentCraft.Mat2Count),
							new Tuple<long?,long?>(parentCraft.Mat3Id, parentCraft.Mat3Count),
							new Tuple<long?,long?>(parentCraft.Mat4Id, parentCraft.Mat4Count)
						}
						.Where(x => x.Item1.HasValue && x.Item1.Value > 0 && x.Item2.HasValue && x.Item2.Value > 0)
						.Select(x => new Tuple<long,long>(x.Item1!.Value, x.Item2.Value)).OrderByDescending(x => x.Item2)];
			foreach (Tuple<long, long> matIds in mats)
			{
				Items mat = allItems.First(x => x.Id!.Value == matIds.Item1);
				upgradeFromMats += $"{{\"name\": \"{mat.Name}\", \"icon\": \"{mat.WikiIconName}\", \"color\": \"{mat.WikiIconColor}\", \"quantity\": {matIds.Item2}}},";
			}
			return upgradeFromMats + "]";
		}

		private static string GetMaterials(BlademasterData parent, WeaponCraftingData parentCraft, Items[] allItems)
		{
			string upgradeFromMats = "[";
			Tuple<long, long>[] mats = [..new Tuple<long?,long?>[] {
							new Tuple<long?,long?>(parentCraft.Mat1Id, parentCraft.Mat1Count),
							new Tuple<long?,long?>(parentCraft.Mat2Id, parentCraft.Mat2Count),
							new Tuple<long?,long?>(parentCraft.Mat3Id, parentCraft.Mat3Count),
							new Tuple<long?,long?>(parentCraft.Mat4Id, parentCraft.Mat4Count)
						}
						.Where(x => x.Item1.HasValue && x.Item1.Value > 0 && x.Item2.HasValue && x.Item2.Value > 0)
						.Select(x => new Tuple<long,long>(x.Item1!.Value, x.Item2.Value)).OrderByDescending(x => x.Item2)];
			foreach (Tuple<long, long> matIds in mats)
			{
				Items mat = allItems.First(x => x.Id!.Value == matIds.Item1);
				upgradeFromMats += $"{{\"name\": \"{mat.Name}\", \"icon\": \"{mat.WikiIconName}\", \"color\": \"{mat.WikiIconColor}\", \"quantity\": {matIds.Item2}}},";
			}
			return upgradeFromMats + "]";
		}

		private static WebToolkitData GetDBElements(BlademasterData src, WebToolkitData ret)
		{
			switch (src.SpecialAbility1Id!.Value)
			{
				case 1:
					ret.Element1 = "Ice";
					ret.ElementDmg1 = "21";
					ret.Element2 = "Blast";
					ret.ElementDmg2 = "21";
					break;
				case 2:
					ret.Element1 = "Ice";
					ret.ElementDmg1 = "24";
					ret.Element2 = "Blast";
					ret.ElementDmg2 = "24";
					break;
				case 3:
					ret.Element1 = "Dragon";
					ret.ElementDmg1 = "20";
					ret.Element2 = "Paralyze";
					ret.ElementDmg2 = "60";
					break;
				case 4:
					ret.Element1 = "Ice";
					ret.ElementDmg1 = "33";
					ret.Element2 = "Blast";
					ret.ElementDmg2 = "33";
					break;
				case 5:
					ret.Element1 = "Poison";
					ret.ElementDmg1 = "27";
					ret.Element2 = "Fire";
					ret.ElementDmg2 = "27";
					break;
				case 6:
					ret.Element1 = "Poison";
					ret.ElementDmg1 = "30";
					ret.Element2 = "Fire";
					ret.ElementDmg2 = "30";
					break;
				case 7:
					ret.Element1 = "Poison";
					ret.ElementDmg1 = "36";
					ret.Element2 = "Fire";
					ret.ElementDmg2 = "36";
					break;
				case 8:
					ret.Element1 = "Thunder";
					ret.ElementDmg1 = "24";
					ret.Element2 = "Dragon";
					ret.ElementDmg2 = "24";
					break;
				case 9:
					ret.Element1 = "Thunder";
					ret.ElementDmg1 = "27";
					ret.Element2 = "Dragon";
					ret.ElementDmg2 = "27";
					break;
				case 10:
					ret.Element1 = "Paralyze";
					ret.ElementDmg1 = "12";
					ret.Element2 = "Sleep";
					ret.ElementDmg2 = "6";
					break;
				case 11:
					ret.Element1 = "Paralyze";
					ret.ElementDmg1 = "15";
					ret.Element2 = "Sleep";
					ret.ElementDmg2 = "9";
					break;
			}
			return ret;
		}

		private static Tuple<string, int> GetSAPhialType(long key)
		{
			return new Dictionary<long, Tuple<string, int>>()
			{
				{ 0, new Tuple<string,int>("Power", 0) },
				{ 1, new Tuple<string,int>("Power_Element", 0) },
				{ 2, new Tuple<string,int>("Dragon", 6) },
				{ 3, new Tuple<string,int>("Dragon", 12) },
				{ 4, new Tuple<string,int>("Dragon", 18) },
				{ 5, new Tuple<string,int>("Dragon", 24) },
				{ 6, new Tuple<string,int>("Dragon", 30) },
				{ 7, new Tuple<string,int>("Dragon", 36) },
				{ 8, new Tuple<string,int>("Dragon", 42) },
				{ 9, new Tuple<string,int>("Dragon", 48) },
				{ 10, new Tuple<string,int>("Dragon", 51) },
				{ 11, new Tuple<string,int>("Dragon", 57) },
				{ 12, new Tuple<string,int>("Exhaust", 9) },
				{ 13, new Tuple<string,int>("Exhaust", 12) },
				{ 14, new Tuple<string,int>("Exhaust", 15) },
				{ 15, new Tuple<string,int>("Exhaust", 18) },
				{ 16, new Tuple<string,int>("Exhaust", 21) },
				{ 17, new Tuple<string,int>("Exhaust", 24) },
				{ 18, new Tuple<string,int>("Exhaust", 27) },
				{ 19, new Tuple<string,int>("Exhaust", 30) },
				{ 20, new Tuple<string,int>("Exhaust", 33) },
				{ 21, new Tuple<string,int>("Exhaust", 36) },
				{ 22, new Tuple<string,int>("Paralysis", 15) },
				{ 23, new Tuple<string,int>("Paralysis", 18) },
				{ 24, new Tuple<string,int>("Paralysis", 21) },
				{ 25, new Tuple<string,int>("Paralysis", 24) },
				{ 26, new Tuple<string,int>("Paralysis", 27) },
				{ 27, new Tuple<string,int>("Paralysis", 30) },
				{ 28, new Tuple<string,int>("Paralysis", 33) },
				{ 29, new Tuple<string,int>("Paralysis", 36) },
				{ 30, new Tuple<string,int>("Paralysis", 39) },
				{ 31, new Tuple<string,int>("Paralysis", 42) },
				{ 32, new Tuple<string,int>("Poison", 6) },
				{ 33, new Tuple<string,int>("Poison", 12) },
				{ 34, new Tuple<string,int>("Poison", 18) },
				{ 35, new Tuple<string,int>("Poison", 24) },
				{ 36, new Tuple<string,int>("Poison", 30) },
				{ 37, new Tuple<string,int>("Poison", 36) },
				{ 38, new Tuple<string,int>("Poison", 42) },
				{ 39, new Tuple<string,int>("Poison", 48) },
				{ 40, new Tuple<string,int>("Poison", 54) },
				{ 41, new Tuple<string,int>("Poison", 60) }
			}[key];
		}

		private static Tuple<string, int> GetGLShellingType(long key)
		{
			return new Dictionary<long, Tuple<string, int>>()
			{
				{ 0, new Tuple<string,int>("Normal", 0) },
				{ 1, new Tuple<string,int>("Normal", 1) },
				{ 2, new Tuple<string,int>("Normal", 2) },
				{ 3, new Tuple<string,int>("Normal", 3) },
				{ 4, new Tuple<string,int>("Normal", 4) },
				{ 5, new Tuple<string,int>("Wide", 0) },
				{ 6, new Tuple<string,int>("Wide", 1) },
				{ 7, new Tuple<string,int>("Wide", 2) },
				{ 8, new Tuple<string,int>("Wide", 3) },
				{ 9, new Tuple<string,int>("Wide", 4) },
				{ 10, new Tuple<string,int>("Long", 0) },
				{ 11, new Tuple<string,int>("Long", 1) },
				{ 12, new Tuple<string,int>("Long", 2) },
				{ 13, new Tuple<string,int>("Long", 3) },
				{ 14, new Tuple<string,int>("Long", 4) },
				{ 15, new Tuple<string,int>("Normal", 5) },
				{ 16, new Tuple<string,int>("Wide", 5) },
				{ 17, new Tuple<string,int>("Long", 5) },
				{ 18, new Tuple<string,int>("Normal", 6) },
				{ 19, new Tuple<string,int>("Wide", 6) },
				{ 20, new Tuple<string,int>("Long", 6) }
			}[key];
		}

		private static string[] GetHHNotes(long key)
		{
			return new Dictionary<long, string[]>()
			{
				{ 0, new string[] { "White", "Red", "Light_Blue" } },
				{ 1, new string[] { "Purple", "Red", "Light_Blue" } },
				{ 2, new string[] { "White", "Dark_Blue", "Green" } },
				{ 3, new string[] { "Purple", "Dark_Blue", "Green" } },
				{ 4, new string[] { "White", "Red", "Green" } },
				{ 5, new string[] { "Purple", "Red", "Green" } },
				{ 6, new string[] { "White", "Red", "Dark_Blue" } },
				{ 7, new string[] { "Purple", "Red", "Dark_Blue" } },
				{ 8, new string[] { "White", "Dark_Blue", "Light_Blue" } },
				{ 9, new string[] { "Purple", "Dark_Blue", "Light_Blue" } },
				{ 10, new string[] { "White", "Dark_Blue", "Yellow" } },
				{ 11, new string[] { "Purple", "Dark_Blue", "Yellow" } },
				{ 12, new string[] { "White", "Green", "Light_Blue" } },
				{ 13, new string[] { "Purple", "Green", "Light_Blue" } },
				{ 14, new string[] { "White", "Green", "Yellow" } },
				{ 15, new string[] { "Purple", "Green", "Yellow" } },
				{ 16, new string[] { "White", "Red", "Yellow" } },
				{ 17, new string[] { "Purple", "Red", "Yellow" } },
				{ 18, new string[] { "White", "Light_Blue", "Yellow" } },
				{ 19, new string[] { "Purple", "Light_Blue", "Yellow" } },
				{ 20, new string[] { "Purple", "Red", "Orange" } },
				{ 21, new string[] { "Purple", "Dark_Blue", "Orange" } },
				{ 22, new string[] { "Purple", "Green", "Orange" } },
				{ 23, new string[] { "Purple", "Light_Blue", "Orange" } },
				{ 24, new string[] { "Purple", "Yellow", "Orange" } },
				{ 25, new string[] { "Purple", "Green", "Light_Blue" } },
				{ 26, new string[] { "White", "Green", "Yellow" } },
				{ 27, new string[] { "Purple", "Green", "Yellow" } },
				{ 28, new string[] { "White", "Light_Blue", "Yellow" } },
				{ 29, new string[] { "Purple", "Light_Blue", "Yellow" } },
				{ 30, new string[] { "Purple", "Red", "Orange" } },
				{ 31, new string[] { "Purple", "Dark_Blue", "Orange" } },
				{ 32, new string[] { "Purple", "Orange", "Green" } },
				{ 33, new string[] { "Purple", "Light_Blue", "Orange" } },
				{ 34, new string[] { "Purple", "Yellow", "Orange" } },
				{ 35, new string[] { "White", "Red", "Dark_Blue" } },
				{ 36, new string[] { "White", "Red", "Dark_Blue" } },
				{ 37, new string[] { "White", "Red", "Dark_Blue" } },
				{ 38, new string[] { "White", "Red", "Dark_Blue" } },
				{ 39, new string[] { "White", "Red", "Dark_Blue" } },
				{ 40, new string[] { "White", "Red", "Dark_Blue" } },
				{ 41, new string[] { "White", "Red", "Dark_Blue" } },
				{ 42, new string[] { "White", "Red", "White" } },
				{ 43, new string[] { "White", "Red", "Light_Blue" } },
				{ 44, new string[] { "White", "Red", "Dark_Blue" } },
				{ 45, new string[] { "White", "Red", "Green" } },
				{ 46, new string[] { "White", "Red", "Yellow" } },
				{ 47, new string[] { "White", "Red", "Orange" } },
				{ 48, new string[] { "White", "Red", "Purple" } },
				{ 49, new string[] { "White", "Red", "Red" } }
			}[key];
		}

		private static string GetWeaponTree(long key)
		{
			return new Dictionary<long, string>()
			{
				{ 0, "Unavailable" },
				{ 1, "Ore Tree" },
				{ 2, "Bone Tree" },
				{ 3, "Great Jagras Tree" },
				{ 4, "Vespoid Tree" },
				{ 5, "Kulu-Ya-Ku Tree" },
				{ 6, "Pukei-Pukei Tree" },
				{ 7, "Jyuratodus Tree" },
				{ 8, "Barroth Tree" },
				{ 9, "Tobi-Kadachi Tree" },
				{ 10, "Anjanath Tree" },
				{ 11, "Nergigante Tree" },
				{ 12, "Rathian Tree" },
				{ 13, "Hornetaur Tree" },
				{ 14, "Great Girros Tree" },
				{ 15, "Tzitzi-Ya-Ku Tree" },
				{ 16, "Paolumu Tree" },
				{ 17, "Legiana Tree" },
				{ 18, "Radobaan Tree" },
				{ 19, "Odogaron Tree" },
				{ 20, "Vaal Hazak Tree" },
				{ 21, "Rathalos Tree" },
				{ 22, "Diablos Tree" },
				{ 23, "Kirin Tree" },
				{ 24, "Dodogama Tree" },
				{ 25, "Lavasioth Tree" },
				{ 26, "Uragaan Tree" },
				{ 27, "Pink Rathian Tree" },
				{ 28, "Azure Rathalos Tree" },
				{ 29, "Black Diablos Tree" },
				{ 30, "Teostra Tree" },
				{ 31, "Kushala Daora Tree" },
				{ 32, "Xeno'jiiva Tree" },
				{ 33, "Bazelgeuse Tree" },
				{ 34, "Zorah Magdaros Tree" },
				{ 35, "Dragonbone Tree" },
				{ 36, "Blacksteel Tree" },
				{ 37, "Water Element Tree" },
				{ 38, "Thunder Element Tree" },
				{ 39, "Ice Element Tree" },
				{ 40, "Thunder Element Tree" },
				{ 41, "Water Element Tree" },
				{ 42, "Workshop Weapon Tree" },
				{ 43, "??? Tree" },
				{ 44, "Deviljho Tree" },
				{ 45, "Lunastra Tree" },
				{ 46, "HARDUMMY" },
				{ 47, "Azure Star Tree" },
				{ 48, "Gae Bolg" },
				{ 49, "Dante's Devil Sword" },
				{ 50, "Wyvern Ignition" },
				{ 51, "Lunastra/Nergigante Tree" },
				{ 52, "Lunastra/Xeno'jiiva Tree" },
				{ 53, "Workshop Weapon Tree" },
				{ 54, "Workshop Weapon Tree" },
				{ 55, "The Witcher Tree" },
				{ 56, "Defender Tree" },
				{ 57, "Unavailable" },
				{ 58, "Unavailable" },
				{ 59, "Unavailable" },
				{ 60, "Stygian Zinogre Tree" },
				{ 61, "Banbaro Tree" },
				{ 62, "Nargacuga Tree" },
				{ 63, "Glavenus Tree" },
				{ 64, "Coral Pukei Tree" },
				{ 65, "Velkhana Tree" },
				{ 66, "Tigrex Tree" },
				{ 67, "Fulgur Anjanath Tree" },
				{ 68, "Nightshade Lumu Tree" },
				{ 69, "Brachydios Tree" },
				{ 70, "Shara Ishvalda Tree" },
				{ 71, "Rajang Tree" },
				{ 72, "Alatreon Tree" },
				{ 73, "Yian Garuga Tree" },
				{ 74, "Zinogre Tree" },
				{ 75, "Barioth Tree" },
				{ 76, "Acidic Glavenus Tree" },
				{ 77, "Viper Kadachi Tree" },
				{ 78, "Namielle Tree" },
				{ 79, "Beotodus Tree" },
				{ 80, "Ebony Odogaron Tree" },
				{ 81, "Brute Tigrex Tree" },
				{ 82, "Workshop Weapon Tree" },
				{ 83, "Workshop Weapon Tree" },
				{ 84, "Guild Palace Tree" },
				{ 85, "Furious Rajang Tree" },
				{ 86, "Raging Brachydios Tree" },
				{ 87, "Unavailable" },
				{ 88, "Frostfang Barioth Tree" },
				{ 89, "Fatalis Tree" },
				{ 90, "HARDUMMY" },
				{ 91, "Unavailable" },
				{ 92, "Azure Era Tree" },
				{ 93, "Unavailable" },
				{ 94, "Unavailable" },
				{ 95, "Unavailable" },
				{ 96, "Unavailable" },
				{ 97, "Unavailable" },
				{ 98, "Unavailable" },
				{ 99, "Unavailable" },
				{ 100, "Unavailable" },
				{ 101, "Unavailable" },
				{ 102, "Unavailable" },
				{ 103, "Unavailable" },
				{ 104, "Unavailable" },
				{ 105, "Unavailable" },
				{ 106, "Unavailable" },
				{ 107, "Unavailable" },
				{ 108, "Unavailable" },
				{ 109, "Unavailable" },
				{ 110, "Unavailable" },
				{ 111, "Unavailable" },
				{ 112, "Unavailable" },
				{ 113, "Unavailable" },
				{ 114, "Unavailable" },
				{ 115, "Unavailable" },
				{ 116, "Unavailable" },
				{ 117, "Unavailable" },
				{ 118, "Unavailable" },
				{ 119, "Unavailable" },
				{ 120, "Unavailable" },
				{ 121, "Unavailable" },
				{ 122, "Unavailable" },
				{ 123, "Unavailable" },
				{ 124, "Unavailable" },
				{ 125, "Unavailable" },
				{ 126, "Unavailable" },
				{ 127, "Unavailable" },
				{ 128, "Unavailable" },
				{ 129, "Unavailable" },
				{ 130, "Unavailable" },
				{ 131, "Unavailable" },
				{ 132, "Unavailable" },
				{ 133, "Unavailable" },
				{ 134, "Unavailable" },
				{ 135, "Unavailable" },
				{ 136, "Unavailable" },
				{ 137, "Unavailable" },
				{ 138, "Unavailable" },
				{ 139, "Unavailable" },
				{ 140, "Unavailable" },
				{ 141, "Unavailable" },
				{ 142, "Unavailable" },
				{ 143, "Unavailable" },
				{ 144, "Unavailable" },
				{ 145, "Unavailable" },
				{ 146, "Unavailable" },
				{ 147, "Unavailable" },
				{ 148, "Unavailable" },
				{ 149, "Unavailable" },
				{ 150, "Unavailable" },
			}[key];
		}

		private static string GetWeaponType(string key)
		{
			return new Dictionary<string, string>()
			{
				{ "Charge Blade", "CB" },
				{ "Dual Blades", "DB" },
				{ "Great Sword", "GS" },
				{ "Gunlance", "GL" },
				{ "Hammer", "Hm" },
				{ "Hunting Horn", "HH" },
				{ "Insect Glaive", "IG" },
				{ "Lance", "Ln" },
				{ "Long Sword", "LS" },
				{ "Switch Axe", "SA" },
				{ "Sword and Shield", "SnS" },
				{ "Bow", "Bo" },
				{ "Heavy Bowgun", "HBG" },
				{ "Light Bowgun", "LBG" }
			}[key];
		}

		public static BlademasterData[] GetBlademasterData()
		{
			SharpnessData[] allSharpness = SharpnessData.GetSharpnessData();
			BlademasterData[] data = FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Raw Data\MHWI\chunk\common\equip\blademasterdata.json"));
			foreach (BlademasterData obj in data)
			{
				obj.Sharpness = allSharpness.First(x => x.Id == obj.SharpnessId!.Value);
			}
			return data;
		}

		public static void GetSimplifiedWeaponData()
		{
			SimplifiedSkill[] skills = SkillDescriptions.GetSimplifiedSkills();
			GunnerData[] gunner = GunnerData.GetGunnerData();
			BlademasterData[] blademaster = GetBlademasterData();
			Skills[] allSkills = MHWI.Skills.GetSkills();
			List<SimplifiedWeapon> allSimpleWeapons = [];
			foreach (BlademasterData data in blademaster.Where(x => x.Name != "HARDUMMY" && x.Name != "Unavailable" && x.Name != "Invalid Message"))
			{
				SimplifiedWeapon simpleWeapon = new SimplifiedWeapon()
				{
					Name = data.Name,
					WeaponType = data.WeaponType,
					Rarity = (int)data.Rarity!.Value
				};
				if (data.SkillId!.Value > 0)
				{
					SimplifiedSkill simpleSkill = skills.First(x => x.Id == data.SkillId!.Value);
					simpleWeapon.Skill = new SimplifiedSkill()
					{
						Description = simpleSkill.Description,
						Id = simpleSkill.Id,
						Level = 1,
						MaxLevel = simpleSkill.MaxLevel,
						LevelDetails = simpleSkill.LevelDetails,
						Name = simpleSkill.Name,
						WikiIconColor = simpleSkill.WikiIconColor
					};
				}
				allSimpleWeapons.Add(simpleWeapon);
			}
			foreach (GunnerData data in gunner.Where(x => x.Name != "HARDUMMY" && x.Name != "Unavailable" && x.Name != "Invalid Message"))
			{
				SimplifiedWeapon simpleWeapon = new SimplifiedWeapon()
				{
					Name = data.Name,
					WeaponType = data.WeaponType,
					Rarity = (int)data.Rarity!.Value
				};
				if (data.SkillId!.Value > 0)
				{
					SimplifiedSkill simpleSkill = skills.First(x => x.Id == data.SkillId!.Value);
					simpleWeapon.Skill = new SimplifiedSkill()
					{
						Description = simpleSkill.Description,
						Id = simpleSkill.Id,
						Level = 1,
						MaxLevel = simpleSkill.MaxLevel,
						LevelDetails = simpleSkill.LevelDetails,
						Name = simpleSkill.Name,
						WikiIconColor = simpleSkill.WikiIconColor
					};
				}
				allSimpleWeapons.Add(simpleWeapon);
			}
			File.WriteAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\mhwi simple weapon data.json", JsonConvert.SerializeObject(allSimpleWeapons, Formatting.Indented));
		}
	}

	public class SimplifiedWeapon()
	{
		public string Name { get; set; }
		public string WeaponType { get; set; }
		public int Rarity { get; set; }
		public SimplifiedSkill Skill { get; set; }
	}

	public partial class BlademasterData
	{
		public static BlademasterData[] FromJson(string json) => JsonConvert.DeserializeObject<BlademasterData[]>(json, MediawikiTranslator.Models.Data.MHWI.BlademasterDataConverter.Settings);
	}

	internal static class BlademasterDataConverter
	{
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			DateParseHandling = DateParseHandling.None,
			Converters =
			{
				new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
			},
		};
	}

	internal class BlademasterDataParseStringConverter : JsonConverter
	{
		public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

		public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null) return null;
			var value = serializer.Deserialize<string>(reader);
			long l;
			if (Int64.TryParse(value.Replace("Unavailable", "0"), out l))
			{
				return l;
			}
			throw new Exception("Cannot unmarshal type long");
		}

		public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}
			var value = (long)untypedValue;
			serializer.Serialize(writer, value.ToString());
			return;
		}

		public static readonly BlademasterDataParseStringConverter Singleton = new BlademasterDataParseStringConverter();
	}
}
