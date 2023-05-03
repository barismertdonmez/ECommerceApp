using Business.Abstract;
using Data.Abstract;
using Data.DataModels;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private ICategoryRepository _categoryRepository;
        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public void Create(Category entity)
        {
            _categoryRepository.Create(entity);
        }

        public void Delete(Category entity)
        {
            _categoryRepository.Delete(entity);
        }

        public void DeleteFromCategory(int productId, int categoryId)
        {
           _categoryRepository.DeleteFromCategory(productId, categoryId);
        }

        public List<Category> GetAll()
        {
            return _categoryRepository.GetAll();
        }

        public Category GetById(int id)
        {
           return _categoryRepository.GetById(id);
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            return _categoryRepository.GetByIdWithProducts(categoryId);
        }

        public void Update(Category entity)
        {
            _categoryRepository.Update(entity);
        }

        public string ErrorMessage { get; set ; }

        public bool Validate(Category entity)
        {
            var isValid = true;

            //if (string.IsNullOrEmpty(entity.Name))
            //{
            //    ErrorMessage += "Ürün ismi girmek zorundasınız.";
            //}
            //if (entity.Price<0)
            //{
            //    ErrorMessage += "Ürün ismi girmek zorundasınız.";
            //}
            return isValid;
        }

        public DataCategoryModel GetByIdWithProducts2(int categoryId)
        {
            return _categoryRepository.GetByIdWithProducts2(categoryId);
        }
    }
}
