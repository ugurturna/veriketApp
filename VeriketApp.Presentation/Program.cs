using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VeriketApp.Core;

namespace VeriketApp.Presentation
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            var configuration = new ConfigurationBuilder().SetBasePath(Path.Combine(AppContext.BaseDirectory)).AddJsonFile("appsettings.json", false, true).AddEnvironmentVariables().Build();

            var serviceProvider = new ServiceCollection()
                .Configure<AppSettings>(x => configuration.GetSection("AppSettings").Bind(x))
               .AddScoped(x => x.GetRequiredService<IOptions<AppSettings>>().Value)
               .AddTransient<Form1>()
               .BuildServiceProvider();



            ApplicationConfiguration.Initialize();
            var mainForm = serviceProvider.GetRequiredService<Form1>();

            Application.Run(mainForm);
        }
    }
}