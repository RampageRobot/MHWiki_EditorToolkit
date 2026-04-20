using Newtonsoft.Json;

namespace MediawikiTranslator.Models.Data.MHWI
{
    class Quests
	{
		public static Models.Quests.WebToolkitData[] FetchQuests()
		{
			return JsonConvert.DeserializeObject<Models.Quests.WebToolkitData[]>(Utilities.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\quest data\questDict.json"))!;
		}
	}
}
