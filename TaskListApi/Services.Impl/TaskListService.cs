using System.ComponentModel.DataAnnotations;
using TaskListApi.Exceptions;
using TaskListApi.Models;
using TaskListApi.Models.DTOs;
using TaskListApi.Repositories;

namespace TaskListApi.Services.Impl;

public class TaskListService(ITaskListRepository repo) : ITaskListService
{
    private const int MAX_NAME_LENGTH = 255;
    
    public async Task<TaskListDto> CreateAsync(string name, Guid userId, CancellationToken ct)
    {
        name = ValidateAndTrimName(name);
        
        var list = new TaskList
        {
            Id = Guid.NewGuid(),
            Name = name,
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(list, ct);
        
        return new TaskListDto(list.Id, list.Name, list.OwnerId, list.CreatedAt, new List<ShareInfoDto>());
    }

    public async Task UpdateAsync(Guid listId, string name, Guid userId, CancellationToken ct)
    {
        name = ValidateAndTrimName(name);
        
        if (!await repo.UpdateNameAsync(listId, userId, name, ct))
            throw new ForbiddenOrNotFound();
    }

    public async Task DeleteAsync(Guid listId, Guid userId, CancellationToken ct)
    {
        if (!await repo.DeleteAsync(listId, userId, ct))
            throw new ForbiddenOrNotFound();
    }

    public async Task<TaskListDto> GetAsync(Guid listId, Guid userId, CancellationToken ct)
    {
        var list = await repo.GetAccessibleAsync(listId, userId, ct);

        if (list == null)
            throw new ForbiddenOrNotFound();
        
        var sharedWithUsers = list.SharedWithUserIds
            .Select(u => new ShareInfoDto(u))
            .ToList();
        return new TaskListDto(list.Id, list.Name, list.OwnerId, list.CreatedAt, sharedWithUsers);
    }

    public async Task<IReadOnlyList<TaskListSummaryDto>> ListAsync(Guid userId, int page, int pageSize,
        SortDirection sortDirection, CancellationToken ct)
    {
        var lists = await repo.ListAccessibleAsync(userId, page, pageSize, sortDirection, ct);
        return lists
            .Select(l => new TaskListSummaryDto(l.Id, l.Name))
            .ToList();
    }

    public async Task ShareAsync(Guid listId, Guid userId, Guid targetUserId, CancellationToken ct)
    {
        await repo.AddShareAsync(listId, userId, targetUserId, ct);
    }

    public async Task<IReadOnlyList<ShareInfoDto>> GetSharesAsync(Guid listId, Guid userId, CancellationToken ct)
    {
        var users = await repo.ListSharesAsync(listId, userId, ct);
        return users.Select(u => new ShareInfoDto(u)).ToList();
    }

    public async Task RemoveShareAsync(Guid listId, Guid userId, Guid targetUserId, CancellationToken ct)
    {
        await repo.RemoveShareAsync(listId, userId, targetUserId, ct);
    }
    
    private static string ValidateAndTrimName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.");
        
        name = name.Trim();
        
        if (name.Length > MAX_NAME_LENGTH)
            throw new ValidationException($"Name length must be <= {MAX_NAME_LENGTH}.");
        
        return name;
    }
}
