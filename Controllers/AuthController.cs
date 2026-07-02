using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KFH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        public class LoginRequest { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            // NOTE: This is a minimal example. Replace with real user validation.
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest();

            var isAdmin = string.Equals(req.Username, "admin", StringComparison.OrdinalIgnoreCase);

            var claims = new[] {
                new Claim(ClaimTypes.Name, req.Username),
                new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
            };

            var key = _config["Jwt:Key"] ?? "dev-default-key-please-change";
            var issuer = _config["Jwt:Issuer"] ?? "KFH";
            var audience = _config["Jwt:Audience"] ?? "KFH_Audience";
            var expiresInMinutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { access_token = tokenStr, expires_in = expiresInMinutes * 60 });
        }
    }
}
