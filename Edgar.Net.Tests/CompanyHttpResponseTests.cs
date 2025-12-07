using Edgar.Net.Http.Companies.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edgar.Net.Tests;

[TestClass]
public class CompanyHttpResponseTests
{
    [TestMethod]
    public void ParseData_Returns_Companies_When_Data_Is_Valid()
    {
        var response = new CompanyHttpResponse
        {
            Data = new List<List<dynamic>>
            {
                new() { "123456", "Test Corp", "TST", "NYSE" },
                new() { "654321", "Sample Inc", "SMP", "NASDAQ" },
            },
        };

        var companies = response.ParseData();
        Assert.AreEqual(2, companies.Count);
        Assert.AreEqual("Test Corp", companies[0].Name);
        Assert.AreEqual("SMP", companies[1].Ticker);
    }

    [TestMethod]
    public void ParseData_Returns_Empty_When_Data_Is_Null()
    {
        var response = new CompanyHttpResponse { Data = null };
        var companies = response.ParseData();
        Assert.AreEqual(0, companies.Count);
    }

    [TestMethod]
    public void ParseData_Skips_Invalid_Entries()
    {
        var response = new CompanyHttpResponse
        {
            Data = new List<List<dynamic>>
            {
                new() { "not-a-cik", "Bad Corp", "BAD" },
                new() { "123456", "Good Corp", "GOOD", "NYSE" },
            },
        };
        var companies = response.ParseData();
        Assert.AreEqual(1, companies.Count);
        Assert.AreEqual("Good Corp", companies[0].Name);
    }

    [TestMethod]
    public void ParseData_Handles_Missing_Exchange()
    {
        var response = new CompanyHttpResponse
        {
            Data = new List<List<dynamic>>
            {
                new() { "123456", "Test Corp", "TST" },
            },
        };
        var companies = response.ParseData();
        Assert.AreEqual(1, companies.Count);
        Assert.AreEqual(string.Empty, companies[0].Exchange);
    }
}
