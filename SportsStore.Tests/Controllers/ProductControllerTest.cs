﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models.Domain;
using SportsStore.Models.ProductViewModels;
using SportsStore.Tests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SportsStore.Tests.Controllers
{
    public class ProductControllerTest
    {
        private readonly ProductController _productController;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly DummyApplicationDbContext _dummyContext = new DummyApplicationDbContext();
        private readonly Product _runningShoes;
        private readonly int _runningShoesId;
        private readonly Product _nieuwProduct;
        private readonly int _nonExistingProductId = 999;
        private readonly int _soccerId = 1;

        public ProductControllerTest()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _runningShoes = _dummyContext.RunningShoes;
            _runningShoesId = _runningShoes.ProductId;

            _productController = new ProductController(_mockProductRepository.Object, _mockCategoryRepository.Object)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };

            _nieuwProduct = new Product()
            {
                Availability = Availability.OnlineOnly,
                Category = _dummyContext.Soccer,
                Description = "nieuw product",
                Name = "nieuw product",
                Price = 10
            };

            _mockProductRepository.Setup(p => p.GetAll()).Returns(_dummyContext.Products);
            _mockProductRepository.Setup(p => p.GetById(_runningShoesId)).Returns(_dummyContext.RunningShoes);
            _mockProductRepository.Setup(p => p.GetById(_nonExistingProductId)).Returns(null as Product);

            _mockCategoryRepository.Setup(p => p.GetAll()).Returns(_dummyContext.Categories);
            _mockCategoryRepository.Setup(c => c.GetById(_soccerId)).Returns(_dummyContext.Soccer);
        }

        #region == Index ==

        [Fact]
        public void Index_AllCategories_PassesAllProductsSortedByNameInModel()
        {
            var result = _productController.Index() as ViewResult;
            List<Product> products = (result?.Model as IEnumerable<Product>)?.ToList();
            Assert.Equal(11, products.Count);
            Assert.Equal("Bling-bling King", products[0].Name);
            Assert.Equal("Unsteady chair", products[10].Name);
        }

        [Fact]
        public void Index_ExistingCategory_PassesSelectListWithAllCategoriesSortedByNameInViewData()
        {
            var result = _productController.Index() as ViewResult;
            var categories = result?.ViewData["Categories"] as SelectList;
            Assert.Equal(3, categories.Count());
        }

        [Fact]
        public void Index_ExistingCategory_PassesAllProductsFromThatCategorySortedByNameInModel()
        {
            _mockProductRepository.Setup(p => p.GetByCategory(_soccerId)).Returns(_dummyContext.Soccer.Products);
            var result = _productController.Index(1) as ViewResult;
            List<Product> products = (result?.Model as IEnumerable<Product>)?.ToList();
            Assert.Equal(4, products.Count);
            Assert.Equal("Corner flags", products[0].Name);
        }

        #endregion

        #region == Edit ==

        [Fact]
        public void EditHttpGet_ValidProductId_PassesProductDetailsInAnEditViewModelToView()
        {
            var result = _productController.Edit(_runningShoesId) as ViewResult;
            var productVm = result?.Model as EditViewModel;
            Assert.Equal("Running shoes", productVm?.Name);
        }

        [Fact]
        public void EditHttpGet_ValidProductId_PassesSelectListWithAllCategoriesSortedByNameInViewData()
        {
            var result = _productController.Edit(_runningShoesId) as ViewResult;
            var categories = result?.ViewData["Categories"] as SelectList;
            Assert.Equal(3, categories.Count());
        }

        [Fact]
        public void EditHttpGet_ValidProductId_PassesSelectListWithAllAvailabilitiesInViewData()
        {
            var result = _productController.Edit(_runningShoesId) as ViewResult;
            var availabilities = result?.ViewData["Availabilities"] as SelectList;
            Assert.Equal(3, availabilities.Count());
        }

        [Fact]
        public void EditHttpGet_ProductNotFound_ReturnsNotFound()
        {
            var result = _productController.Edit(_nonExistingProductId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void EditHttpPost_ValidEdit_UpdatesAndPersistsTheProduct()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes)
            {
                Name = "RunningShoesGewijzigd",
                Price = 1000
            };
            _productController.Edit(_runningShoesId, productVm);
            Assert.Equal("RunningShoesGewijzigd", _runningShoes.Name);
            Assert.Equal(1000, _runningShoes.Price);
            Assert.Equal("Protective and fashionable", _runningShoes.Description);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void EditHttpPost_ValidEdit_RedirectsToIndex()
        {
            var productVm = new EditViewModel(_runningShoes);
            var result = _productController.Edit(_runningShoesId, productVm) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void EditHttpPost_InValidEdit_DoesNotChangeNorPersistProduct()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes) { Price = -1 };
            _productController.Edit(_runningShoesId, productVm);
            var runningShoes = _dummyContext.RunningShoes;
            Assert.Equal("Running shoes", runningShoes.Name);
            Assert.Equal(95, runningShoes.Price);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void EditHttpPost_InValidEdit_RedirectsToActionIndex()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes) { Price = -1 };
            var action = _productController.Edit(_runningShoesId, productVm) as RedirectToActionResult;
            Assert.Equal("Index", action?.ActionName);
        }

        [Fact]
        public void EditHttpPost_ProductNotFound_ReturnsNotFoundResult()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes);
            var action = _productController.Edit(_nonExistingProductId, productVm);
            Assert.IsType<NotFoundResult>(action);
        }

        [Fact]
        public void EditHttpPost_ModelStateErrors_DoesNotChangeNorPersistTheProduct()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes);
            _productController.ModelState.AddModelError("", "Any error");
            _productController.Edit(_runningShoesId, productVm);
            var runningShoes = _dummyContext.RunningShoes;
            Assert.Equal("Running shoes", runningShoes.Name);
            Assert.Equal(95, runningShoes.Price);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void EditHttpPost_ModelStateErrors_PassesEditViewModelInViewResultModel()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes);
            _productController.ModelState.AddModelError("", "Any error");
            var result = _productController.Edit(_runningShoesId, productVm) as ViewResult;
            productVm = result?.Model as EditViewModel;
            Assert.Equal("Running shoes", productVm.Name);
        }

        [Fact]
        public void EditHttpPost_ModelStateErrors_PassesSelectListsInViewData()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes);
            _productController.ModelState.AddModelError("", "Any error");
            var result = _productController.Edit(_runningShoesId, productVm) as ViewResult;
            var categories = result?.ViewData["Categories"] as SelectList;
            var availabilities = result?.ViewData["Availabilities"] as SelectList;
            Assert.Equal(3, categories.Count());
            Assert.Equal(3, availabilities.Count());
        }
        #endregion

        #region == Create ==

        [Fact]
        public void CreateHttpGet_PassesDetailsOfANewProductInAnEditViewModelToView()
        {
            var result = _productController.Create() as ViewResult;
            var productVm = result?.Model as EditViewModel;
            Assert.Null(productVm?.Name);
        }

        [Fact]
        public void CreateHttpPost_ValidProduct_RedirectsToIndex()
        {
            var productVm = new EditViewModel(_nieuwProduct);
            var result = _productController.Create(productVm) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void CreateHttpPost_ValidProduct_AddsNewProductToRepository()
        {
            _mockProductRepository.Setup(p => p.Add(It.IsNotNull<Product>()));
            var productVm = new EditViewModel(_nieuwProduct);
            _productController.Create(productVm);
            _mockProductRepository.Verify(m => m.Add(It.IsNotNull<Product>()), Times.Once);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CreateHttpPost_InvalidProduct_DoesNotCreateNorPersistsProduct()
        {
            _mockProductRepository.Setup(m => m.Add(It.IsAny<Product>()));
            var productVm = new EditViewModel(_nieuwProduct) { Price = -1 };
            _productController.Create(productVm);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Never());
            _mockProductRepository.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
        }

        [Fact]
        public void CreateHttpPost_InvalidProduct_RedirectsToActionIndex()
        {
            var productVm = new EditViewModel(_nieuwProduct) { Price = -1 };
            var action = _productController.Create(productVm) as RedirectToActionResult;
            Assert.Equal("Index", action?.ActionName);
        }

        [Fact]
        public void CreateHttpPost_ModelStateErrors_DoesNotChangeNorPersistTheProduct()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes);
            _productController.ModelState.AddModelError("", "Any error");
            _productController.Create(productVm);
            var runningShoes = _dummyContext.RunningShoes;
            Assert.Equal("Running shoes", runningShoes.Name);
            Assert.Equal(95, runningShoes.Price);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void CreateHttpPost_ModelStateErrors_PassesEditViewModelInViewResultModel()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes);
            _productController.ModelState.AddModelError("", "Any error");
            var result = _productController.Create(productVm) as ViewResult;
            productVm = result?.Model as EditViewModel;
            Assert.Equal("Running shoes", productVm.Name);
        }

        [Fact]
        public void CreateHttpPost_ModelStateErrors_PassesSelectListsInViewData()
        {
            var productVm = new EditViewModel(_dummyContext.RunningShoes);
            _productController.ModelState.AddModelError("", "Any error");
            var result = _productController.Create(productVm) as ViewResult;
            var categories = result?.ViewData["Categories"] as SelectList;
            var availabilities = result?.ViewData["Availabilities"] as SelectList;
            Assert.Equal(3, categories.Count());
            Assert.Equal(3, availabilities.Count());
        }

        #endregion

        #region == Delete ==

        [Fact]
        public void DeleteHttpGet_ProductFound_PassesProductNameInViewDataToView()
        {
            var result = _productController.Delete(_runningShoesId) as ViewResult;
            Assert.Equal("Running shoes", result?.ViewData["ProductName"]);
        }

        [Fact]
        public void DeleteHttpGet_ProductNotFound_ReturnsNotFound()
        {
            var result = _productController.Delete(_nonExistingProductId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteHttpPost_ProductFound_DeletesProduct()
        {
            _mockProductRepository.Setup(p => p.Delete(_dummyContext.RunningShoes));
            _productController.DeleteConfirmed(_runningShoesId);
            _mockProductRepository.Verify(m => m.Delete(_dummyContext.RunningShoes), Times.Once);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteHttpPost_SuccessfullDelete_RedirectsToIndex()
        {
            var result = _productController.DeleteConfirmed(_runningShoesId) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void DeleteHttpPost_UnsuccessfullDelete_RedirectsToIndex()
        {
            _mockProductRepository.Setup(p => p.GetById(111)).Throws<ArgumentException>();
            var result = _productController.DeleteConfirmed(111) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void DeleteHttpPost_ProductNotFound_ReturnsNotFound()
        {
            var result = _productController.DeleteConfirmed(_nonExistingProductId);
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion
    }
}
