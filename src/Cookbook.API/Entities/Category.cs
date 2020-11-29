using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cookbook.API.Entities
{
    public class Category
    {
        public int RecipeId { get; set; }

        public string Name { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public Recipe Recipe { get; set; }
    }
}
