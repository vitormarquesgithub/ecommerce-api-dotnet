using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Api.Models;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.Tests.Repositories {
    public class CustomerRepositoryTests : IDisposable {
        private readonly ECommerceDbContext _context;
        private readonly CustomerRepository _repository;

        public CustomerRepositoryTests() {
            var options = new DbContextOptionsBuilder<ECommerceDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ECommerceDbContext(options);
            _repository = new CustomerRepository(_context);
        }

        [Fact]
        public async Task AddCustomer_AddsAndReturnsCustomer() {
            var cust = new Customer { Username = "u1", Password = "p1", Name="N1", Email="e1", Telephone="t1" };
            var result = await _repository.AddCustomer(cust);
            var saved = await _context.Customers.FindAsync(result.Id);
            Assert.NotNull(saved);
            Assert.Equal("u1", saved.Username);
        }

        [Fact]
        public async Task GetCustomerById_ReturnsCustomer() {
            var cust = new Customer { Username = "u2", Password = "p2", Name="N2", Email="e2", Telephone="t2" };
            _context.Customers.Add(cust);
            await _context.SaveChangesAsync();

            var result = await _repository.GetCustomerById(cust.Id);
            Assert.NotNull(result);
            Assert.Equal(cust.Username, result.Username);
        }

        [Fact]
        public async Task GetAllCustomers_ReturnsList() {
            _context.Customers.AddRange(
                new Customer { Username = "u3", Password = "p3", Name="N3", Email="e3", Telephone="t3" },
                new Customer { Username = "u4", Password = "p4", Name="N4", Email="e4", Telephone="t4" }
            );
            await _context.SaveChangesAsync();

            var list = await _repository.GetAllCustomers();
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetCustomerByUsername_ReturnsCustomer() {
            var cust = new Customer { Username = "u5", Password = "p5", Name="N5", Email="e5", Telephone="t5" };
            _context.Customers.Add(cust);
            await _context.SaveChangesAsync();

            var result = await _repository.GetCustomerByUsername("u5");
            Assert.NotNull(result);
            Assert.Equal(cust.Id, result.Id);
        }

        [Fact]
        public async Task RemoveCustomer_DeletesCustomer() {
            var cust = new Customer { Username = "u6", Password = "p6", Name="N6", Email="e6", Telephone="t6" };
            _context.Customers.Add(cust);
            await _context.SaveChangesAsync();

            await _repository.RemoveCustomer(cust);
            var found = await _repository.GetCustomerById(cust.Id);
            Assert.Null(found);
        }

        public void Dispose() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}