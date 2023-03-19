using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DuyProject.API.Models
{
    public class Logo : EntityBase
    {
        public Logo(string imagePath, string pharmacyId)
        {
            ImagePath = imagePath;
            PharmacyId = pharmacyId;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string ImagePath { get; set; }
        public string PharmacyId { get; set; }
    }
}