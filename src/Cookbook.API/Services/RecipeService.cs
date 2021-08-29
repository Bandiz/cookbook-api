using Cookbook.API.Configuration;
using Cookbook.API.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Cookbook.API.Services
{
    public class RecipeService
    {
        private readonly IMongoCollection<Counter> _counters;
        private readonly IMongoCollection<Recipe> _recipes;
        private readonly IMongoCollection<Category> _categories;
        private readonly ICookbookDatabaseSettings _settings;

        public RecipeService(ICookbookDatabaseSettings settings)
        {
            _settings = settings;
            var mongoClient = new MongoClient(settings.ConnectionString);
            var cookbookDb = mongoClient.GetDatabase(settings.DatabaseName);

            _counters = cookbookDb.GetCollection<Counter>("counters");
            _recipes = cookbookDb.GetCollection<Recipe>(settings.RecipesCollectionName);
            _categories = cookbookDb.GetCollection<Category>("categories");
        }

        public Recipe GetRecipe(int id)
        {
            return _recipes.Find(x => x.Id == id).SingleOrDefault();
        }

        public List<Recipe> GetRecipes(int count)
        {
            return _recipes.Find(x => true).SortBy(x => x.CreatedBy).Limit(count).ToList();
        }

        public Recipe CreateRecipe(Recipe recipe)
        {
            var newId = GetNewRecipeId();
            recipe.Id = newId;

            _recipes.InsertOne(recipe);

            UpdateCategories(recipe);

            return recipe;
        }

        public void UpdateRecipe(Recipe recipe)
        {
            _recipes.ReplaceOne(x => x.Id == recipe.Id, recipe);

            UpdateCategories(recipe);
        }

        public void DeleteRecipe(int id)
        {
            _recipes.DeleteOne(x => x.Id == id);
        }

        private int GetNewRecipeId()
        {
            var update = Builders<Counter>.Update.Inc(x => x.Sequence, 1);
            return _counters
                .FindOneAndUpdate(x => x.Id == _settings.RecipesCollectionName, update)
                .Sequence;
        }

        private void UpdateCategories(Recipe recipe)
        {
            if (recipe.Categories.Count > 0)
            {
                var categories = _categories.Find(x => recipe.Categories.Contains(x.CategoryName)).ToList().Select(x => x.CategoryName);
                var notAddedCategories = recipe.Categories.Where(x => !categories.Contains(x)).ToList();

                if (notAddedCategories.Count == 0)
                {
                    return;
                }
                _categories.InsertMany(notAddedCategories.Select(x => new Category() { CategoryName = x }));
            }
        }
    }
}
