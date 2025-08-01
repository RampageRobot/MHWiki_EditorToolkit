using MediawikiTranslator.Models.Data.MHRS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Data.MHWilds
{
    public class Decoration
    {
        public string DecoId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string DecoType { get; set; } = string.Empty;
        public int Rarity { get; set; }
        public int Price { get; set; }
        public int SlotLevel { get; set; }
        public string IconColor { get; set; } = string.Empty;
        public Skill[] Skills { get; set; } = [];
        public int[] SkillLevels { get; set; } = [];

		public static Decoration[] GetDecorations()
		{
			Skill[] allSkills = Skill.GetSkills();
			JArray decoNames = Utilities.GetWildsMessages(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Equip\Accessory.msg.23.json");
            JObject[] accessoryData = [..JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Common\Equip\AccessoryData.user.3.json"))![0].Value<JObject>("app.user_data.AccessoryData")!.Value<JArray>("_Values")!.Select(x => x.Value<JObject>("app.user_data.AccessoryData.cData"))!];
			List<Decoration> ret = [];
			Dictionary<string, string> colorDict = new() {
				{ "[4]I_PINK", "Pink" },
				{ "[3]I_ROSE", "Rose" },
				{ "[5]I_RED", "Red" },
				{ "[1]I_WHITE", "White" },
				{ "[20]I_PURPLE", "Purple" },
				{ "[21]I_DPURPLE", "Dark Purple" },
				{ "[17]I_BLUE", "Blue" },
				{ "[10]I_YELLOW", "Yellow" },
				{ "[16]I_SKY", "Light Blue" },
				{ "[6]I_VERMILION", "Vermilion" },
				{ "[18]I_ULTRAMARINE", "Dark Blue" },
				{ "[15]I_EMERALD", "Emerald" },
				{ "[11]I_LEMON", "Lemon" },
				{ "[13]I_MOS", "Moss" },
				{ "[2]I_GRAY", "Gray" },
				{ "[9]I_IVORY", "Tan" },
				{ "[8]I_BROWN", "Brown" },
				{ "[14]I_GREEN", "Green" },
				{ "[12]I_SGREEN", "Light Green" }
			};
			foreach (JObject accessory in accessoryData)
			{
				string accessoryLevel = accessory.Value<JObject>("_SlotLevelAcc")!.Value<JObject>("app.EquipDef.SlotLevel_Serializable")!.Value<string>("_Value")!;
				string rarity = accessory.Value<JObject>("_Rare")!.Value<JObject>("app.ItemDef.RARE_Serializable")!.Value<string>("_Value")!;
				ret.Add(new Decoration()
				{
					DecoId = accessory.Value<JObject>("_AccessoryId")!.Value<JObject>("app.EquipDef.ACCESSORY_ID_Serializable")!.Value<string>("_Value")!,
					Name = decoNames.First(y => y.Value<string>("guid")! == accessory.Value<string>("_Name")!)!.Value<JArray>("content")![1].Value<string>()!,
					Description = decoNames.First(y => y.Value<string>("guid")! == accessory.Value<string>("_Explain")!)!.Value<JArray>("content")![1].Value<string>()!,
					Rarity = Convert.ToInt32(rarity.Substring(rarity!.IndexOf("RARE") + 4)) + 1,
					IconColor = colorDict[accessory.Value<string>("_IconColor")!],
					Price = accessory.Value<int>("_Price")!,
					SlotLevel = Convert.ToInt32(accessoryLevel[accessoryLevel.Length - 1]),
					DecoType = accessory.Value<JObject>("_AccessoryType")!.Value<JObject>("app.EquipDef.ACCESSORY_TYPE_Serializable")!.Value<string>("_Value")!.EndsWith("0") ? "Sword" : "Armor",
					SkillLevels = [..accessory.Value<JArray>("_SkillLevel")!.Select(x => x.Value<int>()).Where(x => x > 0)],
					Skills = [..accessory.Value<JArray>("_Skill")!
						.Select(x => x.Value<JObject>("app.HunterDef.Skill_Serializable")!
						.Value<string>("_Value"))
						.Where(x => x != "[0]NONE")
						.Select(x => allSkills.First(y => y.SkillId == x))]
				});
			}
			return [..ret];
		}
	}
}
