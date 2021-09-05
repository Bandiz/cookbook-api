using Cookbook.API.Configuration;
using Cookbook.API.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Cookbook.API.Services
{
    public class CategoriesService
    {
        private readonly IMongoCollection<Category> _categories;

        public CategoriesService(ICookbookDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var cookbookDb = mongoClient.GetDatabase(settings.DatabaseName);
            _categories = cookbookDb.GetCollection<Category>(settings.CategoriesCollectionName);
        }

        public Category GetCategory(string categoryName)
        {
            return _categories.Find(x => x.CategoryName == categoryName).SingleOrDefault();
        }

        public List<Category> GetCategories()
        {
            return _categories.Find(x => true).ToList();
        }

        public Category CreateCategory(Category category)
        {
            _categories.InsertOne(category);

            return category;
        }

        public void DeleteCategory(string categoryName)
        {
            _categories.DeleteOne(x => x.CategoryName == categoryName);
        }

        public List<Category> CreateCategories(List<Category> categories)
        {
            _categories.InsertMany(categories);

            return categories;
        }

    }
}
