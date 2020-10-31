using FlatResults.Exceptions;
using FlatResults.Model;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FlatResults.Tests
{
    public class DocumentMapperConfigTests
    {
        [Fact]
        public void UsingToDocumentWithoutConfigShouldThrowException()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";

            DocumentMapperConfig.ClearConfigs();

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };


            Action action = () => category.ToDocument();


            action.Should().Throw<ConfigNotFoundException>();
        }

        [Fact]
        public void MapWithoutIdShouldUseDefaultMapping()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";

            DocumentMapperConfig.NewConfig<Category>()
                .WithAttribute(c => c.Name)
                .WithAttribute(c => c.Description);

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };


            var document = category.ToDocument();


            var resource = (Resource)document.Data;
            resource.Id.Should().Be(CategoryId.ToString());
        }

        [Fact]
        public void MapSpecificProperties()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";

            DocumentMapperConfig.NewConfig<Category>()
                .WithId(c => c.Id)
                .WithAttribute(c => c.Name);

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };


            var document = category.ToDocument();


            var resource = (Resource)document.Data;
            resource.Id.Should().Be(CategoryId.ToString());
            resource.Attributes.Should().ContainKey(nameof(Category.Name));
            resource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
            resource.Attributes.Should().NotContainKey(nameof(Category.Description));
        }

        [Fact]
        public void MapNullProperties()
        {
            const int CategoryId = 1;
            const string CategoryName = null;

            DocumentMapperConfig.NewConfig<Category>()
                .WithId(c => c.Id)
                .WithAttribute(c => c.Name);

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName
            };


            var document = category.ToDocument();


            var resource = (Resource)document.Data;
            resource.Id.Should().Be(CategoryId.ToString());
            resource.Attributes.Should().ContainKey(nameof(Category.Name));
            resource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
        }

        [Fact]
        public void MapIdAndProperties()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";

            DocumentMapperConfig.NewConfig<Category>()
                .WithId(c => c.Id)
                .WithAttribute(c => c.Name)
                .WithAttribute(c => c.Description);

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };


            var document = category.ToDocument();


            var resource = (Resource)document.Data;
            resource.Id.Should().Be(CategoryId.ToString());
            resource.Attributes.Should().ContainKey(nameof(Category.Name));
            resource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
            resource.Attributes.Should().ContainKey(nameof(Category.Description));
            resource.Attributes[nameof(Category.Description)].Should().Be(CategoryDescription);
        }

        [Fact]
        public void MapRelationship()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";

            DocumentMapperConfig.NewConfig<Product>()
                .WithId(p => p.Id)
                .WithAttribute(p => p.Active)
                .WithRelationship(p => p.Category);

            DocumentMapperConfig.NewConfig<Category>()
                .WithId(p => p.Id)
                .WithAttribute(p => p.Name);

            var product = new Product
            {
                Category = new Category
                {
                    Id = CategoryId,
                    Name = CategoryName
                }
            };


            var document = product.ToDocument();


            var resource = (Resource)document.Data;
            resource.Relationships.Should().ContainKey(nameof(Product.Category));

            var categoryRelationship = (Resource)resource.Relationships[nameof(Product.Category)].Data;
            categoryRelationship.Id.Should().Be(CategoryId.ToString());
            categoryRelationship.Attributes.Should().BeNull();

            document.Included.Should().NotBeNull();
            document.Included.Should().NotBeEmpty();
            var categoryResource = document.Included.ElementAt(0);
            categoryRelationship.Id.Should().Be(CategoryId.ToString());
            categoryResource.Attributes.Should().ContainKey(nameof(Category.Name));
            categoryResource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
        }

        [Fact]
        public void MapNullRelationship()
        {
            DocumentMapperConfig.NewConfig<Product>()
                .WithId(p => p.Id)
                .WithAttribute(p => p.Active)
                .WithRelationship(p => p.Category);

            DocumentMapperConfig.NewConfig<Category>()
                .WithId(p => p.Id)
                .WithAttribute(p => p.Name);

            var product = new Product
            {
                Category = null
            };


            var document = product.ToDocument();


            var resource = (Resource)document.Data;
            resource.Relationships.Should().NotContainKey(nameof(Product.Category));
        }

        [Fact]
        public void MapWithDefaults()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults();

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };


            var document = category.ToDocument();


            var resource = (Resource)document.Data;
            resource.Id.Should().Be(CategoryId.ToString());
            resource.Attributes.Should().ContainKey(nameof(Category.Name));
            resource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
            resource.Attributes.Should().ContainKey(nameof(Category.Description));
            resource.Attributes[nameof(Category.Description)].Should().Be(CategoryDescription);
        }

        [Fact]
        public void MapRelationshipWithDefaults()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults();

            var product = new Product
            {
                Category = new Category
                {
                    Id = CategoryId,
                    Name = CategoryName
                }
            };


            var document = product.ToDocument();


            var resource = (Resource)document.Data;
            resource.Relationships.Should().ContainKey(nameof(Product.Category));

            var categoryRelationship = (Resource)resource.Relationships[nameof(Product.Category)].Data;
            categoryRelationship.Id.Should().Be(CategoryId.ToString());
            categoryRelationship.Attributes.Should().BeNull();

            document.Included.Should().NotBeNull();
            document.Included.Should().NotBeEmpty();
            var categoryResource = document.Included.ElementAt(0);
            categoryRelationship.Id.Should().Be(CategoryId.ToString());
            categoryResource.Attributes.Should().ContainKey(nameof(Category.Name));
            categoryResource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
        }

        [Fact]
        public void MapRelationshipWithDefaultsAlternateOrder()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults();

            var product = new Product
            {
                Category = new Category
                {
                    Id = CategoryId,
                    Name = CategoryName
                }
            };


            var document = product.ToDocument();


            var resource = (Resource)document.Data;
            resource.Relationships.Should().ContainKey(nameof(Product.Category));

            var categoryRelationship = (Resource)resource.Relationships[nameof(Product.Category)].Data;
            categoryRelationship.Id.Should().Be(CategoryId.ToString());
            categoryRelationship.Attributes.Should().BeNull();

            document.Included.Should().NotBeNull();
            document.Included.Should().NotBeEmpty();
            var categoryResource = document.Included.ElementAt(0);
            categoryRelationship.Id.Should().Be(CategoryId.ToString());
            categoryResource.Attributes.Should().ContainKey(nameof(Category.Name));
            categoryResource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
        }

        [Fact]
        public void MapGenericRelationshipWithDefaults()
        {
            const int UnitOfMeasure1Id = 1;
            const string UnitOfMeasure1Name = "Unit1";
            const int UnitOfMeasure2Id = 2;
            const string UnitOfMeasure2Name = "Unit2";

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<UnitOfMeasure>()
                .MapWithDetaults();

            var product = new Product
            {
                Units = new List<UnitOfMeasure>
                {
                    new UnitOfMeasure
                    {
                        Id = UnitOfMeasure1Id,
                        Name = UnitOfMeasure1Name
                    },
                    new UnitOfMeasure
                    {
                        Id = UnitOfMeasure2Id,
                        Name = UnitOfMeasure2Name
                    }
                }
            };


            var document = product.ToDocument();


            var resource = (Resource)document.Data;
            resource.Relationships.Should().ContainKey(nameof(Product.Units));

            var unitRelationship = (ResourceCollection)resource.Relationships[nameof(Product.Units)].Data;
            unitRelationship.Count().Should().Be(2);
        }

        [Fact]
        public void MapGenericRelationshipWithDefaultsAlternateOrder()
        {
            const int UnitOfMeasure1Id = 1;
            const string UnitOfMeasure1Name = "Unit1";
            const int UnitOfMeasure2Id = 2;
            const string UnitOfMeasure2Name = "Unit2";

            DocumentMapperConfig.NewConfig<UnitOfMeasure>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults();

            var product = new Product
            {
                Units = new List<UnitOfMeasure>
                {
                    new UnitOfMeasure
                    {
                        Id = UnitOfMeasure1Id,
                        Name = UnitOfMeasure1Name
                    },
                    new UnitOfMeasure
                    {
                        Id = UnitOfMeasure2Id,
                        Name = UnitOfMeasure2Name
                    }
                }
            };


            var document = product.ToDocument();


            var resource = (Resource)document.Data;
            resource.Relationships.Should().ContainKey(nameof(Product.Units));

            var unitRelationship = (ResourceCollection)resource.Relationships[nameof(Product.Units)].Data;
            unitRelationship.Count().Should().Be(2);
        }

        [Fact]
        public void MapWithDefaultsIgnoringAttributes()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults()
                .Ignore(c => c.Description);

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };


            var document = category.ToDocument();


            var resource = (Resource)document.Data;
            resource.Id.Should().Be(CategoryId.ToString());
            resource.Attributes.Should().ContainKey(nameof(Category.Name));
            resource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
            resource.Attributes.Should().NotContainKey(nameof(Category.Description));
        }

        [Fact]
        public void MapWithDefaultsIgnoringAttributes2()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";

            DocumentMapperConfig.NewConfig<Category>()
                .Ignore(c => c.Description)
                .MapWithDetaults();

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };


            var document = category.ToDocument();


            var resource = (Resource)document.Data;
            resource.Id.Should().Be(CategoryId.ToString());
            resource.Attributes.Should().ContainKey(nameof(Category.Name));
            resource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
            resource.Attributes.Should().NotContainKey(nameof(Category.Description));
        }

        [Fact]
        public void MapRelationshipWithDefaultsIgnoringFields()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults()
                .Ignore(p => p.Category);

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<UnitOfMeasure>()
               .MapWithDetaults();

            var product = new Product
            {
                Category = new Category
                {
                    Id = CategoryId,
                    Name = CategoryName
                }
            };


            var document = product.ToDocument();


            var resource = (Resource)document.Data;
            resource.Relationships.Should().NotContainKey(nameof(Product.Category));
        }

        [Fact]
        public void MapRelationshipWithDefaultsIgnoringFields2()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";

            DocumentMapperConfig.NewConfig<Product>()
                .Ignore(p => p.Category)
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<UnitOfMeasure>()
               .MapWithDetaults();

            var product = new Product
            {
                Category = new Category
                {
                    Id = CategoryId,
                    Name = CategoryName
                }
            };


            var document = product.ToDocument();


            var resource = (Resource)document.Data;
            resource.Relationships.Should().NotContainKey(nameof(Product.Category));
        }

        [Fact]
        public void MapWrapperType()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults()
                .Ignore(c => c.Description);

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };

            var result = new ResultModel<Category>
            {
                Data = category
            };

            DocumentMapperConfig.AddWrapperType(typeof(ResultModel<>), r => r.Data);


            var document = result.ToDocument();


            var resource = (Resource)document.Data;
            resource.Id.Should().Be(CategoryId.ToString());
            resource.Attributes.Should().ContainKey(nameof(Category.Name));
            resource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
            resource.Attributes.Should().NotContainKey(nameof(Category.Description));
        }

        [Fact]
        public void MapWrapperTypeWithMetaInfo()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";
            const int TotalCount = 1;

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults()
                .Ignore(c => c.Description);

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };

            var result = new ResultModel<Category>
            {
                Data = category,
                TotalCount = TotalCount
            };

            DocumentMapperConfig.AddWrapperType(typeof(ResultModel<>), r => r.Data, r => new Dictionary<string, object> { { "TotalCount", r.TotalCount } });


            var document = result.ToDocument();


            document.Meta.Should().NotBeNull();
            document.Meta.Should().ContainKey(nameof(ResultModel<Category>.TotalCount));
            document.Meta[nameof(ResultModel<Category>.TotalCount)].Should().Be(TotalCount);
        }

        [Fact]
        public void MapTypeName()
        {
            const int CategoryId = 1;
            const string CategoryName = null;
            const string CategoryType = "CategoryModel";

            DocumentMapperConfig.NewConfig<Category>()
                .WithTypeName(CategoryType);

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName
            };


            var document = category.ToDocument();


            var resource = (Resource)document.Data;
            resource.Type.Should().Be(CategoryType);
        }

        [Fact]
        public void MapWithDefaultsChangingTypeName()
        {
            const int CategoryId = 1;
            const string CategoryName = "Category1";
            const string CategoryDescription = "Description Category1";
            const string CategoryType = "CategoryModel";

            DocumentMapperConfig.NewConfig<Category>()
                .WithTypeName(CategoryType)
                .MapWithDetaults();

            var category = new Category
            {
                Id = CategoryId,
                Name = CategoryName,
                Description = CategoryDescription
            };


            var document = category.ToDocument();


            var resource = (Resource)document.Data;
            resource.Id.Should().Be(CategoryId.ToString());
            resource.Type.Should().Be(CategoryType);
            resource.Attributes.Should().ContainKey(nameof(Category.Name));
            resource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
            resource.Attributes.Should().ContainKey(nameof(Category.Description));
            resource.Attributes[nameof(Category.Description)].Should().Be(CategoryDescription);
        }

        [Fact]
        public void SelectFields()
        {
            const int ProductId = 1;
            const string ProductName = "Product1";

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults();

            var product = new Product
            {
                Id = ProductId,
                Name = ProductName,
                Cost = 20,
                Price = 25,
                Active = true
            };


            var document = product.ToDocument(fields: new[] { "name" });


            var resource = (Resource)document.Data;
            resource.Attributes.Keys.Count().Should().Be(1);
            resource.Attributes.Should().ContainKey(nameof(Product.Name));
            resource.Attributes[nameof(Product.Name)].Should().Be(ProductName);
        }

        [Fact]
        public void SelectFieldsForRelationship()
        {
            const int ProductId = 1;
            const string ProductName = "Product1";
            const int CategoryId = 1;
            const string CategoryName = "Category1";

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults();

            var product = new Product
            {
                Id = ProductId,
                Name = ProductName,
                Cost = 20,
                Price = 25,
                Active = true,
                Category = new Category
                {
                    Id = CategoryId,
                    Name = CategoryName,
                    Description = "Description"
                }
            };


            var document = product.ToDocument(fields: new[] { "name", "category.name" });

            var categoryResource = document.Included.ElementAt(0);
            categoryResource.Attributes.Keys.Count().Should().Be(1);
            categoryResource.Attributes.Should().ContainKey(nameof(Category.Name));
            categoryResource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
        }

        [Fact]
        public void MapOnlySelectedRelationships()
        {
            const int ProductId = 1;
            const string ProductName = "Product1";
            const int CategoryId = 1;
            const string CategoryName = "Category1";

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<UnitOfMeasure>()
                .MapWithDetaults();

            var product = new Product
            {
                Id = ProductId,
                Name = ProductName,
                Cost = 20,
                Price = 25,
                Active = true,
                Category = new Category
                {
                    Id = CategoryId,
                    Name = CategoryName,
                    Description = "Description"
                },
                Units = new List<UnitOfMeasure>
                {
                    new UnitOfMeasure
                    {
                        Id = 1,
                        Name = "unit1"
                    },
                    new UnitOfMeasure
                    {
                        Id = 2,
                        Name = "unit2"
                    }
                }
            };


            var document = product.ToDocument(fields: new[] { "name", "category.name" });

            (document.Data as Resource).Relationships.Count().Should().Be(1);
        }

        [Fact]
        public void IncludeOnlySelectedRelationships()
        {
            const int ProductId = 1;
            const string ProductName = "Product1";
            const int CategoryId = 1;
            const string CategoryName = "Category1";

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<UnitOfMeasure>()
                .MapWithDetaults();

            var product = new Product
            {
                Id = ProductId,
                Name = ProductName,
                Cost = 20,
                Price = 25,
                Active = true,
                Category = new Category
                {
                    Id = CategoryId,
                    Name = CategoryName,
                    Description = "Description"
                },
                Units = new List<UnitOfMeasure>
                {
                    new UnitOfMeasure
                    {
                        Id = 1,
                        Name = "unit1"
                    },
                    new UnitOfMeasure
                    {
                        Id = 2,
                        Name = "unit2"
                    }
                }
            };


            var document = product.ToDocument(fields: new[] { "name", "category.name" });

            document.Included.Count().Should().Be(1);

            var categoryResource = document.Included.ElementAt(0);
            categoryResource.Attributes.Keys.Count().Should().Be(1);
            categoryResource.Attributes.Should().ContainKey(nameof(Category.Name));
            categoryResource.Attributes[nameof(Category.Name)].Should().Be(CategoryName);
        }

        [Fact]
        public void IncludeOnlySelectedRelationshipsByName()
        {
            const int ProductId = 1;
            const string ProductName = "Product1";
            const int CategoryId = 1;
            const string CategoryName = "Category1";

            DocumentMapperConfig.NewConfig<Product>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<Category>()
                .MapWithDetaults();

            DocumentMapperConfig.NewConfig<UnitOfMeasure>()
                .MapWithDetaults();

            var product = new Product
            {
                Id = ProductId,
                Name = ProductName,
                Cost = 20,
                Price = 25,
                Active = true,
                Category = new Category
                {
                    Id = CategoryId,
                    Name = CategoryName,
                    Description = "Description"
                },
                Units = new List<UnitOfMeasure>
                {
                    new UnitOfMeasure
                    {
                        Id = 1,
                        Name = "unit1"
                    },
                    new UnitOfMeasure
                    {
                        Id = 2,
                        Name = "unit2"
                    }
                }
            };


            var document = product.ToDocument(fields: new[] { "name", "category" });

            document.Included.Count().Should().Be(1);

            var categoryResource = document.Included.ElementAt(0);
            categoryResource.Attributes.Keys.Count().Should().Be(3);
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public float Cost { get; set; }
        public float Price { get; set; }
        public List<UnitOfMeasure> Units { get; set; }
        public bool Active { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UnitOfMeasure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProductParent
    {
        public int Id { get; set; }
        public Product Product { get; set; }
    }

    public class ResultModel<T>
    {
        public T Data { get; set; }
        public int TotalCount { get; set; }
    }
}
