using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CardiacPatientMonitoring.Data;

namespace CardiacPatientMonitoring.Tests;

public class SqliteWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;
    public readonly QueryCountInterceptor QueryCounter = new();

    public SqliteWebApplicationFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        using (var pragmaCommand = _connection.CreateCommand())
        {
            pragmaCommand.CommandText = "PRAGMA foreign_keys = OFF;";
            pragmaCommand.ExecuteNonQuery();
        }

        // Create the schema up-front using the same connection,
        // because Cardiac's Program.cs seeds data unconditionally
        // (not guarded by IsDevelopment), before we get control back.
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(_connection);

        using var schemaContext = new AppDbContext(optionsBuilder.Options);
        schemaContext.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d => d.ServiceType.Namespace != null &&
                            d.ServiceType.Namespace.Contains("EntityFrameworkCore"))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
                options.AddInterceptors(QueryCounter);
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }
}