using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication1.Controllers
{
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

        // Day 1 - Registration
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
                await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Registration failed.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            // Assign the default User role
            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new
            {
                message = "User registered successfully."
            });
        }

        // Day 2 - Login & JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // Find user by email
            var user =
                await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Check password
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

            // Get user's roles
            var roles = await _userManager.GetRolesAsync(user);

            // JWT Claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            // Add roles to JWT claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
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
            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            // Return JWT
            return Ok(new
            {
                token = tokenString
            });
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}