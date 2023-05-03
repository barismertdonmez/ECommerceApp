using Entity;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Display(Name = "Name", Prompt = "Enter category name")]
        [Required(ErrorMessage = "Name zorunlu bir alandır.")]
        [StringLength(60, MinimumLength = 5, ErrorMessage = "Kategori ismi 5-60 karakter arasında olmamlıdır.")]
        public string Name { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<Product> Products{ get; set; }
    }
}
