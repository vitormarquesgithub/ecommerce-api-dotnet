using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Api.Controllers;
using ECommerce.Api.Models;
using ECommerce.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ECommerce.Tests.Controllers {
    public class CustomersControllerTests {
        private readonly Mock<ICustomerRepository> _mockRepo;
        private readonly CustomersController _controller;

        public CustomersControllerTests() {
            _mockRepo = new Mock<ICustomerRepository>();
            _controller = new CustomersController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetCustomers_ReturnsOk() {
            var list = new List<Customer> {
                new Customer { Id = Guid.NewGuid(), Username = "u", Password = "p", Name = "N", Email = "e", Telephone = "t" }
            };
            _mockRepo.Setup(r => r.GetAllCustomers()).ReturnsAsync(list);

            var result = await _controller.GetCustomers();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetCustomerById_NotFound() {
            _mockRepo.Setup(r => r.GetCustomerById(It.IsAny<Guid>())).ReturnsAsync((Customer?)null);
            var result = await _controller.GetCustomerById(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddCustomer_DuplicateUsername_ReturnsConflict() {
            var duplicate = new Customer { Id = Guid.NewGuid(), Username = "dup", Password = "pass", Name = "Name", Email = "e@x.com", Telephone = "123" };
            _mockRepo.Setup(r => r.GetCustomerByUsername("dup")).ReturnsAsync(duplicate);

            var newCust = new Customer { Username = "dup", Password = "p", Name = "N", Email = "e", Telephone = "t" };
            var result = await _controller.AddCustomer(newCust);
            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddCustomer_Valid_ReturnsCreated() {
            var cust = new Customer { Id = Guid.NewGuid(), Username = "u7", Password = "p7", Name = "N7", Email = "e7", Telephone = "t7" };
            _mockRepo.Setup(r => r.GetCustomerByUsername(cust.Username)).ReturnsAsync((Customer?)null);
            _mockRepo.Setup(r => r.AddCustomer(It.IsAny<Customer>())).ReturnsAsync(cust);

            var result = await _controller.AddCustomer(cust);
            var created = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Equal("GetCustomerById", created.RouteName);
        }

        [Fact]
        public async Task UpdateCustomer_NotFound_ReturnsNotFound() {
            _mockRepo.Setup(r => r.GetCustomerById(It.IsAny<Guid>())).ReturnsAsync((Customer?)null);
            var updateData = new Customer { Username = "u", Password = "p", Name = "N", Email = "e", Telephone = "t" };
            var result = await _controller.UpdateCustomer(Guid.NewGuid(), updateData);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_NotFound_ReturnsNotFound() {
            _mockRepo.Setup(r => r.GetCustomerById(It.IsAny<Guid>())).ReturnsAsync((Customer?)null);
            var result = await _controller.DeleteCustomer(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
