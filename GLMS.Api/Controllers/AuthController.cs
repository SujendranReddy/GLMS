using GLMS.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequest)
        {
            var configuredUsername = _configuration["JwtSettings:Username"];
            var configuredPassword = _configuration["JwtSettings:Password"];

            if (loginRequest.Username != configuredUsername ||
                loginRequest.Password != configuredPassword)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            var token = GenerateJwtToken(loginRequest.Username);

            return Ok(new
            {
                token
            });
        }

        private string GenerateJwtToken(string username)
        {
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var key = _configuration["JwtSettings:Key"];

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key!));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}