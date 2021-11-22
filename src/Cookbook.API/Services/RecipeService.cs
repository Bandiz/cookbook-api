﻿using Cookbook.API.Configuration;
using Cookbook.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cookbook.API.Services
{
    public class RecipeService
    {
        private readonly IMongoCollection<Counter> _counters;
        private readonly IMongoCollection<Recipe> _recipes;
        private readonly CategoriesService categoryService;

        public RecipeService(CookbookDatabaseSettings settings, IMongoClient mongoClient, CategoriesService categoryService)
        {
            this.categoryService = categoryService;
            var cookbookDb = mongoClient.GetDatabase(settings.DatabaseName);

            _counters = cookbookDb.GetCollection<Counter>($"{nameof(Counter).ToLower()}s");
            _recipes = cookbookDb.GetCollection<Recipe>($"{nameof(Recipe).ToLower()}s");
        }

        public Recipe GetRecipe(int id)
        {
            return _recipes.Find(x => x.Id == id).SingleOrDefault();
        }

        public List<Recipe> GetRecipes(string text, int count, List<string> categories)
        {
            var filter = Builders<Recipe>.Filter.Empty;

            if (!string.IsNullOrEmpty(text))
            {
                var textFilter = Builders<Recipe>.Filter.Text(text, new TextSearchOptions()
                {
                    CaseSensitive = false
                });
                filter = Builders<Recipe>.Filter.And(filter, textFilter);
            }

            if (categories != null && categories.Count > 0)
            {
                var categoryFilter = Builders<Recipe>.Filter.AnyIn(recipe => recipe.Categories, categories);
                filter = Builders<Recipe>.Filter.And(filter, categoryFilter);
            }

            return _recipes.Find(filter).SortBy(x => x.CreatedBy).Limit(count).ToList();
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
                .FindOneAndUpdate(x => x.Id == nameof(Recipe).ToLower(), update)
                .Sequence;
        }

        private void UpdateCategories(Recipe recipe)
        {
            if (recipe.Categories.Count > 0)
            {
                var categories = categoryService.GetCategories().Select(x => x.CategoryName).ToList();
                var notAddedCategories = recipe.Categories.Where(x => !categories.Contains(x)).ToList();

                if (notAddedCategories.Count == 0)
                {
                    return;
                }

                categoryService.CreateCategories(notAddedCategories.Select(x => new Category()
                {
                    CategoryName = x,
                    CreatedBy = recipe.UpdatedBy == null ? recipe.CreatedBy : recipe.UpdatedBy,
                    CteatedAt = DateTime.UtcNow
                }).ToList());
            }
        }
    }
}
