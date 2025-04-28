using Microsoft.AspNetCore.Mvc;
using ECommerce.Api.Models;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Infrastructure.Repositories;

namespace ECommerce.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository) {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetCustomers() {
            return Ok(_customerRepository.GetAllCustomers());
        }

        [HttpPost]
        public ActionResult<Customer> AddCustomer(Customer customer) {
            var addedCustomer = _customerRepository.AddCustomer(customer);
            return CreatedAtAction(nameof(GetCustomers), new { id = addedCustomer.Id }, addedCustomer);
        }
    }
}
