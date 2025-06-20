using TaskListApi.Models;
using TaskListApi.Models.DTOs;

namespace TaskListApi.Services;

public interface ITaskListService
{
    Task<TaskListDto> CreateAsync(string name, Guid userId, CancellationToken ct);
    Task UpdateAsync(Guid listId, string name, Guid userId, CancellationToken ct);
    Task DeleteAsync(Guid listId, Guid userId, CancellationToken ct);

    Task<TaskListDto> GetAsync(Guid listId, Guid userId, CancellationToken ct);
    Task<IReadOnlyList<TaskListSummaryDto>> ListAsync(Guid userId, int page, int pageSize, SortDirection sortDirection,
        CancellationToken ct);

    Task ShareAsync(Guid listId, Guid userId, Guid targetUserId, CancellationToken ct);
    Task<IReadOnlyList<ShareInfoDto>> GetSharesAsync(Guid listId, Guid userId, CancellationToken ct);
    Task RemoveShareAsync(Guid listId, Guid userId, Guid targetUserId, CancellationToken ct);
}
