using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DuyProject.API.Models
{
    public class FileDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FilePath { get; set; }
    }
}
