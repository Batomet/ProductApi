using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProductApi.Models;
using ProductApi.Data;
using ProductApi.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductsController : ControllerBase
  {
    private readonly ProductService _service;
    private readonly ProductDbContext _context;

    public ProductsController(ProductService service, ProductDbContext context)
    {
      _service = service;
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
      return await _service.GetAllProductsAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
      var product = await _service.GetProductByIdAsync(id);
      if (product == null) return NotFound();
      return product;
    }

    [HttpPost]
    public async Task<ActionResult> AddProduct(Product product)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(GetModelErrors(ModelState));
      }

      if (_context.Products.Any(p => p.Name == product.Name))
      {
        return BadRequest(new { Error = "Product name must be unique." });
      }

      await _service.AddProductAsync(product);
      return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
      if (id != product.Id)
      {
        return BadRequest(new { Error = "Product ID mismatch." });
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(GetModelErrors(ModelState));
      }

      if (_context.Products.Any(p => p.Name == product.Name && p.Id != id))
      {
        return BadRequest(new { Error = "Product name must be unique." });
      }

      var existingProduct = await _context.Products.FindAsync(id);
      if (existingProduct == null)
      {
        return NotFound(new { Error = "Product not found." });
      }

      existingProduct.Name = product.Name;
      existingProduct.Price = product.Price;
      existingProduct.Stock = product.Stock;

      await _context.SaveChangesAsync();
      return NoContent();
    }

    private Dictionary<string, string> GetModelErrors(ModelStateDictionary modelState)
    {
      return modelState
        .Where(m => m.Value.Errors.Any())
        .ToDictionary(
          m => m.Key,
          m => m.Value.Errors.First().ErrorMessage
        );
    }
  }
}
