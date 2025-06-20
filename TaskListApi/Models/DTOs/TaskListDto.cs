namespace TaskListApi.Models.DTOs;

public record TaskListDto(Guid Id, string Name, Guid OwnerId, DateTime CreatedAt, IReadOnlyList<ShareInfoDto> SharedWithUsers);
