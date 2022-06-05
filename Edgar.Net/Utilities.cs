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
    }
}
