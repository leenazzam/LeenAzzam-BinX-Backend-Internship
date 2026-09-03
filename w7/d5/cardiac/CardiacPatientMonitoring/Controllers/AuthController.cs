using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
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

        using var transaction = await _context.Database.BeginTransactionAsync();

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

        var patient = new Patient
        {
            FullName = request.FullName,
            Age = request.Age,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber,
            IdentityUserId = user.Id
        };

        try
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            return BadRequest(new
            {
                message = "Registration failed while creating patient record."
            });
        }

        return Ok(new
        {
            message = "User registered successfully."
        });
    }


    // POST: api/auth/login
    // POST: api/auth/login
[EnableRateLimiting("LoginPolicy")]
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

    // Add PatientId claim if this user has a linked Patient record
    var patient = await _context.Patients
        .FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

    if (patient != null)
    {
        claims.Add(
            new Claim(
                "PatientId",
                patient.Id.ToString()
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

    public string FullName { get; set; } = string.Empty;

    public int Age { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}


// Login DTO
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}