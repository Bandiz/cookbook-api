using Cookbook.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.API.Contexts
{
    public class CookbookDbContext : DbContext
    {
        public CookbookDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Recipe> Recipe { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired();
            });
        }

    }
}
