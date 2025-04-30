using ECommerce.Api.Dtos;
using ECommerce.Api.Models;
using ECommerce.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase {
        private readonly ISaleRepository _repo;
        public SalesController(ISaleRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSales() => Ok(await _repo.GetAllSales());

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Sale>> GetSale(Guid id) {
            var sale = await _repo.GetSaleById(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpPost]
        public async Task<ActionResult<Sale>> CreateSale([FromBody] CreateSaleDto dto) {
            if (dto == null || dto.Products == null || !dto.Products.Any())
                return BadRequest("Sale and products are required.");

            var sale = new Sale {
                CustomerId = dto.CustomerId,
                Date       = dto.Date,
                Total      = dto.Products.Sum(p => p.Amount * p.UnitPrice),
                Products   = dto.Products.Select(p => new ProductSale {
                    ProductId = p.ProductId,
                    Amount    = p.Amount,
                    UnitPrice = p.UnitPrice
                }).ToList()
            };

            var created = await _repo.AddSale(sale);
            return CreatedAtAction(nameof(GetSale), new { id = created.Id }, created);
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSale(Guid id) {
            var existing = await _repo.GetSaleById(id);
            if (existing == null) return NotFound();
            await _repo.RemoveSale(existing);
            return NoContent();
        }

        [HttpGet("analysis")]
        public async Task<ActionResult<SalesAnalysis>> Analyze([FromQuery] DateTime start, [FromQuery] DateTime end)
            => Ok(await _repo.AnalyzeSales(start, end));
    }
}
