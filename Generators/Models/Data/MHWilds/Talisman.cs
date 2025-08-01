using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MediawikiTranslator.Models.Data.MHWilds
{
    public class Talisman
    {
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public int Level { get; set; }
		public int Price { get; set; }
        public int Rarity { get; set; }
		public Skill[] Skills { get; set; } = [];
		public int[] SkillLevels { get; set; } = [];
		public Items[] CraftingItems { get; set; } = [];
		public int[] CraftingItemQtys { get; set; } = [];

		public static Talisman[] GetTalismans()
        {
			Items[] allItems = Items.Fetch();
			Skill[] allSkills = Skill.GetSkills();
			JArray talismanNames = Utilities.GetWildsMessages(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\Amulet.msg.23.json");
			JObject[] amuletData = [.. JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Equip\AmuletData.user.3.json"))![0].Value<JObject>("app.user_data.AmuletData")!.Value<JArray>("_Values")!.Select(x => x.Value<JObject>("app.user_data.AmuletData.cData"))!];
			JObject[] amuletRecipeData = [.. JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Equip\AmuletRecipeData.user.3.json"))![0].Value<JObject>("app.user_data.AmuletRecipeData")!.Value<JArray>("_Values")!.Select(x => x.Value<JObject>("app.user_data.AmuletRecipeData.cData"))!];
			List<Talisman> ret = [];
			foreach (JObject amulet in amuletData)
			{
				string rarity = amulet.Value<JObject>("_Rare")!.Value<JObject>("app.ItemDef.RARE_Serializable")!.Value<string>("_Value")!;
				Talisman newTalisman = new()
				{
					Name = talismanNames.First(y => y.Value<string>("guid")! == amulet.Value<string>("_Name")!)!.Value<JArray>("content")![1].Value<string>()!,
					Description = talismanNames.First(y => y.Value<string>("guid")! == amulet.Value<string>("_Explain")!)!.Value<JArray>("content")![1].Value<string>()!,
					Rarity = Convert.ToInt32(rarity.Substring(rarity!.IndexOf("RARE") + 4)) + 1,
					Price = amulet.Value<int>("_Price")!,
					Level = amulet.Value<int>("_Lv")!,
					SkillLevels = [.. amulet.Value<JArray>("_SkillLevel")!.Select(x => x.Value<int>()).Where(x => x > 0)],
					Skills = [..amulet.Value<JArray>("_Skill")!
						.Select(x => x.Value<JObject>("app.HunterDef.Skill_Serializable")!
						.Value<string>("_Value"))
						.Where(x => x != "[0]NONE")
						.Select(x => allSkills.First(y => y.SkillId == x))]
				};
				JObject? craftingData = amuletRecipeData.FirstOrDefault(x => x.Value<int>("_DataId") == amulet.Value<int>("_DataId"));
				if (craftingData != null)
				{
					int[] itemQtys = [.. craftingData.Value<JArray>("_ItemNum")!.Select(x => x.Value<int>())];
					string[] itemIds = [.. craftingData.Value<JArray>("_ItemId")!.Select(x => x.Value<string>())!];
					List<Items> craftingItems = [];
					foreach (string itemId in itemIds)
					{
						if (itemId != "[1]NONE")
						{
							craftingItems.Add(allItems.First(x => x.ItemID == itemId));
						}
					}
					newTalisman.CraftingItems = [.. craftingItems];
					newTalisman.CraftingItemQtys = itemQtys;
				}
				ret.Add(newTalisman);
			}
			return [.. ret];
		}
    }
}
