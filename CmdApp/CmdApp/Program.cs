using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CmdApp;

public static class Program
{
    static readonly string _baseUrl = "http://localhost:5000/api/todo-items/feed";
    static readonly string _lastIdPath = "lastTodoId.txt"; // File to store the last ID
    static readonly string _jsonFilePath = "ToDoItemsCurrentSnapshot.json"; // JSON file path
    static string _lastTodoId = null; // To store the last received TodoItem ID
    
    static async Task Main(string[] args)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        
        
        HttpClient client = new HttpClient();

        // Try to read the last Todo ID from file if exists
        if (File.Exists(_lastIdPath))
        {
            _lastTodoId = File.ReadAllText(_lastIdPath);
        }

        List<TodoItem> existingItems = new List<TodoItem>();
        if (File.Exists(_jsonFilePath))
        {
            string existingJson = File.ReadAllText(_jsonFilePath);
            existingItems = JsonSerializer.Deserialize<List<TodoItem>>(existingJson) ?? new List<TodoItem>();
        }

        while (true)
        {
            try
            {
                string url = _baseUrl;
                if (!string.IsNullOrEmpty(_lastTodoId))
                {
                    url += $"?lastTodoId={_lastTodoId}&count=5&timeout=30"; // Request 5 items at a time
                }

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var feedResponse = JsonSerializer.Deserialize<TodoItemEvent[]>(content, options);

                    if (feedResponse.Length > 0)
                    {
                        Console.WriteLine($"Received {feedResponse.Length} new items.");

                        HandleFeedResponse(feedResponse, existingItems);
                        
                        Console.WriteLine("Successfully applied snapshot.");
                        Console.WriteLine($"Current items: {existingItems.Count}");
                    }
                    else
                    {
                        Console.WriteLine("No new items received.");
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

    private static void HandleFeedResponse(TodoItemEvent[] feedResponse, List<TodoItem> existingItems)
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
        
        File.WriteAllText(_lastIdPath, _lastTodoId);

        File.WriteAllText(_jsonFilePath,
            JsonSerializer.Serialize(existingItems,
                new JsonSerializerOptions { WriteIndented = true }));
    }

    class FeedResponse
    {
        public List<TodoItemEvent> Data { get; set; }
        public int Count { get; set; }
        public string LastTodoId { get; set; }
        public string Next { get; set; }
    }
    
    public class TodoItemEvent
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public EventMethods Method { get; set; } 
        public DateTime Time { get; set; }
        public string Data { get; set; }
        public string Subject { get; set; }
    }

    public enum EventMethods
    {
        Create,
        Update,
        Delete
    }

    class TodoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}