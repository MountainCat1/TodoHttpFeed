while (true)
{
    using (var client = new HttpClient())
    {
        try
        {
            var response = await client.GetAsync("http://localhost:5000/api/todo-items");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Received snapshot with length: " + content.Length);
            File.WriteAllText("ToDoItemsCurrentSnapshot.json", content);    
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    Thread.Sleep(10000);
}