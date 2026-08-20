using System.Net;
using System.Net.Http.Json;
using Xunit;

public class AuthApiTests : IClassFixture<CustomWebApplicationFactory>
{

   private readonly CustomWebApplicationFactory _factory;
private readonly HttpClient _client;

public AuthApiTests(CustomWebApplicationFactory factory)
{
    _factory = factory;
    _client = factory.CreateClient();
}

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        var email = $"integration_{Guid.NewGuid()}@example.com";
        var password = "Password123!";

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                Email = email,
                Password = password
            });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = password
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var result = await loginResponse.Content
            .ReadFromJsonAsync<LoginResult>();

        Assert.False(string.IsNullOrEmpty(result?.Token));
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = "doesnotexist@example.com",
                Password = "WrongPassword!"
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}