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
        public async Task<IActionResult> Login([FromBody] LoginRequest req) {
            if (req is null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username and password are required.");

            var customer = await _customerRepository.GetCustomerByUsername(req.Username);
            if (customer == null || !BCrypt.Net.BCrypt.Verify(req.Password, customer.Password))
                return Unauthorized();

            var jwtCfg = _config.GetSection("Jwt");
            var secret = jwtCfg["Secret"]!;
            var issuer = jwtCfg["Issuer"]!;
            var audience = jwtCfg["Audience"]!;
            var expiry = double.Parse(jwtCfg["ExpiryInMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Name, customer.Username),
                new Claim(ClaimTypes.Role, customer.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpGet("test/customer")]
        [Authorize(Policy = "Customer")]
        public IActionResult TestCustomer() => Ok("Customer access granted.");

        [HttpGet("test/manager")]
        [Authorize(Policy = "ManageProducts")]
        public IActionResult TestManager() => Ok("Manager or Admin access granted.");

        [HttpGet("test/admin")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult TestAdmin() => Ok("Admin access granted.");
    }
}