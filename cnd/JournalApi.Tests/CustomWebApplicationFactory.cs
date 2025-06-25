using cnd;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Verwijder de bestaande DbContext configuratie (die PostgreSQL gebruikt)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<JournalDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Voeg DbContext toe met InMemoryDatabase
            services.AddDbContext<JournalDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Optionele initialisatie
            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<JournalDbContext>();
                db.Database.EnsureCreated();
            }
        });
    }
}
