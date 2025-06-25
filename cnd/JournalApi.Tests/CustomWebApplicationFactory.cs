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
            // Override configuratie zodat UseInMemoryDb altijd false is (of niet aanwezig)
            var dict = new Dictionary<string, string?>
            {
                ["UseInMemoryDb"] = "false",  // Nooit InMemory gebruiken
                ["Jwt:Key"] = "DezeSleutelMoetMinimaal32TekensLangZijn!",
                ["Jwt:Issuer"] = "JournalApi",
                ["Jwt:Audience"] = "JournalApiUser"
            };
            config.AddInMemoryCollection(dict);
        });

        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Verwijder alle bestaande DbContext-registraties
            var descriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<JournalDbContext>)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Lees configuratie opnieuw om connection string te pakken
            var sp = services.BuildServiceProvider();
            var config = sp.GetRequiredService<IConfiguration>();

            // Haal connection string op voor Postgres
            var connectionString = config.GetConnectionString("DefaultConnection")
                                   ?? System.Environment.GetEnvironmentVariable("CONNECTION_STRING")
                                   ?? throw new InvalidOperationException("No connection string configured.");

            // Registreer DbContext met Postgres provider
            services.AddDbContext<JournalDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            // Database creëren voordat tests starten
            var sp2 = services.BuildServiceProvider();
            using var scope = sp2.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<JournalDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
