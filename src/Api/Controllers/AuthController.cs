using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Api.Models;
using ECommerce.Infrastructure.Repositories;

namespace ECommerce.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly IConfiguration _config;
        private readonly ICustomerRepository _customerRepository;

        public AuthController(IConfiguration config, ICustomerRepository customerRepository) {
            _config = config;
            _customerRepository = customerRepository;
        }

        [HttpPost("login"), AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var customer = _customerRepository.GetCustomerByUsername(req.Username);
            
            if (customer == null || customer.Password != req.Password)
                return Unauthorized();

            var jwtCfg = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtCfg["Secret"]!);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(ClaimTypes.Name, req.Username) };
            var token = new JwtSecurityToken(
                issuer: jwtCfg["Issuer"],
                audience: jwtCfg["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtCfg["ExpiryInMinutes"]!)),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}

