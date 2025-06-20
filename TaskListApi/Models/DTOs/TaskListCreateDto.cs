using System.ComponentModel.DataAnnotations;

namespace TaskListApi.Models.DTOs;

public class TaskListCreateDto
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; }
}
