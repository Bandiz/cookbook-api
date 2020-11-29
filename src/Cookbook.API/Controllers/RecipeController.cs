using Cookbook.API.Contexts;
using Cookbook.API.Entities;
using Cookbook.API.Models.Recipe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cookbook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly CookbookDbContext cookbookContext;

        public RecipeController(CookbookDbContext context)
        {
            this.cookbookContext = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetRecipe(int id)
        {
            var recipe = cookbookContext.Recipe.SingleOrDefault(x => x.Id == id);

            if (recipe == null)
            {
                return NotFound(id);
            }
            return Ok(recipe);
        }

        [HttpGet]
        public IActionResult GetRecipes(int count = 10)
        {
            var recipes = cookbookContext.Recipe.OrderByDescending(x => x.CreatedAt).Take(count);
            return Ok(recipes);
        }

        [HttpPost]
        public IActionResult CreateRecipe(CreateRecipeRequestModel model)
        {
            if (model == null)
            {
                return NotFound(ModelState);
            }

            var recipe = new Recipe
            {
                Title = model.Title,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                CookTimeMinutes = model.CookTimeMinutes,
                PrepTimeMinutes = model.PrepTimeMinutes,
                TotalTimeMinutes = model.TotalTimeMinutes,
                Categories = model.Categories.Select(x => new Category { Name = x, CreatedAt = DateTime.UtcNow, CreatedBy = "System" }).ToList(),
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow
            };

            cookbookContext.Recipe.Add(recipe);
            cookbookContext.SaveChanges();

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }

    }
}