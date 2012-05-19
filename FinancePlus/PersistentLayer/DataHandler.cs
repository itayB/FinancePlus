using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FinancePlus.PersistentLayer;
using System.IO;

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

                while (reader.Read())
                {
                    // Get element name and switch on it.
                    switch (reader.Name)
                    {
                        case "Bank":
                            if (reader.IsStartElement())
                            {
                                bank = new BankAccount();
                                XmlReader bankReader = reader.ReadSubtree();

                                while (bankReader.Read())
                                {
                                    if (bankReader.IsStartElement())
                                    {
                                        switch (bankReader.Name)
                                        {
                                            case "HashCode":
                                                if (bankReader.Read())
                                                    bank.hashCode = Convert.ToInt32(bankReader.Value.Trim());
                                                break;
                                            case "AccountNumber":
                                                if (bankReader.Read())
                                                    bank.accountNumber = bankReader.Value.Trim();
                                                break;
                                            case "BranchNumber":
                                                if (bankReader.Read())
                                                    bank.branchNumber = bankReader.Value.Trim();
                                                break;
                                            case "BankNumber":
                                                if (bankReader.Read())
                                                    bank.bankNumber = bankReader.Value.Trim();
                                                break;
                                            case "BankName":
                                                if (bankReader.Read())
                                                    bank.bankName = bankReader.Value.Trim();
                                                break;
                                            case "BranchName":
                                                if (bankReader.Read())
                                                    bank.branchName = bankReader.Value.Trim();
                                                break;
                                            case "Owner":
                                                if (bankReader.Read())
                                                    bank.owner = bankReader.Value.Trim();
                                                break;
                                        }
                                    }
                                }
                                reader.Skip();
                                Database.bankAccounts.Add(bank);
                            }
                            break;

                        case "CreditCard":
                            if (reader.IsStartElement())
                            {
                                card = new CreditCard();
                                XmlReader cardReader = reader.ReadSubtree();

                                while (cardReader.Read())
                                {
                                    if (cardReader.IsStartElement())
                                    {
                                        switch (cardReader.Name)
                                        {
                                            case "LastFourDigits":
                                                if (cardReader.Read())
                                                    card.lastFourDigits = cardReader.Value.Trim();
                                                break;
                                            case "ExpiryDate":
                                                if (cardReader.Read())
                                                    card.expiryDate = Database.stringDateToDateTime(cardReader.Value.Trim());
                                                break;
                                            case "Description":
                                                if (cardReader.Read())
                                                    card.description = cardReader.Value.Trim();
                                                break;
                                            case "Owner":
                                                if (cardReader.Read())
                                                    card.owner = cardReader.Value.Trim();
                                                break;
                                            case "PaymentType":
                                                if (cardReader.Read())
                                                    card.paymentType = PaymentInfo.parsePaymentTypeString(cardReader.Value.Trim());
                                                break;
                                            case "BankHashCode":
                                                int hashCode;
                                                if (cardReader.Read())
                                                {
                                                    hashCode = Convert.ToInt32(cardReader.Value.Trim());
                                                    foreach (BankAccount b in Database.bankAccounts)
                                                        if (hashCode == b.hashCode)
                                                        {
                                                            card.bank = b;
                                                            break;
                                                        }
                                                }
                                                break;
                                        }
                                    }
                                }
                                reader.Skip();
                                Database.creditCardsList.Add(card);
                            }
                            break;

                    }

                }
            }
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

                foreach (CreditCardReport report in Database.creditCardReportsList)
                {
                    writer.WriteStartElement("CreditCardReport");
                    writer.WriteElementString("ChargeDate", report.chargeDate.ToShortDateString());
                    writer.WriteElementString("CreditCardHashCode", report.creditCard.GetHashCode().ToString());
                    writer.WriteElementString("Total", report.total.ToString());
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
