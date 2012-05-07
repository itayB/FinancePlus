using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using FinancePlus.Storage;
using System.Text.RegularExpressions;

namespace FinancePlus.PersistentLayer
{
    static class IsracardReader
    {

        private static List<string> getNextLine(XmlReader reader)
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

        public static KeyValuePair<DateTime, ArrayList> read1(StreamReader sr, string filename)
        {
            ArrayList transactions = new ArrayList();
            PaymentInfo paymentInfo = null;

            //StreamReader sr = new StreamReader(filename, Encoding.GetEncoding("ISO-8859-8"));
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
                                string creditCardNumber = getCreditCardNumber(filename);

                                /* assuming that this is acually the last line and all the transactions where parsed */
                                DateTime startDate = getFirstTransactionDate(transactions, endDate);
                                /* we should get here only once */
                                paymentInfo = new PaymentInfo(creditCardNumber, PaymentType.Isracard, startDate, endDate);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            foreach (Transaction t in transactions)
                t.paymentInfo = paymentInfo;

            return new KeyValuePair<DateTime, ArrayList>(paymentInfo.getEndDate(), transactions);
        }

        public static CreditCardReport read(StreamReader sr, string filename)
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
                                string creditCardNumber = getCreditCardNumber(filename);

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

        public static DateTime getFirstTransactionDate(ArrayList transactions, DateTime endDate)
        {
            DateTime startDate = endDate;
            foreach (Transaction t in transactions)
                if (t.getChargeDate() < startDate)
                    startDate = t.getChargeDate();
            return startDate;
        }

        private static string getCreditCardNumber(string fullPath)
        {
            string[] parts = fullPath.Split('\\');
            string filename = parts[parts.Count() - 1]; // for example: "2011_10_3267.xls"

            string[] numbers = Regex.Split(filename, @"\D+");
            return numbers[2];
        }

        public static Transaction parseExpense(List<string> row)
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
                e.type = Type.Credit;
                //e.billingPrice = Math.Abs(e.billingPrice);
                //e.transactionPrice = Math.Abs(e.transactionPrice);
            }
            e.receiptId = Convert.ToInt32(row[4]);
            e.details = row[5];
            return e;
        }

        public static Transaction parseInternationalExpense(List<string> row)
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

        public static DateTime parseSumRow(List<string> row)
        {
            if (!row[1].StartsWith("@"))
                throw new Exception();
            return Database.stringDateToDateTime(row[2]);
        }

        public static double parseToalFromSumRow(List<string> row)
        {
            if (!row[1].StartsWith("@"))
                throw new Exception();
            return Double.Parse(row[3]);
        }

#if false
        public static ArrayList read1(string filename)
        {
            int trTagCounter = 0;
            int tdTagCounter = 0;
            Expense e = null;
            ArrayList expenses = new ArrayList();
            bool endFlag = false;
            bool legalExpense = true;
            // Create an XML reader for this file.
            using (XmlReader reader = XmlReader.Create(new StreamReader(filename, Encoding.GetEncoding("ISO-8859-8"))))
            {

                while (!endFlag && reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.Name)
                        {
                            case "TR":
                                trTagCounter++;
                                break;
                            case "TD":
                                if (trTagCounter < 6)
                                    break;

                                if (reader.Read())
                                {
                                    string value = reader.Value.Trim();

                                    switch (tdTagCounter)
                                    {
                                        case 0: // date
                                            if (e != null && legalExpense)
                                                expenses.Add(e);

                                            // last line
                                            if (value.Length == 0)
                                            {
                                                endFlag = true;
                                                break;
                                            }

                                            legalExpense = true;
                                            e = new Expense();
                                            e.date = Database.stringDateToDateTime(value);
                                            break;
                                        case 1:
                                            e.businessName = value;
                                            break;
                                        case 2:
                                            try
                                            {
                                                e.transactionPrice = Double.Parse(value);
                                            }
                                            catch
                                            {
                                                legalExpense = false;
                                            }
                                            break;
                                        case 3:
                                            e.billingPrice = Double.Parse(value);
                                            break;
                                        case 4:
                                            try
                                            {
                                                e.receiptId = Convert.ToInt32(value);
                                            }
                                            catch
                                            {
                                                legalExpense = false;
                                            }
                                            break;
                                        case 5:
                                            e.details = value;
                                            tdTagCounter = -1;
                                            break;
                                    }
                                    tdTagCounter++;
                                }

                                //string attribute = reader["name"];
                                //if (attribute != null)
                                //{
                                //    Console.WriteLine("  Has attribute name: " + attribute);
                                //}
                                break;
                        }
                    }
                }
            }

            //double total = 0;

            //foreach (Expense expense in expenses)
            //    total += expense.billingPrice;

            //total++;

            return expenses;
        }
#endif
    }
}
