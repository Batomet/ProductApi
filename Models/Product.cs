using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models
{
  public class Product
  {
    public int Id { get; set; }
    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 20 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Product name must be alphanumeric.")]
    public string Name { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be at least 0.01.")]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be non-negative.")]
    public int Stock { get; set; }
  }
}
