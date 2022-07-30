using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Edgar.Net
{
    public static class Utilities
    {
        public static async Task<string> DownloadText(string url, bool includeHeader)
        {
            string text = null; 
            var httpClient = new HttpClient();
            if (includeHeader)
            {
                httpClient.DefaultRequestHeaders.Add("user-agent", User.UserAgent);
            }
            using (httpClient)
            {
                var response = await httpClient.GetAsync(url);
                text = await response.Content.ReadAsStringAsync();
            }
            return text;
        }
        public static string CleanString(string dirtyString)
        {
            HashSet<char> removeChars = new HashSet<char>(" ?&^$#@!()+-,:;<>’\\/\"'=-_*");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
                if (!removeChars.Contains(c)) // prevent dirty chars
                    result.Append(c);
            return result.ToString();
        }
    }
}
