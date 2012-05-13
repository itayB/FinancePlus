using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;
using FinancePlus.PersistentLayer;

namespace FinancePlus.Storage
{
    class PoalimReader
    {
        private static List<string> getNextLine(XmlReader reader)
        {
            List<string> line = new List<string>();

            //try
            //{
            // loop until you find row start
            //    while (reader.Read() && !(reader.IsStartElement() && reader.Name.Equals("tr"))) ;
            //}
            //catch { }

            while (reader.Read())
            {
                // Get element name and switch on it.
                switch (reader.Name)
                {

                        /*
                    case "span":
                        if (reader.HasAttributes && reader.GetAttribute("id").Equals("lblTableBrief"))
                        {
                            while (reader.Read())
                                if (reader.Name.Equals("span") && reader.IsStartElement() && reader.Read())
                                    line.Add(reader.Value.Trim());
                                else if (reader.Name.Equals("td"))
                                    return line;
                        }
                        break;
                         */
                    case "TR":
                    case "tr":
                        //if (reader.HasAttributes && reader.GetAttribute("height").Equals("30"))
                        //{
                        //for (int i = 0; i < 3; i++)
                        //reader.ReadToNextSibling("tr");//.MoveToElement();
                        //break;
                        //while (reader.Read() && !reader.Name.Equals("tr")) ;
                        //break;
                        //}

                        if (!reader.IsStartElement())
                            return line;
                        break;
                    case "TD":
                    case "td":
                        if (reader.IsStartElement() && reader.Read())
                            line.Add(reader.Value.Trim());
                        break;
                }
            }

            return null;
        }

        public static KeyValuePair<DateTime, ArrayList> read(StreamReader streamReader)
        {
            ArrayList transactions = new ArrayList();
            string accountNumber = null;
            PaymentInfo paymentInfo = null;
            DateTime startDate = new DateTime();

            string temp = streamReader.ReadToEnd();
            temp = temp.Replace("&nbsp;", "");
            temp = Database.removeScripts(temp);
            //System.IO.File.WriteAllText(@"C:\Users\Itay\Desktop\WriteText.txt", temp);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            sw.Write(temp);
            sw.Close();
            StringReader sr = new StringReader(sb.ToString());

            using (XmlReader reader = XmlReader.Create(sr))
            {
                Transaction e = null;
                List<string> row;

                while ((row = getNextLine(reader)) != null)
                {
                    try
                    {
                        e = parseTransaction(row, startDate);
                        transactions.Add(e);
                    }
                    catch
                    {
                        try
                        {
                            string details = parseDetailsRow(row);
                            e.details = details;
                        }
                        catch
                        {
                            try
                            {
                                startDate = parseStartDateFromSumRow(row);
                                DateTime endDate = parseEndDateFromSumRow(row);

                                /* we should get here only once in START */
                                paymentInfo = new PaymentInfo(accountNumber, PaymentType.Poalim, startDate, endDate);
                            }
                            catch
                            {
                                try
                                {
                                    accountNumber = parseAccountNumber(row);  
                                }
                                catch
                                {
                                }
                            }
                        } 
                    }
                    
                }
            }

            foreach (Transaction t in transactions)
                t.paymentInfo = paymentInfo;

            return new KeyValuePair<DateTime, ArrayList>(startDate, transactions);
        }
/*
        public static ArrayList readIncomes(string filename)
        {
            ArrayList incomes = new ArrayList();
            DateTime date = new DateTime();
            string accountNumber = null;

            string temp = new StreamReader(filename, Encoding.GetEncoding("ISO-8859-8")).ReadToEnd();
            temp = temp.Replace("&nbsp;", "");
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            sw.Write(temp);
            //sw.Write(
            sw.Close();
            StringReader sr = new StringReader(sb.ToString());

            using (XmlReader reader = XmlReader.Create(sr))
            {
                List<string> row;

                while ((row = getNextLine(reader)) != null)
                {
                    try
                    {
                        Income e = parseIncome(row, date);
                        e.creditCardNumber = accountNumber;
                        incomes.Add(e);
                    }
                    catch
                    {
                        try
                        {
                            date = parseDateFromSumRow(row);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return incomes;
        }
        */
        public static string parseDetailsRow(List<string> row)
        {
            if (row.Count != 2 || !row[0].Equals(""))
                throw new Exception();
            return row[1];
        }

        public static Transaction parseTransaction(List<string> row, DateTime date)
        {
            Transaction t = new Transaction();

            t.date = Database.stringDateToDateTime(row[0] + "/" + date.Year);
            if (t.date.Equals(new DateTime(1, 1, 1)))
                throw new Exception();
            t.businessName = row[1];
            t.receiptId = Convert.ToInt32(row[2]);
            try
            {
                t.transactionPrice = Double.Parse(row[4]);
                t.billingPrice = Double.Parse(row[4]);
                t.type = FinancePlus.PersistentLayer.Type.Expense;
            }
            catch
            {
                t.transactionPrice = Double.Parse(row[5]);
                t.billingPrice = Double.Parse(row[5]);
                t.type = FinancePlus.PersistentLayer.Type.Income;
            }
            return t;
        }

        public static Transaction parseIncome(List<string> row, DateTime date)
        {
            Transaction e = new Transaction();

            e.date = Database.stringDateToDateTime(row[0] + "/" + date.Year);
            if (e.date.Equals(new DateTime(1, 1, 1)))
                throw new Exception();
            e.businessName = row[1];
            e.receiptId = Convert.ToInt32(row[2]);
            e.transactionPrice = Double.Parse(row[5]);
            e.billingPrice = Double.Parse(row[5]);
            e.type = FinancePlus.PersistentLayer.Type.Income;
            //e.details = null;
            return e;
        }

        public static Transaction parseInternationalExpense(List<string> row)
        {
            Transaction e = new Transaction();

            e.date = Database.stringDateToDateTime(row[0]);
            Database.stringDateToDateTime(row[1]);
            if (e.date.Equals(new DateTime(1, 1, 1)))
                throw new Exception();
            e.businessName = row[2];
            e.transactionPrice = Double.Parse(row[5]);
            e.billingPrice = Double.Parse(row[5]);
            e.receiptId = 0;// Convert.ToInt32(row[4]);
            e.details = row[7];
            return e;
        }

        public static string parseAccountNumber(List<string> row)
        {
            if (row.Count != 3 || !row[0].Equals("") || !row[1].Equals("חשבון"))
                throw new Exception();

            return row[2];
        }

        public static DateTime parseStartDateFromSumRow(List<string> row)
        {
            if (row.Count == 7)
            {
                if (!row[0].Equals("") || !row[1].Equals("") || row[5].Equals(""))
                    throw new Exception();
                return Database.stringDateToDateTime(row[5]);
            }
            else if (row.Count == 9)
            {
                if (!row[0].Equals("") || !row[1].Equals("") || row[7].Equals(""))
                    throw new Exception();
                return Database.stringDateToDateTime(row[7]);
            }
            throw new Exception();


            /* 
            string[] numbers = Regex.Split(row[0], @"\D+");
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int i = int.Parse(value);
                    Console.WriteLine("Number: {0}", i);
                }
            }
            */
            /*
            if (!row[1].StartsWith("@"))
                throw new Exception();
            return Database.stringDateToDateTime(row[2]);
             */
        }

        public static DateTime parseEndDateFromSumRow(List<string> row)
        {
            if (row.Count == 7)
            {
                if (!row[0].Equals("") || !row[1].Equals("") || row[3].Equals(""))
                    throw new Exception();
                return Database.stringDateToDateTime(row[3]);
            }
            else if (row.Count == 9)
            {
                if (!row[0].Equals("") || !row[1].Equals("") || row[5].Equals(""))
                    throw new Exception();
                return Database.stringDateToDateTime(row[5]);
            }
            throw new Exception();

           // if (row.Count != 7 || !row[0].Equals("") || !row[1].Equals("") || row[3].Equals(""))
            //    throw new Exception();
           // return Database.stringDateToDateTime(row[3]);
        }
    }
}
