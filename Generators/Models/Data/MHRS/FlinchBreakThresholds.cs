using MediawikiTranslator.Models.Monsters;
using Newtonsoft.Json;
using System.Text;

namespace MediawikiTranslator.Models.Data.MHRS
{
	public class FlinchBreakThresholds
	{
		public static PartsTable[] GetWebToolkitData(string monsterId)
		{
			List<PartsTable> ret = [];
			DataTune tune = DataTune.Fetch(monsterId);
			string[] names = GetBreakNames(monsterId);
			int cntr = 0;
			foreach (EnemyPartsDatum datum in tune.SnowEnemyEnemyDataTune!.EnemyPartsData!.Where(x => x.ExtractiveType != "None" || x.Vital != 100))
			{
				string partName = Utilities.Translate(names[cntr]);
				KinsectEssence ess = KinsectEssence.None;
				switch (datum.ExtractiveType)
				{
					case "Red":
						ess = KinsectEssence.Red;
						break;
					case "White":
						ess = KinsectEssence.White;
						break;
					case "Orange":
						ess = KinsectEssence.Orange;
						break;
					case "Green":
						ess = KinsectEssence.Green;
						break;
					case "None":
						break;
				}
				PartsTable part = new()
				{
					PartName = partName,
					Essence = ess,
					Break = (int)datum.Vital!
				};
				cntr++;
			}
			return [.. ret];
		}

		private static string[] GetBreakNames(string monsterId)
		{
			byte[] test = File.ReadAllBytes($@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\unpacked\re_chunk_000\natives\STM\enemy\{monsterId.Split("_")[0]}\{monsterId.Split("_")[1]}\collision\{monsterId}_colliders.rcol.20");
			int cntr = 128;
			List<string> names = [];
			int maxGroups = BitConverter.ToInt32(test, 4);
			int groups = 0;
			while (groups < maxGroups)
			{
				//Get offset
				int originalOffset = Convert.ToInt32(BitConverter.ToUInt64(test, cntr));
				int offset = originalOffset;
				//loop through until double 00
				bool finished = false;
				int stringCntr = 0;
				while (!finished)
				{
					string c = Encoding.Unicode.GetString(test, offset++, 2);
					if (c != "\0")
					{
						stringCntr++;
					}
					else
					{
						finished = true;
					}
				}
				names.Add(Encoding.Unicode.GetString(test, originalOffset, stringCntr));
				cntr += 80;
				groups++;
			}
			return [..names];
		}
	}

	public partial class DataTune
	{
		[JsonProperty("snow.enemy.EnemyDataTune", NullValueHandling = NullValueHandling.Ignore)]
		public SnowEnemyEnemyDataTune? SnowEnemyEnemyDataTune { get; set; }

		public static DataTune Fetch(string monsterId)
		{
			string[] idParts = monsterId.Split('_');
			string species = idParts[0];
			string sub = idParts[1];
			return JsonConvert.DeserializeObject<DataTune>(Utilities.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\natives\stm\enemy\{species}\{sub}\user_data\{species}_{sub}_datatune.user.2.json"))!;
		}
	}

	public partial class SnowEnemyEnemyDataTune
	{
		[JsonProperty("_baseHpVital", NullValueHandling = NullValueHandling.Ignore)]
		public long? BaseHpVital { get; set; }

		[JsonProperty("_masterHpVital", NullValueHandling = NullValueHandling.Ignore)]
		public long? MasterHpVital { get; set; }

		[JsonProperty("_enemyPartsData", NullValueHandling = NullValueHandling.Ignore)]
		public EnemyPartsDatum[]? EnemyPartsData { get; set; }

		[JsonProperty("_enemyPartsBreakDataList", NullValueHandling = NullValueHandling.Ignore)]
		public EnemyPartsBreakDataList[]? EnemyPartsBreakDataList { get; set; }

		[JsonProperty("_enemyPartsLossDataList", NullValueHandling = NullValueHandling.Ignore)]
		public EnemyPartsLossDataList[]? EnemyPartsLossDataList { get; set; }

		[JsonProperty("_enemyMultiPartsVitalSystemData", NullValueHandling = NullValueHandling.Ignore)]
		public EnemyMultiPartsVitalSystemDatum[]? EnemyMultiPartsVitalSystemData { get; set; }

		[JsonProperty("_enemyMultiPartsVitalDataList", NullValueHandling = NullValueHandling.Ignore)]
		public object[]? EnemyMultiPartsVitalDataList { get; set; }

		[JsonProperty("_gimmickVitalData", NullValueHandling = NullValueHandling.Ignore)]
		public VitalData? GimmickVitalData { get; set; }

		[JsonProperty("_marionetteVitalData", NullValueHandling = NullValueHandling.Ignore)]
		public VitalData? MarionetteVitalData { get; set; }

		[JsonProperty("_TerrainActionCheckDist", NullValueHandling = NullValueHandling.Ignore)]
		public long? TerrainActionCheckDist { get; set; }

		[JsonProperty("_AdjustWallPointOffset", NullValueHandling = NullValueHandling.Ignore)]
		public long? AdjustWallPointOffset { get; set; }

		[JsonProperty("_CharacterControllerTuneData", NullValueHandling = NullValueHandling.Ignore)]
		public CharacterControllerTuneDatum[]? CharacterControllerTuneData { get; set; }

		[JsonProperty("_Weight", NullValueHandling = NullValueHandling.Ignore)]
		public string? Weight { get; set; }

		[JsonProperty("_DyingVillageHpVitalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? DyingVillageHpVitalRate { get; set; }

		[JsonProperty("_DyingLowLevelHpVitalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? DyingLowLevelHpVitalRate { get; set; }

		[JsonProperty("_DyingHighLevelHpVitalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? DyingHighLevelHpVitalRate { get; set; }

		[JsonProperty("_DyingMasterClassHpVitalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? DyingMasterClassHpVitalRate { get; set; }

		[JsonProperty("_CaptureVillageHpVitalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? CaptureVillageHpVitalRate { get; set; }

		[JsonProperty("_CaptureLowLevelHpVitalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? CaptureLowLevelHpVitalRate { get; set; }

		[JsonProperty("_CaptureHighLevelHpVitalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? CaptureHighLevelHpVitalRate { get; set; }

		[JsonProperty("_CaptureMasterLevelHpVitalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? CaptureMasterLevelHpVitalRate { get; set; }

		[JsonProperty("_SelfSleepRecoverHpVitalRate", NullValueHandling = NullValueHandling.Ignore)]
		public long? SelfSleepRecoverHpVitalRate { get; set; }

		[JsonProperty("_SelfSleepTime", NullValueHandling = NullValueHandling.Ignore)]
		public long? SelfSleepTime { get; set; }

		[JsonProperty("_InCombatSelfSleepFlag", NullValueHandling = NullValueHandling.Ignore)]
		public bool? InCombatSelfSleepFlag { get; set; }

		[JsonProperty("_DummyShadowScale", NullValueHandling = NullValueHandling.Ignore)]
		public long? DummyShadowScale { get; set; }

		[JsonProperty("_MaxNumForNormalQuest", NullValueHandling = NullValueHandling.Ignore)]
		public long? MaxNumForNormalQuest { get; set; }

		[JsonProperty("_MaxNumForHyakuryuQuest", NullValueHandling = NullValueHandling.Ignore)]
		public long? MaxNumForHyakuryuQuest { get; set; }

		[JsonProperty("_MaxSoundDamageCount", NullValueHandling = NullValueHandling.Ignore)]
		public long? MaxSoundDamageCount { get; set; }
	}

	public partial class CharacterControllerTuneDatum
	{
		[JsonProperty("_Radius", NullValueHandling = NullValueHandling.Ignore)]
		public double? Radius { get; set; }

		[JsonProperty("_OffsetY", NullValueHandling = NullValueHandling.Ignore)]
		public long? OffsetY { get; set; }
	}

	public partial class EnemyMultiPartsVitalSystemDatum
	{
		[JsonProperty("_UseType", NullValueHandling = NullValueHandling.Ignore)]
		public string? UseType { get; set; }

		[JsonProperty("_Priority", NullValueHandling = NullValueHandling.Ignore)]
		public string? Priority { get; set; }

		[JsonProperty("_EnablePartsData", NullValueHandling = NullValueHandling.Ignore)]
		public EnablePartsDatum[]? EnablePartsData { get; set; }

		[JsonProperty("_EnableLastAttackParts", NullValueHandling = NullValueHandling.Ignore)]
		public object[]? EnableLastAttackParts { get; set; }

		[JsonProperty("_IsEnableHyakuryu", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsEnableHyakuryu { get; set; }

		[JsonProperty("_IsEnableOverwriteDown", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsEnableOverwriteDown { get; set; }

		[JsonProperty("_IsPrioDamageCustomize", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsPrioDamageCustomize { get; set; }

		[JsonProperty("_PrioDamageCategoryFlag", NullValueHandling = NullValueHandling.Ignore)]
		public string? PrioDamageCategoryFlag { get; set; }

		[JsonProperty("_IsNotUseDifficultyRate", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsNotUseDifficultyRate { get; set; }

		[JsonProperty("_IsUseMultiRateEx", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsUseMultiRateEx { get; set; }

		[JsonProperty("_MultiPartsVitalData", NullValueHandling = NullValueHandling.Ignore)]
		public MultiPartsVitalDatum[]? MultiPartsVitalData { get; set; }

		[JsonProperty("EnablePartsNames", NullValueHandling = NullValueHandling.Ignore)]
		public object[]? EnablePartsNames { get; set; }

		[JsonProperty("EnablePartsValues", NullValueHandling = NullValueHandling.Ignore)]
		public object[]? EnablePartsValues { get; set; }
	}

	public partial class EnablePartsDatum
	{
		[JsonProperty("_EnableParts", NullValueHandling = NullValueHandling.Ignore)]
		public bool[]? EnableParts { get; set; }
	}

	public partial class MultiPartsVitalDatum
	{
		[JsonProperty("_Vital", NullValueHandling = NullValueHandling.Ignore)]
		public long? Vital { get; set; }

		[JsonProperty("_MasterVital", NullValueHandling = NullValueHandling.Ignore)]
		public long? MasterVital { get; set; }
	}

	public partial class EnemyPartsBreakDataList
	{
		[JsonProperty("_PartsGroup", NullValueHandling = NullValueHandling.Ignore)]
		public string? PartsGroup { get; set; }

		[JsonProperty("_PartsBreakDataList", NullValueHandling = NullValueHandling.Ignore)]
		public PartsBreakDataList[]? PartsBreakDataList { get; set; }
	}

	public partial class PartsBreakDataList
	{
		[JsonProperty("_BreakLevel", NullValueHandling = NullValueHandling.Ignore)]
		public long? BreakLevel { get; set; }

		[JsonProperty("_Vital", NullValueHandling = NullValueHandling.Ignore)]
		public long? Vital { get; set; }

		[JsonProperty("_MasterVital", NullValueHandling = NullValueHandling.Ignore)]
		public long? MasterVital { get; set; }

		[JsonProperty("_IgnoreCondition", NullValueHandling = NullValueHandling.Ignore)]
		public string? IgnoreCondition { get; set; }

		[JsonProperty("_IgnoreCheckCount", NullValueHandling = NullValueHandling.Ignore)]
		public long? IgnoreCheckCount { get; set; }

		[JsonProperty("_RewardData", NullValueHandling = NullValueHandling.Ignore)]
		public long? RewardData { get; set; }
	}

	public partial class EnemyPartsDatum
	{
		[JsonProperty("_Vital", NullValueHandling = NullValueHandling.Ignore)]
		public long? Vital { get; set; }

		[JsonProperty("_MasterVital", NullValueHandling = NullValueHandling.Ignore)]
		public long? MasterVital { get; set; }

		[JsonProperty("_ExtractiveType", NullValueHandling = NullValueHandling.Ignore)]
		public string? ExtractiveType { get; set; }
	}

	public partial class EnemyPartsLossDataList
	{
		[JsonProperty("_PartsGroup", NullValueHandling = NullValueHandling.Ignore)]
		public string? PartsGroup { get; set; }

		[JsonProperty("_PartsLossData", NullValueHandling = NullValueHandling.Ignore)]
		public PartsLossData? PartsLossData { get; set; }
	}

	public partial class PartsLossData
	{
		[JsonProperty("_Vital", NullValueHandling = NullValueHandling.Ignore)]
		public long? Vital { get; set; }

		[JsonProperty("_MasterVital", NullValueHandling = NullValueHandling.Ignore)]
		public long? MasterVital { get; set; }

		[JsonProperty("_PermitDamageAttr", NullValueHandling = NullValueHandling.Ignore)]
		public string? PermitDamageAttr { get; set; }
	}

	public partial class VitalData
	{
		[JsonProperty("_VitalS", NullValueHandling = NullValueHandling.Ignore)]
		public long? VitalS { get; set; }

		[JsonProperty("_VitalM", NullValueHandling = NullValueHandling.Ignore)]
		public long? VitalM { get; set; }

		[JsonProperty("_VitalL", NullValueHandling = NullValueHandling.Ignore)]
		public long? VitalL { get; set; }

		[JsonProperty("_VitalKnockBack", NullValueHandling = NullValueHandling.Ignore)]
		public long? VitalKnockBack { get; set; }
	}

	public partial class MonsterListBossData
	{
		[JsonProperty("snow.data.monsterList.MonsterListBossData", NullValueHandling = NullValueHandling.Ignore)]
		public SnowDataMonsterListMonsterListBossData? SnowDataMonsterListMonsterListBossData { get; set; }
		public static MonsterListBossData Fetch()
		{
			return JsonConvert.DeserializeObject<MonsterListBossData>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\natives\stm\data\define\common\hunternote\monsterlistbossdata_mr.user.2.json"))!;
		}
	}

	public partial class SnowDataMonsterListMonsterListBossData
	{
		[JsonProperty("_DataList", NullValueHandling = NullValueHandling.Ignore)]
		public DataList[]? DataList { get; set; }
	}

	public partial class DataList
	{
		[JsonProperty("_EmType", NullValueHandling = NullValueHandling.Ignore)]
		public string? EmType { get; set; }

		[JsonProperty("_FamilyType", NullValueHandling = NullValueHandling.Ignore)]
		public string? FamilyType { get; set; }

		[JsonProperty("_HabitatArea", NullValueHandling = NullValueHandling.Ignore)]
		public HabitatArea? HabitatArea { get; set; }

		[JsonProperty("_IsLimitOpenLv", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsLimitOpenLv { get; set; }

		[JsonProperty("_PartTableData", NullValueHandling = NullValueHandling.Ignore)]
		public PartTableDatum[]? PartTableData { get; set; }

		[JsonProperty("_MarionetteTableData", NullValueHandling = NullValueHandling.Ignore)]
		public MarionetteTableDatum[]? MarionetteTableData { get; set; }
	}

	public partial class HabitatArea
	{
		[JsonProperty("_Flag", NullValueHandling = NullValueHandling.Ignore)]
		public long[]? Flag { get; set; }
	}

	public partial class MarionetteTableDatum
	{
		[JsonProperty("_AttackType", NullValueHandling = NullValueHandling.Ignore)]
		public string? AttackType { get; set; }

		[JsonProperty("_IsButtonRepeatedly", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsButtonRepeatedly { get; set; }

		[JsonProperty("_IsChangeAir", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsChangeAir { get; set; }

		[JsonProperty("_DescriptionID", NullValueHandling = NullValueHandling.Ignore)]
		public Guid? DescriptionId { get; set; }
	}

	public partial class PartTableDatum
	{
		[JsonProperty("_Part", NullValueHandling = NullValueHandling.Ignore)]
		public string? Part { get; set; }

		[JsonProperty("_CircleSize", NullValueHandling = NullValueHandling.Ignore)]
		public string? CircleSize { get; set; }

		[JsonProperty("_CirclePos", NullValueHandling = NullValueHandling.Ignore)]
		public long[]? CirclePos { get; set; }

		[JsonProperty("_EmPart", NullValueHandling = NullValueHandling.Ignore)]
		public string? EmPart { get; set; }

		[JsonProperty("_EmMeatGroupIdx", NullValueHandling = NullValueHandling.Ignore)]
		public long? EmMeatGroupIdx { get; set; }
	}
}
