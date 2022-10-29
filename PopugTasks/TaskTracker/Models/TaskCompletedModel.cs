using TaskTracker.Entities;

namespace TaskTracker.Models;

public class TaskCompletedModel
{
    public string TaskPublicId { get; set; } = string.Empty;

    public TaskCompletedModel(PopugTask task)
    {
        TaskPublicId = task.PublicId.ToString();
    }
}
