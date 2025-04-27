using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Api.Models;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config) => _config = config;

        [HttpPost("login"), AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (req.Username != "admin" || req.Password != "123456")
                return Unauthorized();

            var jwtCfg = _config.GetSection("Jwt");
            var key    = Encoding.UTF8.GetBytes(jwtCfg["Secret"]!);
            var creds  = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(ClaimTypes.Name, req.Username) };
            var token = new JwtSecurityToken(
                issuer:             jwtCfg["Issuer"],
                audience:           jwtCfg["Audience"],
                claims:             claims,
                expires:            DateTime.UtcNow.AddMinutes(double.Parse(jwtCfg["ExpiryInMinutes"]!)),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
