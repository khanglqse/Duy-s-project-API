namespace DuyProject.API.ViewModels
{
    public class MailModel
    {
        public List<string> Receivers { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
