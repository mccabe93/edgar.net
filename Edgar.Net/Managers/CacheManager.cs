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
        private static Dictionary<string, FormListCacheItem> _inMemoryFormListData = new Dictionary<string, FormListCacheItem>();
        private static Dictionary<string, string> _inMemoryFormTextData = new Dictionary<string, string>();
        public static string CachePath => AssemblyPath + "/.edgarcache/";
        public static string FormListCache => CachePath + "formlists/";
        public static string FormTextCache => CachePath + "formtext/";

        private static string AssemblyPath { get; set; }

        static CacheManager()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //This will strip just the working path name:
            //C:\Program Files\MyApplication
            AssemblyPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            if (!Directory.Exists(CachePath))
            {
                Directory.CreateDirectory(CachePath);
            }
            if (!Directory.Exists(FormListCache))
            {
                Directory.CreateDirectory(FormListCache);
            }
            if (!Directory.Exists(FormTextCache))
            {
                Directory.CreateDirectory(FormTextCache);
            }
        }

        internal static async Task<bool> AddToCache(string query, string response)
        {
            if (!Globals.CacheResults)
            {
                return false;
            }

            var cacheKey = GetCacheKey(query);

            var filePath = GetFormTextCacheItemFilePath(cacheKey);

            File.WriteAllText(filePath, response);

            return File.Exists(filePath);
        }

        internal static async Task<bool> AddToCache(string query, FormListResult response)
        {
            if (!Globals.CacheResults)
            {
                return false;
            }

            var item = new FormListCacheItem()
            {
                Query = query,
                Response = response
            };

            var jsonItem = JsonSerializer.Serialize(item, Globals.JsonSettings);

            var cacheKey = GetCacheKey(query);

            var filePath = GetFormListCacheItemFilePath(cacheKey);

            File.WriteAllText(filePath, jsonItem);

            return File.Exists(filePath);
        }

        internal static async Task<FormListCacheItem> GetFormListFromCache(string query)
        {
            if (!Globals.CacheResults)
            {
                return null;
            }

            var key = GetCacheKey(query);
            
            if (_inMemoryFormListData.ContainsKey(key))
            {
                return _inMemoryFormListData[key];
            }

            string filePath = GetFormListCacheItemFilePath(key);

            if (File.Exists(filePath))
            {
                using (FileStream createStream = File.OpenRead(filePath))
                {
                    var item = await JsonSerializer.DeserializeAsync<FormListCacheItem>(createStream);
                    if(item != null)
                    {
                        _inMemoryFormListData.Add(key, item);
                        return item;
                    }
                }
            }
            return null;
        }

        internal static async Task<string> GetTextFromCache(string request)
        {
            if (!Globals.CacheResults)
            {
                return null;
            }

            var key = GetCacheKey(request);

            if (_inMemoryFormTextData.ContainsKey(key))
            {
                return _inMemoryFormTextData[key];
            }

            string filePath = GetFormTextCacheItemFilePath(key);

            if (File.Exists(filePath))
            {
                string item = File.ReadAllText(filePath);
                _inMemoryFormTextData.Add(key, item);
                return item;

            }
            return null;
        }

        private static string GetFormListCacheItemFilePath(string key)
        {
            return $"{FormListCache}/{key}.json";
        }

        private static string GetFormTextCacheItemFilePath(string key)
        {
            return $"{FormTextCache}/{key}.json";
        }

        private static string GetCacheKey(string query)
        {
            return $"{Utilities.CleanString(query)}";
        }
    }

    internal class FormListCacheItem
    {
        public string Query { get; set; }
        public FormListResult Response { get; set; }
    }

    internal class FormTextCacheItem
    {
        public string Response { get; set; }
    }
}
