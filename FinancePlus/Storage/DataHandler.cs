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
                                            //bank

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
                    writer.WriteElementString("AccountNumber", bank.accountNumber);
                    writer.WriteElementString("BranchNumber", bank.branchNumber);
                    writer.WriteElementString("BankNumber", bank.bankNumber);
                    writer.WriteElementString("BankName", bank.bankName);
                    writer.WriteElementString("BranchName", bank.branchName);
                    writer.WriteElementString("Owner", bank.owner);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.WriteStartElement("CreditCards");

                foreach (CreditCard card in Database.creditCardsList)
                {
                    writer.WriteStartElement("CreditCard");
                    writer.WriteElementString("LastFourDigits", card.lastFourDigits);
                    writer.WriteElementString("ExpiryDate", card.expiryDate.ToShortDateString());
                    writer.WriteElementString("Description", card.description);
                    writer.WriteElementString("Owner", card.owner);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // CreditCards
                writer.WriteEndElement(); // Reports
                writer.WriteEndDocument();
            }
        }

    }
}
