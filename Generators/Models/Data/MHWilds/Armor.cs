﻿// <auto-generated />
using System.Globalization;
using MediawikiTranslator.Models.Data.MHWilds;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
namespace MediawikiTranslator.Models.Data.MHWilds
{

	public partial class Armor
	{
		[JsonProperty("app.user_data.ArmorData", NullValueHandling = NullValueHandling.Ignore)]
		public AppUserDataArmorData AppUserDataArmorData { get; set; }

		public static Armor[] GetArmors()
		{
			JArray seriesMsgs = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\ArmorSeries.msg.23.json")).Value<JArray>("entries");
			JArray armorMsgs = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\Armor.msg.23.json")).Value<JArray>("entries");
			Armor[] src = FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Equip\ArmorData.user.3.json"));
			ArmorSeries srcSeries = ArmorSeries.FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Equip\ArmorSeriesData.user.3.json"))[0];
			foreach (AppUserDataArmorDataCData val in src[0].AppUserDataArmorData.Values.Select(x => x.AppUserDataArmorDataCData))
			{
				if (val.Series.AppArmorDefSeriesSerializable.Value != "[1]ID_000")
				{
					val.ArmorSeries = srcSeries.AppUserDataArmorSeriesData.Values.FirstOrDefault(x => !string.IsNullOrEmpty(val.Series.AppArmorDefSeriesSerializable.Value) && x.AppUserDataArmorSeriesDataCData.Series.AppArmorDefSeriesSerializable.Value == val.Series.AppArmorDefSeriesSerializable.Value).AppUserDataArmorSeriesDataCData;
				}
				if (val.ArmorSeries == null)
				{
					//"NONE" series
					val.ArmorSeries = srcSeries.AppUserDataArmorSeriesData.Values.First().AppUserDataArmorSeriesDataCData;
				}
				val.Name = armorMsgs.First(x => x.Value<string>("guid") == val.NameId!.ToString()).Value<JArray>("content")[1].ToString();
				val.Explain = armorMsgs.First(x => x.Value<string>("guid") == val.ExplainId!.ToString()).Value<JArray>("content")[1].ToString();
				val.ArmorSeries.Name = seriesMsgs.First(x => x.Value<string>("guid") == val.ArmorSeries.NameId!.ToString()).Value<JArray>("content")[1].ToString();
			}
			return src;
		}
	}

	public partial class AppUserDataArmorData
	{
		[JsonProperty("_Values", NullValueHandling = NullValueHandling.Ignore)]
		public Value[] Values { get; set; }
	}

	public partial class Value
	{
		[JsonProperty("app.user_data.ArmorData.cData", NullValueHandling = NullValueHandling.Ignore)]
		public AppUserDataArmorDataCData AppUserDataArmorDataCData { get; set; }
	}

	public partial class AppUserDataArmorDataCData
	{
		[JsonProperty("_Index", NullValueHandling = NullValueHandling.Ignore)]
		public long? Index { get; set; }

		[JsonProperty("_DataValue", NullValueHandling = NullValueHandling.Ignore)]
		public long? DataValue { get; set; }

		[JsonProperty("_Series", NullValueHandling = NullValueHandling.Ignore)]
		public Series Series { get; set; }

		[JsonProperty("_PartsType", NullValueHandling = NullValueHandling.Ignore)]
		public PartsType PartsType { get; set; }

		[JsonProperty("_Name", NullValueHandling = NullValueHandling.Ignore)]
		public Guid? NameId { get; set; }

		[JsonProperty("_Explain", NullValueHandling = NullValueHandling.Ignore)]
		public Guid? ExplainId { get; set; }

		[JsonProperty("_Defense", NullValueHandling = NullValueHandling.Ignore)]
		public long? Defense { get; set; }

		[JsonProperty("_Resistance", NullValueHandling = NullValueHandling.Ignore)]
		public long[] Resistance { get; set; }

		[JsonProperty("_SlotLevel", NullValueHandling = NullValueHandling.Ignore)]
		public SlotLevel[] SlotLevel { get; set; }

		[JsonProperty("_Skill", NullValueHandling = NullValueHandling.Ignore)]
		public Skill[] Skill { get; set; }

		[JsonProperty("_SkillLevel", NullValueHandling = NullValueHandling.Ignore)]
		public long[] SkillLevel { get; set; }

		[JsonIgnore]
		public string Name { get; set; }
		[JsonIgnore]
		public string Explain { get; set; }

		[JsonIgnore]
		public AppUserDataArmorSeriesDataCData ArmorSeries { get; set; }
	}

	public partial class PartsType
	{
		[JsonProperty("app.ArmorDef.ARMOR_PARTS_Serializable", NullValueHandling = NullValueHandling.Ignore)]
		public SeriesAppSerializable AppArmorDefArmorPartsSerializable { get; set; }
	}

	public partial class SeriesAppSerializable
	{
		[JsonProperty("_Value", NullValueHandling = NullValueHandling.Ignore)]
		public string Value { get; set; }
	}

	public partial class Series
	{
		[JsonProperty("app.ArmorDef.SERIES_Serializable", NullValueHandling = NullValueHandling.Ignore)]
		public SeriesAppSerializable AppArmorDefSeriesSerializable { get; set; }
	}

	public partial class Skill
	{
		[JsonProperty("app.HunterDef.Skill_Serializable", NullValueHandling = NullValueHandling.Ignore)]
		public SeriesAppSerializable AppHunterDefSkillSerializable { get; set; }
	}

	public partial class SlotLevel
	{
		[JsonProperty("app.EquipDef.SlotLevel_Serializable", NullValueHandling = NullValueHandling.Ignore)]
		public SeriesAppSerializable AppEquipDefSlotLevelSerializable { get; set; }
	}

	public partial class Armor
	{
		public static Armor[] FromJson(string json) => JsonConvert.DeserializeObject<Armor[]>(json, MediawikiTranslator.Models.Data.MHWilds.ArmorConverter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this Armor[] self) => JsonConvert.SerializeObject(self, MediawikiTranslator.Models.Data.MHWilds.ArmorConverter.Settings);
	}

	internal static class ArmorConverter
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