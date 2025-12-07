using Edgar.Net;

namespace Examples;

/// <summary>
/// Example usage of the CompanyManager for retrieving company information.
/// </summary>
public class CompanyManagerExamples
{
    private readonly EdgarClient _client;

    public CompanyManagerExamples(EdgarClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Retrieves and displays information for all registered companies.
    /// </summary>
    public async Task GetAllCompanyInfoAsync()
    {
        var companies = await _client.CompanyManager.GetAllCompaniesAsync();

        foreach (var company in companies)
        {
            Console.WriteLine($"{company.Name} ({company.Ticker}) -- CIK: {company.CIK}");
        }
    }
}
