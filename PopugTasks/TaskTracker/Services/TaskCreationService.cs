using KafkaTransmitter.Services;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Entities;
using TaskTracker.Models;

namespace TaskTracker.Services
{
    public class TaskCreationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventProducerService _producer;

        public TaskCreationService(ApplicationDbContext context, IEventProducerService producer)
        {
            _context = context;
            _producer = producer;
        }

        public async Task<TasksCreationResult> CreateTaskAsync(PopugTask task)
        {
            TasksCreationResult result = new();

            Popug[] popugs = await _context.Popugs.Where(x => !(Role.Manager | Role.Admin).HasFlag(x.Role)).ToArrayAsync();

            int numPopugs = popugs.Length;

            if (numPopugs == 0)
            {
                result.ErrorMessage = "No popugs.";
                return result;
            }

            Random rnd = new Random();

            task.Assignee = popugs[rnd.Next(0, numPopugs)];

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            _producer.SendMessage("tasks", "TaskCreated", new TaskAssignedModel(task));

            result.IsSuccess = true;
            return result;
        }
    }
}

public class TasksCreationResult
{
    public bool IsSuccess = false;

    public string ErrorMessage = string.Empty;
}