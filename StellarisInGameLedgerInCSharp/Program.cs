using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StellarisInGameLedgerInCSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

		public static IWebHost BuildWebHost(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
						  .ConfigureAppConfiguration((_, builder) => builder.AddJsonFile("appsettings.json", true))
						  .UseStartup<Startup>()
						  .Build();
		}
	}
}
