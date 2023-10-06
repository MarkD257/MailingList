using webapi.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace webapi.FunctionalTests.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<WebMarker>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // This is used to create a scope for seeding data

        var host = builder.Build();
        host.Start();

        var serviceProvider = host.Services;

        using (var scope = serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            var logger = scopedServices
                .GetRequiredService<ILogger<CustomWebApplicationFactory>>();

            // Ensure the database is created.
            db.Database.EnsureCreated();

            try
            {
                SeedData.PopulateTestData(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database. Error: {exceptionMessage}", ex.Message);
            }
        }

        return host;
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            UseInMemoryDatabase(services);

            // Register any other test-specific services here
        });
    }

    private static void UseInMemoryDatabase(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        string inMemoryCollectionName = Guid.NewGuid().ToString();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(inMemoryCollectionName));
    }
}