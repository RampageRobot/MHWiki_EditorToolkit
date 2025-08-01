using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Data.MHWI
{
    class Quests
	{
		public int Id { get; set; }
		public string? Game { get; set; }
		public string? Format { get; set; }
		public string? Rank { get; set; }
		public int RankLevel { get; set; }
		public string? QuestName { get; set; }
		public string? TimePeriod { get; set; }
		public string? QuestType { get; set; }
		public string?[] TargetMonsters { get; set; } = [];
		public string?[] MonsterSpecialStates { get; set; } = [];
		public int[] MonsterAmounts { get; set; } = [];
		public string? DeliveryItemName { get; set; }
		public string? DeliveryItemType { get; set; }
		public string? DeliveryItemColor { get; set; }
		public int? DeliveryItemAmount { get; set; }
		public string? SpecialQuestSummary { get; set; }
		public string? Requirements { get; set; }
		public int ContractFee { get; set; }
		public string? Locale { get; set; }
		public string? EnvironmentDetails { get; set; }
		public string? Season { get; set; }
		public string? SubQuest1 { get; set; }
		public string? SubQuest2 { get; set; }
		public string? SubQuest3 { get; set; }
		public int TimeLimit { get; set; }
		public string? FailureConditions { get; set; }
		public string? RewardMoney { get; set; }
		public string? SpecialRewards { get; set; }
		public string? Notes { get; set; }
		public string?[] OtherMonstersLarge { get; set; } = [];
		public string?[] OtherMonstersSmall { get; set; } = [];
		public string? Client { get; set; }
		public string? Description { get; set; }

		public static Quests[] FetchQuests()
		{
			return JsonConvert.DeserializeObject<Quests[]>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\quest data\questDict.json"))!;
		}
	}
}
