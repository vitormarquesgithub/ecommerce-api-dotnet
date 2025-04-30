using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ECommerce.Api.Models;
using ECommerce.Api.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ECommerce.Tests.Integration {
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>> {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory) {
            _factory = factory;
        }

        [Fact]
        public async Task Login_NoBody_ReturnsBadRequest() {
            var client = _factory.CreateClient();
            var response = await client.PostAsync("/api/auth/login", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized() {
            var client = _factory.CreateClient();
            var req = new LoginRequest { Username = "wrong", Password = "wrong" };
            var response = await client.PostAsJsonAsync("/api/auth/login", req);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("/api/auth/test/customer")]
        [InlineData("/api/auth/test/manager")]
        [InlineData("/api/auth/test/admin")]

        public async Task ProtectedEndpoints_NoToken_ReturnsUnauthorized(string url) {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_InvalidToken_ReturnsUnauthorized() {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid.token");
            var response = await client.GetAsync("/api/auth/test/customer");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<string> CreateUserAndLoginAsync(string username, string password, Role role) {
            var client = _factory.CreateClient();
            var customer = new Customer {
                Username = username,
                Password = password,
                Name = "Test",
                Email = "test@e.com",
                Telephone = "123",
                Role = role
            };
   
            var createResponse = await client.PostAsJsonAsync("/api/customers", customer);
            createResponse.EnsureSuccessStatusCode();
            var loginReq = new LoginRequest { Username = username, Password = password };
            var loginResp = await client.PostAsJsonAsync("/api/auth/login", loginReq);
            loginResp.EnsureSuccessStatusCode();
            var body = await loginResp.Content.ReadFromJsonAsync<TokenResponse>();
            return body!.token;
        }

        [Theory]
        [InlineData(Role.CUSTOMER, "/api/auth/test/customer", HttpStatusCode.OK)]
        [InlineData(Role.CUSTOMER, "/api/auth/test/manager", HttpStatusCode.Forbidden)]
        [InlineData(Role.CUSTOMER, "/api/auth/test/admin", HttpStatusCode.Forbidden)]
        [InlineData(Role.MANAGER, "/api/auth/test/customer", HttpStatusCode.OK)]
        [InlineData(Role.MANAGER, "/api/auth/test/manager", HttpStatusCode.OK)]
        [InlineData(Role.MANAGER, "/api/auth/test/admin", HttpStatusCode.Forbidden)]
        [InlineData(Role.ADMIN, "/api/auth/test/customer", HttpStatusCode.OK)]
        [InlineData(Role.ADMIN, "/api/auth/test/manager", HttpStatusCode.OK)]
        [InlineData(Role.ADMIN, "/api/auth/test/admin", HttpStatusCode.OK)]

        public async Task ProtectedEndpoints_WithValidToken_ReturnsExpected(Role role, string url, HttpStatusCode expected) {
            var token = await CreateUserAndLoginAsync(Guid.NewGuid().ToString(), "Pass123!", role);
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(url);
            Assert.Equal(expected, response.StatusCode);
        }


        private class TokenResponse { public string token { get; set; } = string.Empty; }
    }
}