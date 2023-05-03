using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetProductDetails(int id);
        Product GetByIdWithCategories(int id);
        List<Product> GetPopularProducts();
        List<Product> GetTop5Products();
        List<Product> GetHomePageProducts();
        List<Product> GetSearchResult(string searchString);
        List<Product> GetProductByCategoryName(string name,int page,int pageSize);
        int GetCountByCategory(string category);
        void Update(Product entity, int[] categoryIds);
    }
}

