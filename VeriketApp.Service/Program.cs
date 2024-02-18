using Microsoft.Extensions.Configuration;
using CliWrap;
using System.Reflection;
using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VeriketApp.Core;
using VeriketApp.Service;
using VeriketApp.Service.Services;


var Configuration = new ConfigurationBuilder().SetBasePath(Path.Combine(AppContext.BaseDirectory)).AddJsonFile("appsettings.json", false, true).AddEnvironmentVariables().Build();
var baseConfig = Configuration.GetSection("AppSettings").Get<AppSettings>();
if (args is { Length: 1 })
{
    try
    {
        string executablePath = Assembly.GetEntryAssembly().Location;
        executablePath = Path.ChangeExtension(executablePath, "exe");

        if (args[0] is "/Install")
        {

            await Cli.Wrap("sc.exe")
                .WithArguments(new[] { "create", baseConfig.ServiceName, $"binPath={executablePath}"
                , "start=auto", $"displayname={baseConfig.DisplayName}" })
                .ExecuteAsync();

            await Cli.Wrap("sc.exe")
                .WithArguments(new[] { "description", baseConfig.ServiceName, baseConfig.Description })
                .ExecuteAsync();

            Thread.Sleep(2500);

            await Cli.Wrap("sc.exe")
               .WithArguments(new[] { "start", baseConfig.ServiceName })
               .ExecuteAsync();

        }
        else if (args[0] is "/Uninstall")
        {
            try
            {
                await Cli.Wrap("sc.exe")
              .WithArguments(new[] { "stop", baseConfig.ServiceName })
              .ExecuteAsync();
            }
            catch (Exception) { }

         
            await Cli.Wrap("sc.exe")
                .WithArguments(new[] { "delete", baseConfig.ServiceName })
                .ExecuteAsync();
        }
    }
    catch (Exception ext)
    {
        try
        {
            await Cli.Wrap("sc.exe")
                .WithArguments(new[] { "delete", baseConfig.ServiceName })
                .ExecuteAsync();
        } catch { }

    }


    return;
}


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = baseConfig.ServiceName;
});
builder.Services.AddTransient<ICustomLogService, CustomLogService>();
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<AppSettings>();



var host = builder.Build();


host.Run();