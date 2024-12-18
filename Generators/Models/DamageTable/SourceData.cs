using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
namespace MediawikiTranslator.Models.DamageTable
{
    public partial class SourceData
    {
        [JsonProperty("app.user_data.EmParamParts")]
        public AppUserDataEmParamParts? AppUserDataEmParamParts { get; set; }
    }

    public partial class AppUserDataEmParamParts
    {
        [JsonProperty("ScarStandardSize")]
        public long ScarStandardSize { get; set; }

        [JsonProperty("ReactionPer")]
        public object[] ReactionPer { get; set; } = [];

        [JsonProperty("_BaseHealth")]
        public long BaseHealth { get; set; }

        [JsonProperty("FieldPitfallThreshold_Scar")]
        public long FieldPitfallThresholdScar { get; set; }

        [JsonProperty("FieldPitfallThreshold_LegendaryScar")]
        public long FieldPitfallThresholdLegendaryScar { get; set; }

        [JsonProperty("_PartsArray")]
        public PartsArray? PartsArray { get; set; }

        [JsonProperty("_MeatArray")]
        public MeatArray? MeatArray { get; set; }

        [JsonProperty("_MultiPartsArray")]
        public MultiPartsArray? MultiPartsArray { get; set; }

        [JsonProperty("_PartsBreakArray")]
        public PartsBreakArray? PartsBreakArray { get; set; }

        [JsonProperty("_WeakPointArray")]
        public WeakPointArray? WeakPointArray { get; set; }

        [JsonProperty("_ScarPointArray")]
        public ScarPointArray? ScarPointArray { get; set; }

        [JsonProperty("_LegendaryScarNumArray")]
        public LegendaryScarNumArray[] LegendaryScarNumArray { get; set; } = [];
    }

    public partial class LegendaryScarNumArray
    {
        [JsonProperty("Group")]
        public long Group { get; set; }

        [JsonProperty("Num")]
        public long Num { get; set; }
    }

    public partial class MeatArray
    {
        [JsonProperty("_DataArray")]
        public MeatArrayDataArray[] DataArray { get; set; } = [];
    }

    public partial class MeatArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        public Guid InstanceGuid { get; set; }

        [JsonProperty("_Slash")]
        public long Slash { get; set; }

        [JsonProperty("_Blow")]
        public long Blow { get; set; }

        [JsonProperty("_Shot")]
        public long Shot { get; set; }

        [JsonProperty("_Fire")]
        public long Fire { get; set; }

        [JsonProperty("_Water")]
        public long Water { get; set; }

        [JsonProperty("_Thunder")]
        public long Thunder { get; set; }

        [JsonProperty("_Ice")]
        public long Ice { get; set; }

        [JsonProperty("_Dragon")]
        public long Dragon { get; set; }

        [JsonProperty("_Stun")]
        public long Stun { get; set; }

        [JsonProperty("_LightPlant")]
        public long LightPlant { get; set; }
    }

    public partial class MultiPartsArray
    {
        [JsonProperty("_DataArray")]
        public MultiPartsArrayDataArray[] DataArray { get; set; } = [];
    }

    public partial class MultiPartsArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        public Guid InstanceGuid { get; set; }

        [JsonProperty("_Vital")]
        public Vital[] Vital { get; set; }

        [JsonProperty("_DefaultEnable")]
        public long DefaultEnable { get; set; }

        [JsonProperty("_MaxCount")]
        public long MaxCount { get; set; }

        [JsonProperty("_Action")]
        public long Action { get; set; }

        [JsonProperty("_Attr")]
        public long Attr { get; set; }

        [JsonProperty("_IsSkipUpdateLinkPartsWhenBroken")]
        public bool IsSkipUpdateLinkPartsWhenBroken { get; set; }

        [JsonProperty("_LinkAll")]
        public bool LinkAll { get; set; }

        [JsonProperty("_LinkPartsGuids")]
        public Guid[] LinkPartsGuids { get; set; } = [];

        [JsonProperty("_IsCustomizePriority")]
        public bool IsCustomizePriority { get; set; }

        [JsonProperty("_PriorityConditions")]
        public string PriorityConditions { get; set; } = string.Empty;
    }

    public partial class Vital
    {
        [JsonProperty("_Value")]
        public long Value { get; set; }
    }

    public partial class PartsArray
    {
        [JsonProperty("_DataArray")]
        public PartsArrayDataArray[] DataArray { get; set; } = [];
    }

    public partial class PartsArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        public Guid InstanceGuid { get; set; }

        [JsonProperty("_Vital")]
        public Vital[] Vital { get; set; } = [];

        [JsonProperty("_RodExtract")]
        public long RodExtract { get; set; }

        [JsonProperty("_MeatGuidNormal")]
        public Guid MeatGuidNormal { get; set; }

        [JsonProperty("_MeatGuidBreak")]
        public Guid MeatGuidBreak { get; set; }

        [JsonProperty("_MeatGuidCustom1")]
        public Guid MeatGuidCustom1 { get; set; }

        [JsonProperty("_MeatGuidCustom2")]
        public Guid MeatGuidCustom2 { get; set; }

        [JsonProperty("_MeatGuidCustom3")]
        public Guid MeatGuidCustom3 { get; set; }

        [JsonProperty("_PartsType")]
        public string PartsType { get; set; } = string.Empty;

        [JsonProperty("_IsEnablePartsVital")]
        public bool IsEnablePartsVital { get; set; }
    }

    public partial class PartsBreakArray
    {
        [JsonProperty("_DataArray")]
        public PartsBreakArrayDataArray[] DataArray { get; set; } = [];
    }

    public partial class PartsBreakArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        public Guid InstanceGuid { get; set; }

        [JsonProperty("_TargetCategory")]
        public long TargetCategory { get; set; }

        [JsonProperty("_TargetDataGuid")]
        public Guid TargetDataGuid { get; set; }

        [JsonProperty("_ExcuteCount")]
        public long ExcuteCount { get; set; }

        [JsonProperty("_MaxCount")]
        public long MaxCount { get; set; }

        [JsonProperty("_Condition")]
        public long Condition { get; set; }

        [JsonProperty("_ConditionCount")]
        public long ConditionCount { get; set; }
    }

    public partial class ScarPointArray
    {
        [JsonProperty("_DataArray")]
        public ScarPointArrayDataArray[] DataArray { get; set; } = [];
    }

    public partial class ScarPointArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        public Guid InstanceGuid { get; set; }

        [JsonProperty("_NormalVital")]
        public Vital[] NormalVital { get; set; } = [];

        [JsonProperty("_TearVital")]
        public Vital[] TearVital { get; set; } = [];

        [JsonProperty("_RawScarVital")]
        public Vital[] RawScarVital { get; set; } = [];

        [JsonProperty("_MeatGuid")]
        public Guid MeatGuid { get; set; }

        [JsonProperty("_LinkPartsGuid")]
        public Guid LinkPartsGuid { get; set; }

        [JsonProperty("SizeRate")]
        public double SizeRate { get; set; }

        [JsonProperty("_Num")]
        public long Num { get; set; }
    }

    public partial class WeakPointArray
    {
        [JsonProperty("_DataArray")]
        public WeakPointArrayDataArray[] DataArray { get; set; } = [];
    }

    public partial class WeakPointArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        public Guid InstanceGuid { get; set; }

        [JsonProperty("_Vital")]
        public Vital[] Vital { get; set; } = [];

        [JsonProperty("_MeatGuid")]
        public Guid MeatGuid { get; set; }

        [JsonProperty("_LinkPartsGuid")]
        public Guid LinkPartsGuid { get; set; }
    }

    public partial class SourceData
    {
        public static SourceData FromJson(string json) => JsonConvert.DeserializeObject<SourceData>(json, MediawikiTranslator.Models.DamageTable.Converter.Settings)!;
    }

    public static class Serialize
    {
        public static string ToJson(this SourceData self) => JsonConvert.SerializeObject(self, MediawikiTranslator.Models.DamageTable.Converter.Settings);
    }

    internal static class Converter
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
