using System.Xml.Serialization;
using Edgar.Net.Data.Forms;
using Edgar.Net.Managers;

namespace Edgar.Net.Controllers;

/// <summary>
/// Controller for retrieving SEC forms from EDGAR.
/// </summary>
public class FormRetrievalController(EdgarClient client)
{
    private readonly EdgarClient _client = client;
    private static readonly XmlSerializer _formListSerializer = new(typeof(FormListResult));
    private readonly SemaphoreSlim _throttleLock = new(1, 1);
    private DateTime _lastRequestTime = DateTime.MinValue;

    /// <summary>
    /// Retrieves forms matching the specified criteria.
    /// </summary>
    /// <param name="form">The form type (e.g., "10-K", "10-Q").</param>
    /// <param name="getCurrent">Whether to get only current filings.</param>
    /// <param name="company">Company name filter.</param>
    /// <param name="cik">CIK number filter.</param>
    /// <param name="startDate">Start date for the search range.</param>
    /// <param name="endDate">End date for the search range.</param>
    /// <param name="count">Maximum number of forms to retrieve.</param>
    /// <returns>Form list results matching the criteria.</returns>
    public async Task<FormListResult> GetFormsAsync(
        string form,
        bool getCurrent,
        string? company = null,
        string? cik = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? count = null
    )
    {
        var maxCount = count ?? _client.MaxFormsCount;
        startDate ??= DateTime.Now.AddYears(-5);
        endDate ??= DateTime.Now;

        var owner = (company is not null || cik is not null) ? "only" : "include";
        var action = getCurrent ? "getcurrent" : null;

        var formsPerRequest = Math.Min(maxCount, _client.MaxFormsCount);

        await ThrottleRequestAsync();

        var formResults = await GetFormsAdvancedAsync(
            form,
            company,
            cik,
            owner,
            startDate,
            endDate,
            action: action,
            count: formsPerRequest
        );

        if (formResults.Entries.Count == formsPerRequest)
        {
            for (var i = formsPerRequest; i < maxCount; i += _client.MaxFormsCount)
            {
                await ThrottleRequestAsync();

                var partialResults = await GetFormsAdvancedAsync(
                    form,
                    company,
                    cik,
                    owner,
                    startDate,
                    endDate,
                    action: action,
                    offset: i,
                    count: formsPerRequest
                );

                formResults.Entries.AddRange(partialResults.Entries);
            }
        }

        return formResults;
    }

    /// <summary>
    /// Retrieves forms with advanced filtering options.
    /// </summary>
    public async Task<FormListResult> GetFormsAdvancedAsync(
        string formType,
        string? company = null,
        string? cik = null,
        string? owner = "include",
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? offset = null,
        int? count = null,
        string? action = null
    )
    {
        var request = BuildBrowseEdgarQuery(
            action,
            formType,
            company,
            cik,
            startDate?.ToShortDateString(),
            endDate?.ToShortDateString(),
            owner ?? "include",
            offset ?? 0,
            count,
            "atom"
        );

        var cacheItem = await _client.Cache.GetFormListFromCacheAsync(request);

        if (cacheItem is not null)
        {
            return cacheItem.Response;
        }

        var response = await DownloadFormsAsync(request);
        var forms = DeserializeFormList(response);

        await _client.Cache.AddToCacheAsync(request, forms);

        return forms;
    }

    /// <summary>
    /// Gets the full form text from a form entry.
    /// </summary>
    public async Task<string> GetFormFromEntryAsync(FormListEntry formEntry)
    {
        return await GetFormTextFromIndexLinkAsync(formEntry.FileLink.Url);
    }

    /// <summary>
    /// Downloads forms from the given request URL.
    /// </summary>
    public async Task<string> DownloadFormsAsync(string request)
    {
        var cached = await _client.Cache.GetTextFromCacheAsync(request);

        if (cached is not null)
        {
            return cached;
        }

        var response = await _client.Utilities.DownloadTextAsync(request, includeHeader: true);
        await _client.Cache.AddToCacheAsync(request, response);

        return response;
    }

    /// <summary>
    /// Gets form text from an index link.
    /// </summary>
    public async Task<string> GetFormTextFromIndexLinkAsync(string link)
    {
        var textFormLink = link.Replace("-index.htm", ".txt");

        var cached = await _client.Cache.GetTextFromCacheAsync(textFormLink);

        if (cached is not null)
        {
            return cached;
        }

        var response = await _client.Utilities.DownloadTextAsync(textFormLink, includeHeader: true);
        await _client.Cache.AddToCacheAsync(textFormLink, response);

        return response;
    }

    private async Task ThrottleRequestAsync()
    {
        await _throttleLock.WaitAsync();
        try
        {
            var elapsed = DateTime.Now - _lastRequestTime;
            if (elapsed < TimeSpan.FromSeconds(1))
            {
                await Task.Delay(TimeSpan.FromSeconds(1) - elapsed);
            }
            _lastRequestTime = DateTime.Now;
        }
        finally
        {
            _throttleLock.Release();
        }
    }

    private string BuildBrowseEdgarQuery(
        string? action,
        string type,
        string? company,
        string? cik,
        string? datea,
        string? dateb,
        string owner,
        int start,
        int? count,
        string output
    )
    {
        var queryParams = cik is not null
            ? $"company={company ?? ""}&CIK={cik}&type={type}&owner={owner}&count={count}"
            : $"type={type}&CIK={cik ?? ""}&company={company ?? ""}&datea={datea ?? ""}&dateb={dateb ?? ""}&owner={owner}&start={start}&count={count}";

        var request = $"{_client.BaseUrl}cgi-bin/browse-edgar?{queryParams}";

        if (action is not null)
        {
            request += $"&action={action}";
        }

        return $"{request}&output={output}";
    }

    private static FormListResult DeserializeFormList(string xml)
    {
        using var reader = new StringReader(xml);
        return (FormListResult?)_formListSerializer.Deserialize(reader) ?? new FormListResult();
    }
}
