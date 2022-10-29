using KafkaTransmitter.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskTracker.Entities;
using TaskTracker.Models;

namespace TaskTracker.Services;

public class TasksAssignmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IEventProducerService _producer;

    public TasksAssignmentService(ApplicationDbContext context, IEventProducerService producer)
    {
        _context = context;
        _producer = producer;
    }

    public async Task<TasksAssignmentResult> AssignTasks()
    {
        TasksAssignmentResult result = new();

        List<PopugTask> tasks = await _context.Tasks.ToListAsync<PopugTask>();

        if (tasks.Count == 0)
        {
            result.ErrorMessage = "No tasks.";
            return result;
        }

        Popug[] popugs = await _context.Popugs.Where(x => !(Role.Manager | Role.Admin).HasFlag(x.Role)).ToArrayAsync();

        int numPopugs = popugs.Length;

        if (numPopugs == 0)
        {
            result.ErrorMessage = "No popugs.";
            return result;
        }

        Random rnd = new Random();

        List<TaskAssignedModel> messages = new();

        foreach (var task in tasks)
        {
            var popug = popugs[rnd.Next(0, numPopugs)];
            task.Assignee = popug;

            messages.Add(new TaskAssignedModel(task));
        }

        await _context.SaveChangesAsync();

        result.IsSuccess = _producer.SendMessages("tasks", "TaskAssigned", messages); 

        result.NumTasksAssigned = tasks.Count;
        return result;
    }
}

public class TasksAssignmentResult
{
    public bool IsSuccess = false;

    public int NumTasksAssigned = 0;

    public string ErrorMessage = string.Empty;
}