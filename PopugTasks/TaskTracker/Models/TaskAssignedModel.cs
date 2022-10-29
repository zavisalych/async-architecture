using TaskTracker.Entities;

namespace TaskTracker.Models;

public class TaskAssignedModel
{
    public string TaskPublicId { get; set; } = string.Empty;

    public string PopugPublicId { get; set; } = string.Empty;

    public TaskAssignedModel(PopugTask task)
    {
        TaskPublicId = task.PublicId.ToString();
        PopugPublicId = task.Assignee?.PublicId.ToString() ?? string.Empty;
    }
}
