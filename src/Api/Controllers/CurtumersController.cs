using Microsoft.AspNetCore.Mvc;
using ECommerce.Api.Models;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustumersController : ControllerBase
    {
        private static List<Custumer> _Custumers = new List<Custumer>();

        [HttpGet]
        public ActionResult<IEnumerable<Custumer>> GetCustumers()
        {
            return Ok(_Custumers);
        }

        [HttpPost]
        public ActionResult<Custumer> AddCustumer(Custumer Custumer)
        {
            Custumer.Id = _Custumers.Count + 1;
            _Custumers.Add(Custumer);
            return CreatedAtAction(nameof(GetCustumers), new { id = Custumer.Id }, Custumer);
        }
    }
}
