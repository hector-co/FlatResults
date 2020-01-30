using FlatResults.WebSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FlatResults.WebSample.Controllers
{
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly SampleContext _context = new SampleContext();

        [HttpGet("{id}")]
        public IActionResult Get(int id, bool includeRelatedProducts = false)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (!includeRelatedProducts) return Ok(category);

            var document = category.ToDocument();
            var products = _context.Products.Where(p => p.Category.Id == id);
            document.AppendIncluded(products.ToDocument());
            return Ok(document);
        }
    }
}
