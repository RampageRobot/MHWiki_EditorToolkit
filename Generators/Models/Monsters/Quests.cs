using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    public class Quests
    {
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

		private static Dictionary<string, Quests> QuestInfo { get; set; } = [];

        public static Quests[] FetchQuests(string? monsterName = null)
        {
            if (QuestInfo.Count == 0)
            {
                QuestInfo = JsonConvert.DeserializeObject<Dictionary<string, Quests>>(File.ReadAllText(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\questInfo.json"))!;
            }
            return [..QuestInfo.Where(x => x.Value.ObjectiveMonsters.Any(y => monsterName == null || y == monsterName)).Select(x => x.Value)];
        }

		public static string Format(Quests[] quests, string monsterName)
        {
            Dictionary<string, string>[] objInfo = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(File.ReadAllText(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\questObjectiveTypes.json"))!;
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
