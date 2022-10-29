namespace KafkaTransmitter.Models;

public enum CUDop
{
    Created,
    Updated,
    Deleted
}

public class EventModel
{
    public string EventName { get; set; }

    public string Data { get; set; }

    public EventModel(string eventName, string data)
    {
        EventName = eventName;
        Data = data;
    }
}
