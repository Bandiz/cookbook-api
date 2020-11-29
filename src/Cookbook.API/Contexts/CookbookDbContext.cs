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
                entity.Property(e => e.ImageUrl).IsRequired();
                entity.HasMany(e => e.Categories);
                entity.HasMany(e => e.Instructions);
                entity.HasMany(e => e.Ingredients);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => new { e.Name, e.RecipeId });
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Instruction>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

        }

    }
}
