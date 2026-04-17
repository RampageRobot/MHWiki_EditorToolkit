namespace MediawikiTranslator.Models.Monsters
{
	//not used in MHWilds; parts line up with damage effectiveness table and thus were combined
	public class PartsTable
	{
		public int Order { get; set; }
		public required string PartName { get; set; }
		public int? Stagger { get; set; }
		public int? Break { get; set; }
		public int? BreakCount { get; set; }
		public int? SeverAmt { get; set; }
		public string? SeverType { get; set; }
		public KinsectEssence Essence { get; set; }

		public static List<PartsTable> Get(string monsterName)
		{
			List<PartsTable> allParts = [.. FlinchBreakThresholds.GetFlinchBreakThresholds(monsterName).Select(x => new PartsTable() { PartName = x.Name, Essence = x.Essence })];
			int cntr = 0;
			foreach (PartsTable part in allParts)
			{
				part.Order = cntr;
				cntr++;
			}
			return allParts;
		}
	}
}
