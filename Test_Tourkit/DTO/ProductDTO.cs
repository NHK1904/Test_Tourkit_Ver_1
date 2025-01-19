using System.ComponentModel.DataAnnotations;

namespace Test_Tourkit.DTO
{
    public class ProductDTO
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string ProductCategory1 { get; set; } = null!;
        public DateTime DateAdded { get; set; }
        public string ProductCategoryId { get; set; }

    }
}
