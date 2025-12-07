using Edgar.Net.Data.Companies;

namespace Edgar.Net.Http.Companies.Models;

/// <summary>
/// Response model for company data from the SEC API.
/// </summary>
public class CompanyHttpResponse
{
    public List<string>? Fields { get; set; }
    public List<List<dynamic>>? Data { get; set; }

    /// <summary>
    /// Parses the raw API response data into a list of Company objects.
    /// </summary>
    /// <returns>A list of parsed Company objects.</returns>
    public List<Company> ParseData()
    {
        if (Data is null)
        {
            return [];
        }

        var result = new List<Company>(Data.Count);

        foreach (var companyData in Data)
        {
            if (TryParseCompany(companyData, out var company))
            {
                result.Add(company);
            }
        }

        return result;
    }

    private static bool TryParseCompany(List<dynamic> data, out Company company)
    {
        company = null!;

        if (data.Count < 3)
        {
            return false;
        }

        var cikString = data[0]?.ToString();
        if (!uint.TryParse(cikString, out uint cik))
        {
            return false;
        }

        var name = data[1]?.ToString();
        var ticker = data[2]?.ToString();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(ticker))
        {
            return false;
        }

        var exchange = data.Count > 3 ? data[3]?.ToString() ?? string.Empty : string.Empty;

        company = new Company
        {
            CIK = cik,
            Name = name,
            Ticker = ticker,
            Exchange = exchange,
        };

        return true;
    }
}
