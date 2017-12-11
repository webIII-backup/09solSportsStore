using SportsStore.Models.Domain;
using Xunit;

namespace SportsStore.Tests.Models
{
    public class CategoryTest
    {
        private readonly Category _category;
        private readonly Product _product;

        public CategoryTest()
        {
            _category = new Category("Soccer");
            _product = new Product("Football", 10, _category);
        }

        [Fact]
        public void Category_StartsEmpty()
        {
            Assert.Equal(0, _category.Products.Count);
        }

        [Fact]
        public void Add_NewProduct_AddsProduct()
        {
            _category.AddProduct("Football", 10, null);
            Assert.Equal(1, _category.Products.Count);
        }

        [Fact]
        public void Add_ProductWitheNameNotInCategory_AddsProduct()
        {
            _category.AddProduct("Football", 10, null);
            Assert.Equal(1, _category.Products.Count);
        }

        [Fact]
        public void Add_ProductWithNameAlreadyInCategory_DoesnotAddProduct()
        {
            _category.AddProduct("Football", 10, null);
            _category.AddProduct("Football", 10, null);
            Assert.Equal(1, _category.Products.Count);
        }

        [Fact]
        public void Remove_RemovesProduct()
        {
            _category.AddProduct("Football", 10, null);
            _category.RemoveProduct(_product);
            Assert.Equal(0, _category.Products.Count);
        }

        [Fact]
        public void FindProduct_ProductInCategory_ReturnsProduct()
        {
            _category.AddProduct("Football", 10, null);
            Assert.NotNull(_category.FindProduct("Football"));
        }

        [Fact]
        public void FindProduct_ProductNotInCategory_ReturnsNull()
        {
            Assert.Null(_category.FindProduct("Football"));
        }
    }
}
