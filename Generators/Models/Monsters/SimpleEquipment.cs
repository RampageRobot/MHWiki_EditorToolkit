using DocumentFormat.OpenXml.Drawing.Diagrams;
using MediawikiTranslator.Models.ArmorSets;
using MediawikiTranslator.Models.DamageTable;
using MediawikiTranslator.Models.Data.MHWI;
using Newtonsoft.Json;

namespace MediawikiTranslator.Models.Monsters
{
	public class SimpleWeapon(Weapon.WebToolkitData src)
	{
		public int Order { get; set; } = 0;
		public string? Name { get; set; } = src.Name;
		public long? Rarity { get; set; } = src.Rarity;
		public string? Type { get; set; } = src.Type;
		public string? Tree { get; set; } = src.Tree;
		public string? MonsterName { get; set; } = src.MonsterName;
	}

	public class SimpleArmor(WebToolkitData src)
	{
		public int Order { get; set; } = 0;
		public string SetName { get; set; } = src.SetName;
		public long? Rarity { get; set; } = src.Rarity;
		public string Rank { get; set; } = src.Rank;
		public SimplePiece[] Pieces { get; set; } = [..src.Pieces.Select(x => new SimplePiece(x))];
	}

	public class SimplePiece(Piece src)
	{
		public int Order { get; set; } = 0;
		public string Name { get; set; } = src.Name;
		public string IconType { get; set; } = src.IconType;
	}

	public class SimpleEquipment
	{
		public List<SimpleWeapon> Weapons { get; set; } = [];
		public List<SimpleArmor> Armor { get; set; } = [];
		private static dynamic[] BookInfo { get; set; } = [];
		public static List<string> badMonsterNames { get; set; } = [];

		public SimpleEquipment(string monsterName, Games game)
		{
			if (game == Games.MHWI || game == Games.MHWorld)
			{
				if (BookInfo.Length == 0)
				{
					BookInfo = JsonConvert.DeserializeObject<dynamic[]>(Utilities.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\Monster Data\bookData.json"))!;
				}
				dynamic? thisBookInfo = BookInfo.FirstOrDefault(x => x.Name == monsterName);
				if (thisBookInfo != null)
				{
					Weapon.WebToolkitData[] allBlades = BlademasterData.GetToolkitData(GamesExtensions.ToAcronymString(game));
					Weapon.WebToolkitData[] allGuns = GunnerData.GetToolkitData(GamesExtensions.ToAcronymString(game));
					WebToolkitData[] allArmor = Data.MHWI.Armor.GetWebToolkitData(GamesExtensions.ToAcronymString(game));
					Weapons = [.. allBlades.Where(x => (game == Games.MHWI || x.Rarity < 9) && x.Tree!.StartsWith(monsterName.Replace("Scarred Yian Garuga", "Yian Garuga"))).Select(x => new SimpleWeapon(x))];
					Weapons.AddRange([.. allGuns.Where(x => (game == Games.MHWI || x.Rarity < 9) && x.Tree!.StartsWith(monsterName.Replace("Scarred Yian Garuga", "Yian Garuga"))).Select(x => new SimpleWeapon(x))]);
					Armor = [.. allArmor.Where(x => (game == Games.MHWI || x.Rank != "Master Rank") && x.SetName.Replace("β", "").Replace("α", "").Replace("γ", "").Trim().StartsWith(GetSetName(monsterName, allArmor))).Select(x => new SimpleArmor(x))];
				}
			}
			else if (game == Games.MHWilds)
			{
				Dictionary<string, string> setReplace = new()
				{
					{ "Yian Kut-Ku", "Kut-Ku" },
					{ "Congalala", "Conga" },
					{ "Blangonga", "Blango" },
					{ "Gore Magala", "Gore" },
					{ "Guardian Fulgur Anjanath", "Guardian Fulgur" },
					{ "Guardian Ebony Odogaron", "Guardian Ebony" },
					{ "Jin Dahaad", "Dahaad" },
					{ "Zoh Shia", "Numinous" }
				};
				Dictionary<Weapon.WebToolkitData, string> files = Generators.Weapon.MassGenerate("MHWilds");
				Weapons = [.. files.Where(x => x.Key.MonsterName == monsterName).Select(x => new SimpleWeapon(x.Key))];
				Armor = [.. Data.MHWilds.Armor.GetWebToolkitData().Where(x => x.SetName.StartsWith(monsterName) || (setReplace.ContainsKey(monsterName) && x.SetName.StartsWith(setReplace[monsterName]))).Select(x => new SimpleArmor(x))];
			}
			else if (game == Games.MHRS || game == Games.MHRise)
			{
				Dictionary<string, string> setReplace = new()
				{
					{ "Yian Kut-Ku", "Kut-Ku" },
					{ "Congalala", "Conga" },
					{ "Blangonga", "Blango" },
					{ "Gore Magala", "Gore" },
					{ "Guardian Fulgur Anjanath", "Guardian Fulgur" },
					{ "Guardian Ebony Odogaron", "Guardian Ebony" },
					{ "Jin Dahaad", "Dahaad" },
					{ "Zoh Shia", "Numinous" }
				};
				Dictionary<Weapon.WebToolkitData, string> files = Generators.Weapon.MassGenerate("MHRS");
				Weapons = [.. files.Where(x => (game == Games.MHRS || x.Key.Rarity < 8) && x.Key.MonsterName == monsterName).Select(x => new SimpleWeapon(x.Key))];
				Armor = [.. Data.MHRS.Armor.GetWebToolkitData().Where(x => (game == Games.MHRS || x.Rank != "Master Rank") && x.SetName.StartsWith(monsterName) || (setReplace.ContainsKey(monsterName) && x.SetName.StartsWith(setReplace[monsterName]))).Select(x => new SimpleArmor(x))];
			}
			int cntr = 0;
			foreach (SimpleWeapon weapon in Weapons)
			{
				weapon.Order = cntr;
				cntr++;
			}
			cntr = 0;
			foreach (SimpleArmor armor in Armor)
			{
				armor.Order = cntr;
				int cntr2 = 0;
				foreach (SimplePiece piece in armor.Pieces)
				{
					piece.Order = cntr2;
					cntr2++;
				}
				cntr++;
			}
		}

		public static string GetSetName(string monsterName, WebToolkitData[] allArmor)
		{
			try
			{
				if (allArmor.Any(x => x.SetName.StartsWith(monsterName)))
				{
					return monsterName;
				}
				else
				{
					return new Dictionary<string, string>()
				{
					{ "Ancient Leshen", "Ciri" },
					{ "Anjanath", "Anja" },
					{ "Azure Rathalos", "Rath Soul" },
					{ "Bazelgeuse", "Bazel" },
					{ "Behemoth", "Drachen" },
					{ "Black Diablos", "Diablos Nero" },
					{ "Great Girros", "Girros" },
					{ "Great Jagras", "Jagras" },
					{ "Jyuratodus", "Jyura" },
					{ "Kushala Daora", "Kushala" },
					{ "Leshen", "Geralt" },
					{ "Paolumu", "Lumu" },
					{ "Pink Rathian", "Rath Heart" },
					{ "Pukei-Pukei", "Pukei" },
					{ "Radobaan", "Baan" },
					{ "Tobi-Kadachi", "Kadachi" },
					{ "Tzitzi-Ya-Ku", "Tzitzi" },
					{ "Zorah Magdaros", "Zorah" },
					{ "Beotodus", "Beo" },
					{ "Blackveil Vaal Hazak", "Blackveil Hazak" },
					{ "Coral Pukei-Pukei", "Coral Pukei" },
					{ "Ebony Odogaron", "Death Garon" },
					{ "Fulgur Anjanath", "Fulgur Anja" },
					{ "Gold Rathian", "Golden Lune" },
					{ "Nightshade Paolumu", "Lumu Phantasm" },
					{ "Raging Brachydios", "Raging Brachy" },
					{ "Ruiner Nergigante", "Ruiner Nergi" },
					{ "Savage Deviljho", "Savage Jho" },
					{ "Scarred Yian Garuga", "NO VALID ARMOR SETS" },
					{ "Seething Bazelgeuse", "Seething Bazel" },
					{ "Shrieking Legiana", "Shrieking Legia" },
					{ "Silver Rathalos", "Silver Sol" },
					{ "Stygian Zinogre", "Stygian Zin" },
					{ "Viper Tobi-Kadachi", "Viper Kadachi" }

				}[monsterName];
				}
			}
			catch (Exception)
            {
				return "ERROR";
			}
		}
	}
}
