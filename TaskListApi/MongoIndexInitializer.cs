using MongoDB.Driver;
using TaskListApi.Models;

namespace TaskListApi;

public static class MongoIndexInitializer
{
    public static async Task InitializeIndexesAsync(IMongoDatabase database, MongoOptions options)
    {
        var collection = database.GetCollection<TaskList>(options.Collections.TaskLists);

        var builder = Builders<TaskList>.IndexKeys;
        
        var listIdIdx = new CreateIndexModel<TaskList>(
            builder.Ascending(x => x.Id),
            new CreateIndexOptions { Name = "ix_taskLists_id" });
        
        var ownerIdx = new CreateIndexModel<TaskList>(
            builder.Ascending(x => x.OwnerId),
            new CreateIndexOptions { Name = "ix_taskLists_owner" });
        
        var sharedIdx = new CreateIndexModel<TaskList>(
            builder.Ascending(x => x.SharedWithUserIds),
            new CreateIndexOptions { Name = "ix_taskLists_sharedWith" });

        await collection.Indexes.CreateManyAsync([listIdIdx, ownerIdx, sharedIdx]);
    }
}
