using Edgar.Net;
using Edgar.Net.Controllers;
using Edgar.Net.Data.Companies;
using Edgar.Net.Data.Forms;
using WhaleHunter.Net.Markets.Data;

namespace Examples
{
    public class FormManagerExamples
    {
        /*
        public async Task GetFormDataFeed(string formType, DateTime? startDate = null)
        {
            var data = await FormRetriever.GetForms(formType, true, startDate: startDate ?? DateTime.Now.AddDays(-3));

            foreach (var entry in data.Entries)
            {
                Console.Write(entry.Title);
                Console.WriteLine($"(Full Form: {entry.FileLink.Url})");

                var formData = await FormRetriever.GetFormFromEntry(entry);
                Console.WriteLine(formData);
            }
        }

        public async Task Form4Example()
        {
            var data = await FormRetriever.GetForms("\"4\"", true, startDate: DateTime.Now.AddDays(-60), count: 500);

            foreach (var entry in data.Entries)
            {
                if (entry.Title.StartsWith("4 - "))
                {
                    Console.Write(entry.Title);
                    Console.WriteLine($"(Full Form: {entry.FileLink.Url})");
                    var formData = await FormRetriever.GetFormFromEntry(entry);
                    var form4Data = new Form4(formData);
                    var sharePrice = form4Data.NetTransaction == InsiderNetTransactionType.Acquired ? form4Data.AverageAcquisitionPrice : form4Data.AverageDisposalPrice;
                    Console.WriteLine($@"({form4Data.Transactions.Count} part transaction) {form4Data.NetTransaction}: 
                         {form4Data.NetShareChange} shares for ${sharePrice}");
                }
                else
                {
                    Console.WriteLine($"Bad form: {entry.Title} @ {entry.FileLink.Url}");
                }
            }
        }

        public async Task DEFM14Example()
        {
            var data = await FormRetriever.GetForms("DEFM14", true, startDate: DateTime.Now.AddDays(-30), endDate: DateTime.Now, count: 20);

            foreach (var entry in data.Entries)
            {
                Console.Write(entry.Title);
                Console.WriteLine($"(Full Form: {entry.FileLink.Url})");
                var formData = await FormRetriever.GetFormFromEntry(entry);
                var defm14Data = new DEFM14(entry.Title, formData);
                Console.WriteLine($"Buyout price: {defm14Data.PurchasePrice}");
            }
        }
        */
        public async Task DEFM14Example()
        {
            var openAi = new OpenAIController("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"); 
            var retriever = new FormRetrievalController();
            var processor = new FormProcessingController(openAi);
            var defas = await retriever.GetForms("DEFA14", true, startDate: DateTime.Now.AddDays(-90), endDate: DateTime.Now, count: 1000);
            var defms = await retriever.GetForms("DEFM14", true, startDate: DateTime.Now.AddDays(-90), endDate: DateTime.Now, count: 1000);
            var eightKs = await retriever.GetForms("8-K", true, startDate: DateTime.Now.AddDays(-90), endDate: DateTime.Now, count: 1000);

            var data = new List<FormListEntry>();
            data.AddRange(defas.Entries);
            data.AddRange(defms.Entries);
            data.AddRange(eightKs.Entries);

            foreach (var entry in data)
            {
                var formData = await retriever.GetFormFromEntry(entry);
                var result = await processor.ProcessDocument(formData);
                if (result != null)
                {
                    var ticker = TryParseCompanyData(entry.Title);
                    if(ticker == null)
                    {
                        continue;
                    }
                    Console.Write(entry.Title);
                    var quote = await DataManager.GetCurrentQuote(ticker, WhaleHunter.Net.Core.Models.Enums.MarketType.Stock);
                    Console.WriteLine($"(Full Form: {entry.FileLink.Url})");
                    Console.WriteLine($"{quote.Symbol} Current Price: {quote.Current}");
                    Console.WriteLine($"Buyout Terms: {result}");
                }
            }
        }
        public string TryParseCompanyData(string title)
        {
            try
            {
                var cikTitleBaseData = title.Split('(');
                var cikStr = cikTitleBaseData[1].Substring(0, 10);
                var cik = UInt32.Parse(cikStr);
                bool found = Globals.Companies.TryGetValue(cik, out var companyData);
                if (found)
                {
                    return companyData.Ticker;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
