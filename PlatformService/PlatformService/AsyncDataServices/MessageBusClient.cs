using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task EnsureConnectedAsync()
        {
            if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
            {
                return;
            }

            await _connectionLock.WaitAsync();
            try
            {
                if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
                {
                    return;
                }

                var factory = new ConnectionFactory
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

                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdownAsync;

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
                throw;
            }
            finally
            {
                _connectionLock.Release();
            }
        }


        public async Task PublishNewPlatform(PlatformPublishDto platformPublishDto)
        {
            var message = JsonSerializer.Serialize(platformPublishDto);

            try
            {
                await EnsureConnectedAsync();

                if (_connection is { IsOpen: true })
                {
                    Console.WriteLine($"--> RabbitMQ Connection Open, sending message: {message}");
                    await SendMessageAsync(message);
                }
                else
                {
                    Console.WriteLine("--> RabbitMQ connection is closed, not sending");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send message to Message Bus: {ex.Message}");
            }
        }

        private async Task SendMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _channel!.BasicPublishAsync(
                exchange: "trigger",
                routingKey: string.Empty,
                body: body
            );

            Console.WriteLine($"Message has been sent: {message}");
        }

        private Task RabbitMQ_ConnectionShutdownAsync(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is { IsOpen: true })
            {
                await _channel.CloseAsync();
            }
            _channel?.Dispose();

            if (_connection is { IsOpen: true })
            {
                await _connection.CloseAsync();
            }
            _connection?.Dispose();

            _connectionLock.Dispose();
        }
    }
}
