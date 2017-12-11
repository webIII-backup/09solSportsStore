﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.Domain;
using SportsStore.Models.ProductViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsStore.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace SportsStore.Controllers
{
    [Authorize(Policy = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index(int categoryId = 0)
        {
            IEnumerable<Product> products;
            if (categoryId == 0)
                products = _productRepository.GetAll();
            else
                products = _productRepository.GetByCategory(categoryId);
            products = products.OrderBy(b => b.Name).ToList();
            ViewData["Categories"] = GetCategoriesSelectList(categoryId);
            return View(products);
        }

        public IActionResult Edit(int id)
        {
            Product product = _productRepository.GetById(id);
            if (product == null)
                return NotFound();
            ViewData["IsEdit"] = true;
            ViewData["Availabilities"] = product.Availability.ToSelectList();
            ViewData["Categories"] = GetCategoriesSelectList();
            return View(new EditViewModel(product));
        }

        [HttpPost]
        public IActionResult Edit(int id, EditViewModel editViewModel)
        {
            Product product = _productRepository.GetById(id);
            if (product == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    MapToProduct(editViewModel, product);
                    _productRepository.SaveChanges();
                    TempData["message"] = $"You successfully updated product {product.Name}.";
                }
                catch
                {
                    TempData["error"] = "Sorry, something went wrong, product could not be updated...";
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IsEdit"] = true;
            ViewData["Availabilities"] = product.Availability.ToSelectList();
            ViewData["Categories"] = GetCategoriesSelectList();
            return View(editViewModel);
        }

        public IActionResult Create()
        {
            var product = new Product();
            ViewData["IsEdit"] = false;
            ViewData["Availabilities"] = product.Availability.ToSelectList();
            ViewData["Categories"] = GetCategoriesSelectList();
            return View(nameof(Edit), new EditViewModel(product));
        }

        [HttpPost]
        public IActionResult Create(EditViewModel editViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = new Product();
                    MapToProduct(editViewModel, product);
                    _productRepository.Add(product);
                    _productRepository.SaveChanges();
                    TempData["message"] = $"You successfully added product {product.Name}.";
                }
                catch
                {
                    TempData["error"] = "Sorry, something went wrong, the product was not added...";
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IsEdit"] = false;
            ViewData["Availabilities"] = editViewModel.Availability.ToSelectList();
            ViewData["Categories"] = GetCategoriesSelectList();
            return View(nameof(Edit), editViewModel);
        }

        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
                return NotFound();
            ViewData["ProductName"] = product.Name;
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                Product product = _productRepository.GetById(id);
                if (product == null)
                    return NotFound();
                _productRepository.Delete(product);
                _productRepository.SaveChanges();
                TempData["message"] = $"You successfully deleted product {product.Name}.";
            }
            catch
            {
                TempData["error"] = "Sorry, something went wrong, the product was not deleted...";
            }
            return RedirectToAction(nameof(Index));
        }

        private SelectList GetCategoriesSelectList(int selected = 0)
        {
            return new SelectList(_categoryRepository.GetAll().OrderBy(g => g.Name).ToList(),
                nameof(Category.CategoryId), nameof(Category.Name), selected);
        }

        private void MapToProduct(EditViewModel editViewModel, Product product)
        {
            product.Name = editViewModel.Name;
            product.Description = editViewModel.Description;
            product.Price = editViewModel.Price;
            product.InStock = editViewModel.InStock;
            product.Availability = editViewModel.Availability;
            product.Category = _categoryRepository.GetById(editViewModel.CategoryId);
        }
    }
}