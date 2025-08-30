using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace SmartBank.Api.Controllers;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string Role, DateTime ExpiresAt);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config) => _config = config;

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        // DEMO doğrulama (gerçekte DB/Identity kullanırsın)
        if (string.IsNullOrWhiteSpace(req.Username) || req.Password != "P@ssw0rd!")
            return Unauthorized();

        var issuer = _config["Jwt:Issuer"]!;
        var audience = _config["Jwt:Audience"]!;
        var key = _config["Jwt:Key"]!;
        var minutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

        // ---- Basit rol kuralı (maker/checker/admin)
        var username = req.Username.Trim();
        string role =
            username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" :
            username.StartsWith("checker", StringComparison.OrdinalIgnoreCase) ? "Checker" :
                                                                               "Maker";

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role) // 👈 rol claim
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.Now.AddMinutes(minutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        // Artık düz string yerine detaylı bir yanıt dönüyoruz
        return Ok(new LoginResponse(jwt, role, expiresAt));
    }
}
