using System.Threading.Tasks;
using Edgar.Net;
using Edgar.Net.Data.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edgar.Net.Tests;

[TestClass]
public class CacheManagerTests
{
    private EdgarClient _client = null!;

    [TestInitialize]
    public void Setup()
    {
        _client = new EdgarClient { CacheResults = true };
    }

    [TestMethod]
    public async Task AddToCacheAsync_And_GetTextFromCacheAsync_Works()
    {
        var query = "test-query-" + Guid.NewGuid();
        var response = "test-response";
        var added = await _client.Cache.AddToCacheAsync(query, response);
        Assert.IsTrue(added);
        var cached = await _client.Cache.GetTextFromCacheAsync(query);
        Assert.AreEqual(response, cached);
    }

    [TestMethod]
    public async Task AddToCacheAsync_And_GetFormListFromCacheAsync_Works()
    {
        var query = "form-list-query-" + Guid.NewGuid();
        var formList = new FormListResult { Title = "Test" };
        var added = await _client.Cache.AddToCacheAsync(query, formList);
        Assert.IsTrue(added);
        var cached = await _client.Cache.GetFormListFromCacheAsync(query);
        Assert.IsNotNull(cached);
        Assert.AreEqual("Test", cached.Response.Title);
    }

    [TestMethod]
    public async Task GetTextFromCacheAsync_Returns_Null_When_CacheDisabled()
    {
        var client = new EdgarClient { CacheResults = false };
        var result = await client.Cache.GetTextFromCacheAsync("any-query");
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task AddToCacheAsync_Returns_False_When_CacheDisabled()
    {
        var client = new EdgarClient { CacheResults = false };
        var result = await client.Cache.AddToCacheAsync("any-query", "any-response");
        Assert.IsFalse(result);
    }
}
