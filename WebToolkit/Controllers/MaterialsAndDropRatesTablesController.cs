using Microsoft.AspNetCore.Mvc;

namespace WebToolkit.Controllers
{
	[Route("MaterialsAndDropRatesTablesController")]
	public class MaterialsAndDropRatesTablesController : Controller
	{
		[HttpPost("GenerateTables")]
		public string GenerateTables(string json)
		{
			try
			{
				return MediawikiTranslator.Generators.MaterialsAndDropTables.ParseJson(json);
			}
			catch (Exception ex)
			{
				Response.Clear();
				Response.StatusCode = 500;
				Response.WriteAsync(ex.Message);
				return string.Empty;
			}
		}
	}
}
