using Edgar.Net.Data.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Edgar.Net.Managers
{
    public static class CacheManager
    {
        private static Dictionary<string, CacheItem> _inMemoryData = new Dictionary<string, CacheItem>();
        public static string CachePath { get; set; } = "./EDGARCache/";

        static CacheManager()
        {
            if (!Directory.Exists(CachePath))
            {
                Directory.CreateDirectory(CachePath);
            }
        }

        internal static async Task<bool> AddToCache(string query, FormListResult response)
        {
            if (!Globals.CacheResults)
            {
                return false;
            }

            CacheItem item = new CacheItem()
            {
                Query = query,
                Response = response
            };

            var jsonItem = JsonSerializer.Serialize(item, Globals.JsonSettings);

            var cacheKey = GetCacheKey(query);

            var filePath = GetCacheItemFilePath(cacheKey);

            File.WriteAllText(filePath, jsonItem);

            return File.Exists(filePath);
        }

        internal static async Task<CacheItem> GetFromCache(string query)
        {
            if (!Globals.CacheResults)
            {
                return null;
            }

            var key = GetCacheKey(query);
            
            if (_inMemoryData.ContainsKey(key))
            {
                return _inMemoryData[key];
            }

            string filePath = GetCacheItemFilePath(key);


            if (File.Exists(filePath))
            {
                using (FileStream createStream = File.OpenRead(filePath))
                {
                    var item = await JsonSerializer.DeserializeAsync<CacheItem>(createStream);
                    if(item != null)
                    {
                        _inMemoryData.Add(key, item);
                        return item;
                    }
                }
            }
            return null;
        }

        private static string GetCacheItemFilePath(string key)
        {
            return $"{CachePath}/{key}.json";
        }

        private static string GetCacheKey(string query)
        {
            return $"{Utilities.CleanString(query)}";
        }
    }

    internal class CacheItem
    {
        public string Query { get; set; }
        public FormListResult Response { get; set; }
    }
}
