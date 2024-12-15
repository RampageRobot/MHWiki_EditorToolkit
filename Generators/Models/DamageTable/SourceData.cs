using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
namespace MediawikiTranslator.Models.DamageTable
{
    public partial class SourceData
    {
        [JsonProperty("struct")]
        public Struct? Struct { get; set; }
        public string? FileName { get; set; }
    }

    public partial class Struct
    {
        [JsonProperty(nameof(ScarStandardSize))]
        internal double ScarStandardSize { get; set; }

        [JsonProperty(nameof(ReactionPer))]
        internal object[]? ReactionPer { get; set; }

        [JsonProperty("_BaseHealth")]
        internal long BaseHealth { get; set; }

        [JsonProperty("FieldPitfallThreshold_Scar")]
        internal long FieldPitfallThresholdScar { get; set; }

        [JsonProperty("FieldPitfallThreshold_LegendaryScar")]
        internal long FieldPitfallThresholdLegendaryScar { get; set; }

        [JsonProperty("_PartsArray")]
        internal PartsArray? PartsArray { get; set; }

        [JsonProperty("_MeatArray")]
        public MeatArray? MeatArray { get; set; }

        [JsonProperty("_MultiPartsArray")]
        internal MultiPartsArray? MultiPartsArray { get; set; }

        [JsonProperty("_PartsBreakArray")]
        internal PartsBreakArray? PartsBreakArray { get; set; }

        [JsonProperty("_WeakPointArray")]
        internal WeakPointArray? WeakPointArray { get; set; }

        [JsonProperty("_ScarPointArray")]
        internal ScarPointArray? ScarPointArray { get; set; }

        [JsonProperty("_LegendaryScarNumArray")]
        internal LegendaryScarNumArray[]? LegendaryScarNumArray { get; set; }
    }

    internal partial class LegendaryScarNumArray
    {
        [JsonProperty(nameof(Group))]
        internal long Group { get; set; }

        [JsonProperty(nameof(Num))]
        internal long Num { get; set; }
    }

    public partial class MeatArray
    {
        [JsonProperty("_DataArray")]
        public MeatArrayDataArray[]? DataArray { get; set; }
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

    internal partial class MultiPartsArray
    {
        [JsonProperty("_DataArray")]
        internal MultiPartsArrayDataArray[]? DataArray { get; set; }
    }

    internal partial class MultiPartsArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        internal Guid InstanceGuid { get; set; }

        [JsonProperty("_Vital")]
        internal PriorityConditions[]? Vital { get; set; }

        [JsonProperty("_DefaultEnable")]
        internal long DefaultEnable { get; set; }

        [JsonProperty("_MaxCount")]
        internal long MaxCount { get; set; }

        [JsonProperty("_Action")]
        internal long Action { get; set; }

        [JsonProperty("_Attr")]
        internal long Attr { get; set; }

        [JsonProperty("_IsSkipUpdateLinkPartsWhenBroken")]
        internal bool IsSkipUpdateLinkPartsWhenBroken { get; set; }

        [JsonProperty("_LinkAll")]
        internal bool LinkAll { get; set; }

        [JsonProperty("_LinkPartsGuids")]
        internal Guid[]? LinkPartsGuids { get; set; }

        [JsonProperty("_IsCustomizePriority")]
        internal bool IsCustomizePriority { get; set; }

        [JsonProperty("_PriorityConditions")]
        internal PriorityConditions? PriorityConditions { get; set; }
    }

    internal partial class PriorityConditions
    {
        [JsonProperty("_Value")]
        internal long Value { get; set; }
    }

    internal partial class PartsArray
    {
        [JsonProperty("_DataArray")]
        internal PartsArrayDataArray[]? DataArray { get; set; }
    }

    internal partial class PartsArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        internal Guid InstanceGuid { get; set; }

        [JsonProperty("_Vital")]
        internal PriorityConditions[]? Vital { get; set; }

        [JsonProperty("_RodExtract")]
        internal long RodExtract { get; set; }

        [JsonProperty("_MeatGuidNormal")]
        internal Guid MeatGuidNormal { get; set; }

        [JsonProperty("_MeatGuidBreak")]
        internal Guid MeatGuidBreak { get; set; }

        [JsonProperty("_MeatGuidCustom1")]
        internal Guid MeatGuidCustom1 { get; set; }

        [JsonProperty("_MeatGuidCustom2")]
        internal Guid MeatGuidCustom2 { get; set; }

        [JsonProperty("_MeatGuidCustom3")]
        internal Guid MeatGuidCustom3 { get; set; }

        [JsonProperty("_PartsType")]
        internal PriorityConditions? PartsType { get; set; }

        [JsonProperty("_IsEnablePartsVital")]
        internal bool IsEnablePartsVital { get; set; }
    }

    internal partial class PartsBreakArray
    {
        [JsonProperty("_DataArray")]
        internal PartsBreakArrayDataArray[]? DataArray { get; set; }
    }

    internal partial class PartsBreakArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        internal Guid InstanceGuid { get; set; }

        [JsonProperty("_TargetCategory")]
        internal long TargetCategory { get; set; }

        [JsonProperty("_TargetDataGuid")]
        internal Guid TargetDataGuid { get; set; }

        [JsonProperty("_ExcuteCount")]
        internal long ExcuteCount { get; set; }

        [JsonProperty("_MaxCount")]
        internal long MaxCount { get; set; }

        [JsonProperty("_Condition")]
        internal long Condition { get; set; }

        [JsonProperty("_ConditionCount")]
        internal long ConditionCount { get; set; }
    }

    internal partial class ScarPointArray
    {
        [JsonProperty("_DataArray")]
        internal ScarPointArrayDataArray[]? DataArray { get; set; }
    }

    internal partial class ScarPointArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        internal Guid InstanceGuid { get; set; }

        [JsonProperty("_NormalVital")]
        internal PriorityConditions[]? NormalVital { get; set; }

        [JsonProperty("_TearVital")]
        internal PriorityConditions[]? TearVital { get; set; }

        [JsonProperty("_RawScarVital")]
        internal PriorityConditions[]? RawScarVital { get; set; }

        [JsonProperty("_MeatGuid")]
        internal Guid MeatGuid { get; set; }

        [JsonProperty("_LinkPartsGuid")]
        internal Guid LinkPartsGuid { get; set; }

        [JsonProperty(nameof(SizeRate))]
        internal double SizeRate { get; set; }

        [JsonProperty("_Num")]
        internal long Num { get; set; }
    }

    internal partial class WeakPointArray
    {
        [JsonProperty("_DataArray")]
        internal WeakPointArrayDataArray[] DataArray { get; set; } = [];
    }

    internal partial class WeakPointArrayDataArray
    {
        [JsonProperty("_InstanceGuid")]
        internal Guid InstanceGuid { get; set; }

        [JsonProperty("_Vital")]
        internal PriorityConditions[] Vital { get; set; } = [];

		[JsonProperty("_MeatGuid")]
        internal Guid MeatGuid { get; set; }

        [JsonProperty("_LinkPartsGuid")]
        internal Guid LinkPartsGuid { get; set; }
    }

    public partial class SourceData
    {
        internal static SourceData FromJson(string json) => JsonConvert.DeserializeObject<SourceData>(json, Converter.Settings)!;

        internal static Task<SourceData[]> FromGameSource(byte[] zipFile)
        {
            return Task.Run(async () =>
            {
                DirectoryInfo workspace = Utilities.GetWorkspace();
                string zipPath = Path.Combine(workspace.FullName, Guid.NewGuid().ToString() + ".zip");
                await File.WriteAllBytesAsync(zipPath, zipFile);
                string extractDir = Path.Combine(workspace.FullName, Guid.NewGuid().ToString());
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractDir);
                IEnumerable<string> files = Directory.EnumerateFiles(extractDir);
                List<SourceData> sources = [];
                foreach (string file in files)
                {
                    SourceData? data = null;
                    try
                    {
                        string fileData = await File.ReadAllTextAsync(file);
                        data = SourceData.FromJson(fileData);
                        data!.FileName = Path.GetFileNameWithoutExtension(file);
                    }
                    finally
                    {
                        if (data != null)
                        {
                            sources.Add(data);
                        }
                    }
                }
                return sources.ToArray();
            });
        }
    }

    internal static class Serialize
    {
        internal static string ToJson(this SourceData self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        internal static readonly JsonSerializerSettings Settings = new()
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
