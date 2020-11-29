using System;

namespace Cookbook.API.Entities
{
    public class Instruction
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int Position { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
