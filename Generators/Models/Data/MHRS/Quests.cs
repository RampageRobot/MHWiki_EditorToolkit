using MediawikiTranslator.Models.Data.MH3;
using MediawikiTranslator.Models.Monsters;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace MediawikiTranslator.Models.Data.MHRS
{

	public partial class Quests
	{
		public long? QuestId { get; set; }
		public QuestText? QuestText { get; set; }
		public QuestData? QuestData { get; set; }
		public EnemyData? EnemyData { get; set; }
		public object? RampageData { get; set; }

		public static Quests[] Fetch()
		{
			JArray monsterIds = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\fexty monster ids.json"))!;
			Quests[] src = [..Directory.EnumerateFiles(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\Quests", "q*.json")
				.Select(x => Newtonsoft.Json.JsonConvert.DeserializeObject<Quests>(File.ReadAllText(x))!)];
			foreach (Quests q in src)
			{
				foreach (QuestDataMonster monster in q.QuestData!.Monsters!)
				{
					monster.Name = monsterIds.First(x => x.Value<int>("Id")! == monster.Id).Value<string>("Name")!;
				}
			}
			return src;
		}

		public static Models.Quests.WebToolkitData[] GetWebToolkitData()
		{
			Items[] allItems = Utilities.GetMHRSItems();
			JArray itemIds = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\fexty item ids.json"))!;
			List<Models.Quests.WebToolkitData> ret = [];
			Quests[] src = Fetch();
			Dictionary<long, string> locales = new()
			{ 
				{ 1, "Shrine Ruins" },
				{ 2, "Sandy Plains" },
				{ 3, "Flooded Forest" },
				{ 4, "Frost Caverns" },
				{ 5, "Lava Caverns" },
				{ 7, "Red Stronghold" },
				{ 9, "Infernal Springs" },
				{ 10, "Arena" },
				{ 11, "Coral Palace" },
				{ 12, "Jungle" },
				{ 13, "Citadel" },
				{ 14, "Forlorn Arena" },
				{ 15, "Yawning Abyss" },
			};
			Dictionary<long, string> huntTypes = new()
			{
				{ 0, "None" },
				{ 1, "Gather" },
				{ 2, "Hunt" },
				{ 3, "Kill" },
				{ 4, "Capture" },
				{ 5, "AllMainEnemy" },
				{ 6, "Hunt-a-thon" },
				{ 7, "FinalBarrierDefense" },
				{ 8, "FortLevelUp" },
				{ 9, "PlayerDown" },
				{ 10, "FinalBoss" },
				{ 11, "HuntingMachine" },
				{ 12, "DropItem" },
				{ 13, "EmStun" },
				{ 14, "EmElement" },
				{ 15, "EmCondition" },
				{ 16, "EmCnt_Weapon" },
				{ 17, "EmCnt_HmBallista" },
				{ 18, "EmCnt_HmCannon" },
				{ 19, "EmCnt_HmGatling" },
				{ 20, "EmCnt_HmTrap" },
				{ 21, "EmCnt_HmFlameThrower" },
				{ 22, "EmCnt_HmNpc" },
				{ 23, "EmCnt_HmDragnator" },
				{ 24, "ExtraEmRunaway" }
			};
			//0 is village, LR
			Dictionary<long, string> ranks = new()
			{
				{ 0, "Low Rank" },
				{ 1, "Low Rank" },
				{ 2, "High Rank" },
				{ 3, "Master Rank" },
			};
			Dictionary<long, string> requirements = new()
			{
				{ 0, "None" },
				{ 1, "2 Player" },
				{ 2, "HR2+" },
				{ 3, "HR3+" },
				{ 4, "HR4+" },
				{ 5, "HR5+" },
				{ 6, "HR6+" },
				{ 7, "HR7+" },
				{ 8, "HR8+" },
				{ 9, "HR20+" },
				{ 10, "HR30+" },
				{ 11, "HR40+" },
				{ 12, "HR45+" },
				{ 13, "HR50+" },
				{ 14, "HR90+" },
				{ 15, "HR100+" },
				{ 16, "MR1+" },
				{ 17, "MR2+" },
				{ 18, "MR3+" },
				{ 19, "MR4+" },
				{ 20, "MR5+" },
				{ 21, "MR6+" },
				{ 22, "MR10+" },
				{ 23, "MR20+" },
				{ 24, "MR30+" },
				{ 25, "MR40+" },
				{ 26, "MR50+" },
				{ 27, "MR60+" },
				{ 28, "MR100+" },
				{ 29, "1 Player" }
			};
			foreach (Quests q in src.Where(x => locales.ContainsKey(x.QuestData!.Map!.Value)))
			{
				QuestInfo engQuestText = q.QuestText!.QuestInfo!.First(x => x.Language == "ENG");
				Models.Quests.WebToolkitData retQ = new()
				{
					Client = engQuestText.Client,
					Description = engQuestText.Description,
					QuestName = engQuestText.Name,
					TimeLimit = (int)q.QuestData!.TimeLimit!.Value,
					Locale = locales[q.QuestData!.Map!.Value],
					RewardMoney = q.QuestData!.Reward!.Zenny!.Value.ToString(),
					QuestType = q.QuestData!.QuestType!.Value > 4 ? "Hunt" : huntTypes[q.QuestData!.QuestType!.Value],
					Rank = ranks[q.QuestData.EnemyLevel!.Value],
					Requirements = string.Join(", ", q.QuestData!.QuestConditions!.Select(x => requirements[x])),
					SpecialRewards = $"HR Points: {q.QuestData!.Reward.Hrp!.Value}; Kamura Points: {q.QuestData!.Reward!.Points}",
					Game = "MHRS",
					FailureConditions = engQuestText.Fail!,
					RankLevel = (int)q.QuestData.QuestLevel!.Value!,
					OtherMonstersLarge = [.. q.QuestData!.Monsters!.Where(x => x.Id < 4000 && !q.QuestData!.TargetMonsters!.Any(y => y == x.Id)).Select(x => x.Name)],
					OtherMonstersSmall = [.. q.QuestData!.Monsters!.Where(x => x.Id > 4000 && !q.QuestData!.TargetMonsters!.Any(y => y == x.Id)).Select(x => x.Name)]
				};
				//Any target items that aren't in item id NONE
				if (q.QuestData!.TargetItemIds!.Any(x => x != 67108864))
				{
					retQ.DeliveryItemName = q.QuestData!.TargetItemIds!.Select(x => itemIds.First(y => y.Value<long>("Id") == x)!.Value<string>("Name")).First();
					retQ.DeliveryItemAmount = (int)q.QuestData!.TargetAmounts!.First();
					Items item = allItems.First(x => x.Name == retQ.DeliveryItemName);
					retQ.DeliveryItemColor = item.WikiIconColor;
					retQ.DeliveryItemType = item.WikiIconName;
				}
				else
				{
					retQ.MonsterAmounts = [.. q.QuestData!.TargetAmounts!.Select(x => (int)x)];
					List<string> monsters = [];
					List<string> states = [];
					for (int i = 0; i < q.QuestData!.Monsters!.Length; i++)
					{
						QuestDataMonster mon = q.QuestData!.Monsters![i];
						if (q.QuestData!.TargetMonsters!.Any(x => x == mon.Id))
						{
							monsters.Add(mon.Name);
							long indType = q.EnemyData!.Monsters![i].IndividualType!.Value;
							states.Add(indType == 1 ? "Afflicted" : indType > 1 ? "Risen" : "");
						}
					}
					retQ.TargetMonsters = [..monsters];
					retQ.MonsterSpecialStates = [..states];
				}
				ret.Add(retQ);
			}
			return [.. ret];
		}
	}

	public partial class EnemyData
	{
		public SmallMonsters? SmallMonsters { get; set; }
		public EnemyDataMonster[]? Monsters { get; set; }
	}

	public partial class EnemyDataMonster
	{
		public long? PathId { get; set; }
		public long? PartTable { get; set; }
		public string? SetName { get; set; }
		public long? SubType { get; set; }
		public long? HealthTable { get; set; }
		public long? AttackTable { get; set; }
		public long? OtherTable { get; set; }
		public long? StaminaTable { get; set; }
		public long? Size { get; set; }
		public long? SizeTable { get; set; }
		public long? Difficulty { get; set; }
		public long? MultiTable { get; set; }
		public long? IndividualType { get; set; }
	}

	public partial class SmallMonsters
	{
		public long? SpawnType { get; set; }
		public long? HealthTable { get; set; }
		public long? AttackTable { get; set; }
		public long? PartTable { get; set; }
		public long? OtherTable { get; set; }
		public long? MultiTable { get; set; }
	}

	public partial class QuestData
	{
		public long? QuestType { get; set; }
		public long? QuestLevel { get; set; }
		public long? EnemyLevel { get; set; }
		public long? Map { get; set; }
		public long? BaseTime { get; set; }
		public long? TimeVariation { get; set; }
		public long? TimeLimit { get; set; }
		public long? Carts { get; set; }
		public long[]? QuestConditions { get; set; }
		public long[]? TargetTypes { get; set; }
		public long[]? TargetMonsters { get; set; }
		public long[]? TargetItemIds { get; set; }
		public long[]? TargetAmounts { get; set; }
		public QuestDataMonster[]? Monsters { get; set; }
		public long? ExtraMonsterCount { get; set; }
		public bool? SwapExitRide { get; set; }
		public long[]? SwapFrequencies { get; set; }
		public long[]? SwapConditions { get; set; }
		public long[]? SwapParams { get; set; }
		public long[]? SwapExitTimes { get; set; }
		public long? SwapStopType { get; set; }
		public long? SwapStopParam { get; set; }
		public long? SwapExecType { get; set; }
		public Reward? Reward { get; set; }
		public long? SupplyTable { get; set; }
		public long[]? Icons { get; set; }
		public bool? Tutorial { get; set; }
		public bool? FromNpc { get; set; }
		public ArenaParam? ArenaParam { get; set; }
		public long? AutoMatchHr { get; set; }
		public long? BattleBgmType { get; set; }
		public long? ClearBgmType { get; set; }
	}

	public partial class ArenaParam
	{
		public bool? FenceDefaultActive { get; set; }
		public long? FenceUptime { get; set; }
		public long? FenceInitialDelay { get; set; }
		public long? FenceCooldown { get; set; }
		public bool[]? Pillars { get; set; }
	}

	public partial class QuestDataMonster
	{
		public long? Id { get; set; }
		public long? SpawnCondition { get; set; }
		public long? SpawnParam { get; set; }
		public string Name { get; set; } = string.Empty;
	}

	public partial class Reward
	{
		public long? Zenny { get; set; }
		public long? Points { get; set; }
		public long? Hrp { get; set; }
	}

	public partial class QuestText
	{
		public QuestInfo[]? QuestInfo { get; set; }
		public string? FallbackLanguage { get; set; }
		public string? DebugName { get; set; }
		public string? DebugClient { get; set; }
		public string? DebugDescription { get; set; }
	}

	public partial class QuestInfo
	{
		public string? Language { get; set; }
		public string? Name { get; set; }
		public string? Client { get; set; }
		public string? Description { get; set; }
		public string? Target { get; set; }
		public string? Fail { get; set; }
	}
}
