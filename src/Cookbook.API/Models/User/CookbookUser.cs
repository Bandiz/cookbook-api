using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;

namespace Cookbook.API.Models
{
    [BsonIgnoreExtraElements]
    public class CookbookUser : MongoUser
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }
    }
}
