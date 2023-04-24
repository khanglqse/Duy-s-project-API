using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DuyProject.API.Models
{
    public class Pharmacy : EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public List<string> DrugIds { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<string> DoctorIds { get; set; }
        public string LogoId { get; set; }
        public List<string> FollowUser { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
    }
}