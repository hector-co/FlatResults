using System.Collections.Generic;

namespace FlatResults.WebSample.Models
{
    public class SampleContext
    {
        private static List<CategoryModel> _categories;
        private static List<UnitOfMeasureModel> _unitOfMeasures;
        private static List<ProductModel> _products;

        static SampleContext()
        {
            _categories = new List<CategoryModel>
            {
                new CategoryModel
                {
                    Id = 1,
                    Name = "Category1",
                    Description = "Category1 description"
                },
                new CategoryModel
                {
                    Id = 2,
                    Name = "Category2",
                    Description = "Category2 description"
                }
            };
            _unitOfMeasures = new List<UnitOfMeasureModel>
            {
                new UnitOfMeasureModel
                {
                    Id = 1,
                    Name = "Unit1",
                    Description  = "Unit1 description"
                },
                new UnitOfMeasureModel
                {
                    Id = 2,
                    Name = "Unit2",
                    Description  = "Unit2 description"
                },
                new UnitOfMeasureModel
                {
                    Id = 3,
                    Name = "Unit3",
                    Description  = "Unit3 description"
                }
            };
            _products = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 1,
                    Name = "Product1",
                    Category = _categories[0],
                    Description = "Product1 description",
                    Cost = 5f,
                    Price = 6.5f,
                    Units = new List<UnitOfMeasureModel>
                    {
                        _unitOfMeasures[0],
                        _unitOfMeasures[1]
                    },
                    Active = true
                },
                new ProductModel
                {
                    Id = 2,
                    Name = "Product2",
                    Category = _categories[1],
                    Description = "Product2 description",
                    Cost = 3.5f,
                    Price = 4f,
                    Units = new List<UnitOfMeasureModel>
                    {
                        _unitOfMeasures[0],
                        _unitOfMeasures[2]
                    },
                    Active = false
                },
                new ProductModel
                {
                    Id = 3,
                    Name = "Product3",
                    Category = _categories[0],
                    Description = "Product3 description",
                    Cost = 3.3f,
                    Price = 4.4f,
                    Units = new List<UnitOfMeasureModel>
                    {
                        _unitOfMeasures[1]
                    },
                    Active = false
                },
                new ProductModel
                {
                    Id = 4,
                    Name = "Product4",
                    Category = _categories[1],
                    Description = "Product4 description",
                    Cost = 2f,
                    Price = 2f,
                    Units = new List<UnitOfMeasureModel>
                    {
                        _unitOfMeasures[1],
                        _unitOfMeasures[2]
                    },
                    Active = true
                }
            };
        }

        public List<CategoryModel> Categories => _categories;
        public List<UnitOfMeasureModel> UnitOfMeasures => _unitOfMeasures;
        public List<ProductModel> Products => _products;
    }
}
