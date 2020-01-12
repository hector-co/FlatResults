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
}
