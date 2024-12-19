using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MediawikiTranslator.Models.ArmorSets
{
    public partial class WebToolkitData
    {
        [JsonProperty("setName")]
        public string SetName { get; set; } = string.Empty;

        [JsonProperty("game")]
        public string Game { get; set; } = string.Empty;

        [JsonProperty("maleFrontImg")]
        public string MaleFrontImg { get; set; } = string.Empty;

        [JsonProperty("maleBackImg")]
        public string MaleBackImg { get; set; } = string.Empty;

        [JsonProperty("femaleFrontImg")]
        public string FemaleFrontImg { get; set; } = string.Empty;

        [JsonProperty("femaleBackImg")]
        public string FemaleBackImg { get; set; } = string.Empty;

        [JsonProperty("setSkill1Name")]
        public string SetSkill1Name { get; set; } = string.Empty;

        [JsonProperty("setSkill2Name")]
        public string SetSkill2Name { get; set; } = string.Empty;

        [JsonProperty("groupSkill1Name")]
        public string GroupSkill1Name { get; set; } = string.Empty;

        [JsonProperty("groupSkill2Name")]
        public string GroupSkill2Name { get; set; } = string.Empty;

        [JsonProperty("rarity")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Rarity { get; set; }

        [JsonProperty("pieces")]
        public Piece[] Pieces { get; set; } = [];
    }

    public partial class Piece
    {
        [JsonProperty("skills")]
        public Skill[] Skills { get; set; } = [];

        [JsonProperty("materials")]
        public Material[] Materials { get; set; } = [];

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("rarity")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Rarity { get; set; }
        [JsonProperty("forging-cost")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? ForgingCost { get; set; }

        [JsonProperty("icon-type")]
        public string IconType { get; set; } = string.Empty;

        [JsonProperty("male-image")]
        public string MaleImage { get; set; } = string.Empty;

        [JsonProperty("female-image")]
        public string FemaleImage { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("defense")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Defense { get; set; }

        [JsonProperty("max-defense")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? MaxDefense { get; set; }

        [JsonProperty("fire-res")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? FireRes { get; set; }

        [JsonProperty("water-res")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? WaterRes { get; set; }

        [JsonProperty("thunder-res")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? ThunderRes { get; set; }

        [JsonProperty("ice-res")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? IceRes { get; set; }

        [JsonProperty("dragon-res")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? DragonRes { get; set; }

        [JsonProperty("decos-1")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos1 { get; set; }

        [JsonProperty("decos-2")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos2 { get; set; }

        [JsonProperty("decos-3")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos3 { get; set; }

        [JsonProperty("decos-4")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos4 { get; set; }
    }

    public partial class Material
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonProperty("color")]
        public string Color { get; set; } = string.Empty;

        [JsonProperty("quantity")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Quantity { get; set; }
    }

    public partial class Skill
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("level")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Level { get; set; }
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
