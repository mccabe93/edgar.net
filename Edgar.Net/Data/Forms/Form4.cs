using Edgar.Net.Http.Forms;
using Edgar.Net.Http.Forms.Models.Form4Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Edgar.Net.Data.Forms
{
    public class Form4 : IParsableForm
    {
        public ReportingOwnerId Insider { get; set; }
        public Issuer Company { get; set; }

        public DateTime Date { get; set; }
        public string Security { get; set; }
        public string AcquiredOrDisposed { get; set; }
        public decimal SharePrice { get; set; }
        public uint Shares { get; set; }
        public uint InsiderSharesAfterTransaction { get; set; }

        public decimal TotalTransactionValue => SharePrice * Shares;
        public decimal InsiderHoldingsChange => Shares / (InsiderSharesAfterTransaction + Shares);


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
            using (TextReader reader = new StringReader(xmlData))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Form4Xml));
                form = (Form4Xml)xmlSerializer.Deserialize(reader);
            }

            this.Insider = form.ReportingOwner.Owner;
            this.Company = form.Issuer;
            if (form.TransactionDetails == null || form.TransactionDetails.Transactions == null)
                return;
            this.Date = form.TransactionDetails.Transactions.Date.Value;
            this.Security = form.TransactionDetails.Transactions.Security.Value;
            this.AcquiredOrDisposed = form.TransactionDetails.Transactions.TransactionAmounts.AcquiredOrDisposed.Value;
            this.Shares = form.TransactionDetails.Transactions.TransactionAmounts.TransactionShares.Value;
            this.SharePrice = form.TransactionDetails.Transactions.TransactionAmounts.TransactionPricePerShare.Value;
            this.InsiderSharesAfterTransaction = form.TransactionDetails.Transactions.SharesAfterTransaction.SharesAfterTransaction.Value;

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
        public Transaction Transactions { get; set; }
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
