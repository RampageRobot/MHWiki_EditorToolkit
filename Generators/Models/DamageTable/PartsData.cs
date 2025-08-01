
using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MediawikiTranslator.Models.DamageTable.PartsData
{

    public partial class PartsData
    {
        [JsonProperty("struct")]
        public Struct? Struct { get; set; }
    }

    public partial class Struct
    {
        [JsonProperty("_Values")]
        public Value[] Values { get; set; } = [];
    }

    public partial class Value
    {
        [JsonProperty("_Index")]
        public long Index { get; set; }

        [JsonProperty("_EmPartsType")]
        public string EmPartsType { get; set; } = string.Empty;

        [JsonProperty("_IconType")]
        public IconType IconType { get; set; }

        [JsonProperty("_EmPartsName")]
        public Guid EmPartsName { get; set; }

        [JsonProperty("_EmRottenPartsName")]
        public Guid EmRottenPartsName { get; set; }

        [JsonProperty("_EmPartsExp")]
        public Guid EmPartsExp { get; set; }
    }

    public enum IconType { Invalid, Item0040, Item0042, Item0044, Item0046, Item0047, Item0048 };

    public partial class PartsData
    {
        public static PartsData FromJson(string json) => JsonConvert.DeserializeObject<PartsData>(json, MediawikiTranslator.Models.DamageTable.PartsData.Converter.Settings)!;
    }

    public static class Serialize
    {
        public static string ToJson(this PartsData self) => JsonConvert.SerializeObject(self, MediawikiTranslator.Models.DamageTable.PartsData.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new()
		{
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                IconTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class IconTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(IconType) || t == typeof(IconType?);

        public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "INVALID":
                    return IconType.Invalid;
                case "ITEM_0040":
                    return IconType.Item0040;
                case "ITEM_0042":
                    return IconType.Item0042;
                case "ITEM_0044":
                    return IconType.Item0044;
                case "ITEM_0046":
                    return IconType.Item0046;
                case "ITEM_0047":
                    return IconType.Item0047;
                case "ITEM_0048":
                    return IconType.Item0048;
            }
            throw new Exception("Cannot unmarshal type IconType");
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (IconType)untypedValue;
            switch (value)
            {
                case IconType.Invalid:
                    serializer.Serialize(writer, "INVALID");
                    return;
                case IconType.Item0040:
                    serializer.Serialize(writer, "ITEM_0040");
                    return;
                case IconType.Item0042:
                    serializer.Serialize(writer, "ITEM_0042");
                    return;
                case IconType.Item0044:
                    serializer.Serialize(writer, "ITEM_0044");
                    return;
                case IconType.Item0046:
                    serializer.Serialize(writer, "ITEM_0046");
                    return;
                case IconType.Item0047:
                    serializer.Serialize(writer, "ITEM_0047");
                    return;
                case IconType.Item0048:
                    serializer.Serialize(writer, "ITEM_0048");
                    return;
            }
            throw new Exception("Cannot marshal type IconType");
        }

        public static readonly IconTypeConverter Singleton = new();
    }
}
