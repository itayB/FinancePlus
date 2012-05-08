using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using FinancePlus.PersistentLayer;
using System.Text.RegularExpressions;

namespace FinancePlus.Storage
{
    class IsracardReportReader : ReportReader
    {
        /* Public methods */
        public override PaymentType getPaymentType()
        {
            return PaymentType.Isracard;
        }

        public override string getOpenFileDialogTitle()
        {
            return Database.ISRACARD_STRING;
        }

        public override bool isCreditCardReader() 
        {
            return true;
        }

        public override CreditCardReport readCreditCardReportFile(StreamReader sr)
        {
            CreditCardReport cardData = null;
            ArrayList transactions = new ArrayList();

            using (XmlReader reader = XmlReader.Create(sr))
            {
                List<string> row;

                while ((row = getNextLine(reader)) != null)
                {
                    try
                    {
                        Transaction e = parseExpense(row);
                        transactions.Add(e);
                    }
                    catch
                    {
                        try
                        {
                            Transaction e = parseInternationalExpense(row);
                            transactions.Add(e);
                        }
                        catch
                        {
                            try
                            {
                                DateTime endDate = parseSumRow(row);
                                double total = parseToalFromSumRow(row);
                                //string creditCardNumber = getCreditCardNumber(filename);

                                /* assuming that this is acually the last line and all the transactions where parsed */
                                DateTime startDate = getFirstTransactionDate(transactions, endDate);

                                /* we should get here only once */
                                cardData = new CreditCardReport();
                                cardData.chargeDate = endDate;
                                cardData.total = total;
                                cardData.creditCard = null; // not available in Isracard report
                                //cardData.lastFourDigits = null;    // not available in Isracard report 
                                //cardData.bankAccountNumber = null; // not available in Isracard report
                                //cardData.bankBranchNumber = null;  // not available in Isracard report
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            cardData.transactions = transactions;

            return cardData;
        }

        /* End of Public methods */

        /* Private methods */
        private List<string> getNextLine(XmlReader reader)
        {
            List<string> line = new List<string>();

            try
            {
                // loop until you find row start
                while (reader.Read() && !(reader.IsStartElement() && reader.Name.Equals("TR"))) ;
            }
            catch { }

            while (reader.Read())
            {
                // Get element name and switch on it.
                switch (reader.Name)
                {
                    case "TR":
                        if (!reader.IsStartElement())
                            return line;
                        break;
                    case "TD":
                        if (reader.IsStartElement() && reader.Read())
                            line.Add(reader.Value.Trim());
                        break;
                }
            }

            return null;
        }

        private DateTime getFirstTransactionDate(ArrayList transactions, DateTime endDate)
        {
            DateTime startDate = endDate;
            foreach (Transaction t in transactions)
                if (t.getChargeDate() < startDate)
                    startDate = t.getChargeDate();
            return startDate;
        }

        private string getCreditCardNumber(string fullPath)
        {
            string[] parts = fullPath.Split('\\');
            string filename = parts[parts.Count() - 1]; // for example: "2011_10_3267.xls"

            string[] numbers = Regex.Split(filename, @"\D+");
            return numbers[2];
        }

        private Transaction parseExpense(List<string> row)
        {
            Transaction e = new Transaction();

            e.date = Database.stringDateToDateTime(row[0]);
            if (e.date.Equals(new DateTime(1, 1, 1)))
                throw new Exception();
            e.businessName = row[1];
            e.transactionPrice = Double.Parse(row[2]);
            e.billingPrice = Double.Parse(row[3]);
            if (e.billingPrice < 0)
            {
                e.type = FinancePlus.PersistentLayer.Type.Credit;
                //e.billingPrice = Math.Abs(e.billingPrice);
                //e.transactionPrice = Math.Abs(e.transactionPrice);
            }
            e.receiptId = Convert.ToInt32(row[4]);
            e.details = row[5];
            return e;
        }

        private Transaction parseInternationalExpense(List<string> row)
        {
            Transaction e = new Transaction();

            e.date = Database.stringDateToDateTime(row[0]);
            if (e.date.Equals(new DateTime(1, 1, 1)))
                throw new Exception();
            Database.stringDateToDateTime(row[1]);
            e.businessName = row[2];
            e.transactionPrice = Double.Parse(row[5]);
            e.billingPrice = Double.Parse(row[7]);
            e.receiptId = Convert.ToInt32(row[8]);
            e.details = "";
            return e;
        }

        private static DateTime parseSumRow(List<string> row)
        {
            if (!row[1].StartsWith("@"))
                throw new Exception();
            return Database.stringDateToDateTime(row[2]);
        }

        private double parseToalFromSumRow(List<string> row)
        {
            if (!row[1].StartsWith("@"))
                throw new Exception();
            return Double.Parse(row[3]);
        }

        /* End of Private methods */
    }
}
