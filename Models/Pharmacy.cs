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
        public string Drugs { get; set; }
        public string DoctorId { get; set; }
        public string LogoId { get; set; }
        public string Column { get; set; }
        public string ReferenceImage { get; set; }
        public string Type { get; set; }
    }
}