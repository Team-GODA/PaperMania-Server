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

        var row = 1;
        foreach (var line in lines)
        {
            row++;
            var cols = line.Split(',');

            try
            {
                var data = mapper(cols);
                var key = keySelector(data);

                if (!dict.TryAdd(key, data))
                    throw new InvalidOperationException($"Duplicate key: {key}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"CSV parse error at row {row}: [{line}]", ex);
            }
        }

        return dict;
    }

    public static TEnum ParseEnum<TEnum>(string value, string column)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, true, out var result))
            return result;

        throw new InvalidOperationException(
            $"Invalid enum value '{value}' for {typeof(TEnum).Name} ({column})");
    }
}