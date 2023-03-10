using Azure.Data.Tables;
using Azure.Storage.Queues;
using System.Text.Json;

namespace Pets.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly QueueClient queueClient;
    private readonly TableClient tableClient;
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        var connectionString = configuration.GetConnectionString("Pets");
        queueClient = new QueueClient(connectionString, "mensajes");
        var tableServiceClient = new TableServiceClient(connectionString);
        tableClient = tableServiceClient.GetTableClient("mascotas");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = await queueClient.ReceiveMessagesAsync(10, null, stoppingToken);
            var count = messages.Value.Count();
            if (count > 0)
            {
                _logger.LogInformation($"Se recibieron {count} mensajes");
                foreach (var item in messages.Value)
                {
                    var pet = JsonSerializer.Deserialize<Pet>(item.Body);
                    var entity = new TableEntity("pets", Guid.NewGuid().ToString());
                    entity[nameof(pet.Name)] = pet.Name;
                    entity[nameof(pet.Breed)] = pet.Breed;
                    await tableClient.AddEntityAsync(entity);

                    await queueClient.DeleteMessageAsync(item.MessageId, item.PopReceipt);
                }
            }

            //Escribir el registro


            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}

public record Pet(string Name, string Breed);