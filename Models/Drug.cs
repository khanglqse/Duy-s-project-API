using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DuyProject.API.Models
{
    public class Drug : EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; }
        public string Effect { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 0;
        public double Price { get; set; } = 0;
        public string Type { get; set; }
    }
}