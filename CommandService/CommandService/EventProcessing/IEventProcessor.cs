namespace CommandService.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string message, CancellationToken ct = default);
    }
}
