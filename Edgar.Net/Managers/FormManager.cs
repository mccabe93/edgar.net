using Edgar.Net.Data.Forms;
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
    public static class FormManager
    {

        public async static Task<FormListResult> GetForms(string form, bool getCurrent, string company = null, string cik = null, DateTime? startDate = null, DateTime? endDate = null, int count = Globals.MaxFormsCount)
        {
            if (startDate.HasValue == false)
            {
                startDate = DateTime.Now.AddYears(-5);
            }
            string owner = "include";
            if (company != null || cik != null)
            {
                owner = "only";
            }
            string action = null;
            if (getCurrent)
            {
                action = "getcurrent";
            }
            int formsPerRequest = Math.Min(count, Globals.MaxFormsCount);
            var formResults = await FormManager.GetFormsAdvanced(form,
                    company,
                    cik,
                    owner,
                    startDate,
                    endDate,
                    action: action,
                    count: formsPerRequest);
            for (int i = formsPerRequest; i < count; i += Globals.MaxFormsCount)
            {
                await Task.Delay(1200);
                var partialFormResults = await FormManager.GetFormsAdvanced(form,
                    company,
                    cik,
                    owner,
                    startDate,
                    endDate,
                    action: action,
                    offset: i,
                    count: formsPerRequest);
                formResults?.Entries.AddRange(partialFormResults.Entries);
            }
            return formResults;
        }

        public static async Task<FormListResult> GetFormsAdvanced(string formType, string? company = null, string? cik = null, string? owner = "include", DateTime? startDate = null, DateTime? endDate = null, int? offset = null, int? count = null, string? action = null)
        {
            FormListResult forms = new FormListResult();

            var request = await GetBrowseEdgarQuery(action, formType, company, cik,
                startDate.HasValue ? startDate.Value.ToShortDateString() : null, endDate.HasValue ? endDate.Value.ToShortDateString() : null, owner, offset ?? 0, count, "atom");

            var cacheItem = await CacheManager.GetFormListFromCache(request);
            bool existsInCache = cacheItem != null;

            if (existsInCache)
            {
                return cacheItem.Response;
            }
            else
            {
                var response = await DownloadForms(request);

                using (TextReader reader = new StringReader(response))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(FormListResult));
                    forms = (FormListResult)xmlSerializer.Deserialize(reader);
                }
            }

            if (!existsInCache)
            {
                await CacheManager.AddToCache(request, forms);
            }

            return forms;
        }

        public static async Task<string> GetFormFromEntry(FormListEntry formEntry)
        {
            return await GetFormTextFromIndexLink(formEntry.FileLink.Url);
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
        public static async Task<string> GetBrowseEdgarQuery(string action, string type,
            string? company, string? cik, string? datea, string? dateb,
            string owner, int start,
            int? count, string output)
        {
            var request = Globals.BaseUrl + "cgi-bin/browse-edgar?";
            if (cik != null)
            {
                request += $"company={company ?? ""}&CIK={cik}&type={type}&owner={owner}&count={count}";
            }
            else
            {
                request += $"type={type}&CIK={cik ?? ""}&company={company ?? ""}&datea={datea ?? ""}&dateb={dateb ?? ""}&owner={owner}&start={start}&count={count}";
            }
            if (action != null)
            {
                request += $"&action={action}";
            }
            return request += $"&output={output}";
        }

        private async static Task<string> DownloadForms(string request, bool useWebClient = true)
        {
            var result = await CacheManager.GetTextFromCache(request);
            if (result == null)
            {
                var response = await Utilities.DownloadText(request, true);
                await CacheManager.AddToCache(request, response);
                return response;
            }
            return result;
        }


        private async static Task<string> GetFormTextFromIndexLink(string link)
        {
            string textFormLink = link.Replace("-index.htm", ".txt");
            var result = await CacheManager.GetTextFromCache(textFormLink);
            if (result == null)
            {
                var response = await Utilities.DownloadText(textFormLink, true);
                await CacheManager.AddToCache(textFormLink, response);
                return response;
            }
            return result;
        }
    }
}
