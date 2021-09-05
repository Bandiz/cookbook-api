using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Cookbook.API.Entities
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string CategoryName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CteatedAt { get; set; }
    }
}
