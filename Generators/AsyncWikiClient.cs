using System.Reflection;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace MediawikiTranslator
{
	public class AsyncWikiClient : IDisposable
	{
		public delegate void WikiClientEventHandler(object source, WikiClientLogEventArgs e);
		public event WikiClientEventHandler? ClientLog;
		public Task[] AsyncTasks { get; set; } = [];

		private readonly WikiClient _client;
		private WikiSite? _site;

		public AsyncWikiClient()
		{
			_client = new()
			{
				ClientUserAgent = "MHWikiToolkit/" + Assembly.GetEntryAssembly()!.GetName().Version + " " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			};
			_site = new(_client, "https://monsterhunterwiki.org/api.php");
		}

		public WikiPage GetPage(string pageName)
		{
			return new WikiPage(_site!, pageName);
		}

		public async Task Execute()
		{
			DateTime start = DateTime.Now;
			await _site!.Initialization;
			try
			{
				await _site!.LoginAsync();
				ClientLog!(this, new WikiClientLogEventArgs() { LogMessage = "Logged in." });
				Console.WriteLine($"================================");
				Console.WriteLine($"Processing task queue...");
				Console.WriteLine($"================================");
				Task entireTask = Task.WhenAll(AsyncTasks);
				bool taskIsFinished = false;
				while (!taskIsFinished)
				{
					var timer = Task.Delay(250);
					await Task.WhenAny(entireTask, timer);
					if (entireTask.IsCompleted)
					{
						taskIsFinished = true;
					}
					int progress = AsyncTasks.Count(x => x.IsCompleted);
					Console.WriteLine($"{Math.Round((double)progress / AsyncTasks.Length, 2) * 100}% - ({progress}/{AsyncTasks.Length})");
				}
				entireTask.Wait();
				Console.WriteLine("======================================================");
				Console.WriteLine("Finished!");
				TimeSpan elapsed = DateTime.Now - start;
				Console.WriteLine("Elapsed: " + elapsed.ToString());
			}
			catch (WikiClientException ex)
			{
				ClientLog!(this, new WikiClientLogEventArgs() { LogMessage = "An exception has occurred!", LogType = WikiClientLogTypes.ERROR, Exception = ex });
			}
			await _site!.LogoutAsync();
			ClientLog!(this, new WikiClientLogEventArgs() { LogMessage = "Logged out." });
		}

		public void Dispose()
		{
			_client.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	public class WikiClientLogEventArgs() : EventArgs
	{
		public required string LogMessage { get; set; }
		public WikiClientLogTypes LogType { get; set; } = WikiClientLogTypes.INFO;
		public Exception? Exception { get; set; }
	}

	public enum WikiClientLogTypes
	{
		INFO,
		ERROR,
		WARNING,
		SUCCESS
	}

	public static class WikiSiteExtensions
	{
		public static async Task LoginAsync(this WikiSite site)
		{
			string? usr = System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername");
			string? pswd = System.Configuration.ConfigurationManager.AppSettings.Get("WikiPassword");
			if (usr == null || pswd == null)
			{
				throw new Exception("Credentials not defined in config.xml! Please define your credentials.");
			}
			await site.LoginAsync(usr, pswd);
		}
	}
}
