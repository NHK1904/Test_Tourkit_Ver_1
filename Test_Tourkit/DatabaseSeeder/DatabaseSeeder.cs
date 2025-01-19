using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test_Tourkit.Models;

namespace Test_Tourkit.DatabaseSeeder
{
    public class DatabaseSeeder
    {
        private readonly Test_TourkitContext _context;
        private static readonly Random _random = new Random();

        public DatabaseSeeder(Test_TourkitContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (_context.Products.Any()) return;

            var categories = new List<ProductCategory>();
            for (int i = 0; i < 20; i++)
            {
                categories.Add(new ProductCategory
                {
                    ProductCategory1 = $"Category {i + 1}",
                    DateAdded = DateTime.UtcNow
                });
            }

            _context.ProductCategories.AddRange(categories);
            _context.SaveChanges();

            var products = new List<Product>();
            for (int i = 0; i < 10000; i++)
            {
                var productName = $"Product {i + 1}";
                var price = (decimal)(_random.Next(100, 1000) + _random.NextDouble());
                var dateAdded = DateTime.UtcNow.AddDays(-_random.Next(0, 365));

                var product = new Product
                {
                    ProductName = productName,
                    Price = price,
                    DateAdded = dateAdded
                };

                var selectedCategories = categories.OrderBy(x => _random.Next()).Take(_random.Next(1, 4)).ToList();
                foreach (var category in selectedCategories)
                {
                    product.ProductCategories.Add(category);
                }

                products.Add(product);
            }

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }
    }
}
