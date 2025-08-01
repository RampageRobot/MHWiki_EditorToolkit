using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Data.MHWilds
{
    public class MealSkills
    {
        public string SkillId { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Icon { get; set; } = string.Empty;

        public static MealSkills[] GetMealSkills()
        {
            JArray skillNames = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\MealSkill.msg.23.json"))!.Value<JArray>("entries")!;
            return [..JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Facility\MealSkillData.user.3.json"))![0]
                .Value<JObject>("app.user_data.MealSkillData")!
                .Value<JArray>("_Values")!
				.Select(x => x.Value<JObject>("app.user_data.MealSkillData.cData")!)
				.Select(x => new MealSkills() {
                    SkillId = x.Value<string>("_MealSkill")!,
                    Icon = x.Value<string>("_SkillIcon")!,
                    Name = skillNames.First(y => y.Value<string>("guid")! == x.Value<string>("_Name")!)!.Value<JArray>("content")![1].Value<string>()!,
					Description = skillNames.First(y => y.Value<string>("guid")! == x.Value<string>("_Explain")!)!.Value<JArray>("content")![1].Value<string>()!.Replace("\r\n", " ")
				})];
        }
    }
}
