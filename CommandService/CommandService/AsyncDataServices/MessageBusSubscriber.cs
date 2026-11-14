
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IChannel _channel;
        private string? _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
        }

        private async Task InitializeRabbitMQ(CancellationToken ct = default)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"]!,
                Port = int.Parse(_configuration["RabbitMQPort"]!)
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                    exchange: "trigger",
                    type: ExchangeType.Fanout
                    );

            var queueDeclareResult = await _channel.QueueDeclareAsync(cancellationToken: ct);
            _queueName = queueDeclareResult.QueueName;

            await _channel.QueueBindAsync(
                queue: _queueName,
                exchange: "trigger",
                routingKey: string.Empty,
                cancellationToken: ct
            );

            Console.WriteLine($"--> Listening on the Message Bus: {_queueName}");

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdownAsync;
        }

        private Task RabbitMQ_ConnectionShutdownAsync(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection Shutdown");
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            await InitializeRabbitMQ(stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel!);

            consumer.ReceivedAsync += async (moduleHandle, ea) =>
            {
                Console.WriteLine("--> Event Received!");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                await _eventProcessor.ProcessEvent(notificationMessage);

            };

            await _channel!.BasicConsumeAsync(
                queue: _queueName!,
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }


        public override async void Dispose()
        {
            if (_channel is { IsOpen: true })
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            if (_connection is { IsOpen: true })
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }

            base.Dispose();
        }
    }
}
