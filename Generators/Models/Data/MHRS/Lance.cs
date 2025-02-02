﻿// <auto-generated />
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MediawikiTranslator.Models.Data.MHRS
{

	public partial class Lance
	{
		[JsonProperty("snow.equip.LanceBaseUserData", NullValueHandling = NullValueHandling.Ignore)]
		public SnowEquipLanceBaseUserData SnowEquipLanceBaseUserData { get; set; }

		public static Weapon[] Fetch()
		{
			return FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\natives\stm\data\define\player\weapon\lance\lancebasedata.user.2.json")).SnowEquipLanceBaseUserData.Param;
		}
	}

	public partial class SnowEquipLanceBaseUserData
	{
		[JsonProperty("_Param", NullValueHandling = NullValueHandling.Ignore)]
		public LanceParam[] Param { get; set; }
	}

	public partial class LanceParam : Weapon
	{
	}

	public partial class Lance
	{
		public static Lance FromJson(string json) => JsonConvert.DeserializeObject<Lance>(json, MediawikiTranslator.Models.Data.MHRS.LanceConverter.Settings);
	}

	internal static class LanceConverter
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
