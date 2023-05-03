using Entity;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Display(Name="Name",Prompt ="Enter product name")]
        [Required(ErrorMessage ="Name zorunlu bir alandır.")]
        [StringLength(60,MinimumLength =5,ErrorMessage ="Ürün ismi 5-60 karakter arasında olmamlıdır.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price zorunlu bir alandır.")]
        [Range(1,100000,ErrorMessage ="Fiyat için 1-100000 arasında bir değer girilmeli")]
        public double? Price { get; set; }

        [Required(ErrorMessage = "Description zorunlu bir alandır.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Ürün ismi 5-100 karakter arasında olmamlıdır.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "ImageUrl zorunlu bir alandır.")]
        public string ImageUrl { get; set; }

        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<Category> SelectedCategories { get; set; }
        public List<Category> Categories { get; set; }

    }
}
