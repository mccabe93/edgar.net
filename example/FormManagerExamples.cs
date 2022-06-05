using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    public class FormManagerExamples
    {
        public async Task GetFormData()
        {
            Edgar.Net.Managers.FormManager fm = new Edgar.Net.Managers.FormManager();
            var data = await fm.GetForms("PREM14A", startDate: new DateTime(2020, 1, 1));

            foreach (var entry in data.Entries)
            {
                Console.Write(entry.Title);
                Console.WriteLine($"(Full Form: {entry.FileLink.Url})");
            }
        }
    }
}
