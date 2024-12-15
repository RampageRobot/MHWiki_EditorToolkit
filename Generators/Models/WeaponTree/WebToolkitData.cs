using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.WeaponTree
{
	public partial class WebToolkitData
	{
		[JsonProperty("can-forge")]
		public bool CanForge { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("parent")]
		public string Parent { get; set; } = string.Empty;

		[JsonProperty("icon-type")]
		public string IconType { get; set; } = string.Empty;

		[JsonProperty("rarity")]
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Rarity { get; set; }

		[JsonProperty("attack")]
		public string Attack { get; set; } = string.Empty;

		[JsonProperty("defense")]
		public string Defense { get; set; } = string.Empty;

		[JsonProperty("element")]
		public Element Element { get; set; }

		[JsonProperty("element-damage")]
		public string ElementDamage { get; set; } = string.Empty;

		[JsonProperty("affinity")]
		[JsonConverter(typeof(ParseStringConverter))]
		public long? Affinity { get; set; }

		[JsonProperty("decos")]
		public string Decos { get; set; } = string.Empty;

		[JsonProperty("sharpness")]
		public string Sharpness { get; set; } = string.Empty;
	}

	public class Decoration
	{
		public int Level { get; set; }
		public int Quantity { get; set; }
		public bool IsRampage { get; set; }
	}

	public enum Element { Empty, Fire, Water, Thunder, Dragon, Ice, Poison, Paralysis, Sleep, Blast };

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
				ElementConverter.Singleton,
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

	internal class ElementConverter : JsonConverter
	{
		public override bool CanConvert(Type t) => t == typeof(Element) || t == typeof(Element?);

		public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null) return null;
			var value = serializer.Deserialize<string>(reader);
			switch (value)
			{
				case "":
					return Element.Empty;
				case "Fire":
					return Element.Fire;
				case "Water":
					return Element.Water;
				case "Thunder":
					return Element.Thunder;
				case "Dragon":
					return Element.Dragon;
				case "Ice":
					return Element.Ice;
				case "Poison":
					return Element.Poison;
				case "Paralysis":
					return Element.Paralysis;
				case "Sleep":
					return Element.Sleep;
				case "Blast":
					return Element.Blast;
			}
			throw new Exception("Element type is not valid.");
		}

		public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}
			var value = (Element)untypedValue;
			switch (value)
			{
				case Element.Empty:
					serializer.Serialize(writer, "");
					return;
				case Element.Fire:
					serializer.Serialize(writer, "Fire");
					return;
				case Element.Water:
					serializer.Serialize(writer, "Water");
					return;
				case Element.Thunder:
					serializer.Serialize(writer, "Thunder");
					return;
				case Element.Dragon:
					serializer.Serialize(writer, "Dragon");
					return;
				case Element.Ice:
					serializer.Serialize(writer, "Ice");
					return;
				case Element.Poison:
					serializer.Serialize(writer, "Poison");
					return;
				case Element.Paralysis:
					serializer.Serialize(writer, "Paralysis");
					return;
				case Element.Sleep:
					serializer.Serialize(writer, "Sleep");
					return;
				case Element.Blast:
					serializer.Serialize(writer, "Blast");
					return;

			}
			throw new Exception("Element type is not valid.");
		}

		public static readonly ElementConverter Singleton = new();
	}
}
