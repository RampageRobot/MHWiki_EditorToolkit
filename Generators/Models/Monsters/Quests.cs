using Newtonsoft.Json;
using System.Text;

namespace MediawikiTranslator.Models.Monsters
{
    public class Quests
    {
        public int Order { get; set; } = 0;
        public int Id { get; set; }
        public string Rank { get; set; } = string.Empty;
        public int Stars { get; set; }
        public string ObjectiveType { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Locale { get; set; } = string.Empty;
        public string[] ObjectiveMonsters { get; set; } = [];
		public string[] MonstersInZone { get; set; } = [];
        public bool[] IsTempered { get; set; } = [];
		public bool QuestIsAT { get; set; }

        public static Quests[] FetchQuests(Games game = Games.MHWI, string? monsterName = null)
        {
            if (game == Games.MHWI || game == Games.MHWorld)
            {
				Dictionary<string, Quests> questInfo = JsonConvert.DeserializeObject<Dictionary<string, Quests>>(Utilities.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\Monster Data\questInfo.json"))!;
                if (game == Games.MHWorld)
                {
                    questInfo = questInfo.Where(x => x.Value.Rank != "Master Rank").ToDictionary(x => x.Key, x => x.Value);
                }
                Quests[] allQuestsForMonster = [..questInfo.Where(x => x.Value.ObjectiveMonsters.Any(y => monsterName == null || y == monsterName)).Select(x => x.Value)];
                int cntr = 0;
                foreach (Quests q in allQuestsForMonster)
                {
                    q.Order = cntr;
                    cntr++;
                }
				return allQuestsForMonster;
			}
            else if (game == Games.MHRS || game == Games.MHRise)
            {
                Models.Quests.WebToolkitData[] src = Data.MHRS.Quests.GetWebToolkitData();
                if (game == Games.MHRise)
                {
                    src = [.. src.Where(x => x.Rank != "Master Rank")];
                }
                return [.. src.Where(x => x.TargetMonsters!.Any(y => y == monsterName))
                    .OrderBy(x => x.RankLevel)
                    .Select((x, y) => new Quests() {
                        Locale = x.Locale!,
                        QuestIsAT = false,
                        Id = x.Id,
                        MonstersInZone = x.OtherMonstersLarge!,
                        Name = x.QuestName!,
                        IsTempered = [..x.TargetMonsters.Select(x => false)],
                        ObjectiveMonsters = x.TargetMonsters!,
                        Order = y + 1,
                        ObjectiveType = x.QuestType!,
                        Rank = x.Rank!,
                        Stars = x.RankLevel!
                    })];
            }
            else
            {
                return [];
            }
        }

		public static string Format(Quests[] quests, string monsterName)
        {
            Dictionary<string, string>[] objInfo = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(Utilities.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\Monster DataquestObjectiveTypes.json"))!;
            StringBuilder sb = new();
            sb.AppendLine(@"==Quests Targeting This Monster==
{| class=""wikitable mobile-sm sortable"" style=""text-align:center;margin:auto""
!Rank
!Type
!Name
!Locale
!Targets
|-");
            string[] elderDragons = ["Behemoth", "Kirin", "Kulve Taroth", "Kushala Daora", "Lunastra", "Nergigante", "Teostra", "Vaal Hazak", "Xeno'jiiva", "Zorah Magdaros", "Alatreon", "Namielle", "Ruiner Nergigante", "Safi'jiiva", "Shara Ishvalda", "Blackveil Vaal Hazak", "Velkhana", "Fatalis"];
            foreach (Quests quest in quests.OrderBy(x => x.Stars + (x.Rank == "Master Rank" ? 10 : 0)))
            {
                string rankAbbr = quest.Rank == "Low Rank" ? "LR" : quest.Rank == "High Rank" ? "HR" : "MR";
                string objectiveIconType = quest.ObjectiveType;
                if (quest.ObjectiveType == "Hunt (Multi)")
                {
                    if (!quest.ObjectiveMonsters.Any(x => !elderDragons.Contains(x)))
                    {
                        objectiveIconType = "Slay";
                    }
                    else
                    {
                        objectiveIconType = "Hunt";
                    }
                }
                string assignmentType = objInfo.First(x => quest.Id >= Convert.ToInt32(x["ID >="]) && quest.Id <= Convert.ToInt32(x["ID <="]))["Type"];
				string[] allObjectives = [..quest.ObjectiveMonsters.Select((x, y) => $"[[File:MHWI-{(quest.IsTempered[y] ? (quest.QuestIsAT ? "Arch " : "") + "Tempered " : "")}{x.Replace("[s] ", "")} Icon.png|frameless|32x32px|link={x.Replace("[s] ", "")}/MHWI]]")];
                sb.AppendLine($@"|data-sort-value=""{quest.Stars + (rankAbbr == "MR" ? 10 : 0)}""|{rankAbbr} {quest.Stars}'''★'''
|{(string.IsNullOrEmpty(assignmentType) ? "???" : assignmentType)}
|{{{{UI|UI|{objectiveIconType}|title={objectiveIconType}|nolink=true}}}} [[{quest.Name} (MHWI Quest)|{quest.Name}]]
|[[{(quest.Locale.StartsWith("Arena ") ? $"Arena (New World)|Arena" : quest.Locale)}]]
|data-sort-value=""{allObjectives.Length}""|{(allObjectives.Length > 0 ? string.Join("", allObjectives) : "None")}
|-");
            }
            sb.AppendLine("|}");
            return sb.ToString();
        }
    }
}
