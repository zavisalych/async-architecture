using Confluent.Kafka;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using KafkaTransmitter.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;

namespace KafkaTransmitter.Services;

public class BackgroundConsumerTopicOptions
{
    public Dictionary<string, Func<IServiceProvider, IEventHandlerService>> Handlers;

    public BackgroundConsumerTopicOptions()
    {
        Handlers = new Dictionary<string, Func<IServiceProvider, IEventHandlerService>>();
    }

}

public class BackgroundConsumerService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly BackgroundConsumerTopicOptions _topics;
    private readonly ConsumerConfig _config;

    private IConsumer<Ignore, string>? _consumer;

    public BackgroundConsumerService(IServiceProvider services, BackgroundConsumerTopicOptions topics)
    {
        _services = services;
        _topics = topics;
        _config = services.GetRequiredService<IOptions<ConsumerConfig>>().Value;
        _config.AutoOffsetReset = AutoOffsetReset.Earliest;
        _config.EnableAutoCommit = false;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _consumer = new ConsumerBuilder<Ignore, string>(_config).Build();

            _consumer.Subscribe(_topics.Handlers.Keys);

            Task.Run(() => ConsumeAsync(cancellationToken));

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer?.Close();

        return Task.CompletedTask;
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        if (_consumer == null)
            return;

        try
        {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(stoppingToken);

                    try
                    {
                        var message = JsonSerializer.Deserialize<EventModel>(result.Message.Value);

                        if (message != null && _topics.Handlers.ContainsKey(result.Topic))
                        {
                            using (var scope = _services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                            {
                                var handler = _topics.Handlers[result.Topic](scope.ServiceProvider);

                                await handler.HandleEventAsync(message);
                            }

                            _consumer.Commit(result);
                        }

                    }
                    catch (JsonException ex)
                    {
                        Debug.WriteLine($"Unexpected message format from topic {result.Topic}: {ex.Message} ");
                    }
                }
        }

        catch (OperationCanceledException)
        {
            _consumer.Close();
        }

        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }



}
