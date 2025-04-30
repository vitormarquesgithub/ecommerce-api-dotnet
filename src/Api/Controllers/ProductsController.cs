using Microsoft.AspNetCore.Mvc;
using ECommerce.Api.Models;
using ECommerce.Infrastructure.Repositories;

namespace ECommerce.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase {
        private readonly IProductRepository _ProductRepository;

        public ProductsController(IProductRepository ProductRepository) {
            _ProductRepository = ProductRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() {
            var list = await _ProductRepository.GetAllProducts();
            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<Product>> GetProductById(Guid id) {
            var Product = await _ProductRepository.GetProductById(id);
            if (Product == null)
                return NotFound();
            return Ok(Product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody] Product Product) {
            if (Product == null)
                return BadRequest("Product cannot be null.");

            var created = await _ProductRepository.AddProduct(Product);
            return CreatedAtRoute(
                "GetProductById",
                new { id = created.Id },
                created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] Product Product) {
            if (Product == null)
                return BadRequest("Product cannot be null.");

            var existingProduct = await _ProductRepository.GetProductById(id);
            if (existingProduct == null)
                return NotFound();

            existingProduct.Name = Product.Name;
            existingProduct.ProductType = Product.ProductType;
            existingProduct.Description = Product.Description;
            existingProduct.Price = Product.Price;
            existingProduct.StockCount = Product.StockCount;

            await _ProductRepository.UpdateProduct(existingProduct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id) {
            var Product = await _ProductRepository.GetProductById(id);
            if (Product == null)
                return NotFound();

            await _ProductRepository.RemoveProduct(Product);
            return NoContent();
        }
    }
}