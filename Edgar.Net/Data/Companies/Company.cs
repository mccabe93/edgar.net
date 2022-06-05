using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edgar.Net.Data.Companies
{
    public class Company
    {
        public uint CIK { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Exchange { get; set; }
    }
}
