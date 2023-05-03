using Business.Abstract;
using Entity;
using Microsoft.AspNetCore.Mvc;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    public class ShopController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        public ShopController(IProductService prodductService, ICategoryService categoryService)
        {
            this._productService = prodductService;
            this._categoryService = categoryService;
        }
        public IActionResult List(string category,int page=1)
        {
            const int pageSize = 3;
            var productViewModel = new ProductViewModel()
            {
                //Products = _productService.GetAll(),
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemPerPage = pageSize,
                    CurrentCategory = category,
                },
                Categories = _categoryService.GetAll(),
                Products = _productService.GetProductByCategoryName(category,page,pageSize),
            };
            return View(productViewModel);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetails((int)id);
            var productDetailsModel = new ProductDetailsModel
            {
                Product = product,
                Categories = product.ProductCategories.Select(p => p.Category).ToList(),
            };
            if (product == null)
            {
                return NotFound();
            }
            return View(productDetailsModel);
        }

        public IActionResult Search(string q)
        {
            var productViewModel = new ProductViewModel()
            {
                Categories = _categoryService.GetAll(),
                Products = _productService.GetSearchResult(q),
            };
            return View(productViewModel);
        }
    }
}
