using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DuyProject.API.Models
{
    public class Disease : EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> CauseIds { get; set; }
        public List<string> DrugIds { get; set; }
        public string BasicExperiment { get; set; }
        public string Approach { get; set; }
        public string Treatment { get; set; }
        public string Diet { get; set; }
        public string LivingActivity { get; set; }
        public string ReferenceImage { get; set; }
        public string Type { get; set; }
    }
}