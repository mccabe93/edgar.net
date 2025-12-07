using Edgar.Net.Data.Companies;
using Edgar.Net.Http.Forms;

namespace Edgar.Net.Data.Forms;

/// <summary>
/// Represents a DEFM14 proxy statement form for mergers and acquisitions.
/// </summary>
public class DEFM14 : IParsableForm
{
    private const string ReceiveLookupString = "receive $";
    private const string CashLookupString = "in cash";

    public DateTime Date { get; set; }
    public double? PurchasePrice { get; set; }
    public string? PurchasePriceDescription { get; set; }
    public Company? CompanyData { get; set; }

    private readonly EdgarClient? _client;

    public DEFM14(string title, string data, EdgarClient? client = null)
    {
        _client = client;
        TryParseCompanyData(title);
        ParseData(data);
    }

    /// <summary>
    /// Attempts to parse company data from the form title.
    /// </summary>
    private void TryParseCompanyData(string title)
    {
        var cikStartIndex = title.IndexOf('(');
        if (cikStartIndex < 0 || cikStartIndex + 11 > title.Length)
        {
            return;
        }

        var cikStr = title.Substring(cikStartIndex + 1, 10);

        if (!uint.TryParse(cikStr, out var cik))
        {
            return;
        }

        if (_client?.Companies.TryGetValue(cik, out var companyData) == true)
        {
            CompanyData = companyData;
        }
    }

    /// <summary>
    /// Parses the purchase price and related data from the form content.
    /// </summary>
    public void ParseData(string data)
    {
        var receiveIndex = data.IndexOf(ReceiveLookupString, StringComparison.OrdinalIgnoreCase);
        var cashIndex = data.IndexOf(CashLookupString, StringComparison.OrdinalIgnoreCase);

        string? value = null;
        var lastParagraphStart = 0;
        var nextParagraphEnd = 0;

        if (receiveIndex > 0)
        {
            var valueStart = receiveIndex + ReceiveLookupString.Length;
            if (valueStart + 5 <= data.Length)
            {
                value = data.Substring(valueStart, 5);
                lastParagraphStart = data.LastIndexOf("<P", receiveIndex - 1, receiveIndex);
                nextParagraphEnd = data.IndexOf("</P>", receiveIndex + 1);
            }
        }

        if (cashIndex > 0 && value is null)
        {
            var valueStart = cashIndex - CashLookupString.Length;
            if (valueStart >= 0 && valueStart + 5 <= data.Length)
            {
                value = data.Substring(valueStart, 5);
                lastParagraphStart = data.LastIndexOf("<P", cashIndex - 1, cashIndex);
                nextParagraphEnd = data.IndexOf("</P>", cashIndex + 1);
            }
        }

        if (value is null)
        {
            return;
        }

        var cleanedValue = value.Replace("$", "").Replace(",", "");

        if (double.TryParse(cleanedValue, out var purchasePrice))
        {
            PurchasePrice = purchasePrice;

            if (lastParagraphStart >= 0 && nextParagraphEnd > lastParagraphStart)
            {
                PurchasePriceDescription = data.Substring(
                    lastParagraphStart,
                    nextParagraphEnd - lastParagraphStart
                );
            }
            else
            {
                PurchasePriceDescription = "N/A";
            }
        }
    }
}
