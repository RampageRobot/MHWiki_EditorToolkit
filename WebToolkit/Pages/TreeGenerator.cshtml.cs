using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebToolkit.Pages
{
    public class TreeGeneratorModel : PageModel
	{
		[BindProperty]
		public IFormFile? LoadedData { get; set; }
		[BindProperty]
		public string LoadedSaveData { get; set; } = string.Empty;

		public void OnGet()
        {
        }
	}
}
