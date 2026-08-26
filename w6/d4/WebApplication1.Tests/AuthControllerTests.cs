using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using WebApplication1.Controllers;
using Xunit;

public class AuthControllerTests
{
    private Mock<UserManager<IdentityUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        return new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null);
    }

    private Mock<SignInManager<IdentityUser>> MockSignInManager(
        Mock<UserManager<IdentityUser>> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

        return new Mock<SignInManager<IdentityUser>>(
            userManager.Object, contextAccessor.Object, claimsFactory.Object,
            null, null, null, null);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var userManagerMock = MockUserManager();
        userManagerMock
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new IdentityUser());

        var signInManagerMock = MockSignInManager(userManagerMock);
        var configMock = new Mock<IConfiguration>();

        var controller = new AuthController(
            userManagerMock.Object, signInManagerMock.Object, configMock.Object);

        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Password = "Password123!"
        };

        var result = await controller.Register(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }
        [Fact]
    public async Task Login_UserNotFound_ReturnsUnauthorized()
    {
        var userManagerMock = MockUserManager();
        userManagerMock
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((IdentityUser)null);

        var signInManagerMock = MockSignInManager(userManagerMock);
        var configMock = new Mock<IConfiguration>();

        var controller = new AuthController(
            userManagerMock.Object, signInManagerMock.Object, configMock.Object);

        var request = new LoginRequest
        {
            Email = "notfound@example.com",
            Password = "Password123!"
        };

        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
        [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var userManagerMock = MockUserManager();
        var existingUser = new IdentityUser { Email = "user@example.com" };

        userManagerMock
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        var signInManagerMock = MockSignInManager(userManagerMock);
        signInManagerMock
            .Setup(m => m.CheckPasswordSignInAsync(
                existingUser, It.IsAny<string>(), false))
.ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
        var configMock = new Mock<IConfiguration>();

        var controller = new AuthController(
            userManagerMock.Object, signInManagerMock.Object, configMock.Object);

        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "WrongPassword!"
        };

        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
