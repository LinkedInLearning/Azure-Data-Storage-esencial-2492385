using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Pets.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PetsController : ControllerBase
{
    private readonly QueueClient queueClient;
    public PetsController(IConfiguration configuration)
    {
        queueClient = new QueueClient(configuration.GetConnectionString("Pets"), "mensajes");
    }

    [HttpPost]
    public async Task<IActionResult> Post(Pet pet)
    {
        await queueClient.SendMessageAsync(JsonSerializer.Serialize(pet));
        return Ok(pet);
    }

}

public record Pet(string Name, string Breed);