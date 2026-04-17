using MediawikiTranslator.Models.Monsters;
using Newtonsoft.Json;

namespace MediawikiTranslator.Models.Quests
{
	public class WebToolkitData
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

		public static WebToolkitData[] Fetch(Games game)
		{
			WebToolkitData[] ret = [];
			if (game == Games.MHWI || game == Games.MHWorld)
			{
				ret = Data.MHWI.Quests.FetchQuests();
				if (game == Games.MHWorld)
				{
					ret = [.. ret.Where(x => x.Rank != null && x.Rank != "Master" && x.Rank != "Master Rank")];
				}
			}
			else if (game == Games.MHRS || game == Games.MHRise)
			{
				WebToolkitData[] q = Data.MHRS.Quests.GetWebToolkitData();
				if (game == Games.MHRise)
				{
					ret = [.. ret.Where(x => x.Rank != null && x.Rank != "Master" && x.Rank != "Master Rank")];
				}
			}
			else if (game == Games.MHWilds)
			{

			}
			return ret;
		}
	}
}
