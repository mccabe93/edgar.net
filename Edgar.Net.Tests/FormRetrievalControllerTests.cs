using Edgar.Net;
using Edgar.Net.Controllers;
using Edgar.Net.Data.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edgar.Net.Tests;

[TestClass]
public class FormRetrievalControllerTests
{
    private EdgarClient _client = null!;

    [TestInitialize]
    public void Setup()
    {
        _client = new EdgarClient();
    }

    [TestMethod]
    public void FormRetrievalController_Is_Created_Via_Client()
    {
        Assert.IsNotNull(_client.Forms);
        Assert.IsInstanceOfType(_client.Forms, typeof(FormRetrievalController));
    }

    [TestMethod]
    public void Client_BaseUrl_Is_Configurable()
    {
        var customClient = new EdgarClient { BaseUrl = "https://custom.url/" };
        Assert.AreEqual("https://custom.url/", customClient.BaseUrl);
    }

    [TestMethod]
    public void Client_MaxFormsCount_Is_Configurable()
    {
        var customClient = new EdgarClient { MaxFormsCount = 50 };
        Assert.AreEqual(50, customClient.MaxFormsCount);
    }

    [TestMethod]
    public void Client_UserAgent_Is_Configurable()
    {
        var customClient = new EdgarClient { UserAgent = "CustomApp test@test.com" };
        Assert.AreEqual("CustomApp test@test.com", customClient.UserAgent);
    }

    [TestMethod]
    public void Client_CacheResults_Default_Is_False()
    {
        Assert.IsFalse(_client.CacheResults);
    }

    [TestMethod]
    public void Client_CacheResults_Is_Configurable()
    {
        var customClient = new EdgarClient { CacheResults = true };
        Assert.IsTrue(customClient.CacheResults);
    }

    [TestMethod]
    public void FormListResult_Entries_Is_Empty_By_Default()
    {
        var result = new FormListResult();
        Assert.IsNotNull(result.Entries);
        Assert.AreEqual(0, result.Entries.Count);
    }
}
