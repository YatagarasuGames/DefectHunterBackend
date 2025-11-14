using Auth.API.Abstractions;
using RabbitMQ.Client;
using Shared.Events;
using System.Text;
using System.Text.Json;

namespace Auth.API.Services
{
    public class RabbitMQService : IRabbitMQService
    {

        public async void PublishUserRegisteredEvent(UserRegisteredEvent request)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "user",
                Password = "password",
            };

            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                await channel.QueueDeclareAsync(
                    queue: "RegisterEvents",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "RegisterEvents",
                    body: body
                    );
            }
        }

    }
}
