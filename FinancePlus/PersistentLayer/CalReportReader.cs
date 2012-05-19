using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.PersistentLayer;
using System.IO;
using System.Xml;
using System.Collections;

namespace FinancePlus.Storage
{
    class CalReportReader : ReportReader
    {
        /* Public methods */
        public override PaymentType getPaymentType()
        {
            return PaymentType.Cal;
        }

        public override string getOpenFileDialogTitle()
        {
            return Database.CAL_STRING;
        }

        public override bool isCreditCardReader()
        {
            return true;
        }

        public override CreditCardReport readCreditCardReportFile(StreamReader sr)
        {
            CreditCardReport cardData = null;
            ArrayList transactions = new ArrayList();


            string contentBuffer = sr.ReadToEnd();
            contentBuffer = contentBuffer.Replace("&nbsp;", "");
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            sw.Write(contentBuffer);
            sw.Close();
            StringReader srFixed = new StringReader(sb.ToString());

            using (XmlReader reader = XmlReader.Create(srFixed))
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
                                DateTime endDate = parseDateFromInformationRow(row);
                                double total = 0;// parseToalFromSumRow(row);
                                string creditCardNumber = parseCreditCardFromSumRow(row);

                                /* assuming that this is acually the last line and all the transactions where parsed */
                                //DateTime startDate = getFirstTransactionDate(transactions, endDate);

                                //paymentInfo = new PaymentInfo(creditCardNumber, PaymentType.Cal, startDate, endDate);
                                /* we should get here only once */
                                cardData = new CreditCardReport();
                                cardData.chargeDate = endDate;
                                cardData.total = total;

                                cardData.creditCard = Database.getCreditCardByLastDigits(creditCardNumber);

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
                    case "td":
                        if (reader.IsStartElement() && reader.Read())
                        {
                            // Details of regular expenses (payments, discount info and etc)
                            if (reader.Name.Equals("span"))
                                reader.Read();

                            line.Add(reader.Value.Trim());
                        }
                        break;
                }
            }

            return null;
        }

        private double parseToalFromSumRow(List<string> row)
        {
            if (row.Count != 2)
                throw new Exception();
            return Double.Parse(row[1]);
        }

        private DateTime getFirstTransactionDate(ArrayList transactions, DateTime endDate)
        {
            DateTime startDate = endDate;
            foreach (Transaction t in transactions)
                if (t.getChargeDate() < startDate)
                    startDate = t.getChargeDate();
            return startDate;
        }

        private Transaction parseExpense(List<string> row)
        {
            Transaction e = new Transaction();

            e.date = Database.stringDateToDateTime(row[0]);
            if (e.date.Equals(new DateTime(1, 1, 1)))
                throw new Exception();
            e.businessName = row[1];
            e.transactionPrice = Double.Parse(row[2]);
            e.billingPrice = Double.Parse(row[4]);
            e.receiptId = 0;// Convert.ToInt32(row[4]);
            e.details = row[6];
            return e;
        }

        private Transaction parseInternationalExpense(List<string> row)
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

        private string parseCreditCardFromSumRow(List<string> row)
        {
            // verify that this is a number
            Convert.ToInt32(row[4]);

            return row[4];
        }

        private DateTime parseDateFromInformationRow(List<string> row)
        {
            return Database.stringDateToDateTime("1/" + row[10]);

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
    }
}
