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
    }
}
