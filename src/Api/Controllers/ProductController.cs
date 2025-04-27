using Microsoft.AspNetCore.Mvc;
using ECommerce.Api.Models;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _Products = new List<Product>();

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return Ok(_Products);
        }

        [HttpPost]
        public ActionResult<Product> AddProduct(Product Product)
        {
            Product.Id = _Products.Count + 1;
            _Products.Add(Product);
            return CreatedAtAction(nameof(GetProducts), new { id = Product.Id }, Product);
        }
    }
}
