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
        /// Used for most basic and bulk tasks.
        /// </summary>
        public const string BaseUrl = "https://www.sec.gov/";
        /// <summary>
        /// Used for the new REST API functions.
        /// https://www.sec.gov/edgar/sec-api-documentation
        /// </summary>
        public const string ApiUrl = "https://data.sec.gov/";

        public static JsonSerializerOptions JsonSettings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
