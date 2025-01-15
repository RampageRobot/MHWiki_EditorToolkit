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
		public static SimplifiedSkill[] GetSimplifiedSkillsMHRS()
		{
			Dictionary<string, int> skillEnum = GetMHRISkillEnum();
			Models.Data.MHRS.Skills skillDict = Models.Data.MHRS.Skills.FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\natives\stm\data\define\player\skill\plequipskill\plequipskillbasedata.user.2.json"));
			Dictionary<int, string> mhrsColors = Items.GetMHRSWikiColors();
			List<SimplifiedSkill> simplifiedSkills = [];
			foreach (SkillsParam data in skillDict.SnowDataPlEquipSkillBaseUserData.Param.Where(x => skillEnum.ContainsKey(x.Id!)))
			{
				string skillId = data.Id!.Substring(data.Id!.LastIndexOf('_') + 1);
				SimplifiedSkill newSkill = new()
				{
					Id = (int)Models.Data.MHRS.Skills.GetSkillId(data.Id!),
					Name = CommonMsgs.GetMsg("PlayerSkill_" + skillId + "_Name"),
					MaxLevel = Convert.ToInt32(data.MaxLevel!.Substring(2)),
					Description = CommonMsgs.GetMsg("PlayerSkill_" + skillId + "_Explain").Replace("\r\n", " ")
				};
				if (mhrsColors.ContainsKey(Convert.ToInt32(data.IconColor!.Replace("ITEM_ICON_COLOR_", ""))))
				{
					newSkill.WikiIconColor = mhrsColors[Convert.ToInt32(data.IconColor!.Replace("ITEM_ICON_COLOR_", ""))];
				}
				else
				{
					newSkill.WikiIconColor = "NOT AVAILABLE";
				}
				for (int lvl = 0; lvl < 8; lvl++)
				{
					string lvlDetails = CommonMsgs.GetMsg("PlayerSkill_" + skillId + "_" + lvl.ToString("D2") + "_Detail");
					if (!string.IsNullOrEmpty(lvlDetails) && !lvlDetails.Contains("<COLOR FF0000>#Rejected#</COLOR>"))
					{
						newSkill.LevelDetails.Add(lvl + 1, new() {
							Description = lvlDetails.Replace("\r\n", " ")
						});
					}
				}
				simplifiedSkills.Add(newSkill);
			}
			return [.. simplifiedSkills];
		}

		public static void ParseSkills()
		{
			Models.Data.MHWI.Skills[] mhwiSkills = Models.Data.MHWI.Skills.FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\mhwi skills.json"));
			SkillsExtraInfo[] mhwiSkillsExtraInfo = SkillsExtraInfo.FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\mhwi skills extra info.json"));
			Dictionary<int, string> mhwiColors = Items.GetMHWIWikiColors();
			File.WriteAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\mhwi_skills_simplified.json", Newtonsoft.Json.JsonConvert.SerializeObject(SkillDescriptions.GetSimplifiedSkills().OrderBy(x => x.Name), Newtonsoft.Json.Formatting.Indented));
			Dictionary<string, int> skillEnum = GetMHRISkillEnum();
			Models.Data.MHRS.Skills skillDict = Models.Data.MHRS.Skills.FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\plequipskillbasedata.user.2.json"));
			SkillsNames skillNameDict = SkillsNames.FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\skillnames.json"));
			SkillsExplain skillExplainDict = SkillsExplain.FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\skillexplain.json"));
			SkillsDetails skillDetailsDict = SkillsDetails.FromJson(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\skilldetails.json"));
			Dictionary<int, string> mhrsColors = Items.GetMHRSWikiColors();
			List<SimplifiedSkill> simplifiedSkills = [];
			foreach (SkillsParam data in skillDict.SnowDataPlEquipSkillBaseUserData.Param.Where(x => skillEnum.ContainsKey(x.Id!)))
			{
				string nameKey = "PlayerSkill_" + skillEnum[data.Id!] + "_Name";
				if (skillNameDict.NameToUuid.ContainsKey(nameKey))
				{
					SkillsNamesMsg skillName = skillNameDict.Msgs[skillNameDict.NameToUuid[nameKey].ToString()];
					SimplifiedSkill newSkill = new()
					{
						Name = skillName.Content[1],
						MaxLevel = Convert.ToInt32(data.MaxLevel!.Substring(2)),
					};
					if (mhrsColors.ContainsKey(Convert.ToInt32(data.IconColor!.Replace("ITEM_ICON_COLOR_", ""))))
					{
						newSkill.WikiIconColor = mhrsColors[Convert.ToInt32(data.IconColor!.Replace("ITEM_ICON_COLOR_", ""))];
					}
					else
					{
						newSkill.WikiIconColor = "NOT AVAILABLE";
					}
					string explainKey = "PlayerSkill_" + skillEnum[data.Id!] + "_Explain";
					if (skillExplainDict.Msgs.ContainsKey(skillExplainDict.NameToUuid[explainKey].ToString()))
					{
						SkillsExplainMsg msg = skillExplainDict.Msgs[skillExplainDict.NameToUuid[explainKey].ToString()];
						if (!string.IsNullOrEmpty(msg.Content[1]) && !msg.Content[1].StartsWith("<COLOR FF0000>#Rejected#</COLOR>"))
						{
							newSkill.Description = msg.Content[1].Replace("\r\n", " ");
						}
					}
					int lvl = 1;
					foreach (string key in skillDetailsDict.NameToUuid.Keys.Where(x => x.StartsWith("PlayerSkill_" + skillEnum[data.Id!] + "_")))
					{
						Msg skillsDetails = skillDetailsDict.Msgs[skillDetailsDict.NameToUuid[key].ToString()];
						if (!string.IsNullOrEmpty(skillsDetails.Content[1]) && !skillsDetails.Content[1].StartsWith("<COLOR FF0000>#Rejected#</COLOR>"))
						{
							newSkill.LevelDetails.Add(lvl, new()
							{
								Description = skillsDetails.Content[1].Replace("\r\n", " ")
							});
						}
						lvl++;
					}
					simplifiedSkills.Add(newSkill);
				}
			}
			File.WriteAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\mhrs_skills_simplified.json", Newtonsoft.Json.JsonConvert.SerializeObject(simplifiedSkills.OrderBy(x => x.Name), Newtonsoft.Json.Formatting.Indented));
		}

		private static Dictionary<string, int> GetMHRISkillEnum()
		{
			return new()
			{
				{ "Pl_EquipSkill_000", 1 },
				{ "Pl_EquipSkill_001", 2 },
				{ "Pl_EquipSkill_002", 3 },
				{ "Pl_EquipSkill_003", 4 },
				{ "Pl_EquipSkill_004", 5 },
				{ "Pl_EquipSkill_005", 6 },
				{ "Pl_EquipSkill_006", 7 },
				{ "Pl_EquipSkill_007", 8 },
				{ "Pl_EquipSkill_008", 9 },
				{ "Pl_EquipSkill_009", 10 },
				{ "Pl_EquipSkill_010", 11 },
				{ "Pl_EquipSkill_011", 12 },
				{ "Pl_EquipSkill_012", 13 },
				{ "Pl_EquipSkill_013", 14 },
				{ "Pl_EquipSkill_014", 15 },
				{ "Pl_EquipSkill_015", 16 },
				{ "Pl_EquipSkill_016", 17 },
				{ "Pl_EquipSkill_017", 18 },
				{ "Pl_EquipSkill_018", 19 },
				{ "Pl_EquipSkill_019", 20 },
				{ "Pl_EquipSkill_020", 21 },
				{ "Pl_EquipSkill_021", 22 },
				{ "Pl_EquipSkill_022", 23 },
				{ "Pl_EquipSkill_023", 24 },
				{ "Pl_EquipSkill_024", 25 },
				{ "Pl_EquipSkill_025", 26 },
				{ "Pl_EquipSkill_026", 27 },
				{ "Pl_EquipSkill_027", 28 },
				{ "Pl_EquipSkill_028", 29 },
				{ "Pl_EquipSkill_029", 30 },
				{ "Pl_EquipSkill_030", 31 },
				{ "Pl_EquipSkill_031", 32 },
				{ "Pl_EquipSkill_032", 33 },
				{ "Pl_EquipSkill_033", 34 },
				{ "Pl_EquipSkill_034", 35 },
				{ "Pl_EquipSkill_035", 36 },
				{ "Pl_EquipSkill_036", 37 },
				{ "Pl_EquipSkill_037", 38 },
				{ "Pl_EquipSkill_038", 39 },
				{ "Pl_EquipSkill_039", 40 },
				{ "Pl_EquipSkill_040", 41 },
				{ "Pl_EquipSkill_041", 42 },
				{ "Pl_EquipSkill_042", 43 },
				{ "Pl_EquipSkill_043", 44 },
				{ "Pl_EquipSkill_044", 45 },
				{ "Pl_EquipSkill_045", 46 },
				{ "Pl_EquipSkill_046", 47 },
				{ "Pl_EquipSkill_047", 48 },
				{ "Pl_EquipSkill_048", 49 },
				{ "Pl_EquipSkill_049", 50 },
				{ "Pl_EquipSkill_050", 51 },
				{ "Pl_EquipSkill_051", 52 },
				{ "Pl_EquipSkill_052", 53 },
				{ "Pl_EquipSkill_053", 54 },
				{ "Pl_EquipSkill_054", 55 },
				{ "Pl_EquipSkill_055", 56 },
				{ "Pl_EquipSkill_056", 57 },
				{ "Pl_EquipSkill_057", 58 },
				{ "Pl_EquipSkill_058", 59 },
				{ "Pl_EquipSkill_059", 60 },
				{ "Pl_EquipSkill_060", 61 },
				{ "Pl_EquipSkill_061", 62 },
				{ "Pl_EquipSkill_062", 63 },
				{ "Pl_EquipSkill_063", 64 },
				{ "Pl_EquipSkill_064", 65 },
				{ "Pl_EquipSkill_065", 66 },
				{ "Pl_EquipSkill_066", 67 },
				{ "Pl_EquipSkill_067", 68 },
				{ "Pl_EquipSkill_068", 69 },
				{ "Pl_EquipSkill_069", 70 },
				{ "Pl_EquipSkill_070", 71 },
				{ "Pl_EquipSkill_071", 72 },
				{ "Pl_EquipSkill_072", 73 },
				{ "Pl_EquipSkill_073", 74 },
				{ "Pl_EquipSkill_074", 75 },
				{ "Pl_EquipSkill_075", 76 },
				{ "Pl_EquipSkill_076", 77 },
				{ "Pl_EquipSkill_077", 78 },
				{ "Pl_EquipSkill_078", 79 },
				{ "Pl_EquipSkill_079", 80 },
				{ "Pl_EquipSkill_080", 81 },
				{ "Pl_EquipSkill_081", 82 },
				{ "Pl_EquipSkill_082", 83 },
				{ "Pl_EquipSkill_083", 84 },
				{ "Pl_EquipSkill_084", 85 },
				{ "Pl_EquipSkill_085", 86 },
				{ "Pl_EquipSkill_086", 87 },
				{ "Pl_EquipSkill_087", 88 },
				{ "Pl_EquipSkill_088", 89 },
				{ "Pl_EquipSkill_089", 90 },
				{ "Pl_EquipSkill_090", 91 },
				{ "Pl_EquipSkill_091", 92 },
				{ "Pl_EquipSkill_092", 93 },
				{ "Pl_EquipSkill_093", 94 },
				{ "Pl_EquipSkill_094", 95 },
				{ "Pl_EquipSkill_095", 96 },
				{ "Pl_EquipSkill_096", 97 },
				{ "Pl_EquipSkill_097", 98 },
				{ "Pl_EquipSkill_098", 99 },
				{ "Pl_EquipSkill_099", 100 },
				{ "Pl_EquipSkill_100", 101 },
				{ "Pl_EquipSkill_101", 102 },
				{ "Pl_EquipSkill_102", 103 },
				{ "Pl_EquipSkill_103", 104 },
				{ "Pl_EquipSkill_104", 105 },
				{ "Pl_EquipSkill_105", 106 },
				{ "Pl_EquipSkill_106", 107 },
				{ "Pl_EquipSkill_107", 108 },
				{ "Pl_EquipSkill_108", 109 },
				{ "Pl_EquipSkill_109", 110 },
				{ "Pl_EquipSkill_110", 111 },
				{ "Pl_EquipSkill_200", 112 },
				{ "Pl_EquipSkill_201", 113 },
				{ "Pl_EquipSkill_202", 114 },
				{ "Pl_EquipSkill_203", 115 },
				{ "Pl_EquipSkill_204", 116 },
				{ "Pl_EquipSkill_205", 117 },
				{ "Pl_EquipSkill_206", 118 },
				{ "Pl_EquipSkill_207", 119 },
				{ "Pl_EquipSkill_208", 120 },
				{ "Pl_EquipSkill_209", 121 },
				{ "Pl_EquipSkill_210", 122 },
				{ "Pl_EquipSkill_211", 123 },
				{ "Pl_EquipSkill_212", 124 },
				{ "Pl_EquipSkill_213", 125 },
				{ "Pl_EquipSkill_214", 126 },
				{ "Pl_EquipSkill_215", 127 },
				{ "Pl_EquipSkill_216", 128 },
				{ "Pl_EquipSkill_217", 129 },
				{ "Pl_EquipSkill_218", 130 },
				{ "Pl_EquipSkill_219", 131 },
				{ "Pl_EquipSkill_220", 132 },
				{ "Pl_EquipSkill_221", 133 },
				{ "Pl_EquipSkill_222", 134 },
				{ "Pl_EquipSkill_223", 135 },
				{ "Pl_EquipSkill_224", 136 },
				{ "Pl_EquipSkill_225", 137 },
				{ "Pl_EquipSkill_226", 138 },
				{ "Pl_EquipSkill_227", 139 },
				{ "Pl_EquipSkill_228", 140 },
				{ "Pl_EquipSkill_229", 141 },
				{ "Pl_EquipSkill_230", 142 },
				{ "Pl_EquipSkill_231", 143 },
				{ "Pl_EquipSkill_232", 144 },
				{ "Pl_EquipSkill_233", 145 },
				{ "Pl_EquipSkill_234", 146 },
				{ "Pl_EquipSkill_235", 147 },
				{ "Pl_EquipSkill_236", 148 },
				{ "Pl_EquipSkill_237", 149 },
				{ "Pl_EquipSkill_238", 150 },
				{ "Pl_EquipSkill_239", 151 },
				{ "Pl_EquipSkill_240", 152 },
				{ "Pl_EquipSkill_241", 153 },
				{ "Pl_EquipSkill_242", 154 },
				{ "Pl_EquipSkill_243", 155 },
				{ "Pl_EquipSkill_244", 156 },
				{ "Pl_EquipSkill_245", 157 },
				{ "Pl_EquipSkill_246", 158 },
				{ "Pl_EquipSkill_247", 159 },
				{ "Pl_EquipSkill_248", 160 },
				{ "Pl_EquipSkill_249", 161 },
				{ "Pl_EquipSkill_250", 162 },
				{ "Pl_EquipSkill_251", 163 },
				{ "Pl_EquipSkill_252", 164 },
				{ "Pl_EquipSkill_253", 165 },
				{ "Pl_EquipSkill_254", 166 },
				{ "Pl_EquipSkill_255", 167 },
			};
		}
	}

	public class SimplifiedSkill
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Level { get; set; }
		public int MaxLevel { get; set; } = 1;
		public string WikiIconColor { get; set; } = string.Empty;
		public string? SkillGrantedByBonus1 { get; set; }
		public int? SkillGrantedByBonus1_PiecesRequired { get; set; }
		public string? SkillGrantedByBonus2 { get; set; }
		public int? SkillGrantedByBonus2_PiecesRequired { get; set; }
		public string Description { get; set; } = string.Empty;
		public Dictionary<int, SkillLevelInfo> LevelDetails { get; set; } = [];
	}

	public class SimplifiedSkill2
	{
		public long Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Level { get; set; }
		public int MaxLevel { get; set; } = 1;
		public string WikiIconColor { get; set; } = string.Empty;
		public string? SkillGrantedByBonus1 { get; set; }
		public int? SkillGrantedByBonus1_PiecesRequired { get; set; }
		public string? SkillGrantedByBonus2 { get; set; }
		public int? SkillGrantedByBonus2_PiecesRequired { get; set; }
		public string Description { get; set; } = string.Empty;
		public Dictionary<int, string> LevelDetails { get; set; } = [];
	}
}
