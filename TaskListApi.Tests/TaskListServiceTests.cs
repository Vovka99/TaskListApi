using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Moq;
using TaskListApi.Exceptions;
using TaskListApi.Models;
using TaskListApi.Repositories;
using TaskListApi.Services.Impl;

namespace TaskListApi.Tests;

public class TaskListServiceTests
{
    private readonly Mock<ITaskListRepository> _repoMock = new();
    private readonly Mock<ILogger<TaskListService>> _loggerMock = new();
    private readonly TaskListService _service;

    public TaskListServiceTests()
    {
        _service = new TaskListService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidName_ReturnsTaskListDto()
    {
        var userId = Guid.NewGuid();
        var name = "Test List";
        _repoMock.Setup(r => r.AddAsync(It.IsAny<TaskList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var result = await _service.CreateAsync(name, userId, CancellationToken.None);

        Assert.Equal(name, result.Name);
        Assert.Equal(userId, result.OwnerId);
    }

    [Fact]
    public async Task CreateAsync_EmptyName_ThrowsValidationException()
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.CreateAsync("   ", Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ThrowsForbiddenOrNotFound()
    {
        _repoMock.Setup(r => r.UpdateNameAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<ForbiddenOrNotFound>(() =>
            _service.UpdateAsync(Guid.NewGuid(), "Name", Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ThrowsForbiddenOrNotFound()
    {
        _repoMock.Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<ForbiddenOrNotFound>(() =>
            _service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
    
    [Fact]
    public async Task GetAsync_Found_ReturnsTaskListDto()
    {
        var userId = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var taskList = new TaskList
        {
            Id = listId,
            Name = "List",
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow,
            SharedWithUserIds = [Guid.NewGuid()]
        };
        _repoMock.Setup(r => r.GetAccessibleAsync(listId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        var result = await _service.GetAsync(listId, userId, CancellationToken.None);

        Assert.Equal(listId, result.Id);
        Assert.Equal(userId, result.OwnerId);
        Assert.Single(result.SharedWithUsers);
    }

    [Fact]
    public async Task GetAsync_NotFound_ThrowsForbiddenOrNotFound()
    {
        _repoMock.Setup(r => r.GetAccessibleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskList?)null);

        await Assert.ThrowsAsync<ForbiddenOrNotFound>(() =>
            _service.GetAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ListAsync_ReturnsSummaries()
    {
        var userId = Guid.NewGuid();
        var lists = new List<TaskList>
        {
            new TaskList { Id = Guid.NewGuid(), Name = "A" },
            new TaskList { Id = Guid.NewGuid(), Name = "B" }
        };
        _repoMock.Setup(r => r.ListAccessibleAsync(userId, 1, 10, SortDirection.Ascending, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lists);

        var result = await _service.ListAsync(userId, 1, 10, SortDirection.Ascending, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Name == "A");
        Assert.Contains(result, x => x.Name == "B");
    }

    [Fact]
    public async Task ShareAsync_Success()
    {
        _repoMock.Setup(r => r.AddShareAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _service.ShareAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
    }

    [Fact]
    public async Task ShareAsync_NotFound_ThrowsForbiddenOrNotFound()
    {
        _repoMock.Setup(r => r.AddShareAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<ForbiddenOrNotFound>(() =>
            _service.ShareAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task GetSharesAsync_ReturnsShareInfoDtos()
    {
        var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        _repoMock.Setup(r => r.ListSharesAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userIds);

        var result = await _service.GetSharesAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task RemoveShareAsync_Success()
    {
        _repoMock.Setup(r => r.RemoveShareAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _service.RemoveShareAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
    }

    [Fact]
    public async Task RemoveShareAsync_NotFound_ThrowsForbiddenOrNotFound()
    {
        _repoMock.Setup(r => r.RemoveShareAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<ForbiddenOrNotFound>(() =>
            _service.RemoveShareAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}