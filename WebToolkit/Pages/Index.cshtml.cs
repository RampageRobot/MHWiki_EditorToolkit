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
		}
    }
}
