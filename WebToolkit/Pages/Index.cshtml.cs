using MHWilds = MediawikiTranslator.Models.Data.MHWilds;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
