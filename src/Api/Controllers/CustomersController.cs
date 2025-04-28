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
        public async Task<ActionResult<Customer>> GetCustomerById(int id) {
            var customer = await _customerRepository.GetCustomerById(id);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomer([FromBody] Customer customer) {
            if (customer == null)
                return BadRequest("Customer não pode ser nulo.");

            var existing = await _customerRepository.GetCustomerByUsername(customer.Username);
            if (existing != null)
                return Conflict(new { message = "Username já está em uso." });

            customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);

            var created = await _customerRepository.AddCustomer(customer);
            return CreatedAtRoute(
                "GetCustomerById",
                new { id = created.Id },
                created);
        }
    }
}