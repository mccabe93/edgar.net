using System.Net.Http.Headers;
using System.Text;

namespace Edgar.Net;

/// <summary>
/// Utility methods for HTTP operations and string manipulation.
/// </summary>
public class EdgarUtilities
{
    private readonly HttpClient _httpClient;
    private readonly EdgarClient _client;

    public EdgarUtilities(EdgarClient client)
    {
        _client = client;
        var handler = new HttpClientHandler
        {
            AutomaticDecompression =
                System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
        };
        _httpClient = new HttpClient(handler);
    }

    /// <summary>
    /// Downloads text content from the specified URL.
    /// </summary>
    public async Task<string> DownloadTextAsync(string url, bool includeHeader)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        if (includeHeader)
        {
            request.Headers.UserAgent.ParseAdd(_client.UserAgent);
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            request.Headers.Host = "www.sec.gov";
        }

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Removes special characters from a string to create a safe filename/key.
    /// </summary>
    public string CleanString(string dirtyString)
    {
        var removeChars = new HashSet<char>(" ?&^$#@!()+-,:;<>'\\/\"'=-_*");
        var result = new StringBuilder(dirtyString.Length);

        foreach (var c in dirtyString)
        {
            if (!removeChars.Contains(c))
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
