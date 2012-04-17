using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.PersistentLayer;
using System.Xml;
using System.IO;

namespace FinancePlus.Storage
{
    static class ReportWriter
    {
        public static void write(Month month, string filename)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(filename, xmlWriterSettings))
            //using (XmlWriter writer = XmlWriter.Create(new StreamWriter(filename, false, Encoding.UTF8)))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Expenses");

                foreach (Transaction e in month.getTransactions())
                {
                    writer.WriteStartElement("Expense");

                    //writer.WriteElementString("Main Category", e.category.primary.ToString());
                    //writer.WriteElementString("Second Category", e.category.secondary);
                    writer.WriteElementString("Date", e.date.ToShortDateString());
                    writer.WriteElementString("FirstName", e.businessName);
                    writer.WriteElementString("TransactionPrice", e.transactionPrice.ToString());
                    writer.WriteElementString("BillingPrice", e.billingPrice.ToString());
                    writer.WriteElementString("ReceiptId", e.receiptId.ToString());
                    writer.WriteElementString("Details", e.details.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
