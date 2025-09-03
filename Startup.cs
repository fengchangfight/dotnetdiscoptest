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
        // Register MS.DI services first (IMusic/Jazz)
        services.AddTransient<IMusic, Jazz>();
        
        // Register third-party services (only in MS.DI)
        services.AddTransient<IFoo, ThirdPartyFoo>();
        services.AddTransient<IBar, ThirdPartyBar>();
        // ... all other third-party dependencies
        
        // Register the Jab container with MS.DI service provider
        services.AddSingleton<JabServiceContainer>(provider =>
        {
            return new JabServiceContainer(provider);
        });
        
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
