using Azure;
using Data.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Concrete
{
    public class EfCoreProductRepository :
        EfCoreGenericRepository<Product, ShopContext>, IProductRepository
    {
        public Product GetByIdWithCategories(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products
                              .Where(p => p.Id == id)
                              .Include(p => p.ProductCategories)
                              .ThenInclude(p => p.Category)
                              .FirstOrDefault();
            }
        }

        public int GetCountByCategory(string category)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.Where(p => p.IsApproved).AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                        .Include(p => p.ProductCategories)
                        .ThenInclude(p => p.Category)
                        .Where(p => p.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                }
                return products.Count();
            }
        }

        public List<Product> GetHomePageProducts()
        {
            using (var context = new ShopContext())
            {
                return context.Products
                    .Where(p => p.IsApproved == true && p.IsHome == true)
                    .ToList();
            }
        }

        public List<Product> GetPopularProducts()
        {
            using (var context = new ShopContext())
            {
                return context.Products.ToList();
            }
        }

        public List<Product> GetProductByCategoryName(string name, int page, int pageSize)
        {
            using (var context = new ShopContext())
            {
                var products = context
                    .Products
                    .Where(p => p.IsApproved)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(name))
                {
                    products = products
                        .Include(p => p.ProductCategories)
                        .ThenInclude(p => p.Category)
                        .Where(p => p.ProductCategories.Any(a => a.Category.Name.ToLower() == name.ToLower()));
                }
                return products.Skip((page-1)*pageSize).Take(pageSize).ToList();
            }
        }

        public Product GetProductDetails(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products
                    .Where(p => p.Id == id)
                    .Include(p => p.ProductCategories)
                    .ThenInclude(p => p.Category)
                    .FirstOrDefault();
            }
        }

        public List<Product> GetSearchResult(string searchString)
        {
            using (var context = new ShopContext())
            {
                var products = context
                    .Products
                    .Where(p => p.IsApproved && (p.Name.ToLower().Contains(searchString) || p.Description.ToLower().Contains(searchString)))
                    .AsQueryable();
                return products.ToList();
            }
        }

        public List<Product> GetTop5Products()
        {
            throw new System.NotImplementedException();
        }

        public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new ShopContext())
            {
                var product = context.Products
                                      .Include(p => p.ProductCategories)
                                      .FirstOrDefault(p => p.Id == entity.Id);
                if (product != null)
                {
                    product.IsApproved = entity.IsApproved;
                    product.IsHome = entity.IsHome;
                    product.Name=entity.Name;
                    product.Price =entity.Price;
                    product.Description=entity.Description;
                    product.ImageUrl = entity.ImageUrl;
                    product.ProductCategories = categoryIds.Select(catid=> new ProductCategory()
                    {
                        ProductId = entity.Id,
                        CategoryId = catid,
                    }).ToList();
                    context.SaveChanges();
                }
            }
        }
    }
}
