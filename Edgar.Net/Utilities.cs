using System;
using System.Collections.Generic;
using System.IO.Compression;
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
            var httpClient = new WebClient();
            httpClient.Headers.Clear();
            if (includeHeader)
            {
                httpClient.Headers.Add("user-agent", User.UserAgent);
                httpClient.Headers.Add("accept-encoding", "gzip, deflate");
                httpClient.Headers.Add("host", "www.sec.gov");
            }
            using (httpClient)
            {
                var response = httpClient.DownloadData(url);
                text = DecompressResponse(response);
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

        private static string DecompressResponse(byte[] download)
        {
            using (MemoryStream compressedMemoryStream = new MemoryStream(download, 0, download.Length))
            {
                using (FileStream decompressedFileStream = File.Create("tmpstream.json"))
                {
                    using (GZipStream decompressionStream = new GZipStream(compressedMemoryStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
            return File.ReadAllText("tmpstream.json");
        }
    }
}
