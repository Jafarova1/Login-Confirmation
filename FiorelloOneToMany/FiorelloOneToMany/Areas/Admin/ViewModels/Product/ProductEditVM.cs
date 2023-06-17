using FiorelloOneToMany.Models;
using System.ComponentModel.DataAnnotations;

namespace FiorelloOneToMany.Areas.Admin.ViewModels.Product
{
    public class ProductEditVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int DiscountId { get; set; }
        public List<IFormFile> NewImages { get; set; }
        public List<ProductImage> Images { get; set; }
    }
}
