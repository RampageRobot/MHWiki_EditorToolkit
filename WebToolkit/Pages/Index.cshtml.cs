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
            foreach (string monsterName in Directory.EnumerateDirectories(@"C:\Users\mkast\Desktop\test monster stuff\MHWI").Select(x => new DirectoryInfo(x).Name).Where(x => x != "TranslationQueue" && !x.Contains("Training")))
            {
                MediawikiTranslator.Models.Monsters.Monster mon = new(monsterName);
            }
		}
    }
}
