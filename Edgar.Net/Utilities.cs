using System.IO.Compression;
using System.Net;
using System.Text;

namespace Edgar.Net
{
    public static class Utilities
    {
        private static AutoResetEvent _fileLock = new AutoResetEvent(true);

        public static async Task<string> DownloadText(string url, bool includeHeader)
        {
            var httpClient = new WebClient();
            httpClient.Headers.Clear();
            if (includeHeader)
            {
                httpClient.Headers.Add("user-agent", User.UserAgent);
                httpClient.Headers.Add("accept-encoding", "gzip, deflate");
                httpClient.Headers.Add("host", "www.sec.gov");
            }
            List<byte> data = new List<byte>();
            using (httpClient)
            {
                data = httpClient.DownloadData(url).ToList();
            }
            var json = DecompressResponse(data.ToArray());
            return json;
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
            _fileLock.WaitOne();
            string tmpFileName = $"{Guid.NewGuid()}.json";
            using (MemoryStream compressedMemoryStream = new MemoryStream(download, 0, download.Length))
            {
                using (FileStream decompressedFileStream = File.Create(tmpFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(compressedMemoryStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
            var json = File.ReadAllText(tmpFileName);
            File.Delete(tmpFileName);
            _fileLock.Set();
            return json;
        }
    }
}
