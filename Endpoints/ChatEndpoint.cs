using DuyProject.API.Repositories;

namespace DuyProject.API.Endpoints
{
    public static class ChatEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/ChatUsers", async (string userName, IChatService chatService) =>
            {
                var result = chatService.GetChatUsers(userName);
                return Results.Ok(result);
            });
        }
    }
}