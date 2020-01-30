using System.Collections.Generic;

namespace FlatResults.WebSample.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryModel Category { get; set; }
        public string Description { get; set; }
        public float Cost { get; set; }
        public float Price { get; set; }
        public List<UnitOfMeasureModel> Units { get; set; }
        public bool Active { get; set; }
    }
}
