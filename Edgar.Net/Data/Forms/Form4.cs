using Edgar.Net.Http.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Edgar.Net.Data.Forms
{
    public class Form4Transaction
    {
        public DateTime Date { get; set; }
        public string Security { get; set; }
        public string AcquiredOrDisposed { get; set; }
        public decimal SharePrice { get; set; }
        public uint Shares { get; set; }
        public uint InsiderSharesAfterTransaction { get; set; }
    }

    public enum InsiderNetTransactionType
    {
        Acquired,
        Disposed,
        Neutral
    }

    public class Form4 : IParsableForm
    {
        public ReportingOwnerId Insider { get; set; }
        public Issuer Company { get; set; }

        public List<Form4Transaction> Transactions { get; set; }

        public decimal AverageAcquisitionPrice { get; set; }
        public decimal SharesAcquired { get; set; }
        public decimal AverageDisposalPrice { get; set; }
        public decimal SharesDisposed { get; set; }

        private decimal? _netShareValue = null;
        public decimal NetShareValue
        {
            get 
            {
                if (_netShareValue == null) 
                {
                    decimal totalBought = 0m;
                    decimal totalSold = 0m;
                    foreach (var transaction in Transactions)
                    {
                        switch (transaction.AcquiredOrDisposed.ToUpper())
                        {
                            case "A":
                                SharesAcquired += transaction.Shares;
                                AverageAcquisitionPrice += transaction.SharePrice;
                                totalBought += transaction.Shares * transaction.SharePrice;
                                break;
                            case "D":
                                SharesDisposed += transaction.Shares;
                                AverageDisposalPrice += transaction.SharePrice;
                                totalSold += transaction.Shares * transaction.SharePrice;
                                break;
                        }
                    }
                    if (Transactions.Count > 0)
                    {
                        AverageAcquisitionPrice /= Transactions.Count;
                        AverageDisposalPrice /= Transactions.Count;
                    }
                    _netShareValue = totalSold - totalBought;
                }
                return _netShareValue.Value;
            }
        }
        private decimal? _netShareChange = null;
        public decimal NetShareChange
        {
            get
            {
                if (_netShareChange == null) {
                    decimal netShareValueInitialize = NetShareValue;
                    _netShareChange = SharesAcquired - SharesDisposed;
                }
                return _netShareChange.Value;
            }
        }

        public InsiderNetTransactionType NetTransaction
        {
            get
            {
                if(NetShareChange > 0)
                {
                    return InsiderNetTransactionType.Acquired;
                }
                else if(NetShareChange < 0)
                {
                    return InsiderNetTransactionType.Disposed;
                }
                else
                {
                    return InsiderNetTransactionType.Neutral;
                }
            }
        }


        public Form4(string data)
        {
            ParseData(data);
        }

        public void ParseData(string data)
        {
            int startIndex = data.IndexOf("<?xml version=\"1.0\"?>");
            int endIndex = data.IndexOf("</XML>");
            if (startIndex < 0)
                return;
            string xmlData = data.Substring(startIndex, endIndex - startIndex);
            Form4Xml form = new Form4Xml();
            Transactions = new List<Form4Transaction>();
            using (TextReader reader = new StringReader(xmlData))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Form4Xml));
                try
                {
                    form = (Form4Xml)xmlSerializer.Deserialize(reader);
                }
                catch
                {
                    return;
                }
            }

            this.Insider = form.ReportingOwner.Owner;
            this.Company = form.Issuer;
            if (form.TransactionDetails == null || form.TransactionDetails.Transactions == null)
                return;
            foreach(var transactionDetails in form.TransactionDetails.Transactions)
            {
                if (transactionDetails.SharesAfterTransaction == null || transactionDetails.SharesAfterTransaction.SharesAfterTransaction == null)
                    continue;
                var form4Transaction = new Form4Transaction()
                {
                    Date = transactionDetails.Date.Value,
                    Security = transactionDetails.Security.Value,
                    AcquiredOrDisposed = transactionDetails.TransactionAmounts.AcquiredOrDisposed.Value,
                    Shares = transactionDetails.TransactionAmounts.TransactionShares.Value,
                    SharePrice = transactionDetails.TransactionAmounts.TransactionPricePerShare.Value,
                    InsiderSharesAfterTransaction = transactionDetails.SharesAfterTransaction.SharesAfterTransaction.Value
                };
                Transactions.Add(form4Transaction);
            }

        }
    }
    [XmlRoot("issuer")]
    public class Issuer
    {
        [XmlElement("issuerCIK")]
        public uint CIK { get; set; }
        [XmlElement("issuerName")]
        public string Name { get; set; }
        [XmlElement("issuerTradingSymbol")]
        public string Ticker { get; set; }
    }

    [XmlRoot("reportingOwnerId")]
    public class ReportingOwnerId
    {
        [XmlElement("rptOwnerCik")]
        public uint CIK { get; set; }
        [XmlElement("rptOwnerName")]
        public string Name { get; set; }
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
        public string OfficerTitle { get; set; }

        [XmlElement("otherText", IsNullable = true)]
        public string OtherText { get; set; }
    }

    [XmlRoot("reportingOwner")]
    public class ReportingOwner
    {
        [XmlElement("reportingOwnerId")]
        public ReportingOwnerId Owner { get; set; }
    }

    [XmlRoot("transactionCoding")]
    public class TransactionCoding
    {
        [XmlElement("transactionFormType")]
        public uint FormType { get; set; }
        [XmlElement("transactionCode")]
        public string Code { get; set; }
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
        public string Value { get; set; }
    }

    public class UintNode
    {
        [XmlElement("value")]
        public uint Value { get; set; }
    }

    public class DecimalNode
    {
        [XmlElement("value")]
        public decimal Value { get; set; }
    }

    public class TransactionAmounts
    {
        [XmlElement("transactionShares")]
        public UintNode TransactionShares { get; set; }
        [XmlElement("transactionPricePerShare")]
        public DecimalNode TransactionPricePerShare { get; set; }
        [XmlElement("transactionAcquiredDisposedCode")]
        public StringNode AcquiredOrDisposed { get; set; }
        //[XmlElement("footnoteId")]
        //public object Footnote { get; set; }
    }
    public class OwnershipNature
    {
        [XmlElement("directOrIndirectOwnership")]
        public StringNode Nature { get; set; }
    }
    public class PostTransactionAmounts
    {
        [XmlElement("sharesOwnedFollowingTransaction")]
        public UintNode SharesAfterTransaction { get; set; }
    }

    [XmlRoot("nonDerivativeTransaction")]
    public class Transaction
    {

        [XmlElement("transactionDate")]
        public DateNode Date { get; set; }
        [XmlElement("transactionCoding")]
        public TransactionCoding TransactionCode { get; set; }
        [XmlElement("transactionAmounts")]
        public TransactionAmounts TransactionAmounts { get; set; }
        [XmlElement("securityTitle")]
        public StringNode Security { get; set; }
        [XmlElement("ownershipNature")]
        public OwnershipNature OwnershipNature { get; set; }
        [XmlElement("postTransactionAmounts")]
        public PostTransactionAmounts SharesAfterTransaction { get; set; }
    }

    [XmlRoot("nonDerivativeTable")]
    public class TransactionTable
    {
        [XmlElement("nonDerivativeTransaction")]
        public List<Transaction> Transactions { get; set; }
    }

    [XmlRoot("ownershipDocument")]
    public class Form4Xml
    {
        [XmlElement("issuer")]
        public Issuer Issuer { get; set; }
        [XmlElement("reportingOwner")]
        public ReportingOwner ReportingOwner { get; set; }

        [XmlElement("nonDerivativeTable")]
        public TransactionTable TransactionDetails { get; set; }
    }
}
