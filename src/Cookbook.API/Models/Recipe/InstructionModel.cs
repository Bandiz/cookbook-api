using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cookbook.API.Models.Recipe
{
    public class InstructionModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int Position { get; set; }
    }
}
