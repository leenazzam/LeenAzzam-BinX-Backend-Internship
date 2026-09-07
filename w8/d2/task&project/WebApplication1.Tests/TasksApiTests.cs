using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebApplication1.models;
using Xunit;using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public class TasksApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TasksApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

   private async Task<HttpClient> GetAuthenticatedClientAsync()
{
    var client = _factory.CreateClient();

    using var scope = _factory.Services.CreateScope();

    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(
            new IdentityRole("User"));
    }

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(
            new IdentityRole("Admin"));
    }

    var email =
        $"testuser_{Guid.NewGuid()}@example.com";

    var password = "TestPass123!";

    var registerResponse =
        await client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                Email = email,
                Password = password
            });

    var loginResponse =
        await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = password
            });

    loginResponse.EnsureSuccessStatusCode();

    var loginResult =
        await loginResponse.Content
            .ReadFromJsonAsync<LoginResult>();

    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue(
            "Bearer",
            loginResult!.Token);

    return client;
}

    [Fact]
    public async Task GetTask_ReturnsOkAndTask_WhenTaskExists()
    {
        var client = await GetAuthenticatedClientAsync();

        var project = new Project
        {
            Name = "Test Project",
            Description = "Project for integration test",
            CreatedDate = DateTime.UtcNow
        };

        var projectResponse = await client.PostAsJsonAsync("/api/projects", project);
        var createdProject = await projectResponse.Content
            .ReadFromJsonAsync<Project>();

        var createTaskRequest = new
        {
            Title = "Integration Test Task",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(3),
            ProjectId = createdProject!.Id
        };

        var createResponse = await client.PostAsJsonAsync("/api/tasks", createTaskRequest);
        var createdTask = await createResponse.Content
            .ReadFromJsonAsync<AppTask>();

        var response = await client.GetAsync($"/api/tasks/{createdTask!.Id}");
        var returnedTask = await response.Content
            .ReadFromJsonAsync<AppTask>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(createTaskRequest.Title, returnedTask!.Title);
        Assert.Equal(createTaskRequest.Status, returnedTask.Status);
    }

    [Fact]
    public async Task GetTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        var client = await GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/tasks/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTasks_ReturnsUnauthorized_WhenNoTokenProvided()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

public class LoginResult
{
    public string Token { get; set; } = string.Empty;
}
