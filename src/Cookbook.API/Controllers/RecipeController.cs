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

        [Authorize(Roles = "Admin")]
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
                Ingredients = model.Ingredients.Select((x, index) => new Ingredient
                {
                    Id = index + 1,
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
                CreatedBy = User.Identity.Name,
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

            var updated = false;

            if (!string.IsNullOrEmpty(model.Title))
            {
                updated = true;
                recipe.Title = model.Title;
            }

            if (!string.IsNullOrEmpty(model.Description))
            {
                updated = true;
                recipe.Description = model.Description;
            }

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                updated = true;
                recipe.ImageUrl = model.ImageUrl;
            }

            if (model.PrepTimeMinutes.HasValue)
            {
                updated = true;
                recipe.PrepTimeMinutes = model.PrepTimeMinutes.Value;
            }

            if (model.CookTimeMinutes.HasValue)
            {
                updated = true;
                recipe.CookTimeMinutes = model.CookTimeMinutes.Value;
            }

            if (model.TotalTimeMinutes.HasValue)
            {
                updated = true;
                recipe.TotalTimeMinutes = model.TotalTimeMinutes.Value;
            }

            if (updated)
            {
                recipe.UpdatedBy = User.Identity.Name;
                recipe.UpdatedAt = DateTime.UtcNow;
                recipeService.UpdateRecipe(recipe);
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

        [Authorize(Roles = "Admin")]
        [HttpPost("{recipeId:int}/ingredient")]
        public IActionResult CreateIngredient(int recipeId, CreateIngredientRequestModel model)
        {
            if (model == null)
            {
                return NotFound(ModelState);
            }

            var recipe = recipeService.GetRecipe(recipeId);
            if (recipe == null)
            {
                return NotFound(recipeId);
            }

            var elementAt = recipe.Ingredients.ElementAtOrDefault(model.Position);

            var newIngredient = new Ingredient()
            {
                Id = recipe.Ingredients.Count + 1,
                Name = model.Name,
                Amount = model.Amount,
                MeasurementType = model.MeasurementType,
                Position = elementAt == null ? recipe.Ingredients.Count : model.Position
            };

            if (elementAt == null)
            {
                recipe.Ingredients.Add(newIngredient);
            }
            else
            {
                foreach (var ingredient in recipe.Ingredients.Where(x => x.Position >= model.Position))
                {
                    ingredient.Position++;
                }
                recipe.Ingredients.Add(newIngredient);

            }

            recipe.UpdatedBy = User.Identity.Name;
            recipe.UpdatedAt = DateTime.UtcNow;

            recipeService.UpdateRecipe(recipe);

            return Ok(recipe.Ingredients.OrderBy(x => x.Position).Select(x => new IngredientResponseModel(x)));
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{recipeId:int}/ingredient/{ingredientId:int}")]
        public IActionResult DeleteIngredient(int recipeId, int ingredientId)
        {
            var recipe = recipeService.GetRecipe(recipeId);
            if (recipe == null)
            {
                return NotFound(recipeId);
            }

            var element = recipe.Ingredients.SingleOrDefault(x => x.Id == ingredientId);


            if (element == null)
            {
                return NotFound(ingredientId);
            }

            recipe.Ingredients.Remove(element);

            foreach (var (ingredient, index) in recipe.Ingredients.OrderBy(x => x.Position).Select((x, i) => (x, i)))
            {
                  ingredient.Position = index;
            }

            recipe.UpdatedBy = User.Identity.Name;
            recipe.UpdatedAt = DateTime.UtcNow;

            recipeService.UpdateRecipe(recipe);

            return Ok(recipe.Ingredients.OrderBy(x => x.Position).Select(x => new IngredientResponseModel(x)));
        }

    }
}