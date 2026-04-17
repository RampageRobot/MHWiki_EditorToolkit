using MediawikiTranslator.Models.Data;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MediawikiTranslator.Models.Monsters
{
	public class LargeMonsterPage
	{
		public LargeMonsterPage(string name, Games game, MonsterSize size) 
		{
			MonsterName = name;
			Game = game;
			Size = size;
			HuntersNotes = new HuntersNotes(MonsterName, Game);
			PartsTable = Game == Games.MHWilds ? null : [];
			Enrage = Game == Games.MHWI || Game == Games.MHWorld ? new Enrage(MonsterName) : null;
			Stamina = Game == Games.MHWI || Game == Games.MHWorld ? new Stamina(MonsterName) : null;
			Equipment = new SimpleEquipment(MonsterName, Game);
			Attacks = [..Monsters.Attacks.FetchAttacks(name,  game)];
			InfoBox = new InfoBox(name, game);
			DamageEffectiveness = [.. Monsters.DamageEffectiveness.Generate(name, game)];
			PartsTable = game == Games.MHWilds ? null : Monsters.PartsTable.Get(name);
			ItemEffectiveness = [.. TrapEffectiveness.GetTraps(name, game)];
			StatusEffectiveness = [.. Monsters.StatusEffectiveness.GetStatuses(name, game)];
			if (Game == Games.MHWI || Game == Games.MHWorld)
			{
				DropRates = [..Drops.Fetch(Game, MonsterName, Monster.GetAvailableRanks(MonsterName), [.. HitZoneValues.GetHitZoneValues(MonsterName).Select(x => x.Name)])];
			}
			else
			{
				DropRates = [.. Drops.Fetch(Game, MonsterName)];
			}
			Quests = Monsters.Quests.FetchQuests(game, MonsterName);
		}

		public static LargeMonsterPage[] GetAll(Games game)
		{
			Dictionary<string, MonsterData> srcDict = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
			return [.. srcDict.Where(x => x.Value.LargeMonster && x.Value.GameAppearances.Any(y => y.GameAcronym == game.ToAcronymString()))
				.Select(x => new LargeMonsterPage(x.Key, game, MonsterSize.Large))];
		}

		public string MonsterName { get; set; }
		public Games Game { get; set; }
		public MonsterSize Size { get; set; }
		public MaterialsAndDropTables.WebToolkitData[]? DropRates { get; set; }
		public InfoBox InfoBox { get; set; }
		public HuntersNotes HuntersNotes { get; set; }
		public List<PartsTable>? PartsTable { get; set; }
		public List<DamageEffectiveness> DamageEffectiveness { get; set; } = [];
		public List<TrapEffectiveness> ItemEffectiveness { get; set; } = [];
		public List<StatusEffectiveness> StatusEffectiveness { get; set; } = [];
		public List<Attacks> Attacks { get; set; } = [];
		public Enrage? Enrage { get; set; }
		public Stamina? Stamina { get; set; }
		public Quests[]? Quests { get; set; }
		public SimpleEquipment Equipment { get; set; }
	}

	public enum MonsterSize
	{
		Small,
		Large
	}

	public enum Games
	{
		MH1,
		MHG,
		MHF1,
		MH2,
		MHF2,
		MHFU,
		MH3,
		MHP3,
		MH3U,
		MH4,
		MH4U,
		MHGen,
		MHGU,
		MHWorld,
		MHWI,
		MHRise,
		MHRS,
		MHWilds,
		MHNow,
		MHOutlanders,
		MHPuzzles,
		MHST1,
		MHST2,
		MHST3,
		MHFrontier,
		MHOnline,
		MHExplore,
		MHi,
		MHRiders,
		MHDiary,
		MHDG,
		MHDDX,
		MHBGHQ,
		MHDH,
		MHMH,
		MHPIV,
		MHSpirits
	}

	public static class GamesExtensions
	{
		public static Games ToGamesEnum(string game)
		{
			return new Dictionary<string, Games>()
			{
				{ "MH1", Games.MH1 },
				{ "MHG", Games.MHG },
				{ "MHF1", Games.MHF1 },
				{ "MH2", Games.MH2 },
				{ "MHF2", Games.MHF2 },
				{ "MHFU", Games.MHFU },
				{ "MH3", Games.MH3 },
				{ "MHP3", Games.MHP3 },
				{ "MH3U", Games.MH3U },
				{ "MH4", Games.MH4 },
				{ "MH4U", Games.MH4U },
				{ "MHGen", Games.MHGen },
				{ "MHGU", Games.MHGU },
				{ "MHWorld", Games.MHWorld },
				{ "MHWI", Games.MHWI },
				{ "MHRise", Games.MHRise },
				{ "MHRS", Games.MHRS },
				{ "MHWilds", Games.MHWilds },
				{ "MHNow", Games.MHNow },
				{ "MHOutlanders", Games.MHOutlanders },
				{ "MHPuzzles", Games.MHPuzzles },
				{ "MHST1", Games.MHST1 },
				{ "MHST2", Games.MHST2 },
				{ "MHST3", Games.MHST3 },
				{ "MHFrontier", Games.MHFrontier },
				{ "MHOnline", Games.MHOnline },
				{ "MHExplore", Games.MHExplore },
				{ "MHi", Games.MHi },
				{ "MHRiders", Games.MHRiders },
				{ "MHDiary", Games.MHDiary },
				{ "MHDG", Games.MHDG },
				{ "MHDDX", Games.MHDDX },
				{ "MHBGHQ", Games.MHBGHQ },
				{ "MHDH", Games.MHDH },
				{ "MHMH", Games.MHMH },
				{ "MHPIV", Games.MHPIV },
				{ "MHSpirits", Games.MHSpirits }
			}[game];
		}
		public static string ToAcronymString(this Games game)
		{
			return new Dictionary<Games, string>()
			{
				{ Games.MH1, "MH1" },
				{ Games.MHG, "MHG" },
				{ Games.MHF1, "MHF1" },
				{ Games.MH2, "MH2" },
				{ Games.MHF2, "MHF2" },
				{ Games.MHFU, "MHFU" },
				{ Games.MH3, "MH3" },
				{ Games.MHP3, "MHP3" },
				{ Games.MH3U, "MH3U" },
				{ Games.MH4, "MH4" },
				{ Games.MH4U, "MH4U" },
				{ Games.MHGen, "MHGen" },
				{ Games.MHGU, "MHGU" },
				{ Games.MHWorld, "MHWorld" },
				{ Games.MHWI, "MHWI" },
				{ Games.MHRise, "MHRise" },
				{ Games.MHRS, "MHRS" },
				{ Games.MHWilds, "MHWilds" },
				{ Games.MHNow, "MHNow" },
				{ Games.MHOutlanders, "MHOutlanders" },
				{ Games.MHPuzzles, "MHPuzzles" },
				{ Games.MHST1, "MHST1" },
				{ Games.MHST2, "MHST2" },
				{ Games.MHST3, "MHST3" },
				{ Games.MHFrontier, "MHFrontier" },
				{ Games.MHOnline, "MHOnline" },
				{ Games.MHExplore, "MHExplore" },
				{ Games.MHi, "MHi" },
				{ Games.MHRiders, "MHRiders" },
				{ Games.MHDiary, "MHDiary" },
				{ Games.MHDG, "MHDG" },
				{ Games.MHDDX, "MHDDX" },
				{ Games.MHBGHQ, "MHBGHQ" },
				{ Games.MHDH, "MHDH" },
				{ Games.MHMH, "MHMH" },
				{ Games.MHPIV, "MHPIV" },
				{ Games.MHSpirits, "MHSpirits" }
			}[game];
		}
		public static string ToFriendlyString(this Games game)
		{
			return new Dictionary<Games, string>()
			{
				{ Games.MH1, "Monster Hunter" },
				{ Games.MHG, "Monster Hunter G" },
				{ Games.MHF1, "Monster Hunter Freedom" },
				{ Games.MH2, "Monster Hunter 2" },
				{ Games.MHF2, "Monster Hunter Freedom 2" },
				{ Games.MHFU, "Monster Hunter Freedom Unite" },
				{ Games.MH3, "Monster Hunter 3" },
				{ Games.MHP3, "Monster Hunter Portable 3rd" },
				{ Games.MH3U, "Monster Hunter 3 Ultimate" },
				{ Games.MH4, "Monster Hunter 4" },
				{ Games.MH4U, "Monster Hunter 4 Ultimate" },
				{ Games.MHGen, "Monster Hunter Generations" },
				{ Games.MHGU, "Monster Hunter Generations Ultimate" },
				{ Games.MHWorld, "Monster Hunter: World" },
				{ Games.MHWI, "Monster Hunter World: Iceborne" },
				{ Games.MHRise, "Monster Hunter Rise" },
				{ Games.MHRS, "Monster Hunter Rise: Sunbreak" },
				{ Games.MHWilds, "Monster Hunter Wilds" },
				{ Games.MHNow, "Monster Hunter Now" },
				{ Games.MHOutlanders, "Monster Hunter Outlanders" },
				{ Games.MHPuzzles, "Monster Hunter Puzzles: Felyne Isles" },
				{ Games.MHST1, "Monster Hunter Stories" },
				{ Games.MHST2, "Monster Hunter Stories 2: Wings of Ruin" },
				{ Games.MHST3, "Monster Hunter Stories 3: Twisted Reflection" },
				{ Games.MHFrontier, "Monster Hunter Frontier" },
				{ Games.MHOnline, "Monster Hunter Online" },
				{ Games.MHExplore, "Monster Hunter Explore" },
				{ Games.MHi, "Monster Hunter i" },
				{ Games.MHRiders, "Monster Hunter Riders" },
				{ Games.MHDiary, "Monster_Hunter_Diary: Poka Poka Airou Village" },
				{ Games.MHDG, "Monster_Hunter_Diary: Poka Poka Airou Village G" },
				{ Games.MHDDX, "Monster_Hunter_Diary: Poka Poka Airou Village Deluxe" },
				{ Games.MHBGHQ, "Monster Hunter Big Game Hunting Quest" },
				{ Games.MHDH, "Monster Hunter Dynamic Hunting" },
				{ Games.MHMH, "Monster Hunter Massive Hunting" },
				{ Games.MHPIV, "Monster Hunter: Phantom Island Voyage" },
				{ Games.MHSpirits, "Monster Hunter Spirits" }
			}[game];
		}
	}
}
