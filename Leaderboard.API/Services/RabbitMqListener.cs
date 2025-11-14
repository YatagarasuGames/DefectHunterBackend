using Leaderboard.API.Services.Commands.CreatePlayerScore;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;
using System.Text;
using System.Text.Json;

namespace Leaderboard.API.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMqListener(
            ILogger<RabbitMqListener> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private async Task Init(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    Port = 5672,
                    UserName = "user",
                    Password = "password"
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    queue: "RegisterEvents",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _logger.LogInformation("RabbitMQ connection established");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при инициализации RabbitMQ");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Init(stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                // Создаем scope для каждого сообщения
                using var scope = _serviceScopeFactory.CreateScope();
                var scopedMediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                    _logger.LogInformation("Получено сообщение: {Content}", content);

                    var data = JsonSerializer.Deserialize<UserRegisteredEvent>(content);

                    if (data != null)
                    {
                        await scopedMediator.Send(new CreatePlayerScoreCommand(data.UserId, data.Username, 0));
                        _logger.LogInformation("Обработан пользователь: {Username}", data.Username);
                    }
                    else
                    {
                        _logger.LogWarning("Не удалось десериализовать сообщение");
                    }

                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке сообщения");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync("RegisterEvents", autoAck: false, consumer);
            _logger.LogInformation("RabbitMQ listener started consuming queue: RegisterEvents");
        }

        

        public override async void Dispose()
        {
            try
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при закрытии соединения RabbitMQ");
            }

            _channel?.Dispose();
            _connection?.Dispose();

            base.Dispose();
        }
    }
}