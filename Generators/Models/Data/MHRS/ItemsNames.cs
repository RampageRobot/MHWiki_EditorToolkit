﻿// <auto-generated />
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MediawikiTranslator.Models.Data.MHRS
{

	public partial class ItemsNames
	{
		[JsonProperty("msgs", NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, ItemsNamesMsg> Msgs { get; set; }

		[JsonProperty("name_to_uuid", NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, Guid> NameToUuid { get; set; }
	}

	public partial class ItemsNamesMsg
	{
		[JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Content { get; set; }

		[JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
		public long? Hash { get; set; }

		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }
	}

	public partial class ItemsNames
	{
		public static ItemsNames FromJson(string json) => JsonConvert.DeserializeObject<ItemsNames>(json, MediawikiTranslator.Models.Data.MHRS.ItemsNamesConverter.Settings);
	}

	public static class ItemsNamesSerialize
	{
		public static string ToJson(this ItemsNames self) => JsonConvert.SerializeObject(self, MediawikiTranslator.Models.Data.MHRS.ItemsNamesConverter.Settings);
	}

	internal static class ItemsNamesConverter
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
