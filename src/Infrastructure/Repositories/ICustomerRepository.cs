using ECommerce.Api.Models;

namespace ECommerce.Infrastructure.Repositories {
    public interface ICustomerRepository {
        Task<IEnumerable<Customer>> GetAllCustomers();
        Task<Customer?> GetCustomerById(Guid id);
        Task<Customer?> GetCustomerByUsername(string username);
        Task<Customer> AddCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
        Task RemoveCustomer(Customer customer);
    }
}