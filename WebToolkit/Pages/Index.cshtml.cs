using MHWilds = MediawikiTranslator.Models.Data.MHWilds;
using MHRS = MediawikiTranslator.Models.Data.MHRS;
using MHWI = MediawikiTranslator.Models.Data.MHWI;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using MediawikiTranslator.Generators;
using MediawikiTranslator.Models.Data.MHRS;
using MediawikiTranslator.Models.ArmorSets;
using System.IO;
using Newtonsoft.Json;
using MediawikiTranslator;

namespace WebToolkit.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
			//MHRS.Armor.GetSimplifiedArmorData();
			//MHRS.Skills.GetDecorationsBySkill();
			//System.IO.File.WriteAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\mhrs skills simplified.json", JsonConvert.SerializeObject(MediawikiTranslator.Generators.Skills.GetSimplifiedSkillsMHRS(), Formatting.Indented));
			//Weapon.MassGenerate("MHWI");
			//         ArmorSets.MassGenerate("MHWI");
			//Weapon.MassGenerate("MHRS");
			//ArmorSets.MassGenerate("MHRS");
			Utilities.UploadWeaponsWithAPI("MHWI").Wait();
			//         MHWI.SkillDescriptions.WriteSimplifiedSkills();
			//         MHWI.SkillDescriptions.GetDecorationsBySkill();
			//         MHWI.Armor.GetSimplifiedArmorData();
			//MHWI.BlademasterData.GetSimplifiedWeaponData();
			//var test = MHRS.Quests.GetAllQuests();
			//Debugger.Break();
		}
    }
}
