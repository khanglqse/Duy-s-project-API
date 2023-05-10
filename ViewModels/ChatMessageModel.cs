public class ChatMessageModel
    {
        public string? Id { get; set; }
        public string? ConversationId { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string? Message { get; set; }
        public string? Folder { get; set; }
        public DateTime Timestamp { get; set; }
        public string? FileName { get; set; }
    }