using Business.Abstract;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebUI.Identity;
using WebUI.Models;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        public AdminController(IProductService productService, ICategoryService categoryService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        //**************************************************
        //*********** Product Method ları ******************
        //**************************************************
        public IActionResult ProductList()
        {
            return View(new ProductViewModel()
            {
                Products = _productService.GetAll()
            });
        }
        //Insert Product
        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateProduct(ProductModel model)
        {
            var entity = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
            };
            if (_productService.Create(entity))
            {
                CreateMessage("Kayıt eklendi", "success");
                return RedirectToAction("ProductList");
            }

            CreateMessage(_productService.ErrorMessage, "danger");
            return View(model);
        }
        //Update Product
        [HttpGet]
        public IActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entity = _productService.GetByIdWithCategories((int)id);
            if (entity == null)
            {
                return NotFound();
            }
            var model = new ProductModel()
            {
                IsApproved = entity.IsApproved,
                IsHome = entity.IsHome,
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                ImageUrl = entity.ImageUrl,
                Description = entity.Description,
                SelectedCategories = entity.ProductCategories.Select(p => p.Category).ToList(),
                Categories = _categoryService.GetAll(),
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult EditProduct(ProductModel model, int[] categoryIds)
        {
            var entity = _productService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Price = model.Price;
            entity.ImageUrl = model.ImageUrl;
            entity.IsApproved = model.IsApproved;
            entity.IsHome = model.IsHome;
            _productService.Update(entity, categoryIds);
            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} isimli ürün güncellendi",
                AlertType = "warning"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
            return RedirectToAction("ProductList");
        }
        //Delete Product
        public IActionResult DeleteProduct(int id)
        {
            var entity = _productService.GetById(id);
            if (entity != null)
            {
                _productService.Delete(entity);
            }
            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} isimli ürün silindi",
                AlertType = "danger"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
            return RedirectToAction("ProductList");
        }

        //**************************************************
        //*********** Category Methods ******************
        //**************************************************
        public IActionResult CategoryList()
        {
            return View(new ProductViewModel()
            {
                Categories = _categoryService.GetAll()
            });
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model)
        {
            var entity = new Category()
            {
                Name = model.Name,
                
            };
            _categoryService.Create(entity);
            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} isimli kategori eklendi",
                AlertType = "success"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
            return RedirectToAction("CategoryList");
        }
        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entity = _categoryService.GetByIdWithProducts((int)id);
            if (entity == null)
            {
                return NotFound();
            }
            var model = new CategoryModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Products = entity.ProductCategories.Select(p => p.Product).ToList(),

            };
            return View(model);

        }
        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
            var entity = _categoryService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.Name = model.Name;
            _categoryService.Update(entity);
            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} isimli kategori güncellendi",
                AlertType = "warning"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
            return RedirectToAction("CategoryList");
        }
        public IActionResult DeleteCategory(int id)
        {
            var entity = _categoryService.GetById(id);
            if (entity != null)
            {
                _categoryService.Delete(entity);
            }
            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} isimli kategori silindi",
                AlertType = "danger"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
            return RedirectToAction("CategoryList");
        }
        [HttpPost]
        public IActionResult DeleteFromCategory(int productId, int categoryId)
        {
            _categoryService.DeleteFromCategory(productId, categoryId);
            return RedirectToAction("CategoryList");
        }

        //*****************************************************
        //***************** Yardımcı Methodlar ****************
        //*****************************************************

        private void CreateMessage(string message, string alertType)
        {
            var msg = new AlertMessage()
            {
                Message = message.ToString(),
                AlertType = alertType,
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
        }

        //*****************************************************
        //***************** Role Methods **********************
        //*****************************************************
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        [HttpGet]
        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var user in _userManager.Users.ToList())
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name)
                                ? members : nonmembers;
                list.Add(user);
            }
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            //if (ModelState.IsValid)
            //{
            foreach (var userId in model.IdsToAdd ?? new string[] { })
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            foreach (var userId in model.IdsToDelete ?? new string[] { })
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            //}
            return Redirect("/admin/role/" + model.RoleId);
        }
        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        //*****************************************************
        //***************** User Methods **********************
        //*****************************************************

        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(i => i.Name);
                ViewBag.Roles = roles;
                return View(new UserDetailsModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles,
                });
            }
            return Redirect("~/admin/user/list");
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model, string[] selectedRoles)
        {
            //if (ModelState.IsValid)
            //{
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    selectedRoles = selectedRoles ?? new string[] { };
                    await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
                    await _userManager.RemoveFromRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
                    return Redirect("/admin/user/list");
                }
            }
            //return Redirect("/admin/user/list");
            //}
            return View(model);
        }

    }
}
