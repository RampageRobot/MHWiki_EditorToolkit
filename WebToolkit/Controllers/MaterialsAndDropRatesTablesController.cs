using Microsoft.AspNetCore.Mvc;

namespace WebToolkit.Controllers
{
	[Route("MaterialsAndDropRatesTablesController")]
	public class MaterialsAndDropRatesTablesController : Controller
	{
		[HttpPost("GenerateTables")]
		public string GenerateTables(string json)
		{
			return MediawikiTranslator.Generators.MaterialsAndDropTables.ParseJson(json);
		}
	}
}
