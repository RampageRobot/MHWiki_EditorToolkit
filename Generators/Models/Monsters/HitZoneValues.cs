using Newtonsoft.Json;
using System.Text;

namespace MediawikiTranslator.Models.Monsters
{
    public class HitZoneValues
    {
        public string Name { get; set; } = string.Empty;
        public int SeverEffect { get; set; }
		public int BluntEffect { get; set; }
		public int BulletEffect { get; set; }
		public int SeverTndr { get; set; }
		public int BluntTndr { get; set; }
		public int BulletTndr { get; set; }
        public int FireEffect { get; set; }
        public int WaterEffect { get; set; }
        public int ThunderEffect { get; set; }
        public int IceEffect { get; set; }
        public int DragonEffect { get; set; }
        public int StunEffect { get; set; }

		public static HitZoneValues[] GetHitZoneValues(string monsterName)
		{
			List<HitZoneValues> ret = [];
			try
			{
				string fileName = $@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\Monster Data\{monsterName}\Parts.json";
				if (File.Exists(fileName))
				{
					Dictionary<string, dynamic[]> partData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
					foreach (dynamic dyn in partData["Hitzones"])
					{
						ret.Add(new()
						{
							Name = monsterName == "Alatreon" ? dyn.Name.ToString().Replace(" (Fire)", " (No Element)") : dyn.Name,
							SeverEffect = dyn.Sever,
							SeverTndr = GetTenderValue((int)dyn.Sever, monsterName),
							BluntEffect = dyn.Impact,
							BluntTndr = GetTenderValue((int)dyn.Impact, monsterName),
							BulletEffect = dyn.Shot,
							BulletTndr = GetTenderValue((int)dyn.Shot, monsterName),
							FireEffect = dyn.Fire,
							WaterEffect = dyn.Water,
							IceEffect = dyn.Ice,
							ThunderEffect = dyn.Thunder,
							DragonEffect = dyn.Dragon,
							StunEffect = dyn.Stun
						});
					}
				}
			}
			catch { }
			return [.. ret];
		}

		private static int GetTenderValue(int src, string monsterName)
		{
			int add = 25;
			if (new string[] { "Safi'jiiva", "Safi'Jiiva", "Fatalis" }.Contains(monsterName))
			{
				add = 20;
			}
			else if (new string[] { "Gold Rathian", "Kirin", "Lavasioth", "Namielle", "Savage Deviljho", "Silver Rathalos", "Uragaan" }.Contains(monsterName))
			{
				add = 30;
			}
			return Convert.ToInt32(Math.Round(src * 0.75) + add);
		}

		public static string Format(HitZoneValues[] vals)
		{
			StringBuilder sb = new();
			sb.AppendLine(@"===Damage Effectiveness===
{| class=""wikitable itemtable mobile-sm"" style=""text-align:center;width:100%;""
! colspan=""11"" | Damage Effectiveness <sup style=""color:mediumvioletred"" style=""color:mediumvioletred"">Tenderized</sup>(%)
|-
|- class=""sticky-row""
!'''Part'''
![[File:MHWI-Great_Sword_Icon_Rare_0.png|24x24px]]
![[File:MHWI-Hammer_Icon_Rare_0.png|24x24px]]
![[File:MHWI-Ammo_Icon_White.png|24x24px]]
![[File:UI-Fireblight.png|24x24px]]
![[File:UI-Waterblight.png|24x24px]]
![[File:UI-Thunderblight.png|24x24px]]
![[File:UI-Iceblight.png|24x24px]]
![[File:UI-Dragonblight.png|24x24px]]
![[File:UI-Stun.png|24x24px]]
|-");
			foreach (HitZoneValues val in vals)
			{
				sb.AppendLine($@"|'''{val.Name}'''
|{(val.SeverEffect >= 45 ? "'''" : "") + val.SeverEffect + (val.SeverEffect >= 45 ? "'''" : "")}{(val.SeverTndr != val.SeverEffect ? "<sup style=\"color:mediumvioletred\">" + (val.SeverTndr >= 45 ? "'''" : "") + val.SeverTndr + (val.SeverTndr >= 45 ? "'''" : "") + "</sup>" : "")}
|{(val.BluntEffect >= 45 ? "'''" : "") + val.BluntEffect + (val.BluntEffect >= 45 ? "'''" : "")}{(val.BluntTndr != val.BluntEffect ? "<sup style=\"color:mediumvioletred\">" + (val.BluntTndr >= 45 ? "'''" : "") + val.BluntTndr + (val.BluntTndr >= 45 ? "'''" : "") + "</sup>" : "")}
|{(val.BulletEffect >= 45 ? "'''" : "") + val.BulletEffect + (val.BulletEffect >= 45 ? "'''" : "")}{(val.BulletTndr != val.BulletEffect ? "<sup style=\"color:mediumvioletred\">" + (val.BulletTndr >= 45 ? "'''" : "") + val.BulletTndr + (val.BulletTndr >= 45 ? "'''" : "") + "</sup>" : "")}
|{val.FireEffect}
|{val.WaterEffect}
|{val.ThunderEffect}
|{val.IceEffect}
|{val.DragonEffect}
| {(val.StunEffect > 0 ? val.StunEffect : "-")}
|-");
			}
			sb.AppendLine(@"|}
{{UserHelpBox|
*'''Part''' - the name of the part. Any special states or conditions are shown in parentheses. 
*[[File:MHWilds-Great_Sword_Icon_Rare_0.png|24x24px]][[File:MHWilds-Hammer_Icon_Rare_0.png|24x24px]][[File:MHWilds-Ammo_Icon_White.png|24x24px]] - the effectiveness of '''Cutting''', '''Blunt''', and '''Shot''' raw attack types respectively. For example, if a part has a hitzone value of 40 for Cutting, it means that any Cutting damage dealt to it is reduced by 60%. Hitzone values greater than or equal to '''45''' are considered weak spots by the game, displaying orange damage numbers and activating [[Weakness Exploit (MHWilds)|Weakness Exploit]]. These numbers are given in '''bold'''. Tenderized values are given in Purple.
*{{UI|UI|Fire}}{{UI|UI|Water}}{{UI|UI|Thunder}}{{UI|UI|Ice}}{{UI|UI|Dragon}} - the effectiveness of each elemental damage type. For example, if a part has a hitzone value of 25 for Thunder, it means that any Thunder damage dealt to it is reduced by 75%.
*{{UI|UI|Stun}} - the effectiveness of [[Stun]] against the specified part. For example, a value of 50 means that only 50% of all stun buildup dealt to that part contributes to the monster's stun threshold. A <code>-</code> means that hitting the part with an attack will never contribute to stun buildup, regardless of its properties.
}}");
			return sb.ToString();
		}
	}
}
