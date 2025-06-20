using TaskListApi.Models;

namespace TaskListApi.Repositories;

public interface ITaskListRepository
{
    Task<Guid> AddAsync(TaskList list, CancellationToken ct);
    Task<bool> UpdateNameAsync(Guid id, Guid userId, string name, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken ct);
    Task<TaskList?> GetAccessibleAsync(Guid id, Guid userId, CancellationToken ct);

    Task<IReadOnlyList<TaskList>> ListAccessibleAsync(Guid userId, int page, int size, SortDirection sortDirection,
        CancellationToken ct);

    Task<bool> AddShareAsync(Guid id, Guid userId, Guid targetId, CancellationToken ct);
    Task<bool> RemoveShareAsync(Guid id, Guid userId, Guid targetId, CancellationToken ct);
    Task<IReadOnlyList<Guid>> ListSharesAsync(Guid id, Guid userId, CancellationToken ct);
}
