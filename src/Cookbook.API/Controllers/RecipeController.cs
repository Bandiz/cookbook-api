using Cookbook.API.Entities;
using Cookbook.API.Models.Recipe;
using Cookbook.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Cookbook.API.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeService recipeService;

        public RecipeController(RecipeService recipeService)
        {
            this.recipeService = recipeService;
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public IActionResult GetRecipe(int id)
        {
            var recipe = recipeService.GetRecipe(id);

            if (recipe == null)
            {
                return NotFound(id);
            }
            return Ok(new GetRecipeResponseModel
            {
                Id = recipe.Id,
                Categories = recipe.Categories.ToList(),
                Ingredients = recipe.Ingredients.Select(x => new IngredientResponseModel
                {
                    Amount = x.Amount,
                    Name = x.Name,
                    MeasurementType = x.MeasurementType,
                    Position = x.Position
                }).ToList(),
                Instructions = recipe.Instructions.Select(x => new InstructionResponseModel
                {
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

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetRecipes(int count = 10)
        {
            var recipes = recipeService.GetRecipes(count);
            return Ok(recipes.Select(recipe => new GetRecipesResponseModel
            {
                Id = recipe.Id,
                Categories = recipe.Categories.ToList(),
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
                Categories = model.Categories.ToList(),
                Ingredients = model.Ingredients.Select(x => new Ingredient
                {
                    Amount = x.Amount,
                    MeasurementType = x.MeasurementType,
                    Name = x.Name,
                    Position = x.Position,
                }).ToList(),
                Instructions = model.Instructions.Select(x => new Instruction
                {
                    Description = x.Description,
                    Position = x.Position,
                }).ToList(),
                CreatedBy = "linas.jakseboga@gmail.com",
                CreatedAt = DateTime.UtcNow
            };
            recipeService.CreateRecipe(recipe);

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, new GetRecipeResponseModel
            {
                Id = recipe.Id,
                Categories = recipe.Categories.ToList(),
                Ingredients = recipe.Ingredients.Select(x => new IngredientResponseModel
                {
                    Amount = x.Amount,
                    Name = x.Name,
                    MeasurementType = x.MeasurementType,
                    Position = x.Position
                }).ToList(),
                Instructions = recipe.Instructions.Select(x => new InstructionResponseModel
                {
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
            var recipe = recipeService.GetRecipe(id);
            if (recipe == null)
            {
                return NotFound(id);
            }

            recipeService.DeleteRecipe(id);

            return Ok();
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateRecipe(int id, UpdateRecipeRequestModel model)
        {
            if (model == null)
            {
                return NotFound(ModelState);
            }

            var recipe = recipeService.GetRecipe(id);
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

            recipeService.UpdateRecipe(recipe);

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