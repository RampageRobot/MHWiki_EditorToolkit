using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    public class OtherLanguages
    {
        public Dictionary<string, string> Languages { get; set; } = [];

        public OtherLanguages(string monsterName)
        {
            if (File.Exists($@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\OtherLanguages.json"))
            {
                Languages = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "test monster stuff\MHWI\{monsterName}\OtherLanguages.json"))!;
            }
        }

        public string Format()
        {
            return $@"==Name in Other Languages==
{{{{LanguageNames
{string.Join("\r\n", [.. Languages.Select(x => $"|{x.Key} = {x.Value}")])}
}}}}";
        }
    }
}
