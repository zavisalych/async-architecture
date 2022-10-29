using KafkaTransmitter.Models;
using Confluent.Kafka;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace KafkaTransmitter.Services;

public interface IEventProducerService
{
    bool SendMessages<T>(string topic, string eventName, List<T> messages);

    bool SendMessage<T>(string topic, string eventName, T message);
}

public class EventProducerService: IEventProducerService
{
    private readonly ProducerConfig _config;

    public EventProducerService(IOptions<ProducerConfig> configuration)
    {
        _config = configuration.Value;
        _config.ClientId = Dns.GetHostName();
        _config.Acks = Acks.All;
        _config.EnableIdempotence = true;
    }

    public bool SendMessage<T>(string topic, string eventName, T message)
    {
        List<EventModel> events = new()
        {
            new EventModel(eventName, JsonSerializer.Serialize<T>(message))
        };

        return FireEvents(topic, events);
    }

    public bool SendMessages<T>(string topic, string eventName, List<T> messages)
    {
        List<EventModel> events = new();

        foreach (T message in messages)
        {
            events.Add(new EventModel(eventName, JsonSerializer.Serialize<T>(message)));
        }

        return FireEvents(topic, events);
    }


    private bool FireEvents (string topic, List<EventModel> events)
    {
        try
        {
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                int errorsCount = 0;

                Action<DeliveryReport<Null, string>> delRepAction = (x) => {
                    if (x.Error == ErrorCode.NoError)
                    {
                        Debug.WriteLine($"Delivery Timestamp: {x.Timestamp.UtcDateTime}");
                    } 
                    else
                    {
                        Debug.WriteLine($"Error sending message: {x.Error}");
                        errorsCount++;
                    }
                };

                foreach (var model in events)
                {
                    string message = JsonSerializer.Serialize<EventModel>(model);

                    producer.Produce(topic,
                        new Message <Null, string> { Value = message }, delRepAction);

                }

                return errorsCount == 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured: {ex.Message}");
            return false;
        }
    }
}
