using MediawikiTranslator.Models.Monsters;

namespace MediawikiTranslator.Generators
{
	public class Items
	{
		public string? InternalID { get; set; }
		public int InternalOrder { get; set; }
		public string? Name { get; set; }
		public string? JPName { get; set; }
		public string? Description { get; set; }
		public string? Category { get; set; }
		public int BuyPrice { get; set; }
		public int SellPrice { get; set; }
		public int CarryLimit { get; set; }
		public int BuddyCarryLimit { get; set; }
		public int Rarity { get; set; }
		public string? WikiIconColor { get; set; }
		public string? WikiIconName { get; set; }
		public string? WikiIconDecorations { get; set; }
		public List<ItemCrafting> Combinations { get; set; } = [];
		public List<ItemSource> Sources { get; set; } = [];
		public List<ItemEquipment> Equipment { get; set; } = [];


		public static Items[] FetchGameItems(Games game)
		{
			switch (game)
			{
				case Games.MHWI:
				case Games.MHWorld:
					return Models.Data.MHWI.Items.FetchParsed();
				case Games.MHRS:
				case Games.MHRise:
					return Models.Data.MHRS.Items.FetchParsed();
				case Games.MHWilds:
					return Models.Data.MHWilds.Items.FetchParsed();
				default: return [];
			}
		}
		public static Dictionary<int, string> GetMHWIWikiColors()
		{
			return new()
			{
				{ 0, "White" },
				{ 1, "Red" },
				{ 2, "Green" },
				{ 3, "Blue" },
				{ 4, "Yellow" },
				{ 5, "Purple" },
				{ 6, "Light Blue" },
				{ 7, "Orange" },
				{ 8, "Pink" },
				{ 9, "Lemon" },
				{ 10, "Gray" },
				{ 11, "Brown" },
				{ 12, "Emerald" },
				{ 13, "Moss" },
				{ 14, "Rose" },
				{ 15, "Dark Blue" },
				{ 16, "Dark Purple" },
				{ 17, "NOT AVAILABLE" },
				{ 18, "NOT AVAILABLE" },
				{ 19, "Violet" },
				{ 20, "NOT AVAILABLE" },
				{ 21, "NOT AVAILABLE" },
				{ 22, "NOT AVAILABLE" },
				{ 23, "NOT AVAILABLE" },
				{ 24, "Tan" },
				{ 25, "Vermilion" },
				{ 26, "Light Green" }
			};
		}

		public static Dictionary<int, string> GetMHRSWikiColors()
		{
			return new()
			{
				{ 0, "NOT AVAILABLE" },
				{ 1, "White" },
				{ 2, "Gray" },
				{ 3, "Pink" },
				{ 4, "Yellow" },
				{ 5, "Orange" },
				{ 6, "Vermilion" },
				{ 7, "Red" },
				{ 8, "Green" },
				{ 9, "Purple" },
				{ 10, "Blue" },
				{ 11, "Dark Blue" },
				{ 12, "Light Blue" },
				{ 13, "Brown" },
				{ 14, "Dark Purple" },
				{ 51, "Pink" }
			};
		}
	}

	public class ItemCrafting
	{
		public int Number { get; set; }
		public string Result { get; set; }
		public string MaterialA { get; set; }
		public string MaterialB { get; set; }
	}

	public class ItemEquipment
	{
		public string EquipmentName { get; set; }
		public string EquipmentType { get; set; }
	}

	public class ItemSource
	{
		public string Stage { get; set; }
		public int Zone { get; set; }
		public string MonsterName { get; set; } = string.Empty;
		public string QuestName { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public int Probability { get; set; }
		public int QuantityRare { get; set; }
		public int ProbabilityRare { get; set; }
		public string Rank { get; set; }
		public string Circumstance { get; set; }
	}
}
