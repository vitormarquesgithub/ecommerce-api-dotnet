using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Models;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories {
    public class CustomerRepository : ICustomerRepository {
        private readonly ECommerceDbContext _context;

        public CustomerRepository(ECommerceDbContext context) {
            _context = context;
        }

        public async Task<Customer?> GetCustomerById(int id)
            => await _context.Customers.FindAsync(id);

        public async Task<IEnumerable<Customer>> GetAllCustomers()
            => await _context.Customers
                .AsNoTracking()
                .ToListAsync();

        public async Task<Customer> AddCustomer(Customer customer) {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task UpdateCustomer(Customer customer){
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCustomer(Customer customer){
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChanges()
            => await _context.SaveChangesAsync();

        public async Task<Customer?> GetCustomerByUsername(string username)
            => await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Username == username);
    }
}