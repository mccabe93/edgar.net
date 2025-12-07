using Edgar.Net;
using Edgar.Net.Data.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edgar.Net.Tests;

[TestClass]
public class CompanyTests
{
    [TestMethod]
    public void Company_Record_Properties_Are_Set()
    {
        var company = new Company
        {
            CIK = 123456,
            Name = "Test Corp",
            Ticker = "TST",
            Exchange = "NYSE",
        };

        Assert.AreEqual((uint)123456, company.CIK);
        Assert.AreEqual("Test Corp", company.Name);
        Assert.AreEqual("TST", company.Ticker);
        Assert.AreEqual("NYSE", company.Exchange);
    }

    [TestMethod]
    public void Company_Exchange_Has_Default_Value()
    {
        var company = new Company
        {
            CIK = 123456,
            Name = "Test Corp",
            Ticker = "TST",
        };

        Assert.AreEqual(string.Empty, company.Exchange);
    }

    [TestMethod]
    public void EdgarClient_Companies_Is_Empty_Before_Initialize()
    {
        var client = new EdgarClient();
        Assert.AreEqual(0, client.Companies.Count);
    }
}
