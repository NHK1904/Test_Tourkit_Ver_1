using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Test_Tourkit.DTO;
using Test_Tourkit.Models;

namespace Test_Tourkit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly Test_TourkitContext _context;

        public ProductCategoryController(Test_TourkitContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.ProductCategories
                .Select(c => new { c.ProductCategoryId, c.ProductCategory1 })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("GetProductCategories")]
        public async Task<IActionResult> GetProductCategories()
        {
            var productCategoriesQuery = _context.ProductCategories.Include(p => p.Products).AsQueryable();
            var producCategories = await productCategoriesQuery.
                Select(c => new ProductDTO
                {
                    ProductCategoryId = c.ProductCategoryId.ToString(),
                    ProductCategory1 = c.ProductCategory1,
                    DateAdded = c.DateAdded,
                    ProductId = c.Products.Count 
                }).ToListAsync();
            return Ok(producCategories);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var productCategory = await _context.ProductCategories.Include(c => c.Products).
                FirstOrDefaultAsync(c => c.ProductCategoryId == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            foreach (var category in productCategory.Products.ToList())
            {
                productCategory.Products.Remove(category);
            }

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCategory(int id)
        {
            var productCategory = await _context.ProductCategories
                .Include(p => p.Products) 
                .FirstOrDefaultAsync(p => p.ProductCategoryId == id);

            if (productCategory == null)
            {
                return NotFound("Product not found.");
            }

            var productDto = new ProductDTO
            {
                ProductCategoryId = productCategory.ProductCategoryId.ToString(),
                ProductCategory1 = productCategory.ProductCategory1,
                DateAdded = productCategory.DateAdded,
                ProductId = productCategory.Products.Count
            };

            return Ok(productDto);
        }

        [HttpPost("AddProductCategory")]
        public async Task<IActionResult> AddProductCategory([FromBody] ProductDTO productDto)
        {
            if (string.IsNullOrEmpty(productDto.ProductCategory1))
            {
                return BadRequest(new { message = "Tên loại sản phẩm không được để trống." });
            }

            if (await _context.ProductCategories.AnyAsync(p => p.ProductCategory1 == productDto.ProductCategory1))
            {
                return BadRequest(new { message = "Loại sản phẩm đã tồn tại." });
            }

            if (productDto.DateAdded == null || productDto.DateAdded == DateTime.MinValue)
            {
                return BadRequest(new { message = "Ngày nhập không hợp lệ hoặc bị để trống." });
            }

            var productCategory = new ProductCategory
            {
                ProductCategory1 = productDto.ProductCategory1,
                DateAdded = productDto.DateAdded,
            };

            

            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductCategory), new { id = productCategory.ProductCategoryId }, productCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDto)
        {
            if (id != int.Parse(productDto.ProductCategoryId))
            {
                return BadRequest("ProductCategoryId mismatch.");
            }

            var productCategory = await _context.ProductCategories
                .Include(p => p.Products)
                .FirstOrDefaultAsync(p => p.ProductCategoryId == id);

            if (productCategory == null)
            {
                return NotFound("ProductCategory not found.");
            }

            if (productDto.ProductId == 0 )
            {
                
                if (productCategory == null)
                {
                    return NotFound();
                }

                foreach (var category in productCategory.Products.ToList())
                {
                    productCategory.Products.Remove(category);
                }

                _context.ProductCategories.Remove(productCategory);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            else
            {
                productCategory.ProductCategory1 = productDto.ProductCategory1;
                productCategory.DateAdded = productDto.DateAdded;
                
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok("ProductCategory has been updated successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error updating ProductCategory: {ex.Message}");
                }
            }
        }

    }
}
