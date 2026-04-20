using DocumentFormat.OpenXml.Drawing.Diagrams;
using MediawikiTranslator.Models.ArmorSets;
using MediawikiTranslator.Models.DamageTable;
using MediawikiTranslator.Models.Data.MHWI;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Text;

namespace MediawikiTranslator.Models.Monsters
{
    public class Equipment
    {
        public List<Weapon.WebToolkitData> Weapons { get; set; } = [];
		public List<WebToolkitData> Armor { get; set; } = [];
		private static dynamic[] BookInfo { get; set; } = [];
		public static List<string> badMonsterNames { get; set; } = [];

		public Equipment(string monsterName, Games game)
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
					Weapons = [.. allBlades.Where(x => x.Tree!.StartsWith(monsterName.Replace("Scarred Yian Garuga", "Yian Garuga")))];
					Weapons.AddRange([.. allGuns.Where(x => x.Tree!.StartsWith(monsterName.Replace("Scarred Yian Garuga", "Yian Garuga")))]);
					Armor = [.. allArmor.Where(x => x.SetName.Replace("β", "").Replace("α", "").Replace("γ", "").Trim().StartsWith(GetSetName(monsterName, allArmor)))];
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
				Weapons = [..files.Where(x => x.Key.MonsterName == monsterName).Select(x => x.Key)];
				Armor = [..Data.MHWilds.Armor.GetWebToolkitData().Where(x => x.SetName.StartsWith(monsterName) || (setReplace.ContainsKey(monsterName) && x.SetName.StartsWith(setReplace[monsterName])))];
			}
			int cntr = 0;
			foreach (Weapon.WebToolkitData weapon in Weapons)
			{
				weapon.Order = cntr;
				cntr++;
			}
			cntr = 0;
			foreach (WebToolkitData armor in Armor)
			{
				armor.Order = cntr;
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

		public string Format()
		{
			StringBuilder sb = new();
			sb.AppendLine(@"==Equipment==
===Weapons===
<div class=""threecol"">
<div>");
			int rows = 0;
			int eachCol = Convert.ToInt32(Math.Floor(Weapons.Count / 3f));
			Dictionary<int, int> cols = new() {
				{ 0, eachCol },
				{ 1, eachCol },
				{ 2, eachCol },
			};
			int remainder = Weapons.Count % 3;
			if (remainder > 0)
			{
				for (int i = 0; i < remainder; i++)
				{
					cols[i]++;
				}
			}
			int cells = 0;
			foreach (Weapon.WebToolkitData weapon in Weapons)
			{
				if (cells >= cols[rows])
				{
					cells = 0;
					if (rows < (cols.Count - 1))
					{
						rows++;
					}
					sb.AppendLine("</div>\r\n<div>");
				}
				sb.AppendLine($"*{{{{GenericWeaponLink|MHWI|{weapon.Name}|{weapon.Type}|{weapon.Rarity}}}}}");
				cells++;
			}
			sb.AppendLine(@"</div>
</div>

===Armor Sets===
<div class=""threecol"">
<div>");
			rows = 0;
			eachCol = Convert.ToInt32(Math.Floor(Armor.Count / 3f));
			cols = new()
			{
				{ 0, eachCol },
				{ 1, eachCol },
				{ 2, eachCol },
			};
			remainder = Armor.Count % 3;
			if (remainder > 0)
			{
				for (int i = 0; i < remainder; i++)
				{
					cols[i]++;
				}
			}
			cells = 0;
			foreach (WebToolkitData armor in Armor)
			{
				if (cells >= cols[rows])
				{
					cells = 0;
					if (rows < (cols.Count - 1))
					{
						rows++;
					}
					sb.AppendLine("</div>\r\n<div>");
				}
				sb.AppendLine($"*{{{{GenericArmorLink|MHWI|{armor.SetName} Set|Chestplate|{armor.Rarity}}}}}");
				cells++;
			}
			sb.AppendLine(@"</div>
</div>");
			return sb.ToString();
		}
	}
}
