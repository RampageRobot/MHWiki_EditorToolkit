using System.Globalization;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MediawikiTranslator.Models.MaterialsAndDropTables
{
	public partial class WebToolkitData
	{
		[JsonProperty(nameof(Rank))]
		public string Rank { get; set; } = string.Empty;
		[JsonProperty(nameof(Monster))]
		public string Monster { get; set; } = string.Empty;

		[JsonProperty(nameof(Tables))]
		public Table[] Tables { get; set; } = [];

        internal static WebToolkitData[] FromWebUI(string json)
        {
            return FromJson(json);
        }
    }

	public partial class Table
	{
		[JsonProperty(nameof(Header))]
		public string Header { get; set; } = string.Empty;

		[JsonProperty(nameof(Items))]
		public Item[] Items { get; set; } = [];
	}

	public partial class Item
    {
		[JsonProperty(nameof(Include))]
		public bool Include { get; set; }

		[JsonProperty(nameof(ItemName))]
		public string ItemName { get; set; } = string.Empty;

		[JsonProperty(nameof(Chance))]
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Chance { get; set; }

		[JsonProperty(nameof(Icon))]
		public string Icon { get; set; } = string.Empty;

		[JsonProperty(nameof(IconColor))]
		public string IconColor { get; set; } = string.Empty;

		[JsonProperty(nameof(Description))]
		public string Description { get; set; } = string.Empty;

		[JsonProperty(nameof(Rarity))]
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Rarity { get; set; }

		[JsonProperty(nameof(Price))]
		public string Price { get; set; } = string.Empty;

		[JsonProperty(nameof(Category))]
		public string Category { get; set; } = string.Empty;

		[JsonProperty(nameof(Quantity))]
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Quantity { get; set; }
    }

	public partial class WebToolkitData
	{
		public static WebToolkitData[] FromJson(string json) => JsonConvert.DeserializeObject<WebToolkitData[]>(json, Converter.Settings)!;
	}

	public static class Serialize
	{
		public static string ToJson(this WebToolkitData[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
	}

	internal static class Converter
	{
		public static readonly JsonSerializerSettings Settings = new()
		{
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			DateParseHandling = DateParseHandling.None,
			Converters =
		{
			new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
		},
		};
	}

	internal class ParseStringConverter : JsonConverter
	{
		public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

		public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null) return null;
			var value = serializer.Deserialize<string>(reader);
			if (string.IsNullOrEmpty(value)) return null;
			if (long.TryParse(value, out long l))
			{
				return l;
			}
			throw new Exception("An integer value you have provided is not a number.");
		}

		public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
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

		public static readonly ParseStringConverter Singleton = new();
	}
}
