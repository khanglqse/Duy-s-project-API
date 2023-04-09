using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DuyProject.API.Models
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("ConversationId")]
        public string ConversationId { get; set; }

        [BsonElement("Sender")]
        public string Sender { get; set; }

        [BsonElement("Recipient")]
        public string Recipient { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
        [BsonElement("AttachmentUrl")]
        public string AttachmentUrl { get; set; }
    }
}
