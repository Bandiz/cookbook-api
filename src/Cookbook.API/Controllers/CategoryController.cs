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

        [AllowAnonymous]
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
                CreatedBy = User.Identity.Name,
                CteatedAt = DateTime.UtcNow
            };


            categoriesService.CreateCategory(category);

            return CreatedAtAction(
                nameof(GetCategory),
                new { CategoryName = category.CategoryName },
                new CreateCategoryResponse()
                {
                    CategoryName = category.CategoryName,
                    CreatedBy = category.CreatedBy,
                    CreatedAt = category.CteatedAt
                }
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
    }
}
