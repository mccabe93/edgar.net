using Edgar.Net.Data.Forms;
using Edgar.Net.Managers;

Console.WriteLine("Call some example functions . . . ");

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