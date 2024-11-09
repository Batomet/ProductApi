using ProductApi.Data;
using ProductApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductApi.Services
{
  public class ProductService
  {
    private readonly ProductRepository _repository;

    public ProductService(ProductRepository repository)
    {
      _repository = repository;
    }

    public async Task<List<Product>> GetAllProductsAsync() => await _repository.GetAllAsync();

    public async Task<Product> GetProductByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public async Task AddProductAsync(Product product) => await _repository.AddAsync(product);

    public async Task UpdateProductAsync(Product product) => await _repository.UpdateAsync(product);

    public async Task DeleteProductAsync(int id) => await _repository.DeleteAsync(id);
  }
}
