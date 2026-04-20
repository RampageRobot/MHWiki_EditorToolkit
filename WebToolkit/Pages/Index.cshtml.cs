using MediawikiTranslator;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
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
			Utilities.SetCredentials();
			//var temp = MediawikiTranslator.Models.Data.MHRS.FlinchBreakThresholds.GetWebToolkitData("em092_00");
			//Debugger.Break();
			await Utilities.UploadRenderDB();
			//await Utilities.UploadDBs();
			//await Utilities.UploadWeaponsWithAPI("MHWI", false, true, true);
			//await Utilities.UploadMH3Quests(@"D:\MH_Data Repo\MH_Data\Parsed Files\MH3\mh3 quests");
			//await Utilities.RenderGenerator(new RenderGeneratorOptions() { UpdatePages = false, GenerateMissingRendersReport = true, RefreshProgressLists = true });
		}
	}
}
