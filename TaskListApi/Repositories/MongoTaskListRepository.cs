using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaskListApi.Models;
using SortDirection = TaskListApi.Models.SortDirection;

namespace TaskListApi.Repositories;

public class MongoTaskListRepository : ITaskListRepository
{
    private readonly IMongoCollection<TaskList> _collection;

    public MongoTaskListRepository(IMongoDatabase mongoDb, IOptions<MongoOptions> mongoOpts)
    {
        var collectionNames = mongoOpts.Value.Collections;
        _collection = mongoDb.GetCollection<TaskList>(collectionNames.TaskLists);
    }

    public async Task<Guid> AddAsync(TaskList list, CancellationToken ct)
    {
        await _collection.InsertOneAsync(list, cancellationToken: ct);
        return list.Id;
    }

    public async Task<bool> UpdateNameAsync(Guid id, Guid userId, string name, CancellationToken ct)
    {
        var filter = GetAccessibleFilter(id, userId);
        var update = Builders<TaskList>.Update.Set(x => x.Name, name);
        var res = await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
        return res.ModifiedCount == 1;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken ct)
    {
        var res = await _collection.DeleteOneAsync(x => x.Id == id && x.OwnerId == userId, ct);
        return res.DeletedCount == 1;
    }

    public async Task<TaskList?> GetAccessibleAsync(Guid id, Guid userId, CancellationToken ct)
    {
        var filter = GetAccessibleFilter(id, userId);
        return await _collection.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<TaskList>> ListAccessibleAsync(Guid userId, int page, int size,
        SortDirection sortDirection = SortDirection.Descending, CancellationToken ct = default)
    {
        var query = _collection.Find(x => x.OwnerId == userId || x.SharedWithUserIds.Contains(userId));

        query = sortDirection == SortDirection.Ascending
            ? query.SortBy(x => x.CreatedAt)
            : query.SortByDescending(x => x.CreatedAt);

        return await query
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync(ct);
    }

    public async Task<bool> AddShareAsync(Guid id, Guid userId, Guid targetId, CancellationToken ct)
    {
        var filter = GetAccessibleFilter(id, userId);
        var update = Builders<TaskList>.Update.AddToSet(x => x.SharedWithUserIds, targetId);
        var res = await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
        return res.ModifiedCount == 1;
    }

    public async Task<bool> RemoveShareAsync(Guid id, Guid userId, Guid targetId, CancellationToken ct)
    {
        var filter = GetAccessibleFilter(id, userId);
        var update = Builders<TaskList>.Update.Pull(x => x.SharedWithUserIds, targetId);
        var res = await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
        return res.ModifiedCount == 1;
    }

    public async Task<IReadOnlyList<Guid>> ListSharesAsync(Guid id, Guid userId, CancellationToken ct)
    {
        var list = await GetAccessibleAsync(id, userId, ct);
        return list?.SharedWithUserIds ?? [];
    }

    private FilterDefinition<TaskList> GetAccessibleFilter(Guid id, Guid userId)
    {
        return Builders<TaskList>.Filter.Where(x =>
            x.Id == id && (x.OwnerId == userId || x.SharedWithUserIds.Contains(userId)));
    }
}
