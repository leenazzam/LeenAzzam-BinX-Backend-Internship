using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CardiacPatientMonitoring.Tests;

public class PatientsSummaryQueryCountTests : IClassFixture<SqliteWebApplicationFactory>
{
    private readonly SqliteWebApplicationFactory _factory;

    public PatientsSummaryQueryCountTests(SqliteWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> GetAdminClientAsync()
    {
        var client = _factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "admin@cardiac.com",
            Password = "Admin123!"
        });

        loginResponse.EnsureSuccessStatusCode();

        var body = await loginResponse.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(body).RootElement.GetProperty("token").GetString();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    [Fact]
    public async Task GetPatientsSummary_ExecutesExactlyOneQuery()
    {
        var client = await GetAdminClientAsync();

        // Force a cache miss so the request actually reaches the database,
        // regardless of what earlier test runs left in Redis.
        using var scope = _factory.Services.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        await cache.RemoveAsync("patients:summary");

        _factory.QueryCounter.Reset();

        var response = await client.GetAsync("/api/patients/summary");
        response.EnsureSuccessStatusCode();

        Assert.Equal(1, _factory.QueryCounter.QueryCount);
    }
}