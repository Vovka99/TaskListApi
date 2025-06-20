using System.ComponentModel.DataAnnotations;
using TaskListApi.Exceptions;
using TaskListApi.Models;
using TaskListApi.Models.DTOs;
using TaskListApi.Repositories;

namespace TaskListApi.Services.Impl;

public class TaskListService(ITaskListRepository repo, ILogger<TaskListService> logger) : ITaskListService
{
    private const int MAX_NAME_LENGTH = 255;

    public async Task<TaskListDto> CreateAsync(string name, Guid userId, CancellationToken ct)
    {
        logger.LogInformation("Creating task list for user {UserId} with name '{Name}'", userId, name);
        name = ValidateAndTrimName(name);

        var list = new TaskList
        {
            Id = Guid.NewGuid(),
            Name = name,
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(list, ct);

        logger.LogInformation("Task list {ListId} created for user {UserId}", list.Id, userId);
        return new TaskListDto(list.Id, list.Name, list.OwnerId, list.CreatedAt, new List<ShareInfoDto>());
    }

    public async Task UpdateAsync(Guid listId, string name, Guid userId, CancellationToken ct)
    {
        logger.LogInformation("Updating task list {ListId} for user {UserId} with new name '{Name}'", listId, userId,
            name);
        name = ValidateAndTrimName(name);

        if (!await repo.UpdateNameAsync(listId, userId, name, ct))
        {
            logger.LogWarning("Update failed: Task list {ListId} not found or forbidden for user {UserId}", listId,
                userId);
            throw new ForbiddenOrNotFound();
        }

        logger.LogInformation("Task list {ListId} updated for user {UserId}", listId, userId);
    }

    public async Task DeleteAsync(Guid listId, Guid userId, CancellationToken ct)
    {
        logger.LogInformation("Deleting task list {ListId} for user {UserId}", listId, userId);
        if (!await repo.DeleteAsync(listId, userId, ct))
        {
            logger.LogWarning("Delete failed: Task list {ListId} not found or forbidden for user {UserId}", listId,
                userId);
            throw new ForbiddenOrNotFound();
        }

        logger.LogInformation("Task list {ListId} deleted for user {UserId}", listId, userId);
    }

    public async Task<TaskListDto> GetAsync(Guid listId, Guid userId, CancellationToken ct)
    {
        logger.LogInformation("Getting task list {ListId} for user {UserId}", listId, userId);
        var list = await repo.GetAccessibleAsync(listId, userId, ct);

        if (list == null)
        {
            logger.LogWarning("Get failed: Task list {ListId} not found or forbidden for user {UserId}", listId,
                userId);
            throw new ForbiddenOrNotFound();
        }

        var sharedWithUsers = list.SharedWithUserIds
            .Select(u => new ShareInfoDto(u))
            .ToList();
        logger.LogInformation("Task list {ListId} retrieved for user {UserId}", listId, userId);
        return new TaskListDto(list.Id, list.Name, list.OwnerId, list.CreatedAt, sharedWithUsers);
    }

    public async Task<IReadOnlyList<TaskListSummaryDto>> ListAsync(Guid userId, int page, int pageSize,
        SortDirection sortDirection, CancellationToken ct)
    {
        logger.LogInformation(
            "Listing task lists for user {UserId}, page {Page}, pageSize {PageSize}, sort {SortDirection}", userId,
            page, pageSize, sortDirection);
        var lists = await repo.ListAccessibleAsync(userId, page, pageSize, sortDirection, ct);
        return lists
            .Select(l => new TaskListSummaryDto(l.Id, l.Name))
            .ToList();
    }

    public async Task ShareAsync(Guid listId, Guid userId, Guid targetUserId, CancellationToken ct)
    {
        logger.LogInformation("Sharing task list {ListId} from user {UserId} to user {TargetUserId}", listId, userId,
            targetUserId);
        if (!await repo.AddShareAsync(listId, userId, targetUserId, ct))
        {
            logger.LogWarning("Share failed: Task list {ListId} not found or forbidden for user {UserId}", listId,
                userId);
            throw new ForbiddenOrNotFound();
        }

        logger.LogInformation("Task list {ListId} shared from user {UserId} to user {TargetUserId}", listId, userId,
            targetUserId);
    }

    public async Task<IReadOnlyList<ShareInfoDto>> GetSharesAsync(Guid listId, Guid userId, CancellationToken ct)
    {
        logger.LogInformation("Getting shares for task list {ListId} by user {UserId}", listId, userId);
        var users = await repo.ListSharesAsync(listId, userId, ct);
        return users.Select(u => new ShareInfoDto(u)).ToList();
    }

    public async Task RemoveShareAsync(Guid listId, Guid userId, Guid targetUserId, CancellationToken ct)
    {
        logger.LogInformation("Removing share for task list {ListId} from user {UserId} to user {TargetUserId}", listId,
            userId, targetUserId);
        if (!await repo.RemoveShareAsync(listId, userId, targetUserId, ct))
        {
            logger.LogWarning("Remove share failed: Task list {ListId} not found or forbidden for user {UserId}",
                listId, userId);
            throw new ForbiddenOrNotFound();
        }

        logger.LogInformation("Share removed for task list {ListId} from user {UserId} to user {TargetUserId}", listId,
            userId, targetUserId);
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
