using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Api.Models;

namespace ECommerce.Infrastructure.Repositories
{
    public interface ICustomerRepository
    {
        // Métodos existentes
        Task<Customer?> GetByIdAsync(Guid id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        void Update(Customer customer);
        void Remove(Customer customer);
        Task<int> SaveChangesAsync();
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByEmailWithPasswordAsync(string email);
        
        // Métodos adicionais usados nos controllers
        IEnumerable<Customer> GetAllCustomers();
        Customer AddCustomer(Customer customer);
        Customer? GetCustomerByUsername(string username);
    }
}