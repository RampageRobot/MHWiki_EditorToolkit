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
        [HttpPost("MassGenerateSet")]
        public async Task<string> MassGenerateSet(string data, string game)
        {
            try
            {
                return await ArmorSets.GenerateFromXlsx(data, game);
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.StatusCode = 500;
                Response.WriteAsync(ex.Message).RunSynchronously();
                return string.Empty;
            }
        }
    }
}
