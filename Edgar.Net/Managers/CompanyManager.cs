using Edgar.Net.Data.Companies;
using Edgar.Net.Http.Companies.Models;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Edgar.Net.Managers
{
    public class CompanyManager
    {
        public async Task<List<Company>> GetAllCompanies()
        {
            List<Company> companies = new List<Company>();
            string request = Globals.BaseUrl + "files/company_tickers_exchange.json";
            string json = Utilities.DownloadText(request, false);
            var response = JsonSerializer.Deserialize<CompanyHttpResponse>(json, Globals.JsonSettings);
            return response.ParseData();
        }
    }
}
