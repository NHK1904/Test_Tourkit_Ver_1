using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test_Tourkit.DTO;
using Test_Tourkit.Models;

namespace Test_Tourkit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly Test_TourkitContext _context;

        public ProductsController(Test_TourkitContext context)
        {
            _context = context;
        }
        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts(string? productName, string? productCategory)
        {
            var productsQuery = _context.Products.Include(p => p.ProductCategories).AsQueryable();

            if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(productCategory))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(productName) &&
                                                          p.ProductCategories
                                                              .Any(c => c.ProductCategory1.Contains(productCategory)));
            }
            else
            {
                if (!string.IsNullOrEmpty(productName))
                {
                    productsQuery = productsQuery.Where(p => p.ProductName.Contains(productName));
                }

                if (!string.IsNullOrEmpty(productCategory))
                {
                    productsQuery = productsQuery.Where(p => p.ProductCategories
                        .Any(c => c.ProductCategory1.Contains(productCategory)));
                }
            }

            var products = await productsQuery
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ProductCategory1 = string.Join(", ", p.ProductCategories.Select(c => c.ProductCategory1)),
                    DateAdded = p.DateAdded
                })
                .ToListAsync();

            return Ok(products);
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories) // Bao gồm các liên kết
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            foreach (var category in product.ProductCategories.ToList())
            {
                product.ProductCategories.Remove(category);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories) // Bao gồm thông tin danh mục
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var productDto = new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                ProductCategory1 = string.Join(", ", product.ProductCategories.Select(c => c.ProductCategory1)),
                ProductCategoryId = string.Join(", ", product.ProductCategories.Select(c => c.ProductCategoryId)),
                DateAdded = product.DateAdded
            };

            return Ok(productDto);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDto)
        {

            if (id != productDto.ProductId)
            {
                return BadRequest();
            }

            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = productDto.ProductName;
            product.Price = productDto.Price;
            product.DateAdded = productDto.DateAdded;

            var currentCategories = product.ProductCategories.ToList();
            var newCategories = await _context.ProductCategories
                .Where(pc => productDto.ProductCategoryId.Contains(pc.ProductCategoryId.ToString()))
                .ToListAsync();

            foreach (var category in currentCategories)
            {
                if (!newCategories.Contains(category))
                {
                    product.ProductCategories.Remove(category);
                }
            }

            foreach (var category in newCategories)
            {
                if (!currentCategories.Contains(category))
                {
                    product.ProductCategories.Add(category);
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO productDto)
        {
            if (string.IsNullOrEmpty(productDto.ProductName))
            {
                return BadRequest(new { message = "Tên sản phẩm không được để trống." });
            }

            if (await _context.Products.AnyAsync(p => p.ProductName == productDto.ProductName))
            {
                return BadRequest(new { message = "Tên sản phẩm đã tồn tại." });
            }

            if (productDto.DateAdded == null || productDto.DateAdded == DateTime.MinValue)
            {
                return BadRequest(new { message = "Ngày nhập không hợp lệ hoặc bị để trống." });
            }

            var product = new Product
            {
                ProductName = productDto.ProductName,
                Price = productDto.Price,
                DateAdded = productDto.DateAdded,
            };

            if (!string.IsNullOrEmpty(productDto.ProductCategoryId))
            {
                var categoryIds = productDto.ProductCategoryId.Split(',');
                foreach (var categoryId in categoryIds)
                {
                    var category = await _context.ProductCategories.FindAsync(int.Parse(categoryId.Trim()));
                    if (category != null)
                    {
                        product.ProductCategories.Add(category);
                    }
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }


    }
}
