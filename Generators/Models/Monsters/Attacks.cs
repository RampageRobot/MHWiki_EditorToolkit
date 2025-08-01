using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
	public class Attacks
    {
        public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
        public int Power { get; set; }
        public string Element { get; set; } = string.Empty;
        public int ElementDamage { get; set; }
        public string Status { get; set; } = string.Empty;
		public int StatusBuildup { get; set; }
        public int StaminaCost { get; set; }
        public int GuardKnockback { get; set; }

        public static Attacks[] FetchAttacks(string monsterName)
        {
            List<Attacks> ret = [];
            string fileName = $@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\Attacks.json";
            if (File.Exists(fileName))
            {
                Dictionary<string, dynamic[]> partData = JsonConvert.DeserializeObject<Dictionary<string, dynamic[]>>(File.ReadAllText(fileName))!;
                foreach (dynamic dyn in partData["Names"].Where(x => ((Newtonsoft.Json.Linq.JArray)x.LinkedMove).Count > 0))
                {
                }
            }
            return [.. ret];
        }

        public static string Format(Attacks[] attacks)
        {
            return "";
        }
    }
}
