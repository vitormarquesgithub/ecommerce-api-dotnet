using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ECommerce.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ECommerce.Tests.Integration {
    public class CustomerIntegrationTests : IClassFixture<WebApplicationFactory<Program>> {
        private readonly HttpClient _client;

        public CustomerIntegrationTests(WebApplicationFactory<Program> factory) {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostAndGetCustomer_Works() {
            var cust = new Customer { Username="int", Password="int", Name="Int", Email="int@e.com", Telephone="123" };
            var post = await _client.PostAsJsonAsync("/api/customers", cust);
            Assert.Equal(HttpStatusCode.Created, post.StatusCode);
            var created = await post.Content.ReadFromJsonAsync<Customer>();
            var get = await _client.GetAsync($"/api/customers/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        }
    }
}
