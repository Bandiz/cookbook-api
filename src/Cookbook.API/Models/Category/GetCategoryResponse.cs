using System;

namespace Cookbook.API.Models.Category
{
    public class GetCategoryResponse
    {
        public string CategoryName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
