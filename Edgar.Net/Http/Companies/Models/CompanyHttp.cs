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
                Company company = new Company()
                {
                    CIK = uint.Parse(c[0].ToString()),
                    Name = c[1].ToString(),
                    Ticker = c[2].ToString(),
                    Exchange = c[3].ToString()
                };
                result.Add(company);
            }
            return result;
        }
    }
}
