using MediawikiTranslator.Models.Data.MHWI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
namespace MediawikiTranslator.Models.Weapon
{

    public partial class WebToolkitData
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty("game", NullValueHandling = NullValueHandling.Ignore)]
        public string? Game { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string? Type { get; set; }

        [JsonProperty("tree", NullValueHandling = NullValueHandling.Ignore)]
        public string? Tree { get; set; }

        [JsonProperty("attack", NullValueHandling = NullValueHandling.Ignore)]
        public string Attack { get; set; } = string.Empty;

        [JsonProperty("rarity", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Rarity { get; set; }

        [JsonProperty("forge-cost", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? ForgeCost { get; set; }

        [JsonProperty("forge-materials", NullValueHandling = NullValueHandling.Ignore)]
        public string ForgeMaterials { get; set; } = string.Empty;

        [JsonProperty("defense", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public string Defense { get; set; } = string.Empty;

        [JsonProperty("affinity", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Affinity { get; set; }

        [JsonProperty("element-1", NullValueHandling = NullValueHandling.Ignore)]
        public string Element1 { get; set; } = string.Empty;

		[JsonProperty("element-1-dmg", NullValueHandling = NullValueHandling.Ignore)]
        public string ElementDmg1 { get; set; } = string.Empty;

		[JsonProperty("element-2", NullValueHandling = NullValueHandling.Ignore)]
        public string Element2 { get; set; } = string.Empty;

		[JsonProperty("element-2-dmg", NullValueHandling = NullValueHandling.Ignore)]
        public string ElementDmg2 { get; set; } = string.Empty;

        [JsonProperty("rollback", NullValueHandling = NullValueHandling.Ignore)]
        public string? Rollback { get; set; }

        [JsonProperty("sharpness", NullValueHandling = NullValueHandling.Ignore)]
        public string? Sharpness { get; set; }

		[JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }

        [JsonProperty("decos-1", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos1 { get; set; }

        [JsonProperty("decos-2", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos2 { get; set; }

        [JsonProperty("decos-3", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos3 { get; set; }

        [JsonProperty("decos-4", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos4 { get; set; }

        [JsonProperty("elderseal", NullValueHandling = NullValueHandling.Ignore)]
        public string? Elderseal { get; set; }

        [JsonProperty("armor-skills", NullValueHandling = NullValueHandling.Ignore)]
        public string? ArmorSkills { get; set; }

        [JsonProperty("rampage-skill-slots", NullValueHandling = NullValueHandling.Ignore)]
        public string? RampageSkillSlots { get; set; }

        [JsonProperty("rampage-decoration", NullValueHandling = NullValueHandling.Ignore)]
        public string? RampageDecoration { get; set; }

        [JsonProperty("hh-note-1", NullValueHandling = NullValueHandling.Ignore)]
        public string? HhNote1 { get; set; }

        [JsonProperty("hh-note-2", NullValueHandling = NullValueHandling.Ignore)]
        public string? HhNote2 { get; set; }

        [JsonProperty("hh-note-3", NullValueHandling = NullValueHandling.Ignore)]
        public string? HhNote3 { get; set; }

        [JsonProperty("gl-shelling-type", NullValueHandling = NullValueHandling.Ignore)]
        public string? GlShellingType { get; set; }

        [JsonProperty("gl-shelling-level", NullValueHandling = NullValueHandling.Ignore)]
        public string? GlShellingLevel { get; set; }

        [JsonProperty("sa-phial-type", NullValueHandling = NullValueHandling.Ignore)]
        public string? SaPhialType { get; set; }

        [JsonProperty("cb-phial-type", NullValueHandling = NullValueHandling.Ignore)]
        public string? CbPhialType { get; set; }

        [JsonProperty("ig-kinsect-bonus", NullValueHandling = NullValueHandling.Ignore)]
        public string? IgKinsectBonus { get; set; }

        [JsonProperty("bo-coatings", NullValueHandling = NullValueHandling.Ignore)]
        public string? BoCoatings { get; set; }

        [JsonProperty("hbg-special-ammo-type", NullValueHandling = NullValueHandling.Ignore)]
        public string? HbgSpecialAmmoType { get; set; }

		[JsonProperty("hbg-deviation", NullValueHandling = NullValueHandling.Ignore)]
		public string? HbgDeviation { get; set; }

		[JsonProperty("lbg-special-ammo-type", NullValueHandling = NullValueHandling.Ignore)]
		public string? LbgSpecialAmmoType { get; set; }

		[JsonProperty("lbg-deviation", NullValueHandling = NullValueHandling.Ignore)]
		public string? LbgDeviation { get; set; }

		[JsonProperty("previous-name", NullValueHandling = NullValueHandling.Ignore)]
        public string? PreviousName { get; set; }

        [JsonProperty("previous-rarity", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? PreviousRarity { get; set; }

        [JsonProperty("upgrade-cost", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? UpgradeCost { get; set; }

        [JsonProperty("upgrade-materials", NullValueHandling = NullValueHandling.Ignore)]
        public string? UpgradeMaterials { get; set; }

        [JsonProperty("next-1-name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Next1Name { get; set; }

        [JsonProperty("next-1-rarity", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next1Rarity { get; set; }

        [JsonProperty("next-1-cost", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next1Cost { get; set; }

        [JsonProperty("next-1-materials", NullValueHandling = NullValueHandling.Ignore)]
        public string? Next1Materials { get; set; }

        [JsonProperty("next-2-name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Next2Name { get; set; }

        [JsonProperty("next-2-rarity", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next2Rarity { get; set; }

        [JsonProperty("next-2-cost", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next2Cost { get; set; }

        [JsonProperty("next-2-materials", NullValueHandling = NullValueHandling.Ignore)]
        public string? Next2Materials { get; set; }

        [JsonProperty("next-3-name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Next3Name { get; set; }

        [JsonProperty("next-3-rarity", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next3Rarity { get; set; }

        [JsonProperty("next-3-cost", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next3Cost { get; set; }

        [JsonProperty("next-3-materials", NullValueHandling = NullValueHandling.Ignore)]
        public string? Next3Materials { get; set; }
        [JsonIgnore]
        public ShellTable[]? ShellTable { get; set; }
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
