using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Entities;

public class PopugTask
{
    [Key]
    public int Id { get; set; }

    public Guid PublicId { get; set; }

    public Popug? Assignee { get; set; }

    public string Description { get; set; } = string.Empty;

    public PopugTaskStatus TaskStatus { get; set; } = PopugTaskStatus.Incomplete;

    public PopugTask()
    {
        PublicId = Guid.NewGuid();
    }
}

public enum PopugTaskStatus
{
    Incomplete,
    Completed
}
