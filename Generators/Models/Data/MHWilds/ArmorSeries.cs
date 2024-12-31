﻿// <auto-generated />
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MediawikiTranslator.Models.Data.MHWilds
{

	public partial class ArmorSeries
	{
		[JsonProperty("app.user_data.ArmorSeriesData", NullValueHandling = NullValueHandling.Ignore)]
		public AppUserDataArmorSeriesData AppUserDataArmorSeriesData { get; set; }
	}

	public partial class AppUserDataArmorSeriesData
	{
		[JsonProperty("_Values", NullValueHandling = NullValueHandling.Ignore)]
		public ArmorSeriesValue[] Values { get; set; }
	}

	public partial class ArmorSeriesValue
	{
		[JsonProperty("_Index", NullValueHandling = NullValueHandling.Ignore)]
		public long? Index { get; set; }

		[JsonProperty("_Series", NullValueHandling = NullValueHandling.Ignore)]
		public string Series { get; set; }

		[JsonProperty("_Name", NullValueHandling = NullValueHandling.Ignore)]
		public Guid? NameId { get; set; }

		[JsonProperty("_ModelVariety", NullValueHandling = NullValueHandling.Ignore)]
		public string ModelVariety { get; set; }

		[JsonProperty("_ModId", NullValueHandling = NullValueHandling.Ignore)]
		public long? ModId { get; set; }

		[JsonProperty("_ModSubMaleId", NullValueHandling = NullValueHandling.Ignore)]
		public long? ModSubMaleId { get; set; }

		[JsonProperty("_ModSubFemaleId", NullValueHandling = NullValueHandling.Ignore)]
		public long? ModSubFemaleId { get; set; }

		[JsonProperty("_SortId", NullValueHandling = NullValueHandling.Ignore)]
		public long? SortId { get; set; }

		[JsonProperty("_Rare", NullValueHandling = NullValueHandling.Ignore)]
		public string Rare { get; set; }

		[JsonProperty("_Price", NullValueHandling = NullValueHandling.Ignore)]
		public long? Price { get; set; }

		[JsonProperty("_Color", NullValueHandling = NullValueHandling.Ignore)]
		public string Color { get; set; }
		[JsonIgnore]
		public string Name { get; set; }
	}

	public partial class ArmorSeries
	{
		public static ArmorSeries FromJson(string json) => JsonConvert.DeserializeObject<ArmorSeries>(json, MediawikiTranslator.Models.Data.MHWilds.ArmorSeriesConverter.Settings);
	}

	public static class ArmorSeriesSerialize
	{
		public static string ToJson(this ArmorSeries self) => JsonConvert.SerializeObject(self, MediawikiTranslator.Models.Data.MHWilds.ArmorSeriesConverter.Settings);
	}

	internal static class ArmorSeriesConverter
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