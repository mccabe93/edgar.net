using OpenAI;
using OpenAI.Models;

namespace Edgar.Net.Controllers
{
    public class OpenAIController
    {
        private OpenAIAuthentication _auth;
        private OpenAIClient _client;
        public OpenAIController(string apiKey)
        {
            _auth = new OpenAIAuthentication(apiKey);
            _client = new OpenAIClient(_auth);
        }

        public async Task<string> Prompt(string prompt, string formData, int maxTokens = 1000)
        {
            return (await _client.CompletionsEndpoint.CreateCompletionAsync(
                $@"{prompt} 
                Document: {formData}
                Answer: ",
                maxTokens: maxTokens,
                model: Model.Davinci)).Completions.FirstOrDefault();
        }
    }
}
