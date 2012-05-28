using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FinancePlus.PersistentLayer;
using System.IO;
using System.Text.RegularExpressions;

namespace FinancePlus.Storage
{
    static class DataHandler
    {
        public static void loadReports()
        {
            if (!File.Exists(Database.dataFolder + Database.dataReportsFilename))
                return;

            using (XmlReader reader = XmlReader.Create(new StreamReader(Database.dataFolder + Database.dataReportsFilename, Encoding.GetEncoding("ISO-8859-8"))))
            {
                BankAccount bank = null;
                CreditCard card = null;
                CreditCardReport report = null;

                while (reader.Read())
                {
                    // Get element name and switch on it.
                    switch (reader.Name)
                    {
                        case "Bank":
                            if (reader.IsStartElement())
                            {
                                XmlReader bankReader = reader.ReadSubtree();
                                bank = loadBank(bankReader);
                                reader.Skip();
                                Database.bankAccounts.Add(bank);
                            }
                            break;

                        case "CreditCard":
                            if (reader.IsStartElement())
                            {
                                XmlReader cardReader = reader.ReadSubtree();
                                card = loadCreditCard(cardReader);
                                reader.Skip();
                                Database.creditCardsList.Add(card);
                            }
                            break;
                        case "CreditCardReport":
                            if (reader.IsStartElement())
                            {
                                XmlReader reportReader = reader.ReadSubtree();
                                report = loadCreditCardReport(reportReader);
                                reader.Skip();
                                Database.addCreditCardReport(report);
                            }
                            break;
                    }
                }
            }
        }

        public static BankAccount loadBank(XmlReader bankReader)
        {
            BankAccount bank = new BankAccount();

            while (bankReader.Read())
            {
                if (bankReader.IsStartElement())
                {
                    string name = bankReader.Name;
                    string value = null;
                    if (bankReader.Read())
                        value = bankReader.Value.Trim();
                    switch (name)
                    {
                        case "HashCode":
                            bank.hashCode = Convert.ToInt32(value);
                            break;
                        case "AccountNumber":
                            bank.accountNumber = value;
                            break;
                        case "BranchNumber":
                            bank.branchNumber = value;
                            break;
                        case "BankNumber":
                            bank.bankNumber = value;
                            break;
                        case "BankName":
                            bank.bankName = value;
                            break;
                        case "BranchName":
                            bank.branchName = value;
                            break;
                        case "Owner":
                            bank.owner = value;
                            break;
                    }
                }
            }

            return bank;
        }

        public static CreditCard loadCreditCard(XmlReader cardReader)
        {
            CreditCard card = new CreditCard();

            while (cardReader.Read())
            {
                if (cardReader.IsStartElement())
                {
                    string name = cardReader.Name;
                    string value = null;
                    if (cardReader.Read())
                        value = cardReader.Value.Trim();
                    switch (name)
                    {
                        case "HashCode":
                            card.hashCode = Convert.ToInt32(value);
                            break;
                        case "LastFourDigits":
                            card.lastFourDigits = value;
                            break;
                        case "ExpiryDate":
                            card.expiryDate = Database.stringDateToDateTime(value);
                            break;
                        case "Description":
                            card.description = value;
                            break;
                        case "Owner":
                            card.owner = value;
                            break;
                        case "PaymentType":
                            card.paymentType = PaymentInfo.parsePaymentTypeString(value);
                            break;
                        case "BankHashCode":
                            int hashCode;
                            hashCode = Convert.ToInt32(value);
                            foreach (BankAccount b in Database.bankAccounts)
                                if (hashCode == b.hashCode)
                                {
                                    card.bank = b;
                                    break;
                                }

                            break;
                    }
                }
            }

            return card;
        }

        public static CreditCardReport loadCreditCardReport(XmlReader reportReader)
        {
            CreditCardReport report = new CreditCardReport();
            while (reportReader.Read())
            {
                if (reportReader.IsStartElement())
                {
                    string name = reportReader.Name;
                    string value = null;
                    if (reportReader.Read())
                        value = reportReader.Value.Trim();
                    switch (name)
                    {
                        case "ChargeDate":
                            report.chargeDate = Database.stringDateToDateTime(value);
                            break;
                        case "CreditCardHashCode":
                            int hashCode;
                            hashCode = Convert.ToInt32(value);
                            foreach (CreditCard c in Database.creditCardsList)
                                if (hashCode == c.hashCode)
                                {
                                    report.creditCard = c;
                                    break;
                                }
                            break;
                        case "TotalLocal":
                            report.total = Double.Parse(value);
                            break;
                        case "TotalPair":
                            DateTime date = new DateTime();
                            double total = 0;
                            if (reportReader.Read() && reportReader.Name.Equals("Date") && reportReader.Read())
                                date = Database.stringDateToDateTime(reportReader.Value.Trim());
                            if (reportReader.Read() && reportReader.Read() && reportReader.Read() && reportReader.Name.Equals("Total") && reportReader.Read())
                                total = Double.Parse(reportReader.Value.Trim());
                            report.totalInternational.Add(date, total);
                            break;
                        case "TransactionHashCode":
                            int hash;
                            hash = Convert.ToInt32(value);
                            foreach(KeyValuePair<DateTime,Month> pair in Database.months)
                                foreach (Transaction t in ((Month)pair.Value).getTransactions())
                                    if (hash == t.hashCode)
                                    {
                                        report.transactions.Add(t);
                                        break;
                                    }
                            break;
                    }
                }
            }

            return report;
        }

        public static void saveReportsToXML()
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(Database.dataFolder + Database.dataReportsFilename, xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Reports");
                writer.WriteStartElement("BankAccounts");

                foreach (BankAccount bank in Database.bankAccounts)
                {
                    writer.WriteStartElement("Bank");
                    writer.WriteElementString("HashCode", bank.GetHashCode().ToString());
                    writer.WriteElementString("AccountNumber", bank.accountNumber);
                    writer.WriteElementString("BranchNumber", bank.branchNumber);
                    writer.WriteElementString("BankNumber", bank.bankNumber);
                    writer.WriteElementString("BankName", bank.bankName);
                    writer.WriteElementString("BranchName", bank.branchName);
                    writer.WriteElementString("Owner", bank.owner);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // BankAccounts
                
                writer.WriteStartElement("CreditCards");

                foreach (CreditCard card in Database.creditCardsList)
                {
                    writer.WriteStartElement("CreditCard");
                    writer.WriteElementString("HashCode", card.GetHashCode().ToString());
                    writer.WriteElementString("LastFourDigits", card.lastFourDigits);
                    writer.WriteElementString("ExpiryDate", card.expiryDate.ToShortDateString());
                    writer.WriteElementString("Description", card.description);
                    writer.WriteElementString("Owner", card.owner);
                    writer.WriteElementString("PaymentType", card.paymentType.ToString());
                    if (card.bank != null)
                        writer.WriteElementString("BankHashCode", card.bank.GetHashCode().ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // CreditCards

                writer.WriteStartElement("CreditCardReports");

                foreach (CreditCardReport report in Database.getCreditCardReportsList())
                {
                    writer.WriteStartElement("CreditCardReport");
                    writer.WriteElementString("ChargeDate", report.chargeDate.ToShortDateString());
                    writer.WriteElementString("CreditCardHashCode", report.creditCard.GetHashCode().ToString());
                    writer.WriteElementString("TotalLocal", report.total.ToString());
                    writer.WriteStartElement("TotalInternational");
                    foreach (KeyValuePair<DateTime, double> pair in report.totalInternational)
                    {
                        writer.WriteStartElement("TotalPair");
                        writer.WriteElementString("Date", pair.Key.ToShortDateString());
                        writer.WriteElementString("Total", pair.Value.ToString());
                        writer.WriteEndElement(); // TotalPair
                    }
                    writer.WriteEndElement(); // TotalInternational
                    writer.WriteStartElement("Transactions");
                    foreach (Transaction t in report.transactions)
                    {
                        writer.WriteElementString("TransactionHashCode",t.GetHashCode().ToString());
                    }
                    writer.WriteEndElement(); // Transactions
                    writer.WriteEndElement(); // CreditCardReport
                }

                writer.WriteEndElement(); // CreditCardReports
                writer.WriteEndElement(); // Reports
                writer.WriteEndDocument();
            }
        }

    }
}
