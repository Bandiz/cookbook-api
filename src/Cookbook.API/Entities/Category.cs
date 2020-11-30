using System;

namespace Cookbook.API.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public string Name { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public Recipe Recipe { get; set; }
    }
}
