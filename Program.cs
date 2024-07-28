using Newtonsoft.Json;
using StackExchange.Redis;

class Program
{
    private static async Task Main(string[] args)
    {
        var redis = ConnectionMultiplexer.Connect("localhost");

        if (!redis.IsConnected)
        {
            return;
        }

        var db = redis.GetDatabase();

        string filePath = @"C:\Users\sumey\Downloads\string_value.txt";
        string jsonData = await File.ReadAllTextAsync(filePath);

        var data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonData);

        var batch = db.CreateBatch();
        foreach (var item in data)
        {
            if (item.TryGetValue("Key", out string key) && !string.IsNullOrEmpty(key))
            {
                var hashEntries = new List<HashEntry>();
                foreach (var kvp in item)
                {
                    if (kvp.Key != "Key" && !string.IsNullOrEmpty(kvp.Value))
                    {
                        hashEntries.Add(new HashEntry(kvp.Key, kvp.Value));
                    }
                }
                if (hashEntries.Count > 0)
                {
                    batch.HashSetAsync(key, hashEntries.ToArray());
                }
            }
        }
        batch.Execute();

    }
}
