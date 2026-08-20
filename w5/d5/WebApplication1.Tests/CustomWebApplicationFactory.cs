using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using WebApplication1.Data;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(
                    Guid.NewGuid().ToString());
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                "Test",
                options => { });
        });
    }

protected override IHost CreateHost(IHostBuilder builder)
{
    var host = base.CreateHost(builder);

    using var scope = host.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    db.Database.EnsureCreated();

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    SeedRole(roleManager, "User");
    SeedRole(roleManager, "Admin");

    return host;
}
    private static void SeedRole(
        RoleManager<IdentityRole> roleManager,
        string roleName)
    {
        if (!roleManager.RoleExistsAsync(roleName)
            .GetAwaiter()
            .GetResult())
        {
            var result = roleManager
                .CreateAsync(new IdentityRole(roleName))
                .GetAwaiter()
                .GetResult();

            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Failed to create role '{roleName}': " +
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }
    }
}

public class TestAuthHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                "test-user-id"),

            new Claim(
                ClaimTypes.Name,
                "testuser"),

            new Claim(
                ClaimTypes.Email,
                "test@example.com"),

            new Claim(
                ClaimTypes.Role,
                "User")
        };

        var identity = new ClaimsIdentity(
            claims,
            "Test");

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            "Test");

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}