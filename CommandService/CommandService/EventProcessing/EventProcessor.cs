using AutoMapper;
using CommandService.DTOs;
using CommandService.Models;
using CommandService.Repositories;
using System.Text.Json;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public async Task ProcessEvent(string message, CancellationToken ct = default)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    await addPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private async Task addPlatform(string platformPublishedMessage, CancellationToken ct = default)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

                var platformPublisheDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublisheDto);

                    if (!await repo.ExternalPlatformExists(platform.ExternalId, ct))
                    {
                        repo.CreatePlatform(platform);
                        await repo.SaveChangesAsync(ct);
                    }
                    else
                    {
                        Console.WriteLine($"--> Platform allready exists: {platform}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could't add platform to DB: {ex.Message}");
                }
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine($"--> Determining event: {notificationMessage}");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }

        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
