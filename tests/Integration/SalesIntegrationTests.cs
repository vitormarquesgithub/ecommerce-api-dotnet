using Xunit;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using ECommerce.Api;
using ECommerce.Api.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SalesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SalesIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSales_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/sales");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateSale_ReturnsCreated()
    {
        var dto = new CreateSaleDto
        {
            CustomerId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = new List<CreateProductSaleDto>
            {
                new CreateProductSaleDto
                {
                    ProductId = Guid.NewGuid(),
                    Amount = 5,
                    UnitPrice = 2
                }
            }
        };

        var json = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/sales", json);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
