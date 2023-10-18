using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using System.Diagnostics;
using System.Reflection;

namespace webapi
{
	// [10-14-23 MWD] Set as Startup object in Application properties
	public class Program
	{
		public static void Main(string[] args)
		{
			//BuildWebHost(args).Run();
			if (Debugger.IsAttached || args.Contains("--debug"))
			{
				BuildWebHost(args).Run();
			}
			else
			{
				var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Directory.SetCurrentDirectory(path);
				BuildWebHost(args).Run();
			}
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
				})
				.Build();
	}
}
