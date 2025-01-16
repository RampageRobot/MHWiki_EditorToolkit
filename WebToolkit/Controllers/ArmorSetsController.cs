using Microsoft.AspNetCore.Mvc;
using MediawikiTranslator.Generators;

namespace WebToolkit.Controllers
{
    [Route("ArmorSetsController")]
    public class ArmorSetsController : Controller
	{
		[HttpPost("GenerateSet")]
		public string GenerateSet(string json)
		{
			try
			{
				return ArmorSets.GenerateFromJson(json);
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
