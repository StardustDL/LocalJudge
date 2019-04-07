using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace StarOJ.Server.API
{
    public class Program
    {
        public static uint HttpPort { get; set; }

        public static uint HttpsPort { get; set; }

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
.UseStartup<Startup>();
        }
    }
}
