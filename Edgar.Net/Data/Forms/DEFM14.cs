using Edgar.Net.Data.Companies;
using Edgar.Net.Http.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Edgar.Net.Data.Forms
{
    public class DEFM14 : IParsableForm
    {
        public DateTime Date { get; set; }
        public decimal? PurchasePrice { get; set; } = null;
        public Company CompanyData { get; set; }

        public DEFM14(string data)
        {
            ParseData(data);
        }

        public void TryParseCompanyData(string title)
        {
            try
            {
                var cikTitleBaseData = title.Split('(');
                var cikStr = cikTitleBaseData[1].Substring(0, 10);
                var cik = UInt32.Parse(cikStr);
                Company companyData = null;
                bool found = Globals.Companies.TryGetValue(cik, out companyData);
                if (found)
                {
                    CompanyData = companyData;
                }
            }
            catch
            {

            }
        }

        public void ParseData(string data)
        {
            // This can be much improved . . .
            string dataCpy = data;
            const string receiveLookupString = "receive $";
            const string cashLookupString = "in cash";
            Task<int> receiveLookup = Task.Factory.StartNew<int>(() =>
            {
                return dataCpy.IndexOf(receiveLookupString);
            });
            Task<int> cashLookup = Task.Factory.StartNew<int>(() =>
            {
                return dataCpy.IndexOf(cashLookupString);
            });

            var tasks = new Task<int>[2] { receiveLookup, cashLookup };

            bool completed = Task.WaitAll(tasks, timeout: new TimeSpan(0,1,0));

            if(!completed)
            {
                return;
            }

            string value = null;
            if (receiveLookup.Result > 0)
            {
                value = data.Substring(receiveLookup.Result + receiveLookupString.Length, 5);
            }

            if(cashLookup.Result > 0)
            {
                value = data.Substring(cashLookup.Result - cashLookupString.Length, 5);
            }

            if(value == null)
            {
                return;
            }

            PurchasePrice = decimal.Parse(value);
        }
    }
}
