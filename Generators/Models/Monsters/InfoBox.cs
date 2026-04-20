using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediawikiTranslator.Models.Monsters
{
	public class InfoBox
	{
		private static JObject _monsterData = JsonConvert.DeserializeObject<JObject>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
		public InfoBox(string name, Games game) 
		{
			EnglishName = name;
			CrownSizes = new CrownSizes(name, game);
			JObject monsterDataObj = _monsterData.Value<JObject>(name)!;
			JObject langNames = monsterDataObj.Value<JObject>("LangNames")!;
			if (langNames != null)
			{
				JapaneseName = langNames.Value<string>("Japanese")!;
			}
			Title = monsterDataObj.Value<string>("Title")!;
			Class = monsterDataObj.Value<string>("Classification")!;
			if (game == Games.MHWI || game == Games.MHWorld)
			{
				dynamic[] bookInfo = JsonConvert.DeserializeObject<dynamic[]>(Utilities.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\Monster Data\bookData.json"))!;
				dynamic thisBookInfo = bookInfo.First(x => x.Name == name);
				if (game == Games.MHWorld)
				{
					if (!string.IsNullOrEmpty(thisBookInfo.MHWCapture.ToString()))
					{
						CaptureHP = Convert.ToInt32(thisBookInfo.MHWCapture.ToString());
						LimpHP = Convert.ToInt32(thisBookInfo["MHWDying 2"].ToString() == "" ? "0" : thisBookInfo["MHWDying 2"].ToString());
						Meat = thisBookInfo.MHWMeat.ToString() == "O";
						Dung = Convert.ToInt32(thisBookInfo.MHWDung.ToString() == "" ? "0" : thisBookInfo.MHWDung.ToString());
						Sonic = thisBookInfo.MHWScreamer.ToString() == "O";
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(thisBookInfo.MHWCapture.ToString()))
					{
						CaptureHP = Convert.ToInt32(thisBookInfo.MHWCapture);
						LimpHP = Convert.ToInt32(thisBookInfo["MHWDying 2"].ToString() == "" ? "0" : thisBookInfo["MHWDying 2"].ToString());
						Meat = thisBookInfo.MHWIMeat.ToString() == "O";
						Dung = Convert.ToInt32(thisBookInfo.MHWIDung.ToString() == "" ? "0" : thisBookInfo.MHWIDung.ToString());
						Sonic = thisBookInfo.MHWIScreamer.ToString() == "O";
					}
				}
			}
			else if (game == Games.MHWilds)
			{
				CaptureHP = 20;
				LimpHP = 15;
				TrapEffectiveness[] traps = TrapEffectiveness.GetTraps(name, game);
				Meat = traps.Any(x => x.Type == TrapType.Meat && x.Effectiveness == 100);
				float? dungEff = traps.FirstOrDefault(x => x.Type == TrapType.DungPod)?.Effectiveness;
				dungEff ??= 0;
				Dung = Convert.ToInt32(dungEff.Value);
			}
		}

		public string EnglishName { get; set; }
		public string JapaneseName { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Class { get; set; } = string.Empty;
		public List<ElementType> Elements { get; set; } = [];
		public List<StatusDmgType> StatusEFfects { get; set; } = [];
		public List<ElementType> Weaknesses { get; set; } = [];
		public int CaptureHP { get; set; }
		public int LimpHP { get; set; }
		public bool Meat { get; set; }
		public bool Flash { get; set; }
		public int Dung { get; set; }
		public bool Sonic { get; set; }
		public CrownSizes CrownSizes { get; set; }
	}
	public enum ElementType
	{
		Fire,
		Water,
		Thunder,
		Ice,
		Dragon
	}
	public enum StatusDmgType
	{
		AffinityUp,
		Affinity,
		AttackDown,
		AttackUp,
		Blast,
		Blastblight,
		Blastscourge,
		Bleeding,
		Bubbleblight,
		Confusion,
		DeadlyPoison,
		DefenseDownL,
		DefenseDown,
		DefenseUp,
		Defense,
		Dragonblight,
		Drain,
		Effluvium,
		ElementUp,
		Exhaust,
		Fatigued,
		Fireblight,
		FrenzyVirus,
		Iceblight,
		Mucus,
		Muddy,
		NoxiousPoison,
		Ossified,
		Paralysis,
		Poison,
		ResistanceDown,
		SevereBubbleblight,
		Sleep,
		Snowman,
		Soiled,
		Stun,
		Tarred,
		Thunderblight,
		Waterblight,
		Webbed
	}
}
