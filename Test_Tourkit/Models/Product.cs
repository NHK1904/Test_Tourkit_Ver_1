using System;
using System.Collections.Generic;

namespace Test_Tourkit.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductCategories = new HashSet<ProductCategory>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
