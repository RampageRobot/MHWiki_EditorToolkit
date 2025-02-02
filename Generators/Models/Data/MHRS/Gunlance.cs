﻿// <auto-generated />
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MediawikiTranslator.Models.Data.MHRS
{

	public partial class Gunlance
	{
		[JsonProperty("snow.equip.GunLanceBaseUserData", NullValueHandling = NullValueHandling.Ignore)]
		public SnowEquipGunLanceBaseUserData SnowEquipGunLanceBaseUserData { get; set; }

		public static Weapon[] Fetch()
		{
			return FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\natives\stm\data\define\player\weapon\gunlance\gunlancebasedata.user.2.json")).SnowEquipGunLanceBaseUserData.Param;
		}
	}

	public partial class SnowEquipGunLanceBaseUserData
	{
		[JsonProperty("_Param", NullValueHandling = NullValueHandling.Ignore)]
		public GunlanceParam[] Param { get; set; }
	}

	public partial class GunlanceParam : Weapon
	{

		[JsonProperty("_GunLanceFireType", NullValueHandling = NullValueHandling.Ignore)]
		public string GunLanceFireType { get; set; }

		[JsonProperty("_GunLanceFireLv", NullValueHandling = NullValueHandling.Ignore)]
		public string GunLanceFireLv { get; set; }
	}

	public partial class Gunlance
	{
		public static Gunlance FromJson(string json) => JsonConvert.DeserializeObject<Gunlance>(json, MediawikiTranslator.Models.Data.MHRS.GunlanceConverter.Settings);
	}

	internal static class GunlanceConverter
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
