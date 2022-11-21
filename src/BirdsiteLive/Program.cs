using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace BirdsiteLive
{
    public class Program
    {
        public static string VERSION = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(3) + "+pasture";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var startup = new Startup(builder.Configuration);

            builder.Host.UseLamar(startup.ConfigureContainer);

            startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            startup.Configure(app, app.Environment);
            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
