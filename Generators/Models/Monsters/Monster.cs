using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    public class Monster(string name, string game)
	{
		public string Name { get; set; } = name;
		public Attacks[] Attacks { get; set; } = [];
		public Enrage Enrage { get; set; } = new(name);
		public Stamina Stamina { get; set; } = new(name);
		public StatusEffectiveness[] StatusEffectiveness { get; set; } = Monsters.StatusEffectiveness.GetStatuses(name);
		public TrapEffectiveness[] TrapEffectiveness { get; set; } = Monsters.TrapEffectiveness.GetTraps(name);
		public HitZoneValues[] HitZoneValues { get; set; } = Monsters.HitZoneValues.GetHitZoneValues(name);
        public FlinchBreakThresholds[] FlinchBreakThresholds { get; set; } = Monsters.FlinchBreakThresholds.GetFlinchBreakThresholds(name);
		public CrownSizes CrownSizes { get; set; } = new(name);
		public string Drops { get; set; } = Monsters.Drops.Format(name, GetAvailableRanks(name), [.. Monsters.HitZoneValues.GetHitZoneValues(name).Select(x => x.Name)]);
		public OtherLanguages OtherLanguages = new(name);
		public Quests[] Quests = Monsters.Quests.FetchQuests(name);
		public Equipment Equipment = new(name, game);

		private static readonly Dictionary<string, dynamic> QuestInfo = Utilities.GetMHWIQuestInfo();

		public static MonsterId[] WildsMonsterIds = JsonConvert.DeserializeObject<MonsterId[]>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\monsterIds.json"))!;

		public string Format()
		{
			StringBuilder sb = new();
			sb.AppendLine(CrownSizes.Format());
			sb.AppendLine(@"==Physical Attributes==
<div style=""display: flex; flex-wrap: wrap;"">
<div style=""flex: 1 1 33%; margin-right:5px;"">");
			sb.AppendLine(Monsters.FlinchBreakThresholds.Format(FlinchBreakThresholds));
			sb.AppendLine(@"</div>
<div style=""flex: 1 1 66%; margin-left:5px;"">");
			sb.AppendLine(Monsters.HitZoneValues.Format(HitZoneValues));
			sb.AppendLine(@"{{UserHelpBox|
*'''Part''' - the name of the part. Any special states or conditions are shown in parentheses. 
*[[File:MHWI-Great_Sword_Icon_Rare_0.png|24x24px]][[File:MHWI-Hammer_Icon_Rare_0.png|24x24px]][[File:MHWI-Ammo_Icon_White.png|24x24px]] - the effectiveness of '''Cutting''', '''Blunt''', and '''Shot''' raw attack types respectively. For example, if a part has a hitzone value of 40 for Cutting, it means that any Cutting damage dealt to it is reduced by 60%. Hitzone values greater than or equal to '''45''' are considered weak spots by the game, displaying orange damage numbers and activating [[Weakness Exploit (MHWI)|Weakness Exploit]]. These numbers are given in '''bold'''. Tenderized values are given in Purple.
*{{UI|UI|Fire}}{{UI|UI|Water}}{{UI|UI|Thunder}}{{UI|UI|Ice}}{{UI|UI|Dragon}} - the effectiveness of each elemental damage type. For example, if a part has a hitzone value of 25 for Thunder, it means that any Thunder damage dealt to it is reduced by 75%.
*{{UI|UI|Stun}} - the effectiveness of [[Stun]] against the specified part. For example, a value of 50 means that only 50% of all stun buildup dealt to that part contributes to the monster's stun threshold. A <code>-</code> means that hitting the part with an attack will never contribute to stun buildup, regardless of its properties.
}}
</div>
</div>");
			sb.AppendLine("<div class=\"twocol\">\r\n<div>");
			sb.AppendLine(Monsters.TrapEffectiveness.Format("MHWI", TrapEffectiveness, Name));
			sb.AppendLine("</div>\r\n<div>");
			sb.AppendLine(Monsters.StatusEffectiveness.Format(StatusEffectiveness, Name));
			sb.AppendLine("</div>\r\n</div>");
			sb.AppendLine("==Enrage and Fatigue==\r\n<div class=\"twocol\">\r\n<div>");
			sb.AppendLine(Enrage.Format());
			sb.AppendLine("</div>\r\n<div>");
			sb.AppendLine(Stamina.Format());
			sb.AppendLine("</div>\r\n</div>");
			sb.AppendLine(Drops);
			sb.AppendLine(Monsters.Quests.Format(Quests, Name));
			sb.AppendLine(Equipment.Format());
			return sb.ToString();
		}

		private static int[] GetAvailableRanks(string name)
		{
			List<int> ranksAvailable = [];
			if (QuestInfo.Any(x => x.Value.Rank == "Low Rank"))
			{
				ranksAvailable.Add(0);
			}
			if (QuestInfo.Any(x => x.Value.Rank == "High Rank"))
			{
				ranksAvailable.Add(1);
			}
			if (QuestInfo.Any(x => x.Value.Rank == "Master Rank"))
			{
				ranksAvailable.Add(2);
			}
			return [.. ranksAvailable];
		}
	}
	public class MonsterId
	{
		public string Id { get; set; } = string.Empty;
		public string IDNum { get; set; } = string.Empty;
		public string IDFix { get; set; } = string.Empty;
		public string IDBit { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string BossIcon { get; set; } = string.Empty;
		public string ZakoIcon { get; set; } = string.Empty;
		public string ExtraName { get; set; } = string.Empty;
		public string FrenzyName { get; set; } = string.Empty;
		public string LegendaryName { get; set; } = string.Empty;
		public string MonsterType
		{
			get
			{
				return Id.StartsWith("EM5") ? "Endemic" : Convert.ToInt32(Id.Substring(Id.IndexOf("EM") + 2, 4)) < 165 ? "Large" : "Small";
			}
		}
	}
}
