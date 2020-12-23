using Cookbook.API.Contexts;
using Cookbook.API.Entities;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Cookbook.API.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly CookbookDbContext context;

        public RecipeService(CookbookDbContext context)
        {
            this.context = context;
        }

        public Recipe GetRecipe(int id)
        {
            return context.Recipes.Find(x => x.Id == id).SingleOrDefault();
        }

        public List<Recipe> GetRecipes(int count)
        {
            return context.Recipes.Find(x => true).SortBy(x => x.CreatedBy).Limit(count).ToList();
        }

        public Recipe CreateRecipe(Recipe recipe)
        {
            var newId = context.GetNewRecipeId();
            recipe.Id = newId;
            context.Recipes.InsertOne(recipe);

            return recipe;
        }

        public void UpdateRecipe(Recipe recipe)
        {
            context.Recipes.ReplaceOne(x => x.Id == recipe.Id, recipe);
        }

        public void DeleteRecipe(int id)
        {
            context.Recipes.DeleteOne(x => x.Id == id);
        }
    }
}
