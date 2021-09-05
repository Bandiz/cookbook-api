using Cookbook.API.Models.Category;
using Cookbook.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Cookbook.API.Controllers
{
    [Authorize(Roles="Admin")]
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
            var categories = categoriesService.GetCategories().Select(x => new GetCategoryResponse() { CategoryName = x.CategoryName });
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

            return Ok(new GetCategoryResponse() { CategoryName = category.CategoryName });
        }

        []
        public IActionResult CreateCategory()
        {
            return Ok();
        }
    }
}
