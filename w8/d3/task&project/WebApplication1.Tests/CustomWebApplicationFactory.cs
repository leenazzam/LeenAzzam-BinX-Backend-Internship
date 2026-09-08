using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using WebApplication1.Data;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName =
        $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;

        var context =
            services.GetRequiredService<AppDbContext>();

        context.Database.EnsureCreated();

        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        CreateRole(roleManager, "User");
        CreateRole(roleManager, "Admin");

        return host;
    }

    private static void CreateRole(
        RoleManager<IdentityRole> roleManager,
        string roleName)
    {
        var exists = roleManager
            .RoleExistsAsync(roleName)
            .GetAwaiter()
            .GetResult();

        if (!exists)
        {
            var result = roleManager
                .CreateAsync(new IdentityRole(roleName))
                .GetAwaiter()
                .GetResult();

            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Failed to create role '{roleName}': " +
                    string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description)
                    )
                );
            }
        }
    }
}