using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edgar.Net
{
    public static class Utilities
    {
        public static string DownloadText(string url, bool includeHeader)
        {
            string text = null;
            var webClient = new System.Net.WebClient();
            if (includeHeader)
            {
                webClient.Headers.Add("user-agent", User.UserAgent);
            }
            using (webClient)
            {
                text = webClient.DownloadString(url);
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
