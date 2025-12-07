using System.Text.Json;
using Edgar.Net.Data.Forms;

namespace Edgar.Net.Managers;

/// <summary>
/// Manages caching of EDGAR API responses to reduce API calls.
/// </summary>
public class EdgarCacheManager
{
    private readonly EdgarClient _client;
    private readonly Dictionary<string, FormListCacheItem> _inMemoryFormListData = [];
    private readonly Dictionary<string, string> _inMemoryFormTextData = [];
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    private string AssemblyPath { get; } = GetAssemblyPath();
    public string CachePath => Path.Combine(AssemblyPath, ".edgarcache");
    public string FormListCache => Path.Combine(CachePath, "formlists");
    public string FormTextCache => Path.Combine(CachePath, "formtext");

    public EdgarCacheManager(EdgarClient client)
    {
        _client = client;
        EnsureCacheDirectoriesExist();
    }

    private static string GetAssemblyPath()
    {
        var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        return Path.GetDirectoryName(exePath) ?? Environment.CurrentDirectory;
    }

    private void EnsureCacheDirectoriesExist()
    {
        Directory.CreateDirectory(CachePath);
        Directory.CreateDirectory(FormListCache);
        Directory.CreateDirectory(FormTextCache);
    }

    /// <summary>
    /// Adds a text response to the cache.
    /// </summary>
    public async Task<bool> AddToCacheAsync(string query, string response)
    {
        if (!_client.CacheResults)
        {
            return false;
        }

        var cacheKey = GetCacheKey(query);
        var filePath = GetFormTextCacheItemFilePath(cacheKey);

        await _cacheLock.WaitAsync();
        try
        {
            await File.WriteAllTextAsync(filePath, response);
            _inMemoryFormTextData[cacheKey] = response;
            return File.Exists(filePath);
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    /// <summary>
    /// Adds a form list result to the cache.
    /// </summary>
    public async Task<bool> AddToCacheAsync(string query, FormListResult response)
    {
        if (!_client.CacheResults)
        {
            return false;
        }

        var cacheKey = GetCacheKey(query);
        var item = new FormListCacheItem { Query = query, Response = response };

        var jsonItem = JsonSerializer.Serialize(item, _client.JsonSettings);
        var filePath = GetFormListCacheItemFilePath(cacheKey);

        await _cacheLock.WaitAsync();
        try
        {
            await File.WriteAllTextAsync(filePath, jsonItem);
            _inMemoryFormListData[cacheKey] = item;
            return File.Exists(filePath);
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    /// <summary>
    /// Retrieves a form list from the cache.
    /// </summary>
    public async Task<FormListCacheItem?> GetFormListFromCacheAsync(string query)
    {
        if (!_client.CacheResults)
        {
            return null;
        }

        var key = GetCacheKey(query);

        if (_inMemoryFormListData.TryGetValue(key, out var cachedItem))
        {
            return cachedItem;
        }

        var filePath = GetFormListCacheItemFilePath(key);

        if (!File.Exists(filePath))
        {
            return null;
        }

        await using var stream = File.OpenRead(filePath);
        var item = await JsonSerializer.DeserializeAsync<FormListCacheItem>(stream);

        if (item is not null)
        {
            _inMemoryFormListData[key] = item;
        }

        return item;
    }

    /// <summary>
    /// Retrieves text from the cache.
    /// </summary>
    public async Task<string?> GetTextFromCacheAsync(string request)
    {
        if (!_client.CacheResults)
        {
            return null;
        }

        var key = GetCacheKey(request);

        if (_inMemoryFormTextData.TryGetValue(key, out var cachedText))
        {
            return cachedText;
        }

        var filePath = GetFormTextCacheItemFilePath(key);

        if (!File.Exists(filePath))
        {
            return null;
        }

        var text = await File.ReadAllTextAsync(filePath);
        _inMemoryFormTextData[key] = text;
        return text;
    }

    private string GetFormListCacheItemFilePath(string key) =>
        Path.Combine(FormListCache, $"{key}.json");

    private string GetFormTextCacheItemFilePath(string key) =>
        Path.Combine(FormTextCache, $"{key}.json");

    private string GetCacheKey(string query) => _client.Utilities.CleanString(query);
}

/// <summary>
/// Cache item for form list results.
/// </summary>
public class FormListCacheItem
{
    public required string Query { get; init; }
    public required FormListResult Response { get; init; }
}

/// <summary>
/// Cache item for form text content.
/// </summary>
public class FormTextCacheItem
{
    public required string Response { get; init; }
}
