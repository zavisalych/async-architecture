using Confluent.Kafka;
using KafkaTransmitter.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using TaskTracker.Entities;
using KafkaTransmitter.Models;
using TaskTracker.Models;

namespace TaskTracker.Services;

public class StreamEventHandlerService : IEventHandlerService
    {
        private readonly ApplicationDbContext _context;

        public StreamEventHandlerService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task HandleEventAsync(EventModel model)
    {
        string[] split = model.EventName.Split('.');

        if (split.Length > 1)
        {
            var entity = split[0];
            var op = split[1];

            if (Enum.TryParse(op, out CUDop operation))
            {
                try
                {
                    if (entity == "Account")
                    {
                        var cudModel = JsonSerializer.Deserialize<UserCUDModel>(model.Data);

                        if (cudModel != null)
                        {
                            if (await HandleAccountUpdated(operation, cudModel))

                                Debug.WriteLine($"Processing {entity} {operation} with {cudModel} completed successfully.");
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Unexpected data format for event {model.EventName}: {ex.Message} ");
                }
            }
        }
    }


    private async Task<bool> HandleAccountUpdated(CUDop operation, UserCUDModel model)
    {
        Popug? popug = await _context.Popugs.FirstOrDefaultAsync(p => p.PublicId == model.PublicId); ;

        if (operation == CUDop.Created)
        {
            if (popug != null)
            {
                Debug.WriteLine($"Processing Account {operation} with {model} failed: popug not found.");
                return false;
            }

            popug = new Popug();
            popug.PublicId = model.PublicId;
            popug.Username = model.UserName;
            _context.Popugs.Attach(popug);
        }
        else
        {
            if (popug == null)
            {
                Debug.WriteLine($"Processing Account {operation} with {model} failed: popug not found.");
                return false;
            }
        }

        popug.FirstName = model.FirstName;
        popug.LastName = model.LastName;

        if (!Enum.TryParse(model.Role, out Role role))
        {
            Debug.WriteLine($"Processing Account {operation} with {model} failed: can't parse role {model.Role}.");
            return false;
        }
        else
            popug.Role = role;

        await _context.SaveChangesAsync();

        return true;
    }
}
