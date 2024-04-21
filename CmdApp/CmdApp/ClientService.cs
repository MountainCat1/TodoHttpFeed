using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CmdApp.Models;

namespace CmdApp;

public class ClientService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;
    private string? _lastTodoId;
    private readonly string _lastIdPath;
    private readonly string _jsonFilePath;

    public ClientService(HttpClient client, string lastIdPath, string jsonFilePath)
    {
        _client = client;
        _lastIdPath = lastIdPath;
        _jsonFilePath = jsonFilePath;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    public async Task StartAsync(List<TodoItem> existingItems)
    {
        if (File.Exists(_lastIdPath))
        {
            _lastTodoId = await File.ReadAllTextAsync(_lastIdPath);
        }
        
        while (true)
        {
            try
            {
                string url = "api/todo-items/feed";
                if (!string.IsNullOrEmpty(_lastTodoId))
                {
                    url += $"?lastTodoId={_lastTodoId}&count=3&timeout=30"; // Request 5 items at a time
                }

                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var feedResponse = JsonSerializer.Deserialize<TodoItemEvent[]>(content, _serializerOptions);
                    if(feedResponse == null)
                        throw new SerializationException("Failed to deserialize TodoItemEvent from response content.");

                    if (feedResponse.Length > 0)
                    {
                        Console.WriteLine($"Received {feedResponse.Length} new events.");

                        await HandleFeedResponseAsync(feedResponse, existingItems);

                        Console.WriteLine($"Successfully applied snapshot. Current items: {existingItems.Count}");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("No new events received.");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch data: {response.StatusCode}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            await Task.Delay(500);
        }
        
        // ReSharper disable once FunctionNeverReturns
    }

    private async Task HandleFeedResponseAsync(TodoItemEvent[] feedResponse, List<TodoItem> existingItems)
    {
        _lastTodoId = feedResponse.Last().Id.ToString();

        foreach (var @event in feedResponse)
        {
            if (@event.Method == EventMethods.Create)
            {
                var newItem = JsonSerializer.Deserialize<TodoItem>(@event.Data);

                if (newItem == null)
                    throw new SerializationException($"Failed to deserialize TodoItem from event data. {@event.Data}");

                existingItems.Add(newItem);
                continue;
            }

            if (@event.Method == EventMethods.Update)
            {
                var updatedItem = JsonSerializer.Deserialize<TodoItem>(@event.Data);

                if (updatedItem == null)
                    throw new SerializationException($"Failed to deserialize TodoItem from event data. {@event.Data}");

                var existingItem = existingItems.FirstOrDefault(x => x.Id == updatedItem.Id);
                if (existingItem != null)
                {
                    existingItem.Title = updatedItem.Title;
                    existingItem.Description = updatedItem.Description;
                }
                else
                {
                    existingItems.Add(updatedItem);
                }

                continue;
            }

            if (@event.Method == EventMethods.Delete)
            {
                var deletedItemId = new Guid(@event.Subject);

                existingItems.RemoveAll(x => x.Id == deletedItemId);
            }
        }

        await File.WriteAllTextAsync(_lastIdPath, _lastTodoId);

        await File.WriteAllTextAsync(_jsonFilePath,
            JsonSerializer.Serialize(existingItems,
                new JsonSerializerOptions { WriteIndented = true }));
    }
}