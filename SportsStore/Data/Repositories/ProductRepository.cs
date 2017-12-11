﻿using Microsoft.EntityFrameworkCore;
using SportsStore.Models.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SportsStore.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Product> _products;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
            _products = context.Products;
        }

        public IEnumerable<Product> GetAll()
        {
            return _products.AsNoTracking().ToList();
        }

        public Product GetById(int productId)
        {
            return _products.Include(p => p.Category).SingleOrDefault(p => p.ProductId == productId);
        }

        public IEnumerable<Product> GetByCategory(int categoryId)
        {
            return _products.Where(p => p.Category.CategoryId == categoryId).ToList();
        }

        public IEnumerable<Product> GetByAvailability(IEnumerable<Availability> availabilities)
        {
            return _products.Where(p => availabilities.Contains(p.Availability)).OrderBy(p => p.Name).AsNoTracking().ToList();
        }

        public void Add(Product product)
        {
            _products.Add(product);
        }

        public void Delete(Product product)
        {
            _products.Remove(product);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
