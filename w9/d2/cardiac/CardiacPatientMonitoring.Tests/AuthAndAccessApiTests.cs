using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CardiacPatientMonitoring.Controllers;
using Xunit;

namespace CardiacPatientMonitoring.Tests;

public class AuthAndAccessApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthAndAccessApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Task1_GetVitalSigns_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/vitalsigns");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Task2_Login_WithSeededAdmin_ReturnsOkWithToken()
    {
        var loginRequest = new LoginRequest
        {
            Email = "admin@cardiac.com",
            Password = "Admin123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("token", body);
    }

    [Fact]
    public async Task Task3_AdminTestEndpoint_WithAdminToken_ReturnsOk()
    {
        var loginRequest = new LoginRequest
        {
            Email = "admin@cardiac.com",
            Password = "Admin123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(loginBody);
        var token = doc.RootElement.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/patients/admin-test");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Task4_NewPatientUser_AccessingAdminEndpoint_ReturnsForbidden()
    {
        var registerRequest = new RegisterRequest
        {
            Email = "testpatient@cardiac.com",
            Password = "Patient123!"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = "testpatient@cardiac.com",
            Password = "Patient123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(loginBody);
        var token = doc.RootElement.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/patients/admin-test");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
