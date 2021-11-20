using Cookbook.API.Entities;
using Cookbook.API.Models.Category;
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
    public class CategoryController : ControllerBase
    {
        private readonly CategoriesService categoriesService;

        public CategoryController(CategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = categoriesService.GetCategories().Select(x => x.CategoryName);
            return Ok(categories);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("{categoryName}")]
        public IActionResult GetCategory(string categoryName)
        {
            var category = categoriesService.GetCategory(categoryName);

            if (category == null)
            {
                return NotFound(categoryName);
            }

            return Ok(new GetCategoryResponse() 
            { 
                CategoryName = category.CategoryName, 
                CreatedBy = category.CreatedBy, 
                CreatedAt = category.CteatedAt 
            });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("list")]
        public IActionResult GetCategoriesList()
        {
            var categories = categoriesService.GetCategories(false).Select(MapCategoryResponse);
            return Ok(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateCategory(CreateCategoryRequest model)
        {
            if (model == null)
            {
                return NotFound(ModelState);
            }
            var existingCategory = categoriesService.GetCategory(model.CategoryName);

            if (existingCategory != null)
            {
                return BadRequest($"Category exists: {model.CategoryName}");
            }

            var category = new Category()
            {
                CategoryName = model.CategoryName,
                Visible = model.Visible,
                CreatedBy = User.Identity.Name,
                CteatedAt = DateTime.UtcNow
            };


            categoriesService.CreateCategory(category);

            return base.CreatedAtAction(
                nameof(GetCategory),
                new { CategoryName = category.CategoryName },
                MapCategoryResponse(category)
            );
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoryName}")]
        public IActionResult DeleteCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return BadRequest("Category name required");
            }
            var existingCategory = categoriesService.GetCategory(categoryName);

            if (existingCategory == null)
            {
                return NotFound(categoryName);
            }

            categoriesService.DeleteCategory(categoryName);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{categoryName}/visible")]
        public IActionResult Visible(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return BadRequest("Category name required");
            }
            var existingCategory = categoriesService.GetCategory(categoryName);

            if (existingCategory == null)
            {
                return NotFound(categoryName);
            }

            if (!existingCategory.Visible)
            {
                existingCategory.Visible = true;
                existingCategory.UpdatedAt = DateTime.UtcNow;
                existingCategory.UpdatedBy = User.Identity.Name;
                categoriesService.UpdateCategory(existingCategory);
            }

            return Ok(MapCategoryResponse(existingCategory));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{categoryName}/invisible")]
        public IActionResult Invisible(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return BadRequest("Category name required");
            }
            var existingCategory = categoriesService.GetCategory(categoryName);

            if (existingCategory == null)
            {
                return NotFound(categoryName);
            }

            if (existingCategory.Visible)
            {
                existingCategory.Visible = false;
                existingCategory.UpdatedAt = DateTime.UtcNow;
                existingCategory.UpdatedBy = User.Identity.Name;
                categoriesService.UpdateCategory(existingCategory);
            }

            return Ok(MapCategoryResponse(existingCategory));
        }

        private static CreateCategoryResponse MapCategoryResponse(Category category)
        {
            return new CreateCategoryResponse()
            {
                CategoryName = category.CategoryName,
                Visible = category.Visible,
                CreatedBy = category.CreatedBy,
                CreatedAt = category.CteatedAt,
                UpdatedBy = category.UpdatedBy,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
