using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using WebUI.Identity;
using WebUI.Models;

namespace WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private ICartService _cartService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
        }
        //***********************
        //******* Login *********
        [HttpGet]
        public IActionResult Login()
        {
           return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Bu kullanıcı adı ile daha önce hesap oluşturulmamış");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Lütfen Mail hesabınıza gelen gelen doğrulama linki ile mail adresinizi onaylayın");
                return View(model);
            }

            //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                return Redirect("~/");
            }

            ModelState.AddModelError("", "Girilen kullanıcı maili veya parola yanlış");
            return View(model);
        }


        //***********************
        //******* Register *********
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
            };
            var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {
                _cartService.InitializeCart(user.Id);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code,
                });
                Console.WriteLine(url);

                //email
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }


        //************************
        //******** Logout*********
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        //************************
        //********* Email ********
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                CreateMessage("Geçersiz token bilgisi","danger");
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    CreateMessage("Hesabınız onaylandı", "success");
                    return View();
                }
            }
            CreateMessage("Hesabınız onaylanmadı", "danger");
            return View();
        }

        //*******************************************
        //********* Yardımcı Methodlar ***************
        private void CreateMessage(string message, string alertType)
        {
            var msg = new AlertMessage()
            {
                Message = message.ToString(),
                AlertType = alertType,
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CustomerEdit(string id)
        {
            var customer = await _userManager.FindByIdAsync(id);
            if (customer != null)
            {
                var model = new CustomerModel()
                {
                    Id = customer.Id,
                    UserName = customer.UserName,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email, 
                };
                return View(model);
            }
            return View();
        }

    }
}
