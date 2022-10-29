using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KafkaTransmitter.Models;

namespace KafkaTransmitter.Services;

public interface IEventHandlerService
{
    Task HandleEventAsync(EventModel model);
}
