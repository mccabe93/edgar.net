using System.Text.RegularExpressions;

namespace Edgar.Net.Controllers
{
    public class FormProcessingController
    {
        private OpenAIController _openAIManager;

        public FormProcessingController(OpenAIController openAIManager)
        {
            _openAIManager = openAIManager;
        }

        public async Task<string> ProcessDocument(string data)
        {
            data = StripHTML(data).ToLower();
            string prompt = "Please read this document and return the purchase price per share or summarize the exercise rights of a shareholder after the merger.";
            var indexOfMaterialAgreement = data.IndexOf("material definitive agreement");
            if (indexOfMaterialAgreement == -1)
                return null;
            var result = await _openAIManager.Prompt(prompt, data.Substring(indexOfMaterialAgreement, 5000), 200);
            return result;
        }
        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
