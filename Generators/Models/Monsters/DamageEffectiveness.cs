using MediawikiTranslator.Models.Data.MH3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediawikiTranslator.Models.Monsters
{
	public class DamageEffectiveness()
	{
		public int Order { get; set; } = 0;
		public required string PartName { get; set; }
		//only used in MHWilds or games where there's a kinsect essence for each PartsTable type
		public KinsectEssence? Essence { get; set; }
		public int SeverEffect { get; set; }
		public int BluntEffect { get; set; }
		public int BulletEffect { get; set; }

		//Tndr values are broken values for MHWilds; in MHWorld and MHWI, use GetMHWTender() to calc tenderized values
		public int SeverTndr { get; set; }
		public int BluntTndr { get; set; }
		public int BulletTndr { get; set; }

		//Elemental tndr values not used in MHWorld or MHWI
		public int FireEffect { get; set; }
		public int FireTndr { get; set; }
		public int WaterEffect { get; set; }
		public int WaterTndr { get; set; }
		public int ThunderEffect { get; set; }
		public int ThunderTndr { get; set; }
		public int IceEffect { get; set; }
		public int IceTndr { get; set; }
		public int DragonEffect { get; set; }
		public int DragonTndr { get; set; }
		public bool IsPhase { get; set; }
		public int StunEffect { get; set; }

		private static Dictionary<string, object> _partNames = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Data\EnemyPartsTypeName.msg.23.json"))!;

		public static DamageEffectiveness[] Generate(string name, Games game)
		{
			List<DamageEffectiveness> ret = [];
			try
			{
				if (game == Games.MHWI || game == Games.MHWorld)
				{
					ret.AddRange(HitZoneValues.GetHitZoneValues(name).Select(x => new DamageEffectiveness()
					{
						PartName = x.Name,
						BluntEffect = x.BluntEffect,
						BluntTndr = x.BluntTndr,
						BulletEffect = x.BulletEffect,
						BulletTndr = x.BulletTndr,
						SeverEffect = x.SeverEffect,
						SeverTndr = x.SeverTndr,
						DragonEffect = x.DragonEffect,
						IceEffect = x.IceEffect,
						WaterEffect = x.WaterEffect,
						FireEffect = x.FireEffect,
						ThunderEffect = x.ThunderEffect
					}));
				}
				else if (game == Games.MHWilds)
				{
					MonsterId monster = Monster.WildsMonsterIds.First(x => x.Name == name);
					JObject partDatas = (JObject)Attacks.MonsterMasterlist[monster.Id]["part_datas"];
					Dictionary<string, KinsectEssence> essences = new()
				{
					{ "NONE" , KinsectEssence.None },
					{ "RED", KinsectEssence.Red },
					{ "WHITE", KinsectEssence.White },
					{ "ORANGE", KinsectEssence.Orange },
					{ "GREEN", KinsectEssence.Green }
				};
					foreach (JObject part in partDatas.Value<JObject>("parts")!.Values().Where(x => x.GetType() != typeof(JArray)).Select(x => x.Value<JObject>()!).ToArray())
					{
						string partName = GetWildsPartName(part.Value<JObject>("_PartsType")!.Value<JObject>("app.EnemyDef.PARTS_TYPE_Serializable")!.Value<string>("_Value")!);
						if (string.IsNullOrEmpty(partName.Replace("'", "")))
						{
							partName = "???";
						}
						ret.Add(new()
						{
							PartName = partName,
							SeverEffect = part.Value<JObject>("hzv_normal")!.Value<int>("slash"),
							SeverTndr = part.Value<JObject>("hzv_break") == null ? -1 : part.Value<JObject>("hzv_break")!.Value<int>("slash"),
							BluntEffect = part.Value<JObject>("hzv_normal")!.Value<int>("blow"),
							BluntTndr = part.Value<JObject>("hzv_break") == null ? -1 : part.Value<JObject>("hzv_break")!.Value<int>("blow"),
							BulletEffect = part.Value<JObject>("hzv_normal")!.Value<int>("shot"),
							BulletTndr = part.Value<JObject>("hzv_break") == null ? -1 : part.Value<JObject>("hzv_break")!.Value<int>("shot"),
							WaterEffect = part.Value<JObject>("hzv_normal")!.Value<int>("water"),
							WaterTndr = part.Value<JObject>("hzv_break") == null ? -1 : part.Value<JObject>("hzv_break")!.Value<int>("water"),
							FireEffect = part.Value<JObject>("hzv_normal")!.Value<int>("fire"),
							FireTndr = part.Value<JObject>("hzv_break") == null ? -1 : part.Value<JObject>("hzv_break")!.Value<int>("fire"),
							IceEffect = part.Value<JObject>("hzv_normal")!.Value<int>("ice"),
							IceTndr = part.Value<JObject>("hzv_break") == null ? -1 : part.Value<JObject>("hzv_break")!.Value<int>("ice"),
							DragonEffect = part.Value<JObject>("hzv_normal")!.Value<int>("dragon"),
							DragonTndr = part.Value<JObject>("hzv_break") == null ? -1 : part.Value<JObject>("hzv_break")!.Value<int>("dragon"),
							ThunderEffect = part.Value<JObject>("hzv_normal")!.Value<int>("thunder"),
							ThunderTndr = part.Value<JObject>("hzv_break") == null ? -1 : part.Value<JObject>("hzv_break")!.Value<int>("thunder"),
							Essence = essences[string.Join("", part.Value<string>("_RodExtract")!.Where(char.IsLetter))]
						});
					}
				}
				int cntr = 0;
				foreach (DamageEffectiveness eff in ret)
				{
					eff.Order = cntr;
					cntr++;
				}
			}
			catch { }
			return [.. ret];
		}

		private static string GetWildsPartName(string partType)
		{
			if (partType.Contains("]"))
			{
				partType = partType.Substring(partType.IndexOf("[") + 1, partType.IndexOf("]") - partType.IndexOf("[") - 1);
			}
			dynamic[] entries = Extensions.ToDynamic((JArray)_partNames["entries"]);
			foreach (dynamic entry in entries)
			{
				if (entry.name.ToString() == $"EnemyPartsTypeName_{partType.Replace("-", "m")}")
				{
					return entry.content[1].ToString();
				}
			}
			return "???";
		}
	}
}
