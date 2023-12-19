using RestEase;

namespace Edgar.Net.Http.Forms
{
    public interface IFormApi
    {
        //https://www.sec.gov/cgi-bin/browse-edgar?action=getcurrent&type=10-k&company=&dateb=&owner=include&start=0&count=&output=atom
        [Get("cgi-bin/browse-edgar")]
        Task<string> GetAsync([Query("action")] string action, [Query("type")] string type,
            [Query("company")] string? company, [Query("dateb")] string? dateb,
            [Query("owner")] string owner, [Query("start")] int start,
            [Query("count")] int? count, [Query("output")] string output);
    }
}
