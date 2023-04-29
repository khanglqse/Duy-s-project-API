using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DuyProject.API.Models;

public class User : EntityBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Address Address { get; set; }
    public bool IsCreateBySocialAccount { get; set; }
    public string Roles { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<ConnectedChatUser> ConnectedChatUser { get; set; } = new List<ConnectedChatUser>();
}

public class ConnectedChatUser
{
    public string Id { get; set; }
    public string UserName { get; set; }
}