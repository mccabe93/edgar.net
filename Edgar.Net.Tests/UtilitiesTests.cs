using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edgar.Net.Tests;

[TestClass]
public class UtilitiesTests
{
    private EdgarClient _client = null!;

    [TestInitialize]
    public void Setup()
    {
        _client = new EdgarClient();
    }

    [TestMethod]
    public void CleanString_Removes_Special_Characters()
    {
        var dirty = "A?B&C^D$E#F@G!H()+-,:;<>'\\/\"'=-_*";
        var cleaned = _client.Utilities.CleanString(dirty);
        Assert.AreEqual("ABCDEFGH", cleaned);
    }

    [TestMethod]
    public void CleanString_Keeps_Alphanumeric()
    {
        var dirty = "Test123";
        var cleaned = _client.Utilities.CleanString(dirty);
        Assert.AreEqual("Test123", cleaned);
    }

    [TestMethod]
    public void CleanString_Handles_Empty_String()
    {
        var cleaned = _client.Utilities.CleanString("");
        Assert.AreEqual("", cleaned);
    }

    [TestMethod]
    public void CleanString_Handles_Only_Special_Characters()
    {
        var dirty = "?&^$#@!()+-,:;<>";
        var cleaned = _client.Utilities.CleanString(dirty);
        Assert.AreEqual("", cleaned);
    }
}
