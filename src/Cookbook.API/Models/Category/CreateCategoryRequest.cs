namespace Cookbook.API.Models.Category
{
    public class CreateCategoryRequest
    {
        public string CategoryName { get; set; }

        public bool Visible { get; set; }
    }
}
