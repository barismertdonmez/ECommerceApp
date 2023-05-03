using Data.Abstract;
using Data.DataModels;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Concrete
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category, ShopContext>, ICategoryRepository
    {
        public void DeleteFromCategory(int productId, int categoryId)
        {
            using (var context = new ShopContext())
            {
                var cmd = "delete from productcategory where ProductId=@p0 and CategoryId=@p1";
                context.Database.ExecuteSqlRaw(cmd, productId, categoryId);
            }
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            using (var context = new ShopContext())
            {
                return context.Categories
                    .Where(c => c.Id == categoryId)
                    .Include(c => c.ProductCategories)
                    .ThenInclude(c => c.Product)
                    .FirstOrDefault();
            }
        }

        public DataCategoryModel GetByIdWithProducts2(int categoryId)
        {
            using (var context = new ShopContext())
            {
                var model = context.Categories
                    .Where(c => c.Id == categoryId)
                    .Include(c => c.ProductCategories)
                    .ThenInclude(c => c.Product)
                    .FirstOrDefault();

                return new DataCategoryModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    Products = model.ProductCategories.Select(p => p.Product).ToList(),
                };
            }
        }

        public List<Category> GetPopularCategories()
        {
            throw new NotImplementedException();
        }
    }
}
