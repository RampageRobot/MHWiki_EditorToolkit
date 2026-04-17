using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MediawikiTranslator.Models.ArmorSets
{
	public partial class WebToolkitData
	{
		public int Order { get; set; } = 0;
		public string SetName { get; set; } = string.Empty;
		public string Game { get; set; } = string.Empty;
		public string MaleFrontImg { get; set; } = string.Empty;
		public string MaleBackImg { get; set; } = string.Empty;
		public string FemaleFrontImg { get; set; } = string.Empty;
		public string FemaleBackImg { get; set; } = string.Empty;
		public Skill? SetSkill1 { get; set; }
		public Skill? SetSkill2 { get; set; }
		public Skill? GroupSkill1 { get; set; }
		public Skill? GroupSkill2 { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Rarity { get; set; }
		public Piece[] Pieces { get; set; } = [];
		public string Rank { get; set; } = string.Empty;
		public string? OnlyForGender { get; set; }
	}
	public partial class Piece
	{
		public Skill[] Skills { get; set; } = [];
		public Material[] Materials { get; set; } = [];
		public string Name { get; set; } = string.Empty;
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Rarity { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? ForgingCost { get; set; }
		public string IconType { get; set; } = string.Empty;
		public string MaleImage { get; set; } = string.Empty;
		public string FemaleImage { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Defense { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? MaxDefense { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? FireRes { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? WaterRes { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? ThunderRes { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? IceRes { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? DragonRes { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Decos1 { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Decos2 { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Decos3 { get; set; }
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Decos4 { get; set; }
		public int MaxLevel { get; set; }
	}
	public partial class Material
	{
		public string Name { get; set; } = string.Empty;
		public string Icon { get; set; } = string.Empty;
		public string Color { get; set; } = string.Empty;
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Quantity { get; set; }
	}
	public partial class Skill
	{
		public string Name { get; set; } = string.Empty;
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Level { get; set; }
		public string WikiIconColor { get; set; } = string.Empty;
	}
	public partial class WebToolkitData
	{
		public static WebToolkitData FromJson(string json) => JsonConvert.DeserializeObject<WebToolkitData>(json, Converter.Settings)!;
	}
	public static class Serialize
	{
		public static string ToJson(this WebToolkitData self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
