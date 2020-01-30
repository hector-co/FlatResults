using FlatResults.WebSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FlatResults.WebSample.Controllers
{
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly SampleContext _context = new SampleContext();

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = new ResultModel<ProductModel>
            {
                Data = _context.Products.FirstOrDefault(p => p.Id == id)
            };
            return Ok(result);
        }

        [HttpGet]
        public IActionResult List()
        {
            var result = new ResultModel<IEnumerable<ProductModel>>
            {
                Data = _context.Products,
                TotalCount = _context.Products.Count()
            };
            return Ok(result);
        }
    }
}