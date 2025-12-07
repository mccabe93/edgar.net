using Edgar.Net.Data.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edgar.Net.Tests;

[TestClass]
public class Form4Tests
{
    [TestMethod]
    public void Form4_NetTransaction_Acquired()
    {
        // Minimal valid XML for Form4
        var xml =
            "<?xml version=\"1.0\"?><ownershipDocument><issuer><issuerCIK>123</issuerCIK><issuerName>Test</issuerName><issuerTradingSymbol>TST</issuerTradingSymbol></issuer><reportingOwner><reportingOwnerId><rptOwnerCik>456</rptOwnerCik><rptOwnerName>Insider</rptOwnerName></reportingOwnerId></reportingOwner><nonDerivativeTable><nonDerivativeTransaction><transactionDate><value>2023-01-01</value></transactionDate><transactionAmounts><transactionShares><value>100</value></transactionShares><transactionPricePerShare><value>10</value></transactionPricePerShare><transactionAcquiredDisposedCode><value>A</value></transactionAcquiredDisposedCode></transactionAmounts><securityTitle><value>Stock</value></securityTitle><postTransactionAmounts><sharesOwnedFollowingTransaction><value>200</value></sharesOwnedFollowingTransaction></postTransactionAmounts></nonDerivativeTransaction></nonDerivativeTable></ownershipDocument></XML>";
        var form4 = new Form4(xml);
        Assert.AreEqual(InsiderNetTransactionType.Acquired, form4.NetTransaction);
        Assert.AreEqual(100, form4.NetShareChange);
        Assert.AreEqual(10, form4.AverageAcquisitionPrice);
    }

    [TestMethod]
    public void Form4_NetTransaction_Disposed()
    {
        var xml =
            "<?xml version=\"1.0\"?><ownershipDocument><issuer><issuerCIK>123</issuerCIK><issuerName>Test</issuerName><issuerTradingSymbol>TST</issuerTradingSymbol></issuer><reportingOwner><reportingOwnerId><rptOwnerCik>456</rptOwnerCik><rptOwnerName>Insider</rptOwnerName></reportingOwnerId></reportingOwner><nonDerivativeTable><nonDerivativeTransaction><transactionDate><value>2023-01-01</value></transactionDate><transactionAmounts><transactionShares><value>50</value></transactionShares><transactionPricePerShare><value>20</value></transactionPricePerShare><transactionAcquiredDisposedCode><value>D</value></transactionAcquiredDisposedCode></transactionAmounts><securityTitle><value>Stock</value></securityTitle><postTransactionAmounts><sharesOwnedFollowingTransaction><value>150</value></sharesOwnedFollowingTransaction></postTransactionAmounts></nonDerivativeTransaction></nonDerivativeTable></ownershipDocument></XML>";
        var form4 = new Form4(xml);
        Assert.AreEqual(InsiderNetTransactionType.Disposed, form4.NetTransaction);
        Assert.AreEqual(-50, form4.NetShareChange);
        Assert.AreEqual(20, form4.AverageDisposalPrice);
    }

    [TestMethod]
    public void Form4_NetTransaction_Neutral_When_No_Transactions()
    {
        var xml =
            "<?xml version=\"1.0\"?><ownershipDocument><issuer><issuerCIK>123</issuerCIK><issuerName>Test</issuerName><issuerTradingSymbol>TST</issuerTradingSymbol></issuer><reportingOwner><reportingOwnerId><rptOwnerCik>456</rptOwnerCik><rptOwnerName>Insider</rptOwnerName></reportingOwnerId></reportingOwner><nonDerivativeTable></nonDerivativeTable></ownershipDocument></XML>";
        var form4 = new Form4(xml);
        Assert.AreEqual(InsiderNetTransactionType.Neutral, form4.NetTransaction);
        Assert.AreEqual(0, form4.NetShareChange);
    }

    [TestMethod]
    public void Form4_Parses_Issuer_Info()
    {
        var xml =
            "<?xml version=\"1.0\"?><ownershipDocument><issuer><issuerCIK>123</issuerCIK><issuerName>Test Company</issuerName><issuerTradingSymbol>TST</issuerTradingSymbol></issuer><reportingOwner><reportingOwnerId><rptOwnerCik>456</rptOwnerCik><rptOwnerName>Insider</rptOwnerName></reportingOwnerId></reportingOwner><nonDerivativeTable></nonDerivativeTable></ownershipDocument></XML>";
        var form4 = new Form4(xml);
        Assert.IsNotNull(form4.Company);
        Assert.AreEqual("Test Company", form4.Company.Name);
        Assert.AreEqual("TST", form4.Company.Ticker);
    }

    [TestMethod]
    public void Form4_Parses_Insider_Info()
    {
        var xml =
            "<?xml version=\"1.0\"?><ownershipDocument><issuer><issuerCIK>123</issuerCIK><issuerName>Test</issuerName><issuerTradingSymbol>TST</issuerTradingSymbol></issuer><reportingOwner><reportingOwnerId><rptOwnerCik>456</rptOwnerCik><rptOwnerName>John Doe</rptOwnerName></reportingOwnerId></reportingOwner><nonDerivativeTable></nonDerivativeTable></ownershipDocument></XML>";
        var form4 = new Form4(xml);
        Assert.IsNotNull(form4.Insider);
        Assert.AreEqual("John Doe", form4.Insider.Name);
        Assert.AreEqual((uint)456, form4.Insider.CIK);
    }
}
