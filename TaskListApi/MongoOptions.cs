namespace TaskListApi;

public class MongoOptions
{
    public const string Section = "Mongo";

    public string ConnectionString { get; init; } = null!;
    public string Database { get; init; } = null!;
    public MongoCollectionsOptions Collections { get; init; } = new();
}

public class MongoCollectionsOptions
{
    public string TaskLists { get; init; }
}
