using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

public class ProjectsSummaryQueryCountTests : IClassFixture<SqliteWebApplicationFactory>
{
    private readonly SqliteWebApplicationFactory _factory;

    public ProjectsSummaryQueryCountTests(SqliteWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();

        var email = $"summarytest_{Guid.NewGuid()}@example.com";
        var password = "TestPass123!";

        await client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult!.Token);

        return client;
    }

    [Fact]
    public async Task GetProjectsSummary_ExecutesExactlyOneQuery()
    {
        var client = await GetAuthenticatedClientAsync();

        for (int i = 1; i <= 3; i++)
        {
            var projectResponse = await client.PostAsJsonAsync("/api/projects", new
            {
                Name = $"Query Count Project {i}",
                Description = "Seed project for N+1 regression test"
            });

            var createdProject = await projectResponse.Content
                .ReadFromJsonAsync<WebApplication1.models.Project>();

            await client.PostAsJsonAsync("/api/tasks", new
            {
                Title = $"Task {i}",
                Status = "Pending",
                DueDate = DateTime.UtcNow.AddDays(3),
                ProjectId = createdProject!.Id
            });
        }

        _factory.QueryCounter.Reset();

        var response = await client.GetAsync("/api/projects/summary");
        response.EnsureSuccessStatusCode();

        Assert.Equal(1, _factory.QueryCounter.QueryCount);
    }
}