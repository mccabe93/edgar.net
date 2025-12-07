using Edgar.Net;
using Edgar.Net.Data.Companies;
using Edgar.Net.Data.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edgar.Net.Tests;

[TestClass]
public class DEFM14Tests
{
    [TestMethod]
    public void ParseData_Extracts_PurchasePrice()
    {
        var title = "Acme Corp (0001234567)";
        // The parser looks for "receive $" pattern and extracts 5 characters after it
        var data = "<P>Shareholders will receive $12.34 per share in cash</P>";
        var defm14 = new DEFM14(title, data);
        Assert.AreEqual(12.34, defm14.PurchasePrice);
        Assert.IsNotNull(defm14.PurchasePriceDescription);
    }

    [TestMethod]
    public void ParseData_Extracts_PurchasePrice_Integer()
    {
        var title = "Acme Corp (0001234567)";
        var data = "<P>Shareholders will receive $50.00 per share</P>";
        var defm14 = new DEFM14(title, data);
        Assert.AreEqual(50.00, defm14.PurchasePrice);
    }

    [TestMethod]
    public void ParseData_Returns_Null_When_No_Price()
    {
        var title = "Acme Corp (0001234567)";
        var data = "No price info here";
        var defm14 = new DEFM14(title, data);
        Assert.IsNull(defm14.PurchasePrice);
    }

    [TestMethod]
    public void ParseData_Uses_Client_For_Company_Lookup()
    {
        var client = new EdgarClient();
        client.Companies[1234567] = new Company
        {
            CIK = 1234567,
            Name = "Acme Corp",
            Ticker = "ACME",
            Exchange = "NYSE"
        };

        var title = "Acme Corp (0001234567)";
        var data = "<P>Shareholders will receive $100.0 per share</P>";
        var defm14 = new DEFM14(title, data, client);

        Assert.IsNotNull(defm14.CompanyData);
        Assert.AreEqual("Acme Corp", defm14.CompanyData.Name);
        Assert.AreEqual("ACME", defm14.CompanyData.Ticker);
    }

    [TestMethod]
    public void ParseData_Without_Client_Has_Null_CompanyData()
    {
        var title = "Acme Corp (0001234567)";
        var data = "<P>Shareholders will receive $100.0 per share</P>";
        var defm14 = new DEFM14(title, data);

        Assert.IsNull(defm14.CompanyData);
    }
}
