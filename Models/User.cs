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
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double[] Coordinates { get; set; }
    public string Type { get; set; } = "Point";
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