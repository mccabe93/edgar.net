﻿using Edgar.Net.Data.Companies;
using Edgar.Net.Managers;
using System.Text.Json;

namespace Edgar.Net
{
    public static class Globals
    {
        /// <summary>
        /// If enabled, the cache manager will be used to store a query and its results as json locally
        /// and keep results in memory (in a dictionary.)
        /// This can reduce the number of API calls to EDGAR. Iideally, this increases the amount of data 
        /// you can grab before hitting their request cap.
        /// </summary>
        public static bool CacheResults = false;

        /// <summary>
        /// Used for most basic and bulk tasks.
        /// </summary>
        public const string BaseUrl = "https://www.sec.gov/";
        
        /// <summary>
        /// Used for the new REST API functions.
        /// https://www.sec.gov/edgar/sec-api-documentation
        /// </summary>
        public const string ApiUrl = "https://data.sec.gov/";
        public static Dictionary<uint, Company> Companies { get; set; }

        public const int MaxFormsCount = 100;

        private static AutoResetEvent _lock = new AutoResetEvent(true);

        public static JsonSerializerOptions JsonSettings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        static Globals()
        {
            InitializeCIKDatabase().Wait();
        }

        private static async Task InitializeCIKDatabase()
        {
            _lock.WaitOne();
            Companies = new Dictionary<uint, Company>();
            var results = await CompanyManager.GetAllCompanies();
            foreach (var result in results)
            {
                Companies.TryAdd(result.CIK, result);
            }
            _lock.Set();
        }
    }
}
