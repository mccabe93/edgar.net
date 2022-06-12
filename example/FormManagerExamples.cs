using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    public class FormManagerExamples
    {
        public async Task GetFormDataFeed()
        {
            Edgar.Net.Managers.FormManager fm = new Edgar.Net.Managers.FormManager();
            var data = await fm.GetForms("DEFM14", startDate: new DateTime(2022, 1, 1));

            foreach (var entry in data.Entries)
            {
                Console.Write(entry.Title);
                Console.WriteLine($"(Full Form: {entry.FileLink.Url})");
            }
        }

        public async Task GetFormData(int indexToGet)
        {
            Edgar.Net.Managers.FormManager fm = new Edgar.Net.Managers.FormManager();
            var feedData = await fm.GetForms("PREM14A", startDate: new DateTime(2022, 1, 1));

            var data = feedData.Entries[indexToGet];

            Console.WriteLine($"Data for {data.Title}");

            var formData = await fm.GetFormFromEntry(data);
            Console.WriteLine(formData);
        }
    }
}
