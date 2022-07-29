using Edgar.Net.Data.Companies;
using Edgar.Net.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Edgar.Net
{
    public sealed class Globals
    {
        /// <summary>
        /// If enabled, the cache manager will be used to store a query and its results as json locally
        /// and keep results in memory (in a dictionary.)
        /// This can reduce the number of API calls to EDGAR. Iideally, this increases the amount of data 
        /// you can grab before hitting their request cap.
        /// </summary>
        public static bool CacheResults = true;

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

        public static JsonSerializerOptions JsonSettings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        static Globals()
        {
            //InitializeCIKDatabase();
        }

        private static void InitializeCIKDatabase()
        {
            Companies = new Dictionary<uint, Company>();
            var results = CompanyManager.GetAllCompanies().Result;
            foreach (var result in results)
            {
                Companies.TryAdd(result.CIK, result);
            }
        }
    }
}
