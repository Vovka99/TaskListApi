using MongoDB.Bson.Serialization.Attributes;

namespace TaskListApi.Models;

public class TaskList
{
    [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
    public Guid OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }

    [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
    public List<Guid> SharedWithUserIds { get; set; } = new();
}
