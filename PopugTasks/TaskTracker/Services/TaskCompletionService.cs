using KafkaTransmitter.Services;
using TaskTracker.Entities;
using TaskTracker.Models;

namespace TaskTracker.Services
{
    public class TaskCompletionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventProducerService _producer;

        public TaskCompletionService(ApplicationDbContext context, IEventProducerService producer)
        {
            _context = context;
            _producer = producer;
        }

        public async Task<TasksCompletionResult> CompleteTaskAsync(PopugTask task)
        {
            TasksCompletionResult result = new();

            task.TaskStatus = PopugTaskStatus.Completed;

            _context.Tasks.Attach(task);

            await _context.SaveChangesAsync();

            _producer.SendMessage("tasks", "TaskCompleted", new TaskCompletedModel(task));

            return result;
        }
    }
}

public class TasksCompletionResult
{
    public bool IsSuccess = false;

    public string ErrorMessage = string.Empty;
}