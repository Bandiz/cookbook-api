using Cookbook.API.Configuration;
using Cookbook.API.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cookbook.API.Contexts
{
    public class CookbookDbContext
    {
        private readonly CookbookDatabaseSettings _cookbookDbSettings;
        private MongoClient _mongoClient { get; set; }
        private IMongoDatabase _cookbookDb { get; set; }

        public IMongoCollection<Counter> Counters { get; set; }
        public IMongoCollection<Recipe> Recipes { get; set; }

        public CookbookDbContext(IOptions<CookbookDatabaseSettings> settings)
        {
            _cookbookDbSettings = settings.Value;
            _mongoClient = new MongoClient(_cookbookDbSettings.ConnectionString);
            _cookbookDb = _mongoClient.GetDatabase(_cookbookDbSettings.DatabaseName);

            Counters = _cookbookDb.GetCollection<Counter>("counters");
            Recipes = _cookbookDb.GetCollection<Recipe>(_cookbookDbSettings.RecipesCollectionName);
        }

        public int GetNewRecipeId()
        {
            var update = Builders<Counter>.Update.Inc(x => x.Sequence, 1);
            return Counters
                .FindOneAndUpdate(x => x.Id == _cookbookDbSettings.RecipesCollectionName, update)
                .Sequence;
        }
    }
}
