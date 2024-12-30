using MediawikiTranslator.Models.Data.MHRS;
using MediawikiTranslator.Models.Data.MHWI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Generators
{
	public class Skills
	{
		public static void ParseSkills()
		{
			Models.Data.MHWI.Skills[] mhwiSkills = Models.Data.MHWI.Skills.FromJson(File.ReadAllText(@"C:\Users\mkast\Desktop\mhwi skills.json"));
			SkillsExtraInfo[] mhwiSkillsExtraInfo = SkillsExtraInfo.FromJson(File.ReadAllText(@"C:\Users\mkast\Desktop\mhwi skills extra info.json"));
			Dictionary<int, string> mhwiColors = Items.GetMHWIWikiColors();
			List<SimplifiedSkill> simplifiedSkills = [];
			foreach (SkillsExtraInfo skill in mhwiSkillsExtraInfo)
			{
				SimplifiedSkill newSkill = new()
				{
					Name = skill.Name,
					WikiIconColor = mhwiColors[(int)skill.IconColorId!.Value]
				};
				if (mhwiSkills.Any(x => x.Name == skill.Name + ": " + skill.Id))
				{
					newSkill.Description = mhwiSkills.First(x => x.Name == skill.Name + ": " + skill.Id).Description;
					newSkill.MaxLevel = (int)mhwiSkills.Where(x => x.Name == skill.Name + ": " + skill.Id).OrderByDescending(x => x.Level).First().Level!.Value;
				}
				simplifiedSkills.Add(newSkill);
			}
			File.WriteAllText(@"C:\Users\mkast\Desktop\mhwi_skills_simplified.json", Newtonsoft.Json.JsonConvert.SerializeObject(simplifiedSkills.OrderBy(x => x.Name), Newtonsoft.Json.Formatting.Indented));
			Dictionary<int, string> skillEnum = GetMHRISkillEnum();
			Models.Data.MHRS.Skills skillDict = Models.Data.MHRS.Skills.FromJson(File.ReadAllText(@"C:\Users\mkast\Desktop\plequipskillbasedata.user.2.json"));
			SkillsNames skillNameDict = SkillsNames.FromJson(File.ReadAllText(@"C:\Users\mkast\Desktop\skillnames.json"));
			SkillsExplain skillExplainDict = SkillsExplain.FromJson(File.ReadAllText(@"C:\Users\mkast\Desktop\skillexplain.json"));
			SkillsDetails skillDetailsDict = SkillsDetails.FromJson(File.ReadAllText(@"C:\Users\mkast\Desktop\skilldetails.json"));
			Dictionary<int, string> mhrsColors = Items.GetMHRSWikiColors();
			simplifiedSkills = [];
			foreach (Param data in skillDict.SnowDataPlEquipSkillBaseUserData.Param.Where(x => skillEnum.ContainsKey((int)x.Id!.Value)))
			{
				string nameKey = "PlayerSkill_" + skillEnum[(int)data.Id!.Value] + "_Name";
				if (skillNameDict.NameToUuid.ContainsKey(nameKey))
				{
					SkillsNamesMsg skillName = skillNameDict.Msgs[skillNameDict.NameToUuid[nameKey].ToString()];
					SimplifiedSkill newSkill = new()
					{
						Name = skillName.Content[1],
						MaxLevel = (int)data.MaxLevel!,
					};
					if (mhrsColors.ContainsKey((int)data.IconColor!.Value))
					{
						newSkill.WikiIconColor = mhrsColors[(int)data.IconColor!.Value];
					}
					else
					{
						newSkill.WikiIconColor = "NOT AVAILABLE";
					}
					string explainKey = "PlayerSkill_" + skillEnum[(int)data.Id!.Value] + "_Explain";
					if (skillExplainDict.Msgs.ContainsKey(skillExplainDict.NameToUuid[explainKey].ToString()))
					{
						SkillsExplainMsg msg = skillExplainDict.Msgs[skillExplainDict.NameToUuid[explainKey].ToString()];
						if (!string.IsNullOrEmpty(msg.Content[1]) && !msg.Content[1].StartsWith("<COLOR FF0000>#Rejected#</COLOR>"))
						{
							newSkill.Description = msg.Content[1].Replace("\r\n", " ");
						}
					}
					int lvl = 1;
					foreach (string key in skillDetailsDict.NameToUuid.Keys.Where(x => x.StartsWith("PlayerSkill_" + skillEnum[(int)data.Id!.Value] + "_")))
					{
						Msg skillsDetails = skillDetailsDict.Msgs[skillDetailsDict.NameToUuid[key].ToString()];
						if (!string.IsNullOrEmpty(skillsDetails.Content[1]) && !skillsDetails.Content[1].StartsWith("<COLOR FF0000>#Rejected#</COLOR>"))
						{
							newSkill.LevelDetails.Add(lvl, skillsDetails.Content[1].Replace("\r\n", " "));
						}
						lvl++;
					}
					simplifiedSkills.Add(newSkill);
				}
			}
			File.WriteAllText(@"C:\Users\mkast\Desktop\mhrs_skills_simplified.json", Newtonsoft.Json.JsonConvert.SerializeObject(simplifiedSkills.OrderBy(x => x.Name), Newtonsoft.Json.Formatting.Indented));
		}

		private static Dictionary<int, string> GetMHRISkillEnum()
		{
			return new()
			{
				{ 1, "000" },
				{ 2, "001" },
				{ 3, "002" },
				{ 4, "003" },
				{ 5, "004" },
				{ 6, "005" },
				{ 7, "006" },
				{ 8, "007" },
				{ 9, "008" },
				{ 10, "009" },
				{ 11, "010" },
				{ 12, "011" },
				{ 13, "012" },
				{ 14, "013" },
				{ 15, "014" },
				{ 16, "015" },
				{ 17, "016" },
				{ 18, "017" },
				{ 19, "018" },
				{ 20, "019" },
				{ 21, "020" },
				{ 22, "021" },
				{ 23, "022" },
				{ 24, "023" },
				{ 25, "024" },
				{ 26, "025" },
				{ 27, "026" },
				{ 28, "027" },
				{ 29, "028" },
				{ 30, "029" },
				{ 31, "030" },
				{ 32, "031" },
				{ 33, "032" },
				{ 34, "033" },
				{ 35, "034" },
				{ 36, "035" },
				{ 37, "036" },
				{ 38, "037" },
				{ 39, "038" },
				{ 40, "039" },
				{ 41, "040" },
				{ 42, "041" },
				{ 43, "042" },
				{ 44, "043" },
				{ 45, "044" },
				{ 46, "045" },
				{ 47, "046" },
				{ 48, "047" },
				{ 49, "048" },
				{ 50, "049" },
				{ 51, "050" },
				{ 52, "051" },
				{ 53, "052" },
				{ 54, "053" },
				{ 55, "054" },
				{ 56, "055" },
				{ 57, "056" },
				{ 58, "057" },
				{ 59, "058" },
				{ 60, "059" },
				{ 61, "060" },
				{ 62, "061" },
				{ 63, "062" },
				{ 64, "063" },
				{ 65, "064" },
				{ 66, "065" },
				{ 67, "066" },
				{ 68, "067" },
				{ 69, "068" },
				{ 70, "069" },
				{ 71, "070" },
				{ 72, "071" },
				{ 73, "072" },
				{ 74, "073" },
				{ 75, "074" },
				{ 76, "075" },
				{ 77, "076" },
				{ 78, "077" },
				{ 79, "078" },
				{ 80, "079" },
				{ 81, "080" },
				{ 82, "081" },
				{ 83, "082" },
				{ 84, "083" },
				{ 85, "084" },
				{ 86, "085" },
				{ 87, "086" },
				{ 88, "087" },
				{ 89, "088" },
				{ 90, "089" },
				{ 91, "090" },
				{ 92, "091" },
				{ 93, "092" },
				{ 94, "093" },
				{ 95, "094" },
				{ 96, "095" },
				{ 97, "096" },
				{ 98, "097" },
				{ 99, "098" },
				{ 100, "099" },
				{ 101, "100" },
				{ 102, "101" },
				{ 103, "102" },
				{ 104, "103" },
				{ 105, "104" },
				{ 106, "105" },
				{ 107, "106" },
				{ 108, "107" },
				{ 109, "108" },
				{ 110, "109" },
				{ 111, "110" },
				{ 112, "200" },
				{ 113, "201" },
				{ 114, "202" },
				{ 115, "203" },
				{ 116, "204" },
				{ 117, "205" },
				{ 118, "206" },
				{ 119, "207" },
				{ 120, "208" },
				{ 121, "209" },
				{ 122, "210" },
				{ 123, "211" },
				{ 124, "212" },
				{ 125, "213" },
				{ 126, "214" },
				{ 127, "215" },
				{ 128, "216" },
				{ 129, "217" },
				{ 130, "218" },
				{ 131, "219" },
				{ 132, "220" },
				{ 133, "221" },
				{ 134, "222" },
				{ 135, "223" },
				{ 136, "224" },
				{ 137, "225" },
				{ 138, "226" },
				{ 139, "227" },
				{ 140, "228" },
				{ 141, "229" },
				{ 142, "230" },
				{ 143, "231" },
				{ 144, "232" },
				{ 145, "233" },
				{ 146, "234" },
				{ 147, "235" },
				{ 148, "236" },
				{ 149, "237" },
				{ 150, "238" },
				{ 151, "239" },
				{ 152, "240" },
				{ 153, "241" },
				{ 154, "242" },
				{ 155, "243" },
				{ 156, "244" },
				{ 157, "245" },
				{ 158, "246" },
				{ 159, "247" },
				{ 160, "248" },
				{ 161, "249" },
				{ 162, "250" },
				{ 163, "251" },
				{ 164, "252" },
				{ 165, "253" },
				{ 166, "254" },
				{ 167, "255" }
			};
		}
	}

	class SimplifiedSkill
	{
		public string Name { get; set; } = string.Empty;
		public int MaxLevel { get; set; } = 1;
		public string WikiIconColor { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public Dictionary<int, string> LevelDetails { get; set; } = [];
	}
}
