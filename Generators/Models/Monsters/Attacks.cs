using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediawikiTranslator.Models.Monsters
{
	public class Attacks
	{
		public int Order { get; set; } = 0;
		public string Name { get; set; } = string.Empty;
        public string TranslatedName { get; set;} = string.Empty;
		public string Description { get; set; } = string.Empty;
        public int Power { get; set; }
        public string Element { get; set; } = string.Empty;
        public int ElementDamage { get; set; }
        public string Status { get; set; } = string.Empty;
		public int StatusBuildup { get; set; }
        public int StaminaCost { get; set; }
        public int GuardKnockback { get; set; }
		public bool MTL { get; set; } = true;
        public static readonly Dictionary<string, Dictionary<string, object>> MonsterMasterlist = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\enemies.json"))!;
		private static readonly Dictionary<string, string> _translations = [];

		public static Attacks[] FetchAttacks(string monsterName, Games game = Games.MHWI)
		{
			List<Attacks> ret = [];
            if (game == Games.MHWI)
            {
                string fileName = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\Monster Data\{monsterName}\Attacks.json";
                if (File.Exists(fileName))
				{
					Dictionary<string, string> statuses = new()
					{
						{ "Standard_Poison", "Poison" },
						{ "Deadly_Poison", "Deadly Poison" },
						{ "Paralysis", "Paralysis" },
						{ "Sleep", "Sleep" },
						{ "Blast", "Blast" },
						{ "Blast_Scourge", "Blastscourge" },
						{ "Stun", "Stun" },
						{ "Bleed", "Bleed" },
						{ "Miasma", "Miasma" },
						{ "Ele_Res_Down_Double", "Elemental Resistance Down L" },
						{ "Ele_Res_Down", "Elemental Resistance Down" },
						{ "Def_Down_Double", "Defense Down L" },
						{ "Def_Down", "Defense Down" }
					};
					JArray movesArr = JsonConvert.DeserializeObject<JObject>(Utilities.ReadAllText(fileName))!.Value<JArray>("Moves")!.First().Value<JArray>("Atks")!;
                    foreach (JObject attackSrc in movesArr)
					{
						string jpName = attackSrc.Value<string>("TranslatedName")!;
						string actualTranslatedName = Translate(jpName);
						string status = string.Empty;
						int statusDmg = 0;
						foreach (KeyValuePair<string, string> stat in statuses)
						{
							int thisDmg = attackSrc.Value<int>(stat.Key);
							if (thisDmg > 0)
							{
								status = stat.Value;
								statusDmg = thisDmg;
								break;
							}
						}
						ret.Add(new Attacks()
						{
							Name = actualTranslatedName,
							TranslatedName = jpName,
							Power = attackSrc.Value<int>("Motion_Value")!,
							ElementDamage = attackSrc.Value<int>("Element_Dmg")!,
							Element = attackSrc.Value<string>("BaseElementDamageType") != "None" ? attackSrc.Value<string>("BaseElementDamageType")! : string.Empty,
							GuardKnockback = attackSrc.Value<int>("Knock_back_Type"),
							StaminaCost = attackSrc.Value<int>("Guard_Stamina_Cost"),
							Status = status,
							StatusBuildup = statusDmg
						});
					}
                }
            }
            else if (game == Games.MHWilds)
			{
				MonsterId monster = Monster.WildsMonsterIds.First(x => x.Name == monsterName);
				foreach (JObject attackSrc in ((JArray)MonsterMasterlist[monster.Id]["attacks"]).Select(x => x.Value<JObject>()!))
				{
                    ret.Add(new Attacks()
                    {
                        Name = attackSrc.Value<string>("Name")!,
                        TranslatedName = attackSrc.Value<string>("TranslatedName")!,
                        Power = attackSrc.Value<int>("BaseDamage")!,
                        ElementDamage = attackSrc.Value<int>("BaseElementDamage")!,
                        Element = attackSrc.Value<string>("BaseElementDamageType") != "None" ? attackSrc.Value<string>("BaseElementDamageType")! : string.Empty,
                        GuardKnockback = attackSrc.Value<int>("GuardKnockback"),
                        StaminaCost = attackSrc.Value<int>("Stamina"),
                        Status = attackSrc.Value<string>("Status")!,
                        StatusBuildup = attackSrc.Value<int>("StatusDamage")
					});
				}
			}
			int cntr = 0;
			foreach (Attacks attack in ret)
			{
				attack.Order = cntr;
				cntr++;
			}
            return [.. ret];
        }

        public static string Format(Attacks[] attacks)
        {
            return "";
		}

		public static string Translate(string src)
		{
			if (string.IsNullOrEmpty(src)) return "???";
			if (_translations.ContainsKey(src))
			{
				return _translations[src];
			}
			else
			{
				using (Google.Apis.Translate.v2.TranslateService svc = new Google.Apis.Translate.v2.TranslateService(new Google.Apis.Services.BaseClientService.Initializer()
				{
					ApiKey = "AIzaSyAf783OtXI1UpYjYNNBgpn5EfRSKHWxTF4",
					ApplicationName = "MH Wiki Translation",
				}))
				{
					string res = svc.Translations.Translate(new Google.Apis.Translate.v2.Data.TranslateTextRequest()
					{
						Source = "ja",
						Format = "text",
						Target = "en",
						Q = [src]
					}).Execute().Translations.First().TranslatedText;
					_translations.Add(src, res);
					return res;
				}
			}
		}
	}
}
