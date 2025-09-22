using System.Reflection;

namespace Server.Infrastructure.Service;

public static class CsvLoader
{
    public static async Task<Dictionary<Tkey, T>> LoadCsvAsync<Tkey, T>(
        string url,
        Func<T, Tkey> keySelector
        ) where T : new()
    {
        using var client = new HttpClient();
        string csv = await client.GetStringAsync(url);
        
        var dict = new Dictionary<Tkey, T>();
        var lines = csv.Split("\n")
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l))
            .Skip(1);
        
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var line in lines)
        {
            var cols = line.Split(",");
            
            var obj = new T();
            for (int i = 0; i < props.Length && i < cols.Length; i++)
            {
                var prop = props[i];
                object? value;
                if (prop.PropertyType == typeof(int))
                {
                    value = int.TryParse(cols[i], out int num) ? num : 0;
                }
                else
                {
                    value = cols[i];
                }
                prop.SetValue(obj, value);
            }
            
            dict[keySelector(obj)] = obj;
        }

        return dict;
    }
}