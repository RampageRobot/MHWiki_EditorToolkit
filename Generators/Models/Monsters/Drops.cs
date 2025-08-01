
using DocumentFormat.OpenXml.Drawing.Charts;
using MediawikiTranslator.Models.Data.MHWI;
using MediawikiTranslator.Models.MaterialsAndDropTables;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace MediawikiTranslator.Models.Monsters
{
//RW000 = Carves
//RW001 = Carves(Severed Part)
//RW002 = Carves(Small Monster)
//RW003 = Fishing
//RW004 = Target Rewards
//RW005 = Broken Part(use partsindex)
//RW006 = Wound Destroyed
//RW007 = ???
//RW008 = Carves(Rotten)
//RW009 = Carves(Small Monster)
//RW010 = Not for large monsters
//RW011 = Not for large monsters
//RW012 = Carves(Rotten Severed Part)
//RW013 = Not for large monsters
//RW014 = Not for large monsters
//RW015 = Tempered Wound Destruction
	public class Drops
	{
		private static readonly Data.MHWilds.Items[] MhwildsItems = Data.MHWilds.Items.Fetch();
		private static readonly Dictionary<string, Dictionary<string, object>> WildsMonsterMasterlist = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\enemies.json"))!;
		private static Dictionary<string, object> WildsPartNames = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Data\EnemyPartsTypeName.msg.23.json"))!;

		public static string Format(string monsterName, int[] availableRanks, string[] partNames, string game = "MHWI")
		{
			if (game == "MHWI")
			{
				if (File.Exists($@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\Drops.json"))
				{
					dynamic[] baseObj = [.. ((Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject<dynamic>(File.ReadAllText($@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\Drops.json"))!.data)];
					dynamic[] dropsFile = [.. (Newtonsoft.Json.Linq.JArray)baseObj.First(x => x.GridName == "Entries").list];
					BremInfo[] bremFile = JsonConvert.DeserializeObject<BremInfo[]>(File.ReadAllText($@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\BremRewards.json"))!;
					Items[] allItems = Items.Fetch();
					List<WebToolkitData> dropsFormatted = [];
					foreach (BremRank rank in Enum.GetValues<BremRank>())
					{
						if (availableRanks.Contains((int)rank))
						{
							BremInfo[] bremsForRank = [.. bremFile.Where(x => x.Rank == rank)];
							int[] rankIndices = GetRankIndices(rank);
							dynamic[] dropsForRank = [.. dropsFile.Where(x => rankIndices.Contains((int)x.Index))];
							WebToolkitData rankData = new()
							{
								Rank = new string[] { "Low", "High", "Master" }[(int)rank],
								Monster = monsterName
							};
							foreach (dynamic dropTable in dropsForRank.OrderBy(x => x.Index))
							{
								dynamic[] items = [.. (Newtonsoft.Json.Linq.JArray)dropTable.Items_raw];
								dynamic[] counts = [.. (Newtonsoft.Json.Linq.JArray)dropTable.Counts_raw];
								dynamic[] weights = [.. (Newtonsoft.Json.Linq.JArray)dropTable.Percents_raw];
								for (int i = 0; i < items.Length; i++)
								{
									if (items[i].Item_Id > 0)
									{
										rankData = AddItemToRank((int)items[i].Item_Id, (int)counts[i].Item_Count, (int)weights[i].Item_Weight, allItems, rankData, "Gathered", GetDropName((int)dropTable.Index));
									}
								}
							}
							foreach (BremInfo brem in bremsForRank)
							{
								for (int i = 0; i < brem.ItemIds.Length; i++)
								{
									if (brem.ItemIds[i] > 0)
									{
										bool cont = true;
										string header = "";
										switch (brem.Type)
										{
											case BremType.Capture:
												header = "Capture Rewards";
												break;
											case BremType.PartBreak:
												if (brem.PartBreakIndex!.Value < 10)
												{
													string partName = partNames[brem.PartBreakIndex!.Value];
													header = "Break " + char.ToUpper(partName[0]) + partName[1..];
												}
												else
												{
													cont = false;
												}
												break;
											case BremType.HuntNonTarget:
												header = "Hunt Rewards";
												break;
										}
										if (cont)
										{
											rankData = AddItemToRank(brem.ItemIds[i], brem.Counts[i], brem.Weights[i], allItems, rankData, "Quest Rewards", header);
										}
									}
								}
							}
							dropsFormatted.Add(rankData);
						}
					}
					return Generators.MaterialsAndDropTables.GenerateDataUnescape([.. dropsFormatted], "MHWI");
				}
				else
				{
					return "";
				}
			}
			else
			{
				MonsterId monsterId = Monster.WildsMonsterIds.First(x => x.Name == monsterName);
				string monsterFolder = monsterId.Id.Substring(0, monsterId.Id.IndexOf("_"));
				string subspeciesFolder = monsterId.Id.Substring(monsterId.Id.IndexOf("_") + 1, 2);
				Dictionary<string, string> monsterParts = [];
				foreach (KeyValuePair<string, dynamic> part in ((JObject)((JObject)WildsMonsterMasterlist[monsterId.Id]["part_datas"]).ToObject<Dictionary<string, object>>()!["parts"]).ToObject<Dictionary<string, dynamic>>()!.Where(x => x.Key != "legendary_scar_nums"))
				{
					string partName = GetWildsPartName(part.Value.part_type.ToString());
					if (string.IsNullOrEmpty(partName.Replace("'", "")))
					{
						partName = "???";
					}
					monsterParts.Add(part.Key, partName);
				}
				List<WebToolkitData> dropsFormatted = [];
				DropFileValue[] dropFile = WildsDropFile.FromJson(File.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Enemy\{monsterId.Id}.user.3.json")).First().AppUserDataEnemyRewardData.Values;
				dynamic[] partsLostArray = [];
				string partsLostFile = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Enemy\{monsterFolder}\{subspeciesFolder}\Data\{monsterFolder}_{subspeciesFolder}_Param_PartsLost.user.3.json";
				if (File.Exists(partsLostFile))
				{
					partsLostArray = ((JArray)JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(partsLostFile))![0]["app.user_data.EmParamPartsLost"]._PartsLostArray["ace.cInstanceGuidArray`1<app.user_data.EmParamPartsLost.cPartsLost>"]._DataArray).ToObject<dynamic[]>()!;
				}
				dynamic[] partsBreakArray = [];
				string partsBreakFile = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Enemy\{monsterFolder}\{subspeciesFolder}\Data\{monsterFolder}_{subspeciesFolder}_Param_PartsBreakReward.user.3.json";
				if (File.Exists(partsBreakFile))
				{
					partsBreakArray = ((JArray)JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(partsBreakFile))![0]["app.user_data.EmParamPartsBreakReward"]._PartsBreakArray).ToObject<dynamic[]>()!; ;
				}
				string type = "[0]INVALID";
				foreach (DropFileValue table in dropFile)
				{
					string header = "";
					string category = "";
					int order = 0;
					if (!table.AppUserDataEnemyRewardDataCData.RewardType.AppEnemyRewardRewardTypeSerializable.Value.EndsWith("INVALID") && !table.AppUserDataEnemyRewardDataCData.RewardType.AppEnemyRewardRewardTypeSerializable.Value.EndsWith("INVARID"))
					{
						type = table.AppUserDataEnemyRewardDataCData.RewardType.AppEnemyRewardRewardTypeSerializable.Value;
						type = type[(type.IndexOf(']') + 1)..];
					}
					switch (type)
					{
						case "RW000":
							header = "Gathered";
							category = "Carves";
							break;
						case "RW008":
							header = "Gathered";
							category = "Carves (Rotten)";
							order = 1;
							break;
						case "RW001":
							header = "Gathered";
							category = "Carves (Severed Part)";
							order = 2;
							break;
						case "RW012":
							header = "Gathered";
							category = "Carves (Rotten Severed Part)";
							order = 3;
							break;
						case "RW005":
							header = "Quest Rewards";
							category = "Broken Part Rewards";
							order = 1;
							break;
						case "RW006":
							header = "Quest Rewards";
							category = "Wound Destruction Rewards";
							order = 2;
							break;
						case "RW004":
							header = "Quest Rewards";
							category = "Hunt Rewards";
							break;
						case "RW015":
							header = "Quest Rewards";
							category = "Tempered Wound Destruction Rewards";
							order = 3;
							break;
						case "RW016":
							header = "Gathered";
							category = "Carves (Crystalized)";
							order = 1;
							break;
						default:
							Debugger.Break();
							break;
					}
					if (table.AppUserDataEnemyRewardDataCData.IdStory.AppItemDefIdSerializable.Value != "[0]INVALID" && table.AppUserDataEnemyRewardDataCData.IdStory.AppItemDefIdSerializable.Value != "[0]INVARID")
					{
						Data.MHWilds.Items item = MhwildsItems.First(x => x.ItemID == table.AppUserDataEnemyRewardDataCData.IdStory.AppItemDefIdSerializable.Value);
						int quantity = (int)table.AppUserDataEnemyRewardDataCData.RewardNumStory!;
						int probability = (int)table.AppUserDataEnemyRewardDataCData.ProbabilityStory!;
						Item newItem = new()
						{
							Category = category,
							Chance = probability.ToString(),
							Quantity = quantity,
							Description = item.Description,
							Icon = item.Icon,
							IconColor = item.IconColor,
							Include = false,
							ItemId = table.AppUserDataEnemyRewardDataCData.IdStory.AppItemDefIdSerializable.Value,
							ItemName = item.Name,
							Price = item.BuyPrice.ToString()!,
							Rarity = item.Rarity,
							Order = order
						};
						if (category.Contains("Broken Part Rewards"))
						{
							string name = "";
							if (monsterName == "Guardian Arkveld")
							{
								switch (table.AppUserDataEnemyRewardDataCData.PartsIndex)
								{
									case 0:
										name = "Head";
										break;
									case 1:
										name = "Left Chainblade";
										break;
									case 2:
										name = "Right Chainblade";
										break;
								}
							}
							else
							{
								name = partsBreakArray.First(x => x["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].RewardTableIndex == table.AppUserDataEnemyRewardDataCData.PartsIndex)["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value;
								name = GetWildsPartName(name.Substring(name.IndexOf(']') + 1));
							}
							int breakAmt = 0;
							if (monsterName == "Guardian Arkveld")
							{
								breakAmt = 1;
							}
							else
							{
								breakAmt = (int)((JArray)partsBreakArray.First(x => x["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].RewardTableIndex == table.AppUserDataEnemyRewardDataCData.PartsIndex)["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].PartsBreakData).ToObject<dynamic[]>()![0]["app.user_data.EmParamPartsBreakReward.cPartsBreakData"]._PartsBreakLevel;
							}
							newItem.Category = "Break " + name + (breakAmt == 1 ? "" : " (" + breakAmt + "x)");
							dropsFormatted = AddItemToDrops(newItem, header, "Low", monsterName, dropsFormatted);
						}
						else if (category.Contains("Severed Part"))
						{
							foreach (dynamic part in partsLostArray)
							{
								newItem.Category = "Carve " + GetWildsPartName(part["app.user_data.EmParamPartsLost.cPartsLost"]._PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value.ToString().Substring(part["app.user_data.EmParamPartsLost.cPartsLost"]._PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value.ToString().IndexOf("]") + 1)) + (category.Contains("Rotten") ? " (Rotten)" : "");
								dropsFormatted = AddItemToDrops(newItem, header, "Low", monsterName, dropsFormatted);
							}
						}
						else
						{
							dropsFormatted = AddItemToDrops(newItem, header, "Low", monsterName, dropsFormatted);
						}
					}
					if (table.AppUserDataEnemyRewardDataCData.IdEx.Any(x => x.AppItemDefIdSerializable.Value != "[0]INVALID" && x.AppItemDefIdSerializable.Value != "[0]INVARID"))
					{
						for (int i = 0; i < table.AppUserDataEnemyRewardDataCData.IdEx.Length; i++)
						{
							if (table.AppUserDataEnemyRewardDataCData.IdEx[i].AppItemDefIdSerializable.Value != "[0]INVALID" && table.AppUserDataEnemyRewardDataCData.IdEx[i].AppItemDefIdSerializable.Value != "[0]INVARID")
							{
								Data.MHWilds.Items item = MhwildsItems.First(x => x.ItemID == table.AppUserDataEnemyRewardDataCData.IdEx[i].AppItemDefIdSerializable.Value);
								int quantity = (int)table.AppUserDataEnemyRewardDataCData.RewardNumEx[i]!;
								int probability = (int)table.AppUserDataEnemyRewardDataCData.ProbabilityEx[i]!;
								Item newItem = new()
								{
									Category = category,
									Chance = probability.ToString(),
									Quantity = quantity,
									Description = item.Description,
									Icon = item.Icon,
									IconColor = item.IconColor,
									Include = false,
									ItemId = table.AppUserDataEnemyRewardDataCData.IdEx[i].AppItemDefIdSerializable.Value,
									ItemName = item.Name,
									Price = item.BuyPrice.ToString()!,
									Rarity = item.Rarity,
									Order = order
								};
								if (category == "Hunt Rewards")
								{
									switch (i)
									{
										case 0:
											newItem.Category = "Hunt Rewards";
											dropsFormatted = AddItemToDrops(newItem, header, "High", monsterName, dropsFormatted);
											break;
										case 1:
											newItem.Order = 1;
											newItem.Category = "Investigations (Basic)";
											dropsFormatted = AddItemToDrops(newItem, "Investigation", "High", monsterName, dropsFormatted);
											break;
										case 2:
											newItem.Order = 2;
											newItem.Category = "Investigation (Valuable)";
											dropsFormatted = AddItemToDrops(newItem, "Investigation", "High", monsterName, dropsFormatted);
											break;
										case 3:
											newItem.Order = 3;
											newItem.Category = "Investigation (Rare)";
											dropsFormatted = AddItemToDrops(newItem, "Investigation", "High", monsterName, dropsFormatted);
											break;
									}
								}
								else if (category.Contains("Broken Part Rewards"))
								{
									string name = "";
									if (monsterName == "Guardian Arkveld")
									{
										switch (table.AppUserDataEnemyRewardDataCData.PartsIndex)
										{
											case 0:
												name = "Head";
												break;
											case 1:
												name = "Left Chainblade";
												break;
											case 2:
												name = "Right Chainblade";
												break;
										}
									}
									else
									{
										name = partsBreakArray.First(x => x["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].RewardTableIndex == table.AppUserDataEnemyRewardDataCData.PartsIndex)["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value;
										name = GetWildsPartName(name.Substring(name.IndexOf(']') + 1));
									}
									int breakAmt = 0;
									if (monsterName == "Guardian Arkveld")
									{
										breakAmt = 1;
									}
									else
									{
										breakAmt = (int)((JArray)partsBreakArray.First(x => x["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].RewardTableIndex == table.AppUserDataEnemyRewardDataCData.PartsIndex)["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].PartsBreakData).ToObject<dynamic[]>()![0]["app.user_data.EmParamPartsBreakReward.cPartsBreakData"]._PartsBreakLevel;
									}
									newItem.Category = "Break " + name + (breakAmt == 1 ? "" : " (" + breakAmt + "x)");
									dropsFormatted = AddItemToDrops(newItem, header, "High", monsterName, dropsFormatted);
								}
								else if (category.Contains("Severed Part"))
								{
									foreach (dynamic part in partsLostArray)
									{
										newItem.Category = "Carve " + GetWildsPartName(part["app.user_data.EmParamPartsLost.cPartsLost"]._PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value.ToString().Substring(part["app.user_data.EmParamPartsLost.cPartsLost"]._PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value.ToString().IndexOf("]") + 1)) + (category.Contains("Rotten") ? " (Rotten)" : "");
										dropsFormatted = AddItemToDrops(newItem, header, "High", monsterName, dropsFormatted);
									}
								}
								else
								{
									dropsFormatted = AddItemToDrops(newItem, header, "High", monsterName, dropsFormatted);
								}
							}
						}
					}
				}
				foreach (WebToolkitData data in dropsFormatted)
				{
					string[] orderedHeaders = ["Gathered", "Quest Rewards", "Investigation"];
					data.Tables = [..data.Tables.OrderBy(x => Array.IndexOf(orderedHeaders, x.Header))];
					foreach (Table table in data.Tables)
					{
						table.Items = [..table.Items.OrderBy(x => x.Order).ThenBy(x => x.Category).ThenByDescending(x => Convert.ToInt32(x.Chance)).ThenBy(x => x.ItemName)];
					}
				}
				return Generators.MaterialsAndDropTables.GenerateDataUnescape([.. dropsFormatted], "MHWilds");
			}
		}

		private static List<WebToolkitData> AddItemToDrops(Item newItem, string header, string rank, string monsterName, List<WebToolkitData> dropsFormatted)
		{
			if (dropsFormatted.Any(x => x.Monster == monsterName && x.Rank == rank))
			{
				if (dropsFormatted.First(x => x.Monster == monsterName && x.Rank == rank).Tables.Any(x => x.Header == header))
				{
					dropsFormatted.First(x => x.Monster == monsterName && x.Rank == rank).Tables.First(x => x.Header == header).Items = [.. dropsFormatted.First(x => x.Monster == monsterName && x.Rank == rank).Tables.First(x => x.Header == header).Items.Append(newItem)];
				}
				else
				{
					dropsFormatted.First(x => x.Monster == monsterName && x.Rank == rank).Tables = [..dropsFormatted.First(x => x.Monster == monsterName && x.Rank == rank).Tables.Append(new Table()
					{
						Header = header,
						Items = [newItem]
					})];
				}
			}
			else
			{
				dropsFormatted.Add(new WebToolkitData()
				{
					Monster = monsterName,
					Rank = rank,
					Tables = [new Table() {
						Header = header,
						Items = [
							newItem
						]
					}]
				});
			}
			return dropsFormatted;
		}

		public static List<WebToolkitData> Fetch(string monsterName)
		{
			MonsterId monsterId = Monster.WildsMonsterIds.First(x => x.Name == monsterName);
			string monsterFolder = monsterId.Id.Substring(0, monsterId.Id.IndexOf("_"));
			string subspeciesFolder = monsterId.Id.Substring(monsterId.Id.IndexOf("_") + 1, 2);
			Dictionary<string, string> monsterParts = [];
			foreach (KeyValuePair<string, dynamic> part in ((JObject)((JObject)WildsMonsterMasterlist[monsterId.Id]["part_datas"]).ToObject<Dictionary<string, object>>()!["parts"]).ToObject<Dictionary<string, dynamic>>()!.Where(x => x.Key != "legendary_scar_nums"))
			{
				string partName = GetWildsPartName(part.Value.part_type.ToString());
				if (string.IsNullOrEmpty(partName.Replace("'", "")))
				{
					partName = "???";
				}
				monsterParts.Add(part.Key, partName);
			}
			List<WebToolkitData> dropsFormatted = [];
			DropFileValue[] dropFile = WildsDropFile.FromJson(File.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Enemy\{monsterId.Id}.user.3.json")).First().AppUserDataEnemyRewardData.Values;
			dynamic[] partsLostArray = [];
			string partsLostFile = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Enemy\{monsterFolder}\{subspeciesFolder}\Data\{monsterFolder}_{subspeciesFolder}_Param_PartsLost.user.3.json";
			if (File.Exists(partsLostFile))
			{
				partsLostArray = ((JArray)JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(partsLostFile))![0]["app.user_data.EmParamPartsLost"]._PartsLostArray["ace.cInstanceGuidArray`1<app.user_data.EmParamPartsLost.cPartsLost>"]._DataArray).ToObject<dynamic[]>()!;
			}
			dynamic[] partsBreakArray = [];
			string partsBreakFile = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Enemy\{monsterFolder}\{subspeciesFolder}\Data\{monsterFolder}_{subspeciesFolder}_Param_PartsBreakReward.user.3.json";
			if (File.Exists(partsBreakFile))
			{
				partsBreakArray = ((JArray)JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(partsBreakFile))![0]["app.user_data.EmParamPartsBreakReward"]._PartsBreakArray).ToObject<dynamic[]>()!; ;
			}
			JArray commonRewardData = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Mission\_UserData\_Reward\CommonRewardData.user.3.json"))!.First().Value<JObject>("app.user_data.QuestGeneralRewardData")!.Value<JArray>("_Values")!;
			JArray exEnemies = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Environment\UserData\ExFieldPattern\ExFieldPattern_Default\ExFieldParam_EnemyData.user.3.json"))!.First().Value<JObject>("app.user_data.ExFieldParam_EnemyData")!.Value<JArray>("_ExEnemies")!;
			foreach (JObject exEnemyObj in exEnemies)
			{
				JObject exEnemy = exEnemyObj.Value<JObject>("app.user_data.ExFieldParam_EnemyData.cExEmGlobalParam")!;
				if (exEnemy.Value<string>("_EmID")!.Substring(exEnemy.Value<string>("_EmID")!.IndexOf("]") + 1) == monsterId.Id)
				{
					foreach (JObject commonReward in commonRewardData)
					{
						JObject reward = commonReward.Value<JObject>("app.user_data.QuestGeneralRewardData.cData")!;
						string itemId = reward.Value<JObject>("_itemId")!.Value<JObject>("app.ItemDef.ID_Serializable")!.Value<string>("_Value")!;
						if (reward.Value<int>("_tableId") == exEnemy.Value<int>("_GeneralRewardTblID") && itemId != "[0]INVALID" && itemId != "[1]NONE")
						{
							Data.MHWilds.Items item = MhwildsItems.First(x => x.ItemID == itemId);
							string legendaryMode = reward.Value<string>("_LegendaryID")!;
							dropsFormatted = AddItemToDrops(new()
							{
								Category = "Extra Rewards" + (legendaryMode != "[0]NONE" ? " (Tempered)" : ""),
								Chance = reward.Value<int>("_probability").ToString(),
								Quantity = reward.Value<int>("_num"),
								Description = item.Description,
								Icon = item.Icon,
								IconColor = item.IconColor,
								Include = false,
								ItemId = item.ItemID,
								ItemName = item.Name,
								Price = item.BuyPrice.ToString()!,
								Rarity = item.Rarity,
								Order = legendaryMode != "[0]NONE" ? 5 : 4
							}, "Quest Rewards", "High", monsterName, dropsFormatted);
						}
					}
				}
			}
			string type = "[0]INVALID";
			foreach (DropFileValue table in dropFile)
			{
				string header = "";
				string category = "";
				int order = 0;
				if (!table.AppUserDataEnemyRewardDataCData.RewardType.AppEnemyRewardRewardTypeSerializable.Value.EndsWith("INVALID") && !table.AppUserDataEnemyRewardDataCData.RewardType.AppEnemyRewardRewardTypeSerializable.Value.EndsWith("INVARID"))
				{
					type = table.AppUserDataEnemyRewardDataCData.RewardType.AppEnemyRewardRewardTypeSerializable.Value;
					type = type[(type.IndexOf(']') + 1)..];
				}
				switch (type)
				{
					case "RW000":
						header = "Gathered";
						category = "Carves";
						break;
					case "RW008":
						header = "Gathered";
						category = "Carves (Rotten)";
						order = 1;
						break;
					case "RW001":
						header = "Gathered";
						category = "Carves (Severed Part)";
						order = 2;
						break;
					case "RW012":
						header = "Gathered";
						category = "Carves (Rotten Severed Part)";
						order = 3;
						break;
					case "RW005":
						header = "Quest Rewards";
						category = "Broken Part Rewards";
						order = 1;
						break;
					case "RW006":
						header = "Quest Rewards";
						category = "Wound Destruction Rewards";
						order = 2;
						break;
					case "RW004":
						header = "Quest Rewards";
						category = "Hunt Rewards";
						break;
					case "RW015":
						header = "Quest Rewards";
						category = "Tempered Wound Destruction Rewards";
						order = 3;
						break;
					case "RW016":
						header = "Gathered";
						category = "Carves (Crystalized)";
						order = 1;
						break;
					default:
						Debugger.Break();
						break;
				}
				if (table.AppUserDataEnemyRewardDataCData.IdStory.AppItemDefIdSerializable.Value != "[0]INVALID" && table.AppUserDataEnemyRewardDataCData.IdStory.AppItemDefIdSerializable.Value != "[0]INVARID")
				{
					Data.MHWilds.Items item = MhwildsItems.First(x => x.ItemID == table.AppUserDataEnemyRewardDataCData.IdStory.AppItemDefIdSerializable.Value);
					int quantity = (int)table.AppUserDataEnemyRewardDataCData.RewardNumStory!;
					int probability = (int)table.AppUserDataEnemyRewardDataCData.ProbabilityStory!;
					Item newItem = new()
					{
						Category = category,
						Chance = probability.ToString(),
						Quantity = quantity,
						Description = item.Description,
						Icon = item.Icon,
						IconColor = item.IconColor,
						Include = false,
						ItemId = table.AppUserDataEnemyRewardDataCData.IdStory.AppItemDefIdSerializable.Value,
						ItemName = item.Name,
						Price = item.BuyPrice.ToString()!,
						Rarity = item.Rarity,
						Order = order
					};
					if (category.Contains("Broken Part Rewards"))
					{
						string name = "";
						if (monsterName == "Guardian Arkveld")
						{
							switch (table.AppUserDataEnemyRewardDataCData.PartsIndex)
							{
								case 0:
									name = "Head";
									break;
								case 1:
									name = "Left Chainblade";
									break;
								case 2:
									name = "Right Chainblade";
									break;
							}
						}
						else
						{
							name = partsBreakArray.First(x => x["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].RewardTableIndex == table.AppUserDataEnemyRewardDataCData.PartsIndex)["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value;
							name = GetWildsPartName(name.Substring(name.IndexOf(']') + 1));
						}
						int breakAmt = 0;
						if (monsterName == "Guardian Arkveld")
						{
							breakAmt = 1;
						}
						else
						{
							breakAmt = (int)((JArray)partsBreakArray.First(x => x["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].RewardTableIndex == table.AppUserDataEnemyRewardDataCData.PartsIndex)["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].PartsBreakData).ToObject<dynamic[]>()![0]["app.user_data.EmParamPartsBreakReward.cPartsBreakData"]._PartsBreakLevel;
						}
						newItem.Category = "Break " + name + (breakAmt == 1 ? "" : " (" + breakAmt + "x)");
						dropsFormatted = AddItemToDrops(newItem, header, "Low", monsterName, dropsFormatted);
					}
					else if (category.Contains("Severed Part"))
					{
						foreach (dynamic part in partsLostArray)
						{
							newItem.Category = "Carve " + GetWildsPartName(part["app.user_data.EmParamPartsLost.cPartsLost"]._PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value.ToString().Substring(part["app.user_data.EmParamPartsLost.cPartsLost"]._PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value.ToString().IndexOf("]") + 1)) + (category.Contains("Rotten") ? " (Rotten)" : "");
							dropsFormatted = AddItemToDrops(newItem, header, "Low", monsterName, dropsFormatted);
						}
					}
					else
					{
						dropsFormatted = AddItemToDrops(newItem, header, "Low", monsterName, dropsFormatted);
					}
				}
				if (table.AppUserDataEnemyRewardDataCData.IdEx.Any(x => x.AppItemDefIdSerializable.Value != "[0]INVALID" && x.AppItemDefIdSerializable.Value != "[0]INVARID"))
				{
					for (int i = 0; i < table.AppUserDataEnemyRewardDataCData.IdEx.Length; i++)
					{
						if (table.AppUserDataEnemyRewardDataCData.IdEx[i].AppItemDefIdSerializable.Value != "[0]INVALID" && table.AppUserDataEnemyRewardDataCData.IdEx[i].AppItemDefIdSerializable.Value != "[0]INVARID")
						{
							Data.MHWilds.Items item = MhwildsItems.First(x => x.ItemID == table.AppUserDataEnemyRewardDataCData.IdEx[i].AppItemDefIdSerializable.Value);
							int quantity = (int)table.AppUserDataEnemyRewardDataCData.RewardNumEx[i]!;
							int probability = (int)table.AppUserDataEnemyRewardDataCData.ProbabilityEx[i]!;
							Item newItem = new()
							{
								Category = category,
								Chance = probability.ToString(),
								Quantity = quantity,
								Description = item.Description,
								Icon = item.Icon,
								IconColor = item.IconColor,
								Include = false,
								ItemId = table.AppUserDataEnemyRewardDataCData.IdEx[i].AppItemDefIdSerializable.Value,
								ItemName = item.Name,
								Price = item.BuyPrice.ToString()!,
								Rarity = item.Rarity,
								Order = order
							};
							if (category == "Hunt Rewards")
							{
								switch (i)
								{
									case 0:
										newItem.Category = "Hunt Rewards";
										dropsFormatted = AddItemToDrops(newItem, header, "High", monsterName, dropsFormatted);
										break;
									case 1:
										newItem.Order = 1;
										newItem.Category = "Investigations (Basic)";
										dropsFormatted = AddItemToDrops(newItem, "Investigation", "High", monsterName, dropsFormatted);
										break;
									case 2:
										newItem.Order = 2;
										newItem.Category = "Investigation (Valuable)";
										dropsFormatted = AddItemToDrops(newItem, "Investigation", "High", monsterName, dropsFormatted);
										break;
									case 3:
										newItem.Order = 3;
										newItem.Category = "Investigation (Rare)";
										dropsFormatted = AddItemToDrops(newItem, "Investigation", "High", monsterName, dropsFormatted);
										break;
								}
							}
							else if (category.Contains("Broken Part Rewards"))
							{
								string name = "";
								if (monsterName == "Guardian Arkveld")
								{
									switch (table.AppUserDataEnemyRewardDataCData.PartsIndex)
									{
										case 0:
											name = "Head";
											break;
										case 1:
											name = "Left Chainblade";
											break;
										case 2:
											name = "Right Chainblade";
											break;
									}
								}
								else
								{
									name = partsBreakArray.First(x => x["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].RewardTableIndex == table.AppUserDataEnemyRewardDataCData.PartsIndex)["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value;
									name = GetWildsPartName(name.Substring(name.IndexOf(']') + 1));
								}
								int breakAmt = 0;
								if (monsterName == "Guardian Arkveld")
								{
									breakAmt = 1;
								}
								else
								{
									breakAmt = (int)((JArray)partsBreakArray.First(x => x["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].RewardTableIndex == table.AppUserDataEnemyRewardDataCData.PartsIndex)["app.user_data.EmParamPartsBreakReward.cPartsBreakRewardSettingData"].PartsBreakData).ToObject<dynamic[]>()![0]["app.user_data.EmParamPartsBreakReward.cPartsBreakData"]._PartsBreakLevel;
								}
								newItem.Category = "Break " + name + (breakAmt == 1 ? "" : " (" + breakAmt + "x)");
								dropsFormatted = AddItemToDrops(newItem, header, "High", monsterName, dropsFormatted);
							}
							else if (category.Contains("Severed Part"))
							{
								foreach (dynamic part in partsLostArray)
								{
									newItem.Category = "Carve " + GetWildsPartName(part["app.user_data.EmParamPartsLost.cPartsLost"]._PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value.ToString().Substring(part["app.user_data.EmParamPartsLost.cPartsLost"]._PartsType["app.EnemyDef.PARTS_TYPE_Serializable"]._Value.ToString().IndexOf("]") + 1)) + (category.Contains("Rotten") ? " (Rotten)" : "");
									dropsFormatted = AddItemToDrops(newItem, header, "High", monsterName, dropsFormatted);
								}
							}
							else
							{
								dropsFormatted = AddItemToDrops(newItem, header, "High", monsterName, dropsFormatted);
							}
						}
					}
				}
			}
			foreach (WebToolkitData data in dropsFormatted)
			{
				string[] orderedHeaders = ["Gathered", "Quest Rewards", "Investigation"];
				data.Tables = [.. data.Tables.OrderBy(x => Array.IndexOf(orderedHeaders, x.Header))];
				foreach (Table table in data.Tables)
				{
					table.Items = [.. table.Items.OrderBy(x => x.Order).ThenBy(x => x.Category).ThenByDescending(x => Convert.ToInt32(x.Chance)).ThenBy(x => x.ItemName)];
				}
			}
			return dropsFormatted;
		}

		private static WebToolkitData AddItemToRank(int itemId, int count, int weight, Items[] allItems, WebToolkitData rankData, string header, string itemHeader)
		{
			Items baseItem = allItems.First(x => x.Id == itemId);
			Item item = new()
			{
				Category = itemHeader,
				Chance = weight.ToString(),
				Description = baseItem.Description,
				Icon = baseItem.WikiIconName,
				IconColor = GetIconColorName(baseItem.WikiIconColor!.Value),
				Include = true,
				ItemId = baseItem.Id!.Value!.ToString(),
				ItemName = baseItem.Name,
				Price = baseItem.BuyPrice!.Value!.ToString(),
				Quantity = count,
				Rarity = baseItem.Rarity
			};
			if (rankData.Tables.Any(x => x.Header == header))
			{
				rankData.Tables.First(x => x.Header == header).Items = [.. rankData.Tables.First(x => x.Header == header).Items.Append(item)];
			}
			else
			{
				rankData.Tables = [..rankData.Tables.Append(new Table()
				{
					Header = header,
					Items = [item]
				})];
			}
			return rankData;
		}

		private static string GetIconColorName(WikiIconColor color)
		{
			switch (color)
			{
				case WikiIconColor.NA:
					return "N/A";
				case WikiIconColor.Blue:
					return "Blue";
				case WikiIconColor.Brown:
					return "Brown";
				case WikiIconColor.DarkPurple:
					return "Dark Purple";
				case WikiIconColor.Emerald:
					return "Emerald";
				case WikiIconColor.Gray:
					return "Gray";
				case WikiIconColor.Green:
					return "Green";
				case WikiIconColor.Lemon:
					return "Lemon";
				case WikiIconColor.LightBlue:
					return "Light Blue";
				case WikiIconColor.Moss:
					return "Moss";
				case WikiIconColor.NotAvailable:
					return "NOT AVAILABLE";
				case WikiIconColor.Orange:
					return "Orange";
				case WikiIconColor.Pink:
					return "Pink";
				case WikiIconColor.Purple:
					return "Purple";
				case WikiIconColor.Red:
					return "Red";
				case WikiIconColor.Rose:
					return "Rose";
				case WikiIconColor.Tan:
					return "Tan";
				case WikiIconColor.Violet:
					return "Violet";
				case WikiIconColor.White:
					return "White";
				case WikiIconColor.Yellow:
					return "Yellow";
				case WikiIconColor.Vermilion:
					return "Vermilion";
				case WikiIconColor.LightGreen:
					return "Light Green";
				default: return "N/A";
			}
		}

		private static string GetCategoryName(TypeEnum cat)
		{
			return new Dictionary<TypeEnum, string>()
			{
				{ TypeEnum.AccountItem, "Account Item" },
				{ TypeEnum.AmmoOrCoating, "Ammo/Coating" },
				{ TypeEnum.Item, "Item" },
				{ TypeEnum.Jewel, "Jewel" },
				{ TypeEnum.Material, "Material" },
				{ TypeEnum.RoomDecoration, "Room Decoration" }
			}[cat];
		}

		private static string GetDropName(int index)
		{
			return new Dictionary<int, string>()
			{
				{ 0, "Carves" },
				{ 1, "Tail Carved" },
				{ 2, "Shiny Drops" },
				{ 3, "Monster Tracks" },
				{ 4, "Carves" },
				{ 5, "Tail Carved" },
				{ 6, "Shiny Drops" },
				{ 7, "Monster Tracks" },
				{ 8, "Bandit Mantle" },
				{ 9, "Bandit Mantle" },
				{ 10, "Plunderblade" },
				{ 11, "Plunderblade" },
				{ 12, "Palico" },
				{ 13, "Palico" },
				{ 14, "Bones" },
				{ 15, "Bones" },
				{ 16, "Bones" },
				{ 17, "Bones" },
				{ 18, "Bones" },
				{ 19, "Bones" },
				{ 20, "Monster Tracks" },
				{ 21, "Monster Tracks" },
				{ 22, "Monster Tracks" },
				{ 23, "Carves" },
				{ 24, "Tail Carved" },
				{ 25, "Shiny Drops" },
				{ 26, "Bones" },
				{ 27, "Bandit Mantle" },
				{ 28, "Plunderblade" },
				{ 29, "Palico" },
				{ 30, "Bones 2" },
				{ 31, "Guiding Lands (Low)" },
				{ 32, "Guiding Lands (Mid)" },
				{ 33, "Guiding Lands (High)" },
				{ 34, "Guiding Lands (Tempered)" }
			}[index];
		}

		private static int[] GetRankIndices(BremRank rank)
		{
			return new Dictionary<int, int[]>()
			{
				{ 0, [0, 1, 2, 3, 8, 10, 12 ]},
				{ 1, [4, 5, 6, 7, 9, 11, 13 ]},
				{ 2, [22, 23, 24, 25, 27, 28, 29 ]}
			}[(int)rank];
		}
		private static string GetWildsPartName(string partType)
		{
			int partId = new Dictionary<string, int>()
			{
				{ "INVALID", 486590176 },
{ "FULL_BODY", 1733044864 },
{ "HEAD", -212024896 },
{ "UPPER_BODY", -1382295680 },
{ "BODY", -2054210560 },
{ "TAIL", 2000370944 },
{ "TAIL_TIP", 1886418560 },
{ "NECK", -1466497792 },
{ "TORSO", 1210068992 },
{ "STOMACH", 1603494400 },
{ "BACK", 18080514 },
{ "FRONT_LEGS", 1777993216 },
{ "LEFT_FRONT_LEG", -891913216 },
{ "RIGHT_FRONT_LEG", 1920497920 },
{ "HIND_LEGS", 1429619328 },
{ "LEFT_HIND_LEG", 304214656 },
{ "RIGHT_HIND_LEG", 591465408 },
{ "LEFT_LEG", 731472640 },
{ "RIGHT_LEG", -142058256 },
{ "LEFT_LEG_FRONT_AND_REAR", 102373496 },
{ "RIGHT_LEG_FRONT_AND_REAR", -5591398 },
{ "LEFT_WING", -240678336 },
{ "RIGHT_WING", 665420480 },
{ "ASS", -941150464 },
{ "NAIL", -226704768 },
{ "LEFT_NAIL", 1750977664 },
{ "RIGHT_NAIL", 63041352 },
{ "TONGUE", -526417856 },
{ "PETAL", 1000875456 },
{ "VEIL", -279541920 },
{ "SAW", 655333504 },
{ "FEATHER", -1137775744 },
{ "TENTACLE", 499612832 },
{ "UMBRELLA", -1564619520 },
{ "LEFT_FRONT_ARM", 1177888256 },
{ "RIGHT_FRONT_ARM", -1885998720 },
{ "LEFT_SIDE_ARM", -1584832512 },
{ "RIGHT_SIDE_ARM", 1154422144 },
{ "LEFT_HIND_ARM", -1605643392 },
{ "RIGHT_HIND_ARM", 1925104512 },
{ "Head", 517550944 },
{ "CHEST", -1314889600 },
{ "MANTLE", 509608864 },
{ "MANTLE_UNDER", 789930048 },
{ "POISONOUS_THORN", -1222144512 },
{ "ANTENNAE", -945112512 },
{ "LEFT_WING_LEGS", -1235127936 },
{ "RIGHT_WING_LEGS", 702074176 },
{ "WATERFILM_RIGHT_HEAD", -101670456 },
{ "WATERFILM_LEFT_HEAD", 1730846080 },
{ "WATERFILM_RIGHT_BODY", 1917146240 },
{ "WATERFILM_LEFT_BODY", -727805760 },
{ "WATERFILM_RIGHT_FRONT_LEG", -15677196 },
{ "WATERFILM_LEFT_FRONT_LEG", -445884256 },
{ "WATERFILM_TAIL", -1410796160 },
{ "WATERFILM_LEFT_TAIL", 1725614208 },
{ "MOUTH", -1110329472 },
{ "TRUNK", 1481421312 },
{ "LEFT_WING_BLADE", 767347712 },
{ "RIGHT_WING_BLADE", -1392586368 },
{ "FROZEN_CORE_HEAD", 1395139584 },
{ "FROZEN_CORE_TAIL", -912870400 },
{ "FROZEN_CORE_WAIST", 876321664 },
{ "FROZEN_BIGCORE_BEFORE", 1063213696 },
{ "FROZEN_BIGCORE_AFTER", -1328528384 },
{ "NOSE", -643264000 },
{ "HEAD_WEAR", 6538 },
{ "HEAD_HIDE", 30311 },
{ "WING_ARM", 10580 },
{ "WING_ARM_WEAR", 23560 },
{ "LEFT_WING_ARM_WEAR", 2383 },
{ "RIGHT_WING_ARM_WEAR", 2323 },
{ "LEFT_WING_ARM", 22650 },
{ "RIGHT_WING_ARM", 30763 },
{ "LEFT_WING_ARM_HIDE", 10831 },
{ "RIGHT_WING_ARM_HIDE", 21608 },
{ "CHELICERAE", 15433 },
{ "BOTH_WINGS", 30838 },
{ "BOTH_WINGS_BLADE", 24658 },
{ "BOTH_LEG", 15859 },
{ "ARM", 12265 },
{ "LEG", 23097 },
{ "HIDE", 28141 },
{ "SHARP_CORNERS", 10456 },
{ "NEEDLE_HAIR", 23256 },
{ "PARALYSIS_CORNERS", 31285 },
{ "HEAD_OIL", 8217 },
{ "UMBRELLA_OIL", 1199 },
{ "TORSO_OIL", 19946 },
{ "ARM_OIL", 10275 },
{ "WATERFILM_RIGHT_TAIL", 31953 },
{ "TAIL_HAIR", 2015 },
{ "STOMACH_SECOND", 10869 },
{ "HEAD_SECOND", 20534 },
{ "POISONOUS_THORN_SECOND", 5823 },
{ "TAIL_THIRD", 11977 },
{ "TAIL_FIFTH", 9871 },
{ "DORSAL_FIN", 1809 },
{ "HEAD_FIRST", 26403 },
{ "CORNER", 11138 },
{ "FANG", 25689 },
{ "FANG_FIRST", 6609 },
{ "FANG_SECOND", 29797 },
{ "LEFT_FRONT_LEGARMOR", 27651 },
{ "RIGHT_FRONT_LEGARMOR", 8246 },
{ "HEAD_ARMOR", 17094 },
{ "LEFT_WING_ARM_ARMOR", 24769 },
{ "RIGHT_WING_ARM_ARMOR", 15310 }
			}[partType];
			dynamic[] entries = Extensions.ToDynamic((JArray)WildsPartNames["entries"]);
			foreach (dynamic entry in entries)
			{
				if (entry.name.ToString() == $"EnemyPartsTypeName_{partId.ToString().Replace("-", "m")}")
				{
					return entry.content[1].ToString();
				}
			}
			return "???";
		}

	}

	static class Extensions
	{
		public static dynamic[] ToDynamic(JArray src)
		{
			return src.ToObject<dynamic[]>()!;
		}

		public static void TryAddToChildDict(this Dictionary<string, Dictionary<string, object>> src, string parentKey, string childKey, object childValue)
		{
			if (src.ContainsKey(parentKey))
			{
				if (src[parentKey].ContainsKey(childKey))
				{
					src[parentKey][childKey] = childValue;
				}
				else
				{
					src[parentKey].Add(childKey, childValue);
				}
			}
		}
	}

	class BremInfo
	{
		public BremRank Rank { get; set; }
		public BremType Type { get; set; }
		public string MonsterName { get; set; } = string.Empty;
		public int? PartBreakIndex { get; set; }
		public int[] ItemIds { get; set; } = new int[8];
		public int[] Counts { get; set; } = new int[8];
		public int[] Weights { get; set; } = new int[8];
	}

	enum BremType
	{
		Capture,
		PartBreak,
		HuntNonTarget
	}

	enum BremRank
	{
		Low,
		High,
		Master
	}
}
