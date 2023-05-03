using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.Identity;
using WebUI.Models;

namespace WebUI.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private ICartService _cartService;
        private UserManager<User> _userManager;

        public CartController(ICartService cartService,UserManager<User> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }
    
        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            var model = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(p => new CartItemModel()
                {
                    CartItemId = p.Id,
                    Name = p.Product.Name,
                    Price = (double)p.Product.Price,
                    ImageUrl = p.Product.ImageUrl,
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    
                }).ToList(),
                
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId,int quantity)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.AddToCArt(userId,productId,quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.DeleteFromCart(userId, productId);
            return RedirectToAction("Index");
        }


    }
}
