using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MixDiTest.Services;

namespace MixDiTest;

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
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureServices((context, services) =>
            {
                // Register the Jab container
                services.AddSingleton<JabServiceContainer>();
                
                // Register the mixed service provider
                services.AddSingleton<MixedServiceProvider>(provider =>
                {
                    var jabContainer = provider.GetRequiredService<JabServiceContainer>();
                    return new MixedServiceProvider(jabContainer, provider);
                });
                
                // Replace the default service provider and scope factory with our mixed one
                services.AddSingleton<IServiceProvider>(provider =>
                    provider.GetRequiredService<MixedServiceProvider>());
                services.AddSingleton<IServiceScopeFactory>(provider =>
                    provider.GetRequiredService<MixedServiceProvider>());
            });
}
