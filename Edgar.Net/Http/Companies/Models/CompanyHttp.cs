using Edgar.Net.Data.Companies;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edgar.Net.Http.Companies.Models
{
    public class CompanyHttpResponse
    {
        public List<string>? Fields { get; set; }
        public List<List<dynamic>>? Data { get; set; }

        public List<Company> ParseData()
        {
            List<Company> result = new List<Company>();
            foreach (var c in Data)
            {
                try
                {
                    string name = c[1].ToString();
                    string ticker = c[2].ToString();
                    string exchange = null;
                    try
                    {
                        exchange = c[3].ToString();
                    }
                    catch
                    {
                        exchange = "";
                    }
                    Company company = new Company()
                    {
                        CIK = uint.Parse(c[0].ToString()),
                        Name = name,
                        Ticker = ticker,
                        Exchange = exchange
                    };
                    result.Add(company);
                }
                catch
                {
                    ;
                }
            }
            return result;
        }
    }
}
