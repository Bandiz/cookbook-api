using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cookbook.API.Entities
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("Name")]
        public string CategoryName { get; set; }
    }
}
