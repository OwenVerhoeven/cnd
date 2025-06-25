using cnd;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["UseInMemoryDb"] = "true",  // Set to true to use InMemory DB in tests by default
                ["Jwt:Key"] = "DezeSleutelMoetMinimaal32TekensLangZijn!",
                ["Jwt:Issuer"] = "JournalApi",
                ["Jwt:Audience"] = "JournalApiUser"
            };
            config.AddInMemoryCollection(dict);
        });

        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registrations
            var descriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<JournalDbContext>)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Read config to decide which DB provider to use
            var sp = services.BuildServiceProvider();
            var config = sp.GetRequiredService<IConfiguration>();
            bool useInMemory = config.GetValue<bool>("UseInMemoryDb");

            if (useInMemory)
            {
                // Use InMemory for testing
                services.AddDbContext<JournalDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            }
            else
            {
                // Use Postgres if flag is false
                var connectionString = config.GetConnectionString("DefaultConnection")
                                       ?? System.Environment.GetEnvironmentVariable("CONNECTION_STRING")
                                       ?? throw new InvalidOperationException("No connection string configured.");

                services.AddDbContext<JournalDbContext>(options =>
                {
                    options.UseNpgsql(connectionString);
                });
            }

            // Ensure database is created before tests run
            var sp2 = services.BuildServiceProvider();
            using var scope = sp2.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<JournalDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
