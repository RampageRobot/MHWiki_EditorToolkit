using Newtonsoft.Json;
using System.Data;
using System.Text;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;
using WikiClientLibrary;
using MediawikiTranslator.Models.Weapon;
using MediawikiTranslator.Generators;
using WikiClientLibrary.Generators;
using System.Diagnostics;
using MediawikiTranslator.Models.Data.MHWI;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;

namespace MediawikiTranslator
{
	public static class Utilities
    {
		public static Models.Data.MHRS.Items[] GetMHRSItems()
		{
			return JsonConvert.DeserializeObject<Models.Data.MHRS.Items[]>(Properties.Resources.mhrs_items, Models.Data.MHWI.Converter.Settings)!;
		}

		public static Models.Data.MHWI.Items[] GetMHWIItems()
		{
			return JsonConvert.DeserializeObject<Models.Data.MHWI.Items[]>(Encoding.UTF8.GetString(Properties.Resources.mhwi_items), Models.Data.MHWI.Converter.Settings)!;
		}

		public static DirectoryInfo GetWorkspace()
        {
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), @"\MHWikiToolkit_Generation\"));
        }

		public static async Task FixWikiPages(string game)
		{
			Console.WriteLine("Please enter your username.");
			string username = Console.ReadLine()!;
			Console.WriteLine("Please enter your password.");
			string password = Console.ReadLine()!;
			WebToolkitData[] bmData = [.. BlademasterData.GetToolkitData().Where(x => x.Name != "HARDUMMY")];
			WebToolkitData[] gData = [.. GunnerData.GetToolkitData().Where(x => x.Name != "HARDUMMY")];
			WebToolkitData[] src = new WebToolkitData[bmData.Length + gData.Length];
			bmData.CopyTo(src, 0);
			gData.CopyTo(src, bmData.Length);
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.0.8 RampageRobot"
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.LoginAsync(username, password);
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				var rcg = new RecentChangesGenerator(site)
				{
					UserName = username
				};
				int cntr = 1;
				DateTime lastWrite = DateTime.MinValue;
				foreach (WebToolkitData data in src.Where(x => x.Name!.Contains("Safi's")))
				{
					string name = data.Name! + " (MR)";
					var page = new WikiPage(site, $"{name} ({game})");
					try
					{
						await page.MoveAsync(data.Name! + " (MHWI)", "Weirdness with Safi weapons on the backend causing duplicates.", PageMovingOptions.NoRedirect);
						Console.WriteLine("Moved page " + name);
					}
					catch (OperationFailedException _)
					{
					}
					cntr++;
					if (DateTime.Now - lastWrite <= new TimeSpan(0, 1, 0) && cntr >= 8)
					{
						Thread.Sleep((new TimeSpan(0, 1, 0) - (DateTime.Now - lastWrite)).Milliseconds);
						cntr = 1;
					}
					else
					{
						cntr = 1;
					}
				}
				Debugger.Break();
				await site.LogoutAsync();
			}
		}

		public static async Task UploadWeaponsWithAPI(string game)
		{
			Console.WriteLine("Please enter your username.");
			string username = Console.ReadLine()!;
			Console.WriteLine("Please enter your password.");
			string password = Console.ReadLine()!;
			Dictionary<WebToolkitData, string> src = Weapon.MassGenerate(game, false);
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.0.8 " + username
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.LoginAsync(username, password);
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				int cntr = 1;
				DateTime lastWrite = DateTime.MinValue;
				foreach (KeyValuePair<WebToolkitData, string> data in src)
				{
					string name = data.Key.Name!;
					if (src.Keys.Count(x => x.Name == data.Key.Name!) > 1 && Weapon.GetRank(game, data.Key.Rarity) == "MR")
					{
						name = data.Key.Name + " (MR)";
					}
					WikiPage page = new(site, $"{name}_({game})");
					await page.EditAsync(new WikiPageEditOptions()
					{
						Content = data.Value,
						Minor = false,
						Bot = true,
						Summary = "Auto-updated using the API through MH Wiki Toolkit."
					});
					cntr++;
					Console.WriteLine("Edited page " + name);
					if (DateTime.Now - lastWrite <= new TimeSpan(0, 1, 0) && cntr >= 8)
					{
						Thread.Sleep((new TimeSpan(0, 1, 0) - (DateTime.Now - lastWrite)).Milliseconds);
						cntr = 1;
					}
					else
					{
						cntr = 1;
					}
				}
				await site.LogoutAsync();
			}
		}
	}
}
