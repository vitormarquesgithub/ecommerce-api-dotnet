using Microsoft.AspNetCore.Mvc;
using ECommerce.Api.Models;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private static List<Sale> _Sales = new List<Sale>();

        [HttpGet]
        public ActionResult<IEnumerable<Sale>> GetSales()
        {
            return Ok(_Sales);
        }

        [HttpPost]
        public ActionResult<Sale> AddSale(Sale Sale)
        {
            Sale.Id = _Sales.Count + 1;
            Sale.Total = Sale.Products.Sum(p => p.Amount * p.UnitPrice); 
            _Sales.Add(Sale);
            return CreatedAtAction(nameof(GetSales), new { id = Sale.Id }, Sale);
        }
    }
}
