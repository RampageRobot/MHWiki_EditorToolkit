using Microsoft.AspNetCore.Mvc;

namespace WebToolkit.Controllers
{
	[Route("WeaponTreeController")]
	public class WeaponTreeController : Controller
	{
		[HttpPost("GenerateTree")]
		public string GenerateTree(string json, string sharpnessBase, int maxSharpnessCount, string pathName, string defaultIcon)
		{
			try
			{
				return MediawikiTranslator.Generators.WeaponTree.ParseJson(json, sharpnessBase, maxSharpnessCount, pathName, defaultIcon);
			}
			catch (Exception ex)
			{
				Response.Clear();
				Response.StatusCode = 500;
				Response.WriteAsync(ex.Message);
				return string.Empty;
			}
		}

        [HttpPost("ParseCsv")]
        public string ParseCsv(string csvFile, bool duplicateSharpness)
        {
            try
            {
                return MediawikiTranslator.Generators.WeaponTree.ParseCsv(csvFile, duplicateSharpness);
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
