using Cookbook.API.Entities;
using System.Collections.Generic;

namespace Cookbook.API.Services
{
    public interface IRecipeService
    {
        public Recipe GetRecipe(int id);
        public List<Recipe> GetRecipes(int count);
        public Recipe CreateRecipe(Recipe recipe);
        public void UpdateRecipe(Recipe recipe);
        public void DeleteRecipe(int id);
    }
}