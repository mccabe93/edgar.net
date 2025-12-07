using System.Xml.Serialization;
using Edgar.Net.Http.Forms;

namespace Edgar.Net.Data.Forms;

/// <summary>
/// Represents a transaction from a Form 4 filing.
/// </summary>
public record Form4Transaction
{
    public required DateTime Date { get; init; }
    public required string Security { get; init; }
    public required string AcquiredOrDisposed { get; init; }
    public required double SharePrice { get; init; }
    public required uint Shares { get; init; }
    public required uint InsiderSharesAfterTransaction { get; init; }
}

/// <summary>
/// The net result of insider transactions.
/// </summary>
public enum InsiderNetTransactionType
{
    Acquired,
    Disposed,
    Neutral,
}

/// <summary>
/// Represents a Form 4 insider trading statement.
/// </summary>
public class Form4 : IParsableForm
{
    private static readonly XmlSerializer _serializer = new(typeof(Form4Xml));

    public ReportingOwnerId? Insider { get; private set; }
    public Issuer? Company { get; private set; }
    public List<Form4Transaction> Transactions { get; } = [];

    public double AverageAcquisitionPrice { get; private set; }
    public double SharesAcquired { get; private set; }
    public double AverageDisposalPrice { get; private set; }
    public double SharesDisposed { get; private set; }

    private double? _netShareValue;
    private double? _netShareChange;

    public double NetShareValue
    {
        get
        {
            if (_netShareValue is null)
            {
                CalculateNetValues();
            }
            return _netShareValue!.Value;
        }
    }

    public double NetShareChange
    {
        get
        {
            if (_netShareChange is null)
            {
                _ = NetShareValue; // Ensure calculations are done
                _netShareChange = SharesAcquired - SharesDisposed;
            }
            return _netShareChange.Value;
        }
    }

    public InsiderNetTransactionType NetTransaction =>
        NetShareChange switch
        {
            > 0 => InsiderNetTransactionType.Acquired,
            < 0 => InsiderNetTransactionType.Disposed,
            _ => InsiderNetTransactionType.Neutral,
        };

    public Form4(string data)
    {
        ParseData(data);
    }

    public void ParseData(string data)
    {
        const string xmlStart = "<?xml version=\"1.0\"?>";
        const string xmlEnd = "</XML>";

        var startIndex = data.IndexOf(xmlStart);
        var endIndex = data.IndexOf(xmlEnd);

        if (startIndex < 0 || endIndex < 0)
        {
            return;
        }

        var xmlData = data[startIndex..endIndex];

        using var reader = new StringReader(xmlData);

        Form4Xml? form;
        try
        {
            form = (Form4Xml?)_serializer.Deserialize(reader);
        }
        catch
        {
            return;
        }

        if (form is null)
        {
            return;
        }

        Insider = form.ReportingOwner?.Owner;
        Company = form.Issuer;

        var transactions = form.TransactionDetails?.Transactions;
        if (transactions is null)
        {
            return;
        }

        foreach (var detail in transactions)
        {
            if (!TryCreateTransaction(detail, out var transaction))
            {
                continue;
            }

            Transactions.Add(transaction);
        }
    }

    private static bool TryCreateTransaction(Transaction detail, out Form4Transaction transaction)
    {
        transaction = null!;

        if (
            detail.SharesAfterTransaction?.SharesAfterTransaction?.Value is null
            || detail.Date?.Value is null
            || detail.Security?.Value is null
            || detail.TransactionAmounts?.AcquiredOrDisposed?.Value is null
            || detail.TransactionAmounts?.TransactionShares?.Value is null
            || detail.TransactionAmounts?.TransactionPricePerShare?.Value is null
        )
        {
            return false;
        }

        transaction = new Form4Transaction
        {
            Date = detail.Date.Value,
            Security = detail.Security.Value,
            AcquiredOrDisposed = detail.TransactionAmounts.AcquiredOrDisposed.Value,
            Shares = detail.TransactionAmounts.TransactionShares.Value,
            SharePrice = detail.TransactionAmounts.TransactionPricePerShare.Value,
            InsiderSharesAfterTransaction = detail
                .SharesAfterTransaction
                .SharesAfterTransaction
                .Value,
        };

        return true;
    }

    private void CalculateNetValues()
    {
        double totalBought = 0;
        double totalSold = 0;
        int acquisitionCount = 0;
        int disposalCount = 0;

        foreach (var transaction in Transactions)
        {
            switch (transaction.AcquiredOrDisposed.ToUpperInvariant())
            {
                case "A":
                    SharesAcquired += transaction.Shares;
                    AverageAcquisitionPrice += transaction.SharePrice;
                    totalBought += transaction.Shares * transaction.SharePrice;
                    acquisitionCount++;
                    break;
                case "D":
                    SharesDisposed += transaction.Shares;
                    AverageDisposalPrice += transaction.SharePrice;
                    totalSold += transaction.Shares * transaction.SharePrice;
                    disposalCount++;
                    break;
            }
        }

        if (acquisitionCount > 0)
        {
            AverageAcquisitionPrice /= acquisitionCount;
        }

        if (disposalCount > 0)
        {
            AverageDisposalPrice /= disposalCount;
        }

        _netShareValue = totalSold - totalBought;
    }
}

#region XML Serialization Models

[XmlRoot("issuer")]
public class Issuer
{
    [XmlElement("issuerCIK")]
    public uint CIK { get; set; }

    [XmlElement("issuerName")]
    public string? Name { get; set; }

    [XmlElement("issuerTradingSymbol")]
    public string? Ticker { get; set; }
}

[XmlRoot("reportingOwnerId")]
public class ReportingOwnerId
{
    [XmlElement("rptOwnerCik")]
    public uint CIK { get; set; }

    [XmlElement("rptOwnerName")]
    public string? Name { get; set; }
}

[XmlRoot("reportingOwnerRelationship")]
public class ReportingOwnerRelationship
{
    [XmlElement("isDirector", IsNullable = true)]
    public bool IsDirector { get; set; }

    [XmlElement("isOfficer", IsNullable = true)]
    public bool IsOfficer { get; set; }

    [XmlElement("isTenPercentOwner", IsNullable = true)]
    public bool IsTenPercentOwner { get; set; }

    [XmlElement("isOther", IsNullable = true)]
    public bool IsOther { get; set; }

    [XmlElement("officerTitle", IsNullable = true)]
    public string? OfficerTitle { get; set; }

    [XmlElement("otherText", IsNullable = true)]
    public string? OtherText { get; set; }
}

[XmlRoot("reportingOwner")]
public class ReportingOwner
{
    [XmlElement("reportingOwnerId")]
    public ReportingOwnerId? Owner { get; set; }
}

[XmlRoot("transactionCoding")]
public class TransactionCoding
{
    [XmlElement("transactionFormType")]
    public uint FormType { get; set; }

    [XmlElement("transactionCode")]
    public string? Code { get; set; }

    [XmlElement("equitySwapInvolved")]
    public uint SwapInvolved { get; set; }
}

public class DateNode
{
    [XmlElement("value")]
    public DateTime Value { get; set; }
}

public class StringNode
{
    [XmlElement("value")]
    public string? Value { get; set; }
}

public class UintNode
{
    [XmlElement("value")]
    public uint Value { get; set; }
}

public class DoubleNode
{
    [XmlElement("value")]
    public double Value { get; set; }
}

public class TransactionAmounts
{
    [XmlElement("transactionShares")]
    public UintNode? TransactionShares { get; set; }

    [XmlElement("transactionPricePerShare")]
    public DoubleNode? TransactionPricePerShare { get; set; }

    [XmlElement("transactionAcquiredDisposedCode")]
    public StringNode? AcquiredOrDisposed { get; set; }
}

public class OwnershipNature
{
    [XmlElement("directOrIndirectOwnership")]
    public StringNode? Nature { get; set; }
}

public class PostTransactionAmounts
{
    [XmlElement("sharesOwnedFollowingTransaction")]
    public UintNode? SharesAfterTransaction { get; set; }
}

[XmlRoot("nonDerivativeTransaction")]
public class Transaction
{
    [XmlElement("transactionDate")]
    public DateNode? Date { get; set; }

    [XmlElement("transactionCoding")]
    public TransactionCoding? TransactionCode { get; set; }

    [XmlElement("transactionAmounts")]
    public TransactionAmounts? TransactionAmounts { get; set; }

    [XmlElement("securityTitle")]
    public StringNode? Security { get; set; }

    [XmlElement("ownershipNature")]
    public OwnershipNature? OwnershipNature { get; set; }

    [XmlElement("postTransactionAmounts")]
    public PostTransactionAmounts? SharesAfterTransaction { get; set; }
}

[XmlRoot("nonDerivativeTable")]
public class TransactionTable
{
    [XmlElement("nonDerivativeTransaction")]
    public List<Transaction>? Transactions { get; set; }
}

[XmlRoot("ownershipDocument")]
public class Form4Xml
{
    [XmlElement("issuer")]
    public Issuer? Issuer { get; set; }

    [XmlElement("reportingOwner")]
    public ReportingOwner? ReportingOwner { get; set; }

    [XmlElement("nonDerivativeTable")]
    public TransactionTable? TransactionDetails { get; set; }
}

#endregion
