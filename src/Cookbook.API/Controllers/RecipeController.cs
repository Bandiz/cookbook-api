using Cookbook.API.Contexts;
using Cookbook.API.Entities;
using Cookbook.API.Models.Recipe;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            var recipe = cookbookContext.Recipe
                .Include(x => x.Categories)
                .Include(x => x.Ingredients)
                .Include(x => x.Instructions)
                .SingleOrDefault(x => x.Id == id);

            if (recipe == null)
            {
                return NotFound(id);
            }
            return Ok(new GetRecipeResponseModel
            {
                Id = recipe.Id,
                Categories = recipe.Categories.Select(x => x.Name).ToList(),
                Ingredients = recipe.Ingredients.Select(x => new IngredientResponseModel
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Name = x.Name,
                    MeasurementType = x.MeasurementType,
                    Position = x.Position
                }).ToList(),
                Instructions = recipe.Instructions.Select(x => new InstructionResponseModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    Position = x.Position
                }).ToList(),
                Title = recipe.Title,
                Description = recipe.Description,
                ImageUrl = recipe.ImageUrl,
                CookTimeMinutes = recipe.CookTimeMinutes,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                TotalTimeMinutes = recipe.TotalTimeMinutes,
            });
        }

        [HttpGet]
        public IActionResult GetRecipes(int count = 10)
        {
            var recipes = cookbookContext.Recipe.OrderByDescending(x => x.CreatedAt).Take(count);
            return Ok(recipes.Select(recipe => new GetRecipesResponseModel
            {
                Id = recipe.Id,
                Categories = recipe.Categories.Select(x => x.Name).ToList(),
                Title = recipe.Title,
                ImageUrl = recipe.ImageUrl,
                TotalTimeMinutes = recipe.TotalTimeMinutes,
            }));
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
                Categories = model.Categories.Select(x => new Category { 
                    Name = x, 
                    CreatedAt = DateTime.UtcNow, 
                    CreatedBy = "linas.jakseboga@gmail.com" }
                ).ToList(),
                Ingredients = model.Ingredients.Select(x => new Ingredient
                {
                    Amount = x.Amount,
                    MeasurementType = x.MeasurementType,
                    Name = x.Name,
                    Position = x.Position,
                    CreatedBy = "linas.jakseboga@gmail.com",
                    CreatedAt = DateTime.UtcNow,
                }).ToList(),
                Instructions = model.Instructions.Select(x => new Instruction
                {
                    Description = x.Description,
                    Position = x.Position,
                    CreatedBy = "linas.jakseboga@gmail.com",
                    CreatedAt = DateTime.UtcNow,
                }).ToList(),
                CreatedBy = "linas.jakseboga@gmail.com",
                CreatedAt = DateTime.UtcNow
            };
            cookbookContext.Recipe.Add(recipe);
            cookbookContext.SaveChanges();

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, new GetRecipeResponseModel
            {
                Id = recipe.Id,
                Categories = recipe.Categories.Select(x => x.Name).ToList(),
                Ingredients = recipe.Ingredients.Select(x => new IngredientResponseModel
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Name = x.Name,
                    MeasurementType = x.MeasurementType,
                    Position = x.Position
                }).ToList(),
                Instructions = recipe.Instructions.Select(x => new InstructionResponseModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    Position = x.Position
                }).ToList(),
                Title = recipe.Title,
                Description = recipe.Description,
                ImageUrl = recipe.ImageUrl,
                CookTimeMinutes = recipe.CookTimeMinutes,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                TotalTimeMinutes = recipe.TotalTimeMinutes,
            });
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteRecipe(int id)
        {
            var recipe = cookbookContext.Recipe.SingleOrDefault(x => x.Id == id);
            if (recipe == null)
            {
                return NotFound(id);
            }

            cookbookContext.Recipe.Remove(recipe);
            cookbookContext.SaveChanges();

            return Ok();
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateRecipe(int id, UpdateRecipeRequestModel model)
        {
            if (model == null)
            {
                return NotFound(ModelState);
            }

            var recipe = cookbookContext.Recipe.SingleOrDefault(x => x.Id == id);
            if (recipe == null)
            {
                return NotFound(id);
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                recipe.Title = model.Title;
            }

            if (!string.IsNullOrEmpty(model.Description))
            {
                recipe.Description = model.Description;
            }

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                recipe.ImageUrl = model.ImageUrl;
            }

            if (model.PrepTimeMinutes.HasValue)
            {
                recipe.PrepTimeMinutes = model.PrepTimeMinutes.Value;
            }

            if (model.CookTimeMinutes.HasValue)
            {
                recipe.CookTimeMinutes = model.CookTimeMinutes.Value;
            }

            if (model.TotalTimeMinutes.HasValue)
            {
                recipe.TotalTimeMinutes = model.TotalTimeMinutes.Value;
            }

            return Ok(new GetRecipeResponseModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                ImageUrl = recipe.ImageUrl,
                CookTimeMinutes = recipe.CookTimeMinutes,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                TotalTimeMinutes = recipe.TotalTimeMinutes,
            });
        }

    }
}