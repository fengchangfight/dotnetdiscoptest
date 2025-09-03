using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MixDiTest.Interfaces;
using MixDiTest.Services;

namespace MixDiTest;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
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
        
        // Register MS.DI services (IMusic/Jazz)
        services.AddTransient<IMusic, Jazz>();
        
        // Add controllers
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
