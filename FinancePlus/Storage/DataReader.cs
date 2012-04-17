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
    static class DataReader
    {
        public static void read()
        {
            string[][] allFiles = {Directory.GetFiles(@"C:\Users\Itay\Desktop\דוחות אשראי\Isracard\", "*.xls"),
                                   Directory.GetFiles(@"C:\Users\Itay\Desktop\דוחות אשראי\Cal\", "*.xls"),
                                   Directory.GetFiles(@"C:\Users\Itay\Desktop\דוחות אשראי\Poalim\", "*.xls")
                                  };

            for (PaymentType p = PaymentType.Poalim ; p <= PaymentType.Cal ; p++) 
            {
                string[] files = allFiles[(int)p];
                foreach (string filename in files)
                {
                    readFile(null, p, filename);
                    /*
                    KeyValuePair<DateTime, ArrayList> pair = new KeyValuePair<DateTime,ArrayList>();

                    switch (i)
                    {
                        case 0:
                            pair = IsracardReader.read(filename);
                            break;
                        case 1:
                            pair = CalReader.read(filename);
                            break;
                        case 2:
                            pair = PoalimReader.read(filename);
                            break;
                    }

                    ArrayList expenses = pair.Value;

                    foreach (Transaction e in expenses)
                    {
                        //if (filterExpense(e))
                        //    continue;

                        DateTime date = pair.Key;
                        //if (i != 2) // not relevant to bank hapoalim 
                        date = mapExpenseAndBillingMonthToDate(e, date);
                        Month month = Database.getMonth(date);
                        month.addTransaction(e);
                    }
                */
                }
            }
            
        }

        public static StreamReader getStreamReader(string filename)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename, Encoding.GetEncoding("ISO-8859-8"));
            }
            catch
            {
            }
            return sr;
        }

        public static void readFile(StreamReader sr, PaymentType paymentType,string filename)
        {
            KeyValuePair<DateTime, ArrayList> pair = new KeyValuePair<DateTime, ArrayList>();

            switch (paymentType)
            {
                case PaymentType.Isracard:
                    pair = IsracardReader.read(sr,filename);
                    break;
                case PaymentType.Cal:
                    pair = CalReader.read(sr);
                    break;
                case PaymentType.Poalim:
                    pair = PoalimReader.read(sr);
                    break;
            }

            ArrayList expenses = pair.Value;

            foreach (Transaction e in expenses)
            {
                DateTime date = pair.Key;
                //if (i != 2) // not relevant to bank hapoalim 
                date = mapExpenseAndBillingMonthToDate(e, date);
                Month month = Database.getMonth(date);
                month.addTransaction(e);
            }
        }


        private static DateTime mapExpenseAndBillingMonthToDate(Transaction expense, DateTime billDate)
        {
            int paymentNum = 0;
            if (expense.details != null && expense.details.StartsWith("תשלום"))
            {
                string[] numbers = Regex.Split(expense.details, @"\D+");
                if (numbers.Count() == 3)
                    paymentNum = int.Parse(numbers[1]) - 1; 
            }

            return new DateTime(expense.date.Year, expense.date.Month, 1).AddMonths(paymentNum);
        }


/*
        private static DateTime mapExpenseDateAndBillingMonthToDate(DateTime expenseDate, DateTime billDate)
        {
            bool payments = false;
            if (billDate.Day == 2)
                billDate = billDate.AddDays(-1);

            if (expenseDate.AddMonths(1) < billDate)
                payments = true;

            if (!payments)
                return new DateTime(expenseDate.Year, expenseDate.Month, 1);
            else
            {
                int newMonth = billDate.Month;
                int newYear = billDate.Year;
                if (expenseDate.Day >= billDate.Day)
                {
                    if (newMonth == 1)
                    {
                        newMonth = 12;
                        newYear--;
                    }
                    else
                        newMonth--;
                }

                return new DateTime(newYear, newMonth, 1);
            }
           // throw new Exception();
        }
 */ 
    }
 
}
