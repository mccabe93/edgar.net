using Edgar.Net.Data.Companies;
using Edgar.Net.Http.Companies.Models;
using System.Text.Json;

namespace Edgar.Net.Managers
{
    public static class CompanyManager
    {
        public static async Task<List<Company>> GetAllCompanies()
        {
            List<Company> companies = new List<Company>();
            string request = Globals.BaseUrl + "files/company_tickers_exchange.json";
            var cachedCompanyData = await CacheManager.GetTextFromCache(request);
            if (cachedCompanyData == null)
            {
                cachedCompanyData = await Utilities.DownloadText(request, true);
            }
            await CacheManager.AddToCache(request, cachedCompanyData);
            var response = JsonSerializer.Deserialize<CompanyHttpResponse>(cachedCompanyData, Globals.JsonSettings);
            return response.ParseData();
        }
    }
}
