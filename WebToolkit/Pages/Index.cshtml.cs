using MHWilds = MediawikiTranslator.Models.Data.MHWilds;
using MHRS = MediawikiTranslator.Models.Data.MHRS;
using MHWI = MediawikiTranslator.Models.Data.MHWI;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using MediawikiTranslator.Generators;
using MediawikiTranslator.Models.Data.MHRS;
using MediawikiTranslator.Models.Weapon;
using System.IO;
using Newtonsoft.Json;
using MediawikiTranslator;
using MediawikiTranslator.Models.Data.MHWI;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using MediawikiTranslator.Models.Monsters;

namespace WebToolkit.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
		}

		public async Task OnGet()
		{
			Utilities.GetMonstersFiles();
			await Utilities.MonsterAppearanceGenerator(new MonsterAppearanceGeneratorOptions()
			{
				WhitelistGameAcronyms = ["MHST3"],
				UpdatePages = true,
				GenerateNewPages = true,
				RefreshProgressLists = true
			});
			//Task list:
			//2) Follow up on Talk Page extension
			//3) Work on getting files extracted for old games, then getting any appropriate tools to view files
			//await Utilities.RenderGenerator(new RenderGeneratorOptions() { UpdatePages = false, GenerateMissingRendersReport = true, RefreshProgressLists = true });
		}
    }
}
