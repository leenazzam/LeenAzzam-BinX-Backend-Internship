using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace CardiacPatientMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }


    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "Email and password are required."
            });
        }

        var existingUser =
            await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return BadRequest(new
            {
                message = "Email is already registered."
            });
        }

        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result =
            await _userManager.CreateAsync(
                user,
                request.Password
            );

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Registration failed.",
                errors = result.Errors.Select(e => e.Description)
            });
        }
await _userManager.AddToRoleAsync(user, "Patient");
        return Ok(new
        {
            message = "User registered successfully."
        });
    }


    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user =
            await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var result =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                false
            );

        if (!result.Succeeded)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }


        // JWT Claims
        var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id
            ),

            new Claim(
                ClaimTypes.Email,
                user.Email!
            )
        };


        // Get user's roles
        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role
                )
            );
        }


        // JWT Secret Key
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );


        // Signing credentials
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );


        // Create JWT
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );


        // Convert JWT to string
        var tokenString =
            new JwtSecurityTokenHandler()
                .WriteToken(token);


        return Ok(new
        {
            token = tokenString
        });
    }
    [Authorize(Roles = "Admin")]
[HttpPost("create-doctor")]
public async Task<IActionResult> CreateDoctor(RegisterRequest request)
{
    var existingUser = await _userManager.FindByEmailAsync(request.Email);

    if (existingUser != null)
    {
        return BadRequest(new
        {
            message = "Email is already registered."
        });
    }

    var doctor = new IdentityUser
    {
        UserName = request.Email,
        Email = request.Email
    };

    var result = await _userManager.CreateAsync(
        doctor,
        request.Password
    );

    if (!result.Succeeded)
    {
        return BadRequest(new
        {
            message = "Doctor creation failed.",
            errors = result.Errors.Select(e => e.Description)
        });
    }

    await _userManager.AddToRoleAsync(doctor, "Doctor");

    return Ok(new
    {
        message = "Doctor created successfully."
    });
}
}


// Register DTO
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}


// Login DTO
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}