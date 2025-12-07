using System.Text.Json;
using Edgar.Net.Data.Companies;
using Edgar.Net.Http.Companies.Models;

namespace Edgar.Net.Managers;

/// <summary>
/// Manages retrieval of company data from the SEC.
/// </summary>
public class EdgarCompanyManager(EdgarClient client)
{
    private readonly EdgarClient _client = client;
    private const string CompanyTickersEndpoint = "files/company_tickers_exchange.json";

    /// <summary>
    /// Retrieves all companies registered with the SEC.
    /// </summary>
    /// <returns>A list of all companies.</returns>
    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        var requestUrl = _client.BaseUrl + CompanyTickersEndpoint;

        var cachedData = await _client.Cache.GetTextFromCacheAsync(requestUrl);

        if (cachedData is null)
        {
            cachedData = await _client.Utilities.DownloadTextAsync(requestUrl, includeHeader: true);
            await _client.Cache.AddToCacheAsync(requestUrl, cachedData);
        }

        var response = JsonSerializer.Deserialize<CompanyHttpResponse>(
            cachedData,
            _client.JsonSettings
        );

        return response?.ParseData() ?? [];
    }
}
