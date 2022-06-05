using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edgar.Net
{
    public static class User
    {
        /// <summary>
        /// User agent value passed into header when requesting from EDGAR.
        /// </summary>
        public static string UserAgent { get; set; } = "Edgar.NET";
    }
}
