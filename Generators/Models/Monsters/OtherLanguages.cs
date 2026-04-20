using Newtonsoft.Json;

namespace MediawikiTranslator.Models.Monsters
{
    public class OtherLanguages
    {
        public Dictionary<string, string> Languages { get; set; } = [];

        public OtherLanguages(string monsterName)
        {
            if (File.Exists($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\Monster Data\{monsterName}\OtherLanguages.json"))
            {
                Languages = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\Monster Data\{monsterName}\OtherLanguages.json"))!;
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
