using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WebToolkit
{
    public class Program
    {
        public static void Main(string[] args)
        {
			if (File.Exists(@"D:\Wiki Files\wikicredentials.txt"))
			{
				string[] lines = File.ReadAllLines(@"D:\Wiki Files\wikicredentials.txt");
				System.Configuration.ConfigurationManager.AppSettings.Set("WikiUsername", lines[0]);
				System.Configuration.ConfigurationManager.AppSettings.Set("WikiPassword", lines[1]);
				System.Configuration.ConfigurationManager.AppSettings.Set("DesktopPath", lines[2]);
			}
			var builder = WebApplication.CreateBuilder(args);
			// Add services to the container.
			builder.Services.AddRazorPages();
			builder.Services.AddControllersWithViews();

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
			app.MapDefaultControllerRoute();
#if ! DEBUG
			OpenBrowser("http://localhost:5000/");
#endif
			app.Run();
        }

		public static void OpenBrowser(string url)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Process.Start("xdg-open", url);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start("open", url);
			}
			else
			{
				// throw 
			}
		}
	}
}
