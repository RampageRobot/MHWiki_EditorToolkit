using MediawikiTranslator.Models.ArmorSets;
using MediawikiTranslator.Models.Data.MHWI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    public class Equipment
    {
        public List<Weapon.WebToolkitData> Weapons { get; set; } = [];
		public List<WebToolkitData> Armor { get; set; } = [];
		private static dynamic[] BookInfo { get; set; } = [];

		public Equipment(string monsterName, string game)
		{
			if (game == "MHWI")
			{
				if (BookInfo.Length == 0)
				{
					BookInfo = JsonConvert.DeserializeObject<dynamic[]>(File.ReadAllText(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\bookData.json"))!;
				}
				dynamic thisBookInfo = BookInfo.First(x => x.Name == monsterName);
				Weapon.WebToolkitData[] allBlades = BlademasterData.GetToolkitData();
				Weapon.WebToolkitData[] allGuns = GunnerData.GetToolkitData();
				WebToolkitData[] allArmor = Data.MHWI.Armor.GetWebToolkitData();
				Weapons = [.. allBlades.Where(x => x.Tree!.StartsWith(monsterName.Replace("Scarred Yian Garuga", "Yian Garuga")))];
				Weapons.AddRange([.. allGuns.Where(x => x.Tree!.StartsWith(monsterName.Replace("Scarred Yian Garuga", "Yian Garuga")))]);
				if (!string.IsNullOrEmpty(thisBookInfo["Armor Set Name"].ToString()))
				{
					Armor = [.. allArmor.Where(x => x.SetName.StartsWith(thisBookInfo["Armor Set Name"].ToString()))];
				}
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
