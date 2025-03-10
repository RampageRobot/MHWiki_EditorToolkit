﻿// <auto-generated />
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MediawikiTranslator.Models.Data.MHRS
{

	public partial class QuestsRewardLotTables
	{
		[JsonProperty("snow.data.RewardIdLotTableUserData", NullValueHandling = NullValueHandling.Ignore)]
		public SnowDataRewardIdLotTableUserData SnowDataRewardIdLotTableUserData { get; set; }
#nullable enable
		private static Items[]? _MHRSItems { get; set; }
		private static QuestsRewardLotTablesParam[]? _LootTables { get; set; }
#nullable disable

		public static QuestsRewardLotTablesParam[] GetAllLootTables()
		{
			if (_LootTables != null)
			{
				return _LootTables;
			}
			if (_MHRSItems == null)
			{
				_MHRSItems = Utilities.GetMHRSItems();
			}
			QuestsRewardLotTablesParam[] lootTablesLrhr = FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\natives\stm\data\define\quest\system\questrewardsystem\rewardidlottabledata.user.2.json")).SnowDataRewardIdLotTableUserData.Param;
			QuestsRewardLotTablesParam[] lootTablesMr = FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\natives\stm\data\define\quest\system\questrewardsystem\rewardidlottabledata_mr.user.2.json")).SnowDataRewardIdLotTableUserData.Param;
			_LootTables = new QuestsRewardLotTablesParam[lootTablesLrhr.Length + lootTablesMr.Length];
			lootTablesLrhr.CopyTo(_LootTables, 0);
			lootTablesMr.CopyTo(_LootTables, lootTablesLrhr.Length);
			foreach (QuestsRewardLotTablesParam table in _LootTables)
			{
				table.Items = new Items[table.ItemIdList.Length];
				int cntr = 0;
				foreach (long itemId in table.ItemIdList)
				{
					//Items item = _MHRSItems.FirstOrDefault(x => x.Id == itemId);
					//if (item != null)
					//{
					//	table.Items[cntr] = item;
					//}
					cntr++;
				}
			}
			return _LootTables;
		}
	}

	public partial class SnowDataRewardIdLotTableUserData
	{
		[JsonProperty("_Param", NullValueHandling = NullValueHandling.Ignore)]
		public QuestsRewardLotTablesParam[] Param { get; set; }
	}

	public partial class QuestsRewardLotTablesParam
	{
		[JsonProperty("_Id", NullValueHandling = NullValueHandling.Ignore)]
		public long? Id { get; set; }

		[JsonProperty("_LotRule", NullValueHandling = NullValueHandling.Ignore)]
		public long? LotRule { get; set; }

		[JsonProperty("_ItemIdList", NullValueHandling = NullValueHandling.Ignore)]
		public long[] ItemIdList { get; set; }
		[JsonIgnore]
		public Items[] Items { get; set; }

		[JsonProperty("_NumList", NullValueHandling = NullValueHandling.Ignore)]
		public long[] NumList { get; set; }

		[JsonProperty("_ProbabilityList", NullValueHandling = NullValueHandling.Ignore)]
		public long[] ProbabilityList { get; set; }
	}

	public partial class QuestsRewardLotTables
	{
		public static QuestsRewardLotTables FromJson(string json) => JsonConvert.DeserializeObject<QuestsRewardLotTables>(json, MediawikiTranslator.Models.Data.MHRS.QuestsRewardLotTablesConverter.Settings);
	}

	internal static class QuestsRewardLotTablesConverter
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
}
