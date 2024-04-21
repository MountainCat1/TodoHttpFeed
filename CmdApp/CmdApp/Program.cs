using System.Text.Json;

namespace CmdApp;

public static class Program
{
    static async Task Main(string[] args)
    {
        HttpClient client = new HttpClient();
        string baseUrl = "http://localhost:5000/api/todo-items/feed";
        string lastTodoId = null; // To store the last received TodoItem ID
        string lastIdPath = "lastTodoId.txt"; // File to store the last ID
        string jsonFilePath = "ToDoItemsCurrentSnapshot.json"; // JSON file path

        // Try to read the last Todo ID from file if exists
        if (File.Exists(lastIdPath))
        {
            lastTodoId = File.ReadAllText(lastIdPath);
        }

        List<TodoItem> existingItems = new List<TodoItem>();
        if (File.Exists(jsonFilePath))
        {
            string existingJson = File.ReadAllText(jsonFilePath);
            existingItems = JsonSerializer.Deserialize<List<TodoItem>>(existingJson) ?? new List<TodoItem>();
        }

        while (true)
        {
            try
            {
                string url = baseUrl;
                if (!string.IsNullOrEmpty(lastTodoId))
                {
                    url += $"?lastTodoId={lastTodoId}&count=5&timeout=30"; // Request 5 items at a time
                }

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var feedResponse = JsonSerializer.Deserialize<FeedResponse>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (feedResponse.Data.Count > 0)
                    {
                        lastTodoId = feedResponse.LastTodoId;
                        File.WriteAllText(lastIdPath, lastTodoId);

                        existingItems.AddRange(feedResponse.Data);

                        File.WriteAllText(jsonFilePath,
                            JsonSerializer.Serialize(existingItems,
                                new JsonSerializerOptions { WriteIndented = true }));
                        Console.WriteLine($"Received {feedResponse.Data.Count} new items, snapshot updated.");
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

    class FeedResponse
    {
        public List<TodoItem> Data { get; set; }
        public int Count { get; set; }
        public string LastTodoId { get; set; }
        public string Next { get; set; }
    }

    class TodoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}