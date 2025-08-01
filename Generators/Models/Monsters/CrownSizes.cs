using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    public class CrownSizes
    {
        public float Base { get; set; }
        public float Small { get; set; }
        public float Silver { get; set; }
        public float Gold { get; set; }
		public float SmallLimit { get; set; }
		public float SilverLimit { get; set; }
		public float GoldLimit { get; set; }

        public CrownSizes(string name)
        {
			Dictionary<string, dynamic[]> partData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\crownInfo.json"))!;
			dynamic monsterSize = partData["Monsters"].First(x => GetMonsterId(name) == (int)x.Monster_Id && x.Monster_Id_button == $"{GetMonsterId(name)}: {name}");
			Base = monsterSize.Base_Size;
			SmallLimit = monsterSize.Gold_Small_Crown_Limit;
			Small = (SmallLimit / 100) * Base;
			SilverLimit = monsterSize.Silver_Crown_Limit;
			Silver = (SilverLimit / 100) * Base;
			GoldLimit = monsterSize.Gold_Big_Crown_Limit;
			Gold = (GoldLimit / 100) * Base;
		}

		public string Format()
		{
			return $@"==Crown Sizes==
{{{{CrownSizes
|Small Crown cm = {Math.Round(Small, 2):#.##}
|Small Crown % = {Math.Round(SmallLimit, 2):#.##}

|Average Size cm = {Math.Round(Base, 2):#.##}
|Average Size % = 100

|Silver Crown cm = {Math.Round(Silver, 2):#.##}
|Silver Crown % = {Math.Round(SilverLimit, 2):#.##}

|Gold Crown cm = {Math.Round(Gold, 2):#.##}
|Gold Crown % = {Math.Round(GoldLimit, 2):#.##}
}}}}
";
		}

        public int GetMonsterId(string name)
        {
			return new Dictionary<string, int>()
			{
				{ "Rathian", 9 },
				{ "Pink Rathian", 10 },
				{ "Gold Rathian", 88 },
				{ "Rathalos", 1 },
				{ "Azure Rathalos", 11 },
				{ "Silver Rathalos", 89 },
				{ "Diablos", 12 },
				{ "Black Diablos", 13 },
				{ "Kirin", 14 },
				{ "Fatalis", 101 },
				{ "Yian Garuga", 90 },
				{ "Scarred Yian Garuga", 99 },
				{ "Rajang", 91 },
				{ "Furious Rajang", 92 },
				{ "Kushala Daora", 16 },
				{ "Lunastra", 17 },
				{ "Teostra", 18 },
				{ "Tigrex", 61 },
				{ "Brute Tigrex", 93 },
				{ "Lavasioth", 19 },
				{ "Nargacuga", 62 },
				{ "Barioth", 63 },
				{ "Frostfang Barioth", 100 },
				{ "Deviljho", 20 },
				{ "Savage Deviljho", 64 },
				{ "Barroth", 21 },
				{ "Uragaan", 22 },
				{ "Alatreon", 87 },
				{ "Zinogre", 94 },
				{ "Stygian Zinogre", 95 },
				{ "Brachydios", 65 },
				{ "Raging Brachydios", 96 },
				{ "Glavenus", 66 },
				{ "Acidic Glavenus", 67 },
				{ "Anjanath", 0 },
				{ "Fulgur Anjanath", 68 },
				{ "Great Jagras", 7 },
				{ "Pukei-Pukei", 24 },
				{ "Coral Pukei-Pukei", 69 },
				{ "Nergigante", 25 },
				{ "Ruiner Nergigante", 70 },
				{ "Safi'jiiva", 97 },
				{ "Xeno'jiiva", 26 },
				{ "Zorah Magdaros", 4 },
				{ "Kulu-Ya-Ku", 27 },
				{ "Jyuratodus", 29 },
				{ "Tobi-Kadachi", 30 },
				{ "Viper Tobi-Kadachi", 71 },
				{ "Paolumu", 31 },
				{ "Nightshade Paolumu", 72 },
				{ "Legiana", 32 },
				{ "Shrieking Legiana", 73 },
				{ "Great Girros", 33 },
				{ "Odogaron", 34 },
				{ "Ebony Odogaron", 74 },
				{ "Radobaan", 35 },
				{ "Vaal Hazak", 36 },
				{ "Blackveil Vaal Hazak", 75 },
				{ "Dodogama", 37 },
				{ "Kulve Taroth", 38 },
				{ "Bazelgeuse", 39 },
				{ "Seething Bazelgeuse", 76 },
				{ "Tzitzi-Ya-Ku", 28 },
				{ "Behemoth", 15 },
				{ "Beotodus", 77 },
				{ "Banbaro", 78 },
				{ "Velkhana", 79 },
				{ "Namielle", 80 },
				{ "Shara Ishvalda", 81 },
				{ "Leshen", 23 },
				{ "Ancient Leshen", 51 },
				{ "Aptonoth", 2 },
				{ "Apceros", 40 },
				{ "Kelbi (Male)", 41 },
				{ "Kelbi (Female)", 42 },
				{ "Mosswine", 5 },
				{ "Hornetaur", 43 },
				{ "Vespoid", 44 },
				{ "Popo", 82 },
				{ "Anteka", 83 },
				{ "Gajau", 6 },
				{ "Jagras", 3 },
				{ "Mernos", 45 },
				{ "Kestodon (Male)", 8 },
				{ "Kestodon (Female)", 46 },
				{ "Raphinos", 47 },
				{ "Shamos", 48 },
				{ "Barnos", 49 },
				{ "Girros", 50 },
				{ "Gastodon", 52 },
				{ "Noios", 53 },
				{ "Gajalaka", 56 },
				{ "Boaboa", 86 },
				{ "Small Training Barrel", 57 },
				{ "Large Training Barrel", 58 },
				{ "Training Pole", 59 },
				{ "Training Wagon", 60 },
				{ "Wulg", 84 },
				{ "Cortos", 64 }
			}[name];
		}
    }
}
