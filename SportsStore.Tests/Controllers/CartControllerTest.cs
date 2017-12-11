using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models.Domain;
using SportsStore.Tests.Data;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SportsStore.Tests.Controllers
{
    public class CartControllerTest
    {
        private readonly CartController _controller;
        private readonly Cart _cart;

        public CartControllerTest()
        {
            var context = new DummyApplicationDbContext();

            var productRepository = new Mock<IProductRepository>();
            productRepository.Setup(p => p.GetById(4)).Returns(context.RunningShoes);
            productRepository.Setup(p => p.GetById(1)).Returns(context.Football);

            _controller = new CartController(productRepository.Object)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };

            _cart = new Cart();
            _cart.AddLine(context.Football, 2);
        }

        #region Index
        [Fact]
        public void Index_EmptyCart_ShowsEmptyCartView()
        {
            var emptycart = new Cart();
            var result = _controller.Index(emptycart) as ViewResult;
            Assert.Equal("EmptyCart", result?.ViewName);
        }

        [Fact]
        public void Index_NonEmptyCart_PassesCartLinesToViewViaModel()
        {
            var result = _controller.Index(_cart) as ViewResult;
            var cartresult = result?.Model as IEnumerable<CartLine>;
            Assert.Equal(1, cartresult?.Count());
        }

        [Fact]
        public void Index_NonEmptyCart_StoresTotalInViewData()
        {
            var result = _controller.Index(_cart) as ViewResult;
            Assert.Equal(50M, result.ViewData["Total"]);
        }
        #endregion

        #region Add
        [Fact]
        public void Add_Successful_RedirectsToActionIndexOfStore()
        {
            var result = _controller.Add(4, 2, _cart) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Store", result?.ControllerName);
        }

        [Fact]
        public void Add_Successful_AddsProductToCart()
        {
            _controller.Add(4, 2, _cart);
            Assert.Equal(2, _cart.NumberOfItems);
        }
        #endregion

        #region Remove
        [Fact]
        public void Remove_Successful_RedirectsToIndex()
        {
            var result = _controller.Remove(1, _cart) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void Remove_Successful_RemovesProductFromCart()
        {
            _controller.Remove(1, _cart);
            Assert.Equal(0, _cart.NumberOfItems);
        }
        #endregion

        #region Plus
        [Fact]
        public void Plus_Successful_RedirectsToIndex()
        {
            var result = _controller.Plus(1, _cart) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void Plus_Successful_IncreasesQuantity()
        {
            _controller.Plus(1, _cart);
            CartLine line = _cart.CartLines.ToList()[0];
            Assert.Equal(3, line.Quantity);
        }
        #endregion

        #region Min
        [Fact]
        public void Min_Successful_RedirectsToIndex()
        {
            var result = _controller.Min(1, _cart) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void Min_Successful_DecreasesQuantity()
        {
            _controller.Min(1, _cart);
            CartLine line = _cart.CartLines.ToList()[0];
            Assert.Equal(1, line.Quantity);
        }
        #endregion
    }
}