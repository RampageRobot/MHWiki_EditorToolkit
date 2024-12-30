using MediawikiTranslator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace WebToolkit.Pages
{
    public class MaterialsAndDropTableGeneratorModel : PageModel
    {
        public string MHWIDropdowns { get; set; } = string.Empty;
        public string MHRSDropdowns { get; set; } = string.Empty;
        public void OnGet()
        {
            MediawikiTranslator.Models.Data.MHWI.Items[] mhwiItems = Utilities.GetMHWIItems();
			MediawikiTranslator.Models.Data.MHRS.Items[] mhrsItems = Utilities.GetMHRSItems();
			MHWIDropdowns = string.Join("\r\n", mhwiItems
				.Where(x => x.Name != "Unavailable")
				.OrderBy(x => x.Name)
				.Select(x => "<option value=\"" + x.Id + "\">" + x.Name + "</option>")
				.ToArray());
			MHRSDropdowns = string.Join("\r\n", mhrsItems
				.Where(x => x.Name != "Unavailable" && !x.Name.Contains("#Rejected"))
				.OrderBy(x => x.Name)
				.Select(x => "<option value=\"" + x.Id + "\">" + x.Name + "</option>")
				.ToArray());
		}
	}
}
