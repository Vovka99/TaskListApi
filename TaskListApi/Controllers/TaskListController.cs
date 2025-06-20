using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TaskListApi.Models;
using TaskListApi.Models.DTOs;
using TaskListApi.Services;

namespace TaskListApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskListController(ITaskListService taskListService) : ControllerBase
{
    private Guid CurrentUserId => (Guid)HttpContext.Items["UserId"]!;

    [HttpPost]
    public async Task<ActionResult<TaskListDto>> Create(TaskListCreateDto dto, CancellationToken ct)
    {
        var taskList = await taskListService.CreateAsync(dto.Name, CurrentUserId, ct);
        return Ok(taskList);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, TaskListUpdateDto dto, CancellationToken ct)
    {
        await taskListService.UpdateAsync(id, dto.Name, CurrentUserId, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await taskListService.DeleteAsync(id, CurrentUserId, ct);
        return NoContent();
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskListDto>> Get(Guid id, CancellationToken ct)
    {
        var list = await taskListService.GetAsync(id, CurrentUserId, ct);
        return Ok(list);
    }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskListSummaryDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] SortDirection sortDirection = SortDirection.Descending,
        CancellationToken ct = default)
    {
        var items = await taskListService.ListAsync(CurrentUserId, page, pageSize, sortDirection, ct);
        return Ok(items);
    }
    
    [HttpPost("{id:guid}/shares/{targetUserId:guid}")]
    public async Task<ActionResult> Share(Guid id, Guid targetUserId, CancellationToken ct)
    {
        await taskListService.ShareAsync(id, CurrentUserId, targetUserId, ct);
        return NoContent();
    }
    
    [HttpGet("{id:guid}/shares")]
    public async Task<ActionResult<IReadOnlyList<ShareInfoDto>>> GetShares(Guid id, CancellationToken ct)
    {
        var users = await taskListService.GetSharesAsync(id, CurrentUserId, ct);
        return Ok(users);
    }
    
    [HttpDelete("{id:guid}/shares/{targetUserId:guid}")]
    public async Task<ActionResult> RemoveShare(Guid id, Guid targetUserId, CancellationToken ct)
    {
        await taskListService.RemoveShareAsync(id, CurrentUserId, targetUserId, ct);
        return NoContent();
    }
}
