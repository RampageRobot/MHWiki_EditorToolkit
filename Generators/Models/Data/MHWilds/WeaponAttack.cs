using MediawikiTranslator.Models.Data.MHRS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Data.MHWilds
{
	public class WeaponAttack
	{
		public string Name { get; set; } = string.Empty;
		public int MotionValue { get; set; }
		public string DamageType { get; set; } = string.Empty;
		public bool MindsEye { get; set; }
		public string FixedSharpness { get; set; } = string.Empty;
		public int OffsetDamage { get; set; } = 0;
		public int RideDamage { get; set; } = 0;
		internal static readonly string[] WeaponNames = ["GS", "SnS", "DB", "LS", "Hm", "HH", "Ln", "GL", "SA", "CB", "IG", "Bo", "HBG", "LBG"];

		public static void JsonifyWeaponAttackData()
		{
			Dictionary<string, object> vals = [];
			for ( int i = 0; i < WeaponNames.Length; i++)
			{
				ActionGuideData[] agd = ActionGuideData.Fetch(i);
				ActionGuideData_ComboDetail[] agdc = ActionGuideData_ComboDetail.Fetch(i);
				vals.Add(WeaponNames[i], new { Standard = agd, Combo = agdc });
			}
			File.WriteAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\weaponAttackData.json", JsonConvert.SerializeObject(vals, Formatting.Indented));
		}

		//public Dictionary<string, WeaponAttack[]> FetchAll()
		//{
		//	return WeaponNames.Select((x, y) => new { Name = x, Attacks = Fetch(y) }).ToDictionary(x => x.Name, x => x.Attacks);
		//}

		//public WeaponAttack[] Fetch(int wp)
		//{

		//}
	}

	/// <summary>
	/// Combo descriptions and availability determinations. Links back to <see cref="ActionGuideData_Name"/> on <see cref="Action"/>.
	/// </summary>
	internal class ActionGuideData
	{
		[JsonProperty("_Index", NullValueHandling = NullValueHandling.Ignore)]
		public long? Index { get; set; }

		[JsonProperty("_UniqueIndex", NullValueHandling = NullValueHandling.Ignore)]
		public long? UniqueIndex { get; set; }

		///<summary>Action ID, pretty much. Links back to <see cref="ActionGuideData_Name"/>.</summary>
		[JsonProperty("_Action", NullValueHandling = NullValueHandling.Ignore)]
		public string? Action { get; set; }
		public ActionGuideData_Name? AGD_Name { get; set; }

		[JsonProperty("_TransitionAction", NullValueHandling = NullValueHandling.Ignore)]
		public string? TransitionAction { get; set; }

		[JsonProperty("_IsProgramCall", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsProgramCall { get; set; }

		[JsonProperty("_IsBeforeCharge", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsBeforeCharge { get; set; }

		[JsonProperty("_BeforeChargeAction", NullValueHandling = NullValueHandling.Ignore)]
		public string? BeforeChargeAction { get; set; }

		[JsonProperty("_ViewType", NullValueHandling = NullValueHandling.Ignore)]
		public string? ViewType { get; set; }

		[JsonProperty("_Input", NullValueHandling = NullValueHandling.Ignore)]
		public Input[]? Input { get; set; }

		[JsonProperty("_InputType", NullValueHandling = NullValueHandling.Ignore)]
		public string[]? InputType { get; set; }

		[JsonProperty("_LStickInput", NullValueHandling = NullValueHandling.Ignore)]
		public string? LStickInput { get; set; }

		[JsonProperty("_RStickInput", NullValueHandling = NullValueHandling.Ignore)]
		public string? RStickInput { get; set; }

		[JsonProperty("_MkbInput", NullValueHandling = NullValueHandling.Ignore)]
		public MkbInput[]? MkbInput { get; set; }

		[JsonProperty("_MkbInputType", NullValueHandling = NullValueHandling.Ignore)]
		public string[]? MkbInputType { get; set; }

		[JsonProperty("_UIInputType", NullValueHandling = NullValueHandling.Ignore)]
		public string[]? UiInputType { get; set; }

		[JsonProperty("_UIMkbInputType", NullValueHandling = NullValueHandling.Ignore)]
		public string[]? UiMkbInputType { get; set; }

		[JsonProperty("_MultiInputType", NullValueHandling = NullValueHandling.Ignore)]
		public string? MultiInputType { get; set; }
		static Dictionary<int, ActionGuideData[]> Cache = [];

		public static ActionGuideData[] Fetch(int wp)
		{
			if (!Cache.TryGetValue(wp, out ActionGuideData[]? value))
			{
				ActionGuideData[] ret = [.. JsonConvert.DeserializeObject<JArray>(File.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Player\ActionGuide\ActionGuideData_Wp{wp:D2}.user.3.json"))!.First()
					.Value<JObject>($"app.user_data.ActionGuideData_Wp{wp:D2}")!.Value<JArray>("_Values")!
					.Select(x => JsonConvert.DeserializeObject<ActionGuideData>(x.Value<JToken>($"app.user_data.ActionGuideData_Wp{wp:D2}.cData")!.ToString()))!];
				foreach (ActionGuideData agd in ret)
				{
					agd.AGD_Name = ActionGuideData_Name.Fetch(wp, agd.Action!);
				}

				value = [..ret.Where(x => x.AGD_Name != null)];
				Cache.Add(wp, value);
			}
			return value;
		}
	}

	/// <summary>
	/// Action visibility controls. Contains link to <see cref="ActionGuideDataMsg"/> on <see cref="ActionName"/> and <see cref="ActionGuideData"/> on <see cref="Action"/>.
	/// </summary>
	internal class ActionGuideData_Name
	{
		[JsonProperty("_Index", NullValueHandling = NullValueHandling.Ignore)]
		public long? Index { get; set; }

		///<summary>Action ID, pretty much. Links back to <see cref="ActionGuideData"/>.</summary>
		[JsonProperty("_Action", NullValueHandling = NullValueHandling.Ignore)]
		public string Action { get; set; } = string.Empty;

		///<summary>Name GUID. Links back to ActionGuideDataNameText.</summary>
		[JsonProperty("_ActionName", NullValueHandling = NullValueHandling.Ignore)]
		public Guid? ActionName { get; set; }
		public string? Name { get; set; }

		///<summary>Unsure. May not ever be different than <see cref="IsTrainingInvisible"/>.</summary>
		[JsonProperty("_IsInvisible", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsInvisible { get; set; }

		///<summary>Whether the move is visible in the training area (<see langword="false"/>) or not (<see langword="true"/>).</summary>
		[JsonProperty("_IsTrainingInvisible", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsTrainingInvisible { get; set; }

		///<summary>Determines whether this can be combo'd off of? I think.</summary>
		[JsonProperty("_IsComboEnd", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsComboEnd { get; set; }
		static Dictionary<int, ActionGuideData_Name[]> Cache = [];

		public static ActionGuideData_Name? Fetch(int wp, string action)
		{
			if (!Cache.TryGetValue(wp, out ActionGuideData_Name[]? value))
			{
				ActionGuideDataMsg[] msgs = ActionGuideDataMsg.Fetch(wp, ActionGuideDataMsgType.Name);
				ActionGuideData_Name[] vals = [.. JsonConvert.DeserializeObject<JArray>(File.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Player\ActionGuide\ActionGuideData_Wp{wp:D2}Name.user.3.json"))!.First()
					.Value<JObject>($"app.user_data.ActionGuideData_Wp{wp:D2}Name")!.Value<JArray>("_Values")!
					.Select(x => JsonConvert.DeserializeObject<ActionGuideData_Name>(x.Value<JToken>($"app.user_data.ActionGuideData_Wp{wp:D2}Name.cData")!.ToString())!)];
				foreach (ActionGuideData_Name agdn in vals)
				{
					agdn.Name = msgs.First(x => x.Guid == agdn.ActionName).Content![1];
				}

				value = vals;
				Cache.Add(wp, value);
			}
			try
			{
				return value.FirstOrDefault(x => x.Action == action);
			}
			catch (Exception)
			{
				Debugger.Break();
				return null;
			}
		}
	}

	/// <summary>
	/// Names of actions. Contains link to <see cref="ActionGuideData_Name"/> on <see cref="Guid"/>.
	/// </summary>
	internal class ActionGuideDataMsg
	{
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string? Name { get; set; }

		[JsonProperty("guid", NullValueHandling = NullValueHandling.Ignore)]
		public Guid? Guid { get; set; }

		[JsonProperty("crc?", NullValueHandling = NullValueHandling.Ignore)]
		public long? Crc { get; set; }

		[JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
		public long? Hash { get; set; }

		[JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
		public object[]? Attributes { get; set; }

		[JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
		public string[]? Content { get; set; }

		public static ActionGuideDataMsg[] Fetch(int wp, ActionGuideDataMsgType type)
		{
			return [.. JsonConvert.DeserializeObject<JObject>(File.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Action\ActionGuideData{(type == ActionGuideDataMsgType.Name ? "Name" : "Combo")}Text_Wp{wp:D2}.msg.23.json"))!.Value<JArray>("entries")!.Select(x => JsonConvert.DeserializeObject<ActionGuideDataMsg>(x.Value<JToken>()!.ToString()))!];
		}
	}

	/// <summary>
	/// Equivalent of <see cref="ActionGuideData"/> for specifically combos. I <i>think</i> it's just for the combos shown on the bottom left of the training area. 
	/// Contains link to <see cref="ActionGuideData_Combo"/> on <see cref="ComboId"/>.
	/// </summary>
	internal class ActionGuideData_ComboDetail
	{
		[JsonProperty("_Index", NullValueHandling = NullValueHandling.Ignore)]
		public long? Index { get; set; }

		///<summary>Links back to <see cref="ActionGuideData_Combo"/>.</summary>
		[JsonProperty("_ComboID", NullValueHandling = NullValueHandling.Ignore)]
		public long? ComboId { get; set; }
		public ActionGuideData_Combo? AGD_Combo { get; set; }

		[JsonProperty("_DetailNo", NullValueHandling = NullValueHandling.Ignore)]
		public long? DetailNo { get; set; }

		[JsonProperty("_IsSupportText", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsSupportText { get; set; }

		[JsonProperty("_IsSupportTextStart", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsSupportTextStart { get; set; }

		[JsonProperty("_SupportText", NullValueHandling = NullValueHandling.Ignore)]
		public Guid? SupportText { get; set; }

		[JsonProperty("_Input", NullValueHandling = NullValueHandling.Ignore)]
		public Input[]? Input { get; set; }

		[JsonProperty("_InputType", NullValueHandling = NullValueHandling.Ignore)]
		public string[]? InputType { get; set; }

		[JsonProperty("_LStickInput", NullValueHandling = NullValueHandling.Ignore)]
		public string? LStickInput { get; set; }

		[JsonProperty("_RStickInput", NullValueHandling = NullValueHandling.Ignore)]
		public string? RStickInput { get; set; }

		[JsonProperty("_MkbInput", NullValueHandling = NullValueHandling.Ignore)]
		public MkbInput[]? MkbInput { get; set; }

		[JsonProperty("_MkbInputType", NullValueHandling = NullValueHandling.Ignore)]
		public string[]? MkbInputType { get; set; }

		[JsonProperty("_UIInputType", NullValueHandling = NullValueHandling.Ignore)]
		public string[]? UiInputType { get; set; }

		[JsonProperty("_UIMkbInputType", NullValueHandling = NullValueHandling.Ignore)]
		public string[]? UiMkbInputType { get; set; }

		[JsonProperty("_MultiInputType", NullValueHandling = NullValueHandling.Ignore)]
		public string? MultiInputType { get; set; }
		static Dictionary<int, ActionGuideData_ComboDetail[]> Cache = [];

		public static ActionGuideData_ComboDetail[] Fetch(int wp)
		{
			if (!Cache.TryGetValue(wp, out ActionGuideData_ComboDetail[]? value))
			{
				ActionGuideData_ComboDetail[] ret = [.. JsonConvert.DeserializeObject<JArray>(File.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Player\ActionGuide\ActionGuideData_Wp{wp:D2}ComboDetail.user.3.json"))!.First().Value<JObject>($"app.user_data.ActionGuideData_Wp{wp:D2}ComboDetail")!.Value<JArray>("_Values")!.Select(x => JsonConvert.DeserializeObject<ActionGuideData_ComboDetail>(x.Value<JToken>($"app.user_data.ActionGuideData_Wp{wp:D2}ComboDetail.cData")!.ToString()!)!)];
				foreach (ActionGuideData_ComboDetail agd in ret)
				{
					agd.AGD_Combo = ActionGuideData_Combo.Fetch(wp, agd.ComboId);
				}
				value = [.. ret.Where(x => x.AGD_Combo != null)];
				Cache.Add(wp, [.. ret.Where(x => x.AGD_Combo != null)]);
			}
			return value;
		}
	}

	/// <summary>
	/// Names of combos. I <i>think</i> it's just for the combos shown on the bottom left of the training area. Contains link to <see cref="ActionGuideData_ComboDetail"/> on <see cref="ComboId"/> and 
	/// <see cref="ActionGuideDataMsg"/> on <see cref="ComboName"/>.
	/// </summary>
	internal class ActionGuideData_Combo
	{
		[JsonProperty("_Index", NullValueHandling = NullValueHandling.Ignore)]
		public long? Index { get; set; }

		///<summary>Links back to <see cref="ActionGuideData_ComboDetail"/>.</summary>
		[JsonProperty("_ComboID", NullValueHandling = NullValueHandling.Ignore)]
		public long? ComboId { get; set; }

		///<summary>Links back to <see cref="ActionGuideDataMsg"/>.</summary>
		[JsonProperty("_ComboName", NullValueHandling = NullValueHandling.Ignore)]
		public Guid? ComboName { get; set; }
		public string? Name { get; set; }
		static Dictionary<int, ActionGuideData_Combo[]> Cache = [];

		public static ActionGuideData_Combo? Fetch(int wp, long? comboId)
		{
			if (!Cache.TryGetValue(wp, out ActionGuideData_Combo[]? value))
			{
				ActionGuideDataMsg[] msgs = ActionGuideDataMsg.Fetch(wp, ActionGuideDataMsgType.Combo);
				ActionGuideData_Combo[] vals = [.. JsonConvert.DeserializeObject<JArray>(File.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Player\ActionGuide\ActionGuideData_Wp{wp:D2}Combo.user.3.json"))!.First().Value<JObject>($"app.user_data.ActionGuideData_Wp{wp:D2}Combo")!.Value<JArray>("_Values")!.Select(x => JsonConvert.DeserializeObject<ActionGuideData_Combo>(x.Value<JToken>($"app.user_data.ActionGuideData_Wp{wp:D2}Combo.cData")!.ToString())!)];
				foreach (ActionGuideData_Combo agdn in vals)
				{
					agdn.Name = msgs.First(x => x.Guid == agdn.ComboName).Content![1];
				}

				value = vals;
				Cache.Add(wp, value);
			}
			try
			{
				return value.FirstOrDefault(x => x.ComboId == comboId);
			}
			catch (Exception)
			{
				Debugger.Break();
				return null;
			}
		}
	}


	internal enum ActionGuideDataMsgType
	{
		Name,
		Combo
	}

	internal partial class Input
	{
		[JsonProperty("app.PlayerKey.PAD_Serializable", NullValueHandling = NullValueHandling.Ignore)]
		public AppPlayerKeySerializable? AppPlayerKeyPadSerializable { get; set; }
	}

	internal partial class AppPlayerKeySerializable
	{
		[JsonProperty("_Value", NullValueHandling = NullValueHandling.Ignore)]
		public string? Value { get; set; }
	}

	internal partial class MkbInput
	{
		[JsonProperty("app.PlayerKey.MKB_Serializable", NullValueHandling = NullValueHandling.Ignore)]
		public AppPlayerKeySerializable? AppPlayerKeyMkbSerializable { get; set; }
	}
}
