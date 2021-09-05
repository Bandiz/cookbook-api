namespace Cookbook.API.Configuration
{
    public class CookbookDatabaseSettings : ICookbookDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string RecipesCollectionName { get; set; }
        public string CategoriesCollectionName { get; set; }
        public string CountersCollectionName { get; set; }
    }
}
