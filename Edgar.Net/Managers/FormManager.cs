﻿using Edgar.Net.Data.Forms;
using Edgar.Net.Http.Forms;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Edgar.Net.Managers
{
    public class FormManager
    {
        public async Task<FormListResult> GetForms(string formType, string? company = null, DateTime? startDate = null, DateTime? endDate = null, int? count = null)
        {
            FormListResult forms = new FormListResult();
            var response = await GetBrowseEdgarXml("getcurrent", formType, company,
                startDate.HasValue ? startDate.Value.ToShortDateString() : null, endDate.HasValue ? endDate.Value.ToShortDateString() : null, "include", 0, count, "atom");
            using (TextReader reader = new StringReader(response))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(FormListResult));
                forms = (FormListResult)xmlSerializer.Deserialize(reader);
            }
            return forms;
        }

        public async Task<string> GetFormFromEntry(FormListEntry formEntry)
        {
            return GetFormTextFromIndexLink(formEntry.FileLink.Url);
        }

        /// <summary>
        /// Utility function for calling the browse-edgar endpoint
        /// </summary>
        /// <param name="action"></param>
        /// <param name="type"></param>
        /// <param name="company"></param>
        /// <param name="dateb"></param>
        /// <param name="owner"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public async Task<string> GetBrowseEdgarXml(string action, string type,
            string? company, string? datea, string? dateb,
            string owner, int start,
            int? count, string output)
        {
            var request = Globals.BaseUrl +
                $"cgi-bin/browse-edgar?action={action}&type={type}&company={company ?? ""}&datea={datea ?? ""}&dateb={dateb ?? ""}&owner={owner}&start={start}&count={count}&output={output}";

            return Utilities.DownloadText(request, true);
        }


        private string GetFormTextFromIndexLink(string link)
        {
            string textFormLink = link.Replace("-index.htm", ".txt");
            return Utilities.DownloadText(textFormLink, true);
        }
    }
}
