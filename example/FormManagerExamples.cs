using Edgar.Net.Data.Forms;
using Edgar.Net.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    public class FormManagerExamples
    {
        public async Task GetFormDataFeed(string formType, DateTime? startDate = null)
        {
            var data = await FormManager.GetForms(formType, true, startDate: startDate ?? DateTime.Now.AddDays(-3));

            foreach (var entry in data.Entries)
            {
                Console.Write(entry.Title);
                Console.WriteLine($"(Full Form: {entry.FileLink.Url})");

                var formData = await FormManager.GetFormFromEntry(entry);
                Console.WriteLine(formData);
            }
        }

        public async Task Form4Example()
        {
            var data = await FormManager.GetForms("\"4\"", true, startDate: DateTime.Now.AddDays(-60), count: 500);

            foreach (var entry in data.Entries)
            {
                if (entry.Title.StartsWith("4 - "))
                {
                    Console.Write(entry.Title);
                    Console.WriteLine($"(Full Form: {entry.FileLink.Url})");
                    var formData = await FormManager.GetFormFromEntry(entry);
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
            var data = await FormManager.GetForms("DEFM14", true, startDate: DateTime.Now.AddDays(-30), endDate: DateTime.Now, count: 20);

            foreach (var entry in data.Entries)
            {
                Console.Write(entry.Title);
                Console.WriteLine($"(Full Form: {entry.FileLink.Url})");
                var formData = await FormManager.GetFormFromEntry(entry);
                var defm14Data = new DEFM14(formData);
                Console.WriteLine($"Buyout price: {defm14Data.PurchasePrice}");
            }
        }
    }
}
