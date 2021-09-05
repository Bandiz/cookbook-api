using Cookbook.API.Configuration;
using Cookbook.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cookbook.API.Services
{
    public class CategoriesService
    {
        private readonly IMongoCollection<Category> _categories;
        private readonly ICookbookDatabaseSettings _settings;

        public CategoriesService(ICookbookDatabaseSettings settings)
        {
            _settings = settings;
            var mongoClient = new MongoClient(settings.ConnectionString);
            var cookbookDb = mongoClient.GetDatabase(settings.DatabaseName);
            _categories = cookbookDb.GetCollection<Category>("categories");
        }

        public Category GetCategory(string categoryName)
        {
            return _categories.Find(x => x.CategoryName == categoryName).SingleOrDefault();
        }

        public List<Category> GetCategories()
        {
            return _categories.Find(x => true).ToList();
        }
    }
}
