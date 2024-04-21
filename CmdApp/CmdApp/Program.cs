using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CmdApp.Models;

namespace CmdApp;

public static class Program
{
    private const string BaseUrl = "http://localhost:5000/";
    private const string LastIdPath = "lastTodoId.txt"; // File to store the last ID
    private const string JsonFilePath = "todo-items.json"; // JSON file path
    
    static async Task Main(string[] args)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        
        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("BACKEND_URL") ?? BaseUrl)
        };

        List<TodoItem> existingItems = new List<TodoItem>();
        if (File.Exists(JsonFilePath))
        {
            string existingJson = File.ReadAllText(JsonFilePath);
            existingItems = JsonSerializer.Deserialize<List<TodoItem>>(existingJson) ?? new List<TodoItem>();
        }
        
        var clientService = new ClientService(client, LastIdPath, JsonFilePath);
        await clientService.StartAsync(existingItems);
    }
}