using Edgar.Net;
using Edgar.Net.Controllers;
using Edgar.Net.Data.Forms;

namespace Examples;

/// <summary>
/// Example usage of the Edgar.NET library for form retrieval.
/// </summary>
public class FormManagerExamples
{
    private readonly EdgarClient _client;
    private readonly FormRetrievalController _retriever;

    public FormManagerExamples(EdgarClient client)
    {
        _client = client;
        _retriever = client.Forms;
    }

    /// <summary>
    /// Retrieves a feed of forms of the specified type.
    /// </summary>
    public async Task GetFormDataFeedAsync(string formType, DateTime? startDate = null)
    {
        var data = await _retriever.GetFormsAsync(
            formType,
            getCurrent: true,
            startDate: startDate ?? DateTime.Now.AddDays(-3)
        );

        foreach (var entry in data.Entries)
        {
            Console.Write(entry.Title);
            Console.WriteLine($" (Full Form: {entry.FileLink.Url})");

            var formData = await _retriever.GetFormFromEntryAsync(entry);
            Console.WriteLine(formData);
        }
    }

    /// <summary>
    /// Example demonstrating Form 4 (insider trading) retrieval and parsing.
    /// </summary>
    public async Task Form4ExampleAsync()
    {
        var data = await _retriever.GetFormsAsync(
            "\"4\"",
            getCurrent: true,
            startDate: DateTime.Now.AddDays(-60),
            count: 500
        );

        foreach (var entry in data.Entries)
        {
            if (!entry.Title?.StartsWith("4 - ") ?? true)
            {
                Console.WriteLine($"Skipping: {entry.Title} @ {entry.FileLink.Url}");
                continue;
            }

            Console.Write(entry.Title);
            Console.WriteLine($" (Full Form: {entry.FileLink.Url})");

            var formData = await _retriever.GetFormFromEntryAsync(entry);
            var form4Data = new Form4(formData);

            var sharePrice =
                form4Data.NetTransaction == InsiderNetTransactionType.Acquired
                    ? form4Data.AverageAcquisitionPrice
                    : form4Data.AverageDisposalPrice;

            Console.WriteLine(
                $"({form4Data.Transactions.Count} part transaction) {form4Data.NetTransaction}: "
                    + $"{form4Data.NetShareChange} shares for ${sharePrice:F2}"
            );
        }
    }

    /// <summary>
    /// Example demonstrating DEFM14 (merger proxy) retrieval and parsing.
    /// </summary>
    public async Task DEFM14ExampleAsync()
    {
        var data = await _retriever.GetFormsAsync(
            "DEFM14",
            getCurrent: true,
            startDate: DateTime.Now.AddDays(-30),
            endDate: DateTime.Now,
            count: 20
        );

        foreach (var entry in data.Entries)
        {
            Console.Write(entry.Title);
            Console.WriteLine($" (Full Form: {entry.FileLink.Url})");

            var formData = await _retriever.GetFormFromEntryAsync(entry);
            var defm14Data = new DEFM14(entry.Title ?? string.Empty, formData, _client);

            Console.WriteLine($"Company: {defm14Data.CompanyData?.Name ?? "Unknown"}");
            Console.WriteLine($"Buyout price: {defm14Data.PurchasePrice?.ToString("C") ?? "N/A"}");
        }
    }

    /// <summary>
    /// Attempts to parse company data from a form title.
    /// </summary>
    public string? TryParseCompanyTicker(string title)
    {
        var cikStartIndex = title.IndexOf('(');
        if (cikStartIndex < 0 || cikStartIndex + 11 > title.Length)
        {
            return null;
        }

        var cikStr = title.Substring(cikStartIndex + 1, 10);

        if (!uint.TryParse(cikStr, out var cik))
        {
            return null;
        }

        return _client.Companies.TryGetValue(cik, out var companyData) ? companyData.Ticker : null;
    }
}
