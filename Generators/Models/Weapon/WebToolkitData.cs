using MediawikiTranslator.Models.Data.MHWI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
namespace MediawikiTranslator.Models.Weapon
{

    public partial class WebToolkitData
	{
		public int Order { get; set; } = 0;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Game { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Tree { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Attack { get; set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Rarity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? ForgeCost { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ForgeMaterials { get; set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public string Defense { get; set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Affinity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Element1 { get; set; } = string.Empty;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ElementDmg1 { get; set; } = string.Empty;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Element2 { get; set; } = string.Empty;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ElementDmg2 { get; set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Rollback { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Sharpness { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Decos4 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Elderseal { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? ArmorSkills { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? RampageSkillSlots { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? RampageDecoration { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? HhMelodies { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? HhNote1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? HhNote2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? HhNote3 { get; set; }
		public string? HhSpecialMelody { get; set; }
		public string? HhEchoBubble { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? GlShellingType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? GlShellingLevel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? SaPhialType { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? SaPhialDamage { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? CbPhialType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? IgKinsectBonus { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? BoCoatings { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? HbgSpecialAmmoType { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? HbgSpecialAmmoType1 { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? HbgSpecialAmmoType2 { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? HbgDefaultMod1 { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? HbgDefaultMod2 { get; set; }
		public string? HbgIgnitionGauge { get; set; }
		public string? HbgStandardIgnitionType { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? HbgDeviation { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? LbgSpecialAmmoType { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? LbgDefaultMod1 { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? LbgDefaultMod2 { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? LbgDeviation { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? PreviousName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? PreviousRarity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? UpgradeCost { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? UpgradeMaterials { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Next1Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next1Rarity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next1Cost { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Next1Materials { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Next2Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next2Rarity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next2Cost { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Next2Materials { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Next3Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next3Rarity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Next3Cost { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Next4Materials { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Next4Name { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Next4Rarity { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Next4Cost { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Next3Materials { get; set; }
        public ShellTable[]? ShellTable { get; set; }
        public string? MonsterName { get; set; }
        [JsonIgnore]
        public string ShellTableWikitext { get; set; } = string.Empty;
        public WebToolkitData[]? UnlocksForgingFor { get; set; }
        public WebToolkitData? UnlockedByForging { get; set; }
        public string? SrcDescription { get; set; }
        public string? HighFreqEnum { get; set; }
		public int SrcAttack { get; set; }

		public WebToolkitData Clone()
        {
            return new WebToolkitData()
            {
                Attack = Attack,
                Affinity = Affinity,
                ArmorSkills = ArmorSkills,
                HbgSpecialAmmoType = HbgSpecialAmmoType,
                LbgSpecialAmmoType = LbgSpecialAmmoType,
                BoCoatings = BoCoatings,
                CbPhialType = CbPhialType,
                Decos1 = Decos1,
                Decos2 = Decos2,
                Decos3 = Decos3,
                Decos4 = Decos4,
                Defense = Defense,
                Description = Description,
                Elderseal = Elderseal,
                Element1 = Element1,
                Element2 = Element2,
                ElementDmg1 = ElementDmg1,
                ElementDmg2 = ElementDmg2,
                ForgeCost = ForgeCost,
                ForgeMaterials = ForgeMaterials,
                Game = Game,
                GlShellingLevel = GlShellingLevel,
                GlShellingType = GlShellingType,
                HbgDeviation = HbgDeviation,
                HhNote1 = HhNote1,
                HhNote2 = HhNote2,
                HhNote3 = HhNote3,
                IgKinsectBonus = IgKinsectBonus,
                LbgDeviation = LbgDeviation,
                Name = Name,
                Next1Cost = Next1Cost,
                Next1Materials = Next1Materials,
                Next1Name = Next1Name,
                Next1Rarity = Next1Rarity,
                Next2Cost = Next2Cost,
                Next2Materials = Next2Materials,
                Next2Name = Next2Name,
                Next2Rarity = Next2Rarity,
                Next3Cost = Next3Cost,
                Next3Materials = Next3Materials,
                Next3Name = Next3Name,
                Next3Rarity = Next3Rarity,
                PreviousName = PreviousName,
                PreviousRarity = PreviousRarity,
                RampageDecoration = RampageDecoration,
                RampageSkillSlots = RampageSkillSlots,
                Rarity = Rarity,
                Rollback = Rollback,
                SaPhialType = SaPhialType,
                Sharpness = Sharpness,
                ShellTable = ShellTable,
                Tree = Tree,
                Type = Type,
                UpgradeCost = UpgradeCost,
                UpgradeMaterials = UpgradeMaterials,
                HbgSpecialAmmoType1 = HbgSpecialAmmoType1,
                HbgSpecialAmmoType2 = HbgSpecialAmmoType2,
                HbgDefaultMod1 = HbgDefaultMod1,
                HbgDefaultMod2 = HbgDefaultMod2,
                HhEchoBubble = HhEchoBubble,
                HhMelodies = HhMelodies,
                HhSpecialMelody = HhSpecialMelody,
                LbgDefaultMod1 = LbgDefaultMod1,
                LbgDefaultMod2 = LbgDefaultMod2,
                Next4Cost = Next4Cost,
                Next4Materials = Next4Materials,
                Next4Name = Next4Name,
                Next4Rarity = Next4Rarity,
                SaPhialDamage = SaPhialDamage,
                ShellTableWikitext = ShellTableWikitext,
                UnlockedByForging = UnlockedByForging,
                UnlocksForgingFor = UnlocksForgingFor
            };
        }
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
            var value = long.Parse(untypedValue!.ToString()!);
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new();
    }
}
