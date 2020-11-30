﻿using System;
using System.Collections.Generic;

namespace Cookbook.API.Entities
{
    public class Recipe
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int PrepTimeMinutes { get; set; }

        public int CookTimeMinutes { get; set; }

        public int TotalTimeMinutes { get; set; }

        public string ImageUrl { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<Instruction> Instructions { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }


    }
}
