﻿using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.API.Entities
{
    public class Product
    {

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public required string Name { get; set; }
        public required string Category { get; set; }
        public required string Summary { get; set; }
        public required string Description { get; set; }
        public required string ImageFile { get; set; }
        public decimal Price { get; set; }
    }
}
