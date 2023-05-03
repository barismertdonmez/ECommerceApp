using Business.Abstract;
using Data.Abstract;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductRepository _productRepository;
        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public bool Create(Product entity)
        {
            if (Validate(entity))
            {
                _productRepository.Create(entity);
                return true;
            }
            return false;
        }

        public void Delete(Product entity)
        {
            // iş kuralları
            _productRepository.Delete(entity);
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public List<Product> GetProductByCategoryName(string name,int page, int pageSize)
        {
            return _productRepository.GetProductByCategoryName(name,page,pageSize);
        }

        public Product GetProductDetails(int id)
        {
            return _productRepository.GetProductDetails(id);
        }

        public List<Product> GetSearchResult(string searchString)
        {
            return _productRepository.GetSearchResult(searchString);
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public bool Update(Product entity, int[] categoryIds)
        {
            if (Validate(entity))
            {
                if (categoryIds.Length == 0)
                {
                    ErrorMessage += "Ürün için en az bir kategori seçmelisiniz";
                    return false;
                }
                _productRepository.Update(entity, categoryIds);
                return true;
            }
            return false;
        }

        public string ErrorMessage { get; set ; }
        public bool Validate(Product entity)
        {
            var isValid = true;
            if (string.IsNullOrEmpty(entity.Name))
            {
                ErrorMessage += "Ürün ismi girmelisiniz.";
                isValid = false;
            }
            if (entity.Price < 0)
            {
                ErrorMessage += "Ürün fiyatı negatif olamaz.";
                isValid = false;
            }
            return isValid;
        }
    }
}
