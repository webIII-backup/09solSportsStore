using SportsStore.Models.Domain;
using Xunit;

namespace SportsStore.Tests.Models
{
    public class OrderTest
    {
        private readonly Order _order;
        private readonly Product _p1;
        private readonly Product _p3;

        public OrderTest()
        {
            var category = new Category("Category1");
            var cart = new Cart();
            var p2 = new Product(2, "Short", 5, category);
            _p1 = new Product(1, "Football", 10, category);
            _p3 = new Product (3, "NotInOrder",  5, category);
            cart.AddLine(_p1, 1);
            cart.AddLine(p2, 10);
            _order = new Order(cart, null, false, "Teststraat 10", new City(){Postalcode = "3000", Name="Gent"});
        }

        [Fact]
        public void Total_ReturnsSumOfOrderlines()
        {
               Assert.Equal(60, _order.Total);
        }

        [Fact]
        public void HasOrdered_ProductInOrder_ReturnsTrue()
        {
            Assert.True(_order.HasOrdered(_p1));
        }

        [Fact]
        public void HasOrdered_ProductNotInOrder_ReturnsFalse()
        {
            Assert.False(_order.HasOrdered(_p3));
        }
    }
}
