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
