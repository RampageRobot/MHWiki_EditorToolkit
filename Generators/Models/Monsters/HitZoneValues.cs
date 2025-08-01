using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			string fileName = $@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\Parts.json";
			if (File.Exists(fileName))
			{
				Dictionary<string, dynamic[]> partData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
				foreach (dynamic dyn in partData["Hitzones"])
				{
					ret.Add(new()
					{
						Name = monsterName == "Alatreon" ? dyn.Name.ToString().Replace(" (Fire)", " (No Element)") : dyn.Name,
						SeverEffect = dyn.Sever,
						SeverTndr = GetTenderValue((int)dyn.Sever),
						BluntEffect = dyn.Impact,
						BluntTndr = GetTenderValue((int)dyn.Impact),
						BulletEffect = dyn.Shot,
						BulletTndr = GetTenderValue((int)dyn.Shot),
						FireEffect = dyn.Fire,
						WaterEffect = dyn.Water,
						IceEffect = dyn.Ice,
						ThunderEffect = dyn.Thunder,
						DragonEffect = dyn.Dragon,
						StunEffect = dyn.Stun
					});
				}
			}
			return [.. ret];
		}

		private static int GetTenderValue(int src)
		{
			return Convert.ToInt32(Math.Round(src + ((100 - src) * 0.25)));
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
			sb.AppendLine(@"|}");
			return sb.ToString();
		}
	}
}
