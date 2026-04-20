using MediawikiTranslator.Models.MaterialsAndDropTables;
using MediawikiTranslator.Models.Monsters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Data.MHRS
{

	public class MonsterDrops
	{
		public int Order { get; set; } = 0;
		[JsonProperty(nameof(Rank))]
		public string Rank { get; set; } = string.Empty;
		[JsonProperty(nameof(Monster))]
		public string Monster { get; set; } = string.Empty;

		[JsonProperty(nameof(Tables))]
		public Table[] Tables { get; set; } = [];

		public static WebToolkitData[] GetWebToolkitData()
		{
			Items[] srcItems = Items.Fetch();
			MonsterDrops[] src = Fetch();
			List<WebToolkitData> ret = [];
			int cntr = 1;
			foreach (MonsterDrops drop in src)
			{
				ret.Add(new()
				{
					Order = cntr,
					Monster = drop.Monster!,
					Rank = drop.Rank!,
					Tables = drop.Tables!
				});
			}
			return [.. ret];
		}

		public static MonsterDrops[] Fetch()
		{
			Generators.Items[] srcItems = Items.FetchParsed();
			List<MonsterDrops> drops = [];
			int order = 0;
			foreach (string file in Directory.EnumerateFiles(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\Transcribed Drop Tables"))
			{
				JObject list = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file))!;
				foreach (string monsterRaw in list.Properties().Select(x => x.Name))
				{
					string monster = string.Join(" ", monsterRaw.Split(" ").Select(x => char.ToUpper(x[0]) + x[1..]));
					monster = string.Join("-", monster.Split("-").Select(x => char.ToUpper(x[0]) + x[1..])).Replace("The", "the");
					JObject monsterObj = list.Value<JObject>(monsterRaw)!;
					foreach (string rank in monsterObj.Properties().Select(x => x.Name))
					{
						JObject rankObj = monsterObj.Value<JObject>(rank)!;
						List<Table> tables = [];
						int catOrder = 0;
						foreach (string category in rankObj.Properties().Select(x => x.Name))
						{
							List<Item> tableItems = [];
							JArray categoryObj = rankObj.Value<JArray>(category)!;
							int itemOrder = 0;
							foreach (JObject item in categoryObj)
							{
								try
								{
									Generators.Items itemToAdd = srcItems.First(x => x.Name!.Replace(" ", "").ToUpper() == item.Value<string>("item")!.Replace(" ", "").ToUpper());
									tableItems.Add(new()
									{
										Category = category,
										Chance = Math.Round((100 * Math.Round(item.Value<float>("chance"), 2))).ToString(),
										ItemId = itemToAdd.InternalID!,
										ItemName = itemToAdd.Name!,
										Description = itemToAdd.Description?.Replace("<COL YEL>(Account Item)</COL>", "<span style=\"color:yellow\">(Account Item)</span>") ?? "",
										Icon = itemToAdd.WikiIconName!,
										IconColor = itemToAdd.WikiIconColor!,
										Include = true,
										Order = itemOrder++,
										Price = itemToAdd.BuyPrice!.ToString()!,
										Quantity = item.Value<int>("quantity"),
										Rarity = itemToAdd.Rarity
									});
								}
								catch (Exception ex)
								{
									Debugger.Break();
								}
							}
							tables.Add(new()
							{
								Header = string.Join("", category.Split(" ").Select(x => char.ToUpper(x[0]) + x[1..])),
								Order = catOrder++,
								Items = [..tableItems]
							});
							if (tableItems.Sum(x => Convert.ToInt32(x.Chance)) != 100)
							{
								Debugger.Break();
							}
						}
						drops.Add(new()
						{
							Monster = monster,
							Rank = string.Join("", rank.Split(" ").Select(x => char.ToUpper(x[0]) + x[1..])),
							Order = order++,
							Tables = [..tables]
						});
					}
				}
			}
			return [..drops];
		}
	}
}
