namespace DuyProject.API.Models
{
    public class MailData
    {
        public List<string> To { get; }

        // Sender
        public string? From { get; }

        public string? DisplayName { get; }

        // Content
        public string Subject { get; }

        public string? Body { get; }

        public MailData(List<string> to, string subject, string? body = null, string? from = null, string? displayName = null)
        {
            // Receiver
            To = to;

            // Sender
            From = from;
            DisplayName = displayName;

            // Content
            Subject = subject;
            Body = body;
        }
    }
}
