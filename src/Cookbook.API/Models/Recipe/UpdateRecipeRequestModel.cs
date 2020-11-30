﻿namespace Cookbook.API.Models.Recipe
{
    public class UpdateRecipeRequestModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int? PrepTimeMinutes { get; set; }

        public int? CookTimeMinutes { get; set; }

        public int? TotalTimeMinutes { get; set; }

        public string ImageUrl { get; set; }
    }
}
