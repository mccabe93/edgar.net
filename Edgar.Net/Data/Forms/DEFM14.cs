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
        public string PurchasePriceDescription { get; set; }
        public Company CompanyData { get; set; }

        public DEFM14(string title, string data)
        {
            TryParseCompanyData(title);
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

            int lastParagraphStart = 0;
            int nextParagraphEnd = 0;

            string value = null;
            if (receiveLookup.Result > 0)
            {
                value = data.Substring(receiveLookup.Result + receiveLookupString.Length, 5);
                lastParagraphStart = data.LastIndexOf("<P", receiveLookup.Result - 1, receiveLookup.Result);
                nextParagraphEnd = data.IndexOf("</P>", receiveLookup.Result + 1);
            }

            if(cashLookup.Result > 0 && value == null)
            {
                value = data.Substring(cashLookup.Result - cashLookupString.Length, 5);
                lastParagraphStart = data.LastIndexOf("<P", cashLookup.Result - 1, cashLookup.Result);
                nextParagraphEnd = data.IndexOf("</P>", cashLookup.Result + 1);
            }

            if(value == null)
            {
                return;
            }

            if(decimal.TryParse(value.Replace("$","").Replace(",",""), out var purchasePrice))
            {
                PurchasePrice = purchasePrice;
                PurchasePriceDescription = data.Substring(lastParagraphStart, nextParagraphEnd);
            }
        }
    }
}
