using Edgar.Net.Http.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Edgar.Net.Data.Forms
{
    public class DEFM14 : IParsableForm
    {
        public DateTime Date { get; set; }
        public decimal PurchasePrice { get; set; }

        public DEFM14(string data)
        {
            ParseData(data);
        }

        public void ParseData(string data)
        {
            // This can be much improved . . .
            string v1Find = "receive $";
            int index = data.IndexOf(v1Find);
            if (index == -1)
            {
                return;
            }
            string value = data.Substring(index + v1Find.Length, 5);
            PurchasePrice = decimal.Parse(value);
        }
    }
}
