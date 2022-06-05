using Edgar.Net.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    public class CompanyManagerExamples
    {
        public async Task GetAllCompanyInfo()
        {
            CompanyManager manager = new CompanyManager();
            var companies = await manager.GetAllCompanies();
            
            foreach(var company in companies)
            {
                Console.WriteLine($"{company.Name} ({company.Ticker}) -- CIK: {company.CIK}");
            }
        }
    }
}
