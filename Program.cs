using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        //.UseUrls("https://192.168.0.19:4000");
                        .UseUrls("http://192.168.0.19:4000");
                        //.UseUrls("http://localhost:4000");
                        //.UseUrls("https://webapijanusz.azurewebsites.net:4000");
                });
    }
}
