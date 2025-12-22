namespace Server.Infrastructure.StaticData;

public static class CsvHelper
{
    public static async Task<Dictionary<TKey, T>> LoadAsync<TKey, T>(
        HttpClient httpClient,
        string url,
        Func<string[], T> mapper,
        Func<T, TKey> keySelector)
        where TKey : notnull
    {
        var csv = await httpClient.GetStringAsync(url);

        var dict = new Dictionary<TKey, T>();

        var lines = csv.Split('\n')
            .Skip(1)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l));

        foreach (var line in lines)
        {
            var cols = line.Split(',');

            var data = mapper(cols);
            var key = keySelector(data);

            if (!dict.TryAdd(key, data))
                throw new InvalidOperationException($"Duplicate key: {key}");
        }

        return dict;
    }
}