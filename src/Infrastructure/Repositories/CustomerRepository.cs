using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Models;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ECommerceDbContext _context;

        public CustomerRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
            => await _context.Customers.FindAsync(id);

        public async Task<IEnumerable<Customer>> GetAllAsync()
            => await _context.Customers
                .AsNoTracking()
                .ToListAsync();

        public IEnumerable<Customer> GetAllCustomers()
            => _context.Customers.AsNoTracking().ToList();

        public async Task AddAsync(Customer customer)
            => await _context.Customers.AddAsync(customer);

        public Customer AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return customer;
        }

        public void Update(Customer customer)
            => _context.Customers.Update(customer);

        public void Remove(Customer customer)
            => _context.Customers.Remove(customer);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task<Customer?> GetByEmailAsync(string email)
            => await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);

        public async Task<Customer?> GetByEmailWithPasswordAsync(string email)
            => await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

        public Customer? GetCustomerByUsername(string username)
            => _context.Customers
                .FirstOrDefault(c => c.Username == username);
    }
}