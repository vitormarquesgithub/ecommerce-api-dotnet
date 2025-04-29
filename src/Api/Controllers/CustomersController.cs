using Microsoft.AspNetCore.Mvc;
using ECommerce.Api.Models;
using ECommerce.Infrastructure.Repositories;

namespace ECommerce.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository) {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers() {
            var list = await _customerRepository.GetAllCustomers();
            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetCustomerById")]
        public async Task<ActionResult<Customer>> GetCustomerById(Guid id) {
            var customer = await _customerRepository.GetCustomerById(id);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<Customer>> GetCustomerByUsername(string username) {
            var customer = await _customerRepository.GetCustomerByUsername(username);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomer([FromBody] Customer customer) {
            if (customer == null)
                return BadRequest("Customer cannot be null.");

            var existing = await _customerRepository.GetCustomerByUsername(customer.Username);
            if (existing != null)
                return Conflict(new { message = "Username is already in use." });

            customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);

            var created = await _customerRepository.AddCustomer(customer);
            return CreatedAtRoute(
                "GetCustomerById",
                new { id = created.Id },
                created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] Customer customer) {
            if (customer == null)
                return BadRequest("Customer cannot be null.");

            var existingCustomer = await _customerRepository.GetCustomerById(id);
            if (existingCustomer == null)
                return NotFound();

            if (existingCustomer.Username != customer.Username) {
                var usernameExists = await _customerRepository.GetCustomerByUsername(customer.Username);
                if (usernameExists != null)
                    return Conflict(new { message = "Username is already in use." });
            }

            existingCustomer.Username = customer.Username;
            existingCustomer.Name = customer.Name;
            existingCustomer.Email = customer.Email;
            existingCustomer.Telephone = customer.Telephone;
            existingCustomer.Role = customer.Role;

            if (!string.IsNullOrEmpty(customer.Password)) {
                existingCustomer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);
            }

            await _customerRepository.UpdateCustomer(existingCustomer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id) {
            var customer = await _customerRepository.GetCustomerById(id);
            if (customer == null)
                return NotFound();

            await _customerRepository.RemoveCustomer(customer);
            return NoContent();
        }
    }
}