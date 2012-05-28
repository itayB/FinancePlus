using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FinancePlus.Storage;
using System.Text.RegularExpressions;

namespace FinancePlus.PersistentLayer
{
    class TempFix
    {
        public static void fixOldTransactions()
        {
            loadAllInputReports();
            Logger.log("finish loadAllInputReports");
            addOldTransactionsToReports();
            Logger.log("finish addOldTransactionsToReports");

            foreach (CreditCardReport report in Database.getCreditCardReportsList())
            {
                if (report.creditCard == null)
                    Logger.log("Card dosen't exist for report: " + report.ToString());
                else if (report.creditCard.paymentType == PaymentType.Isracard)
                {
                    if ((new IsracardReportReader()).isValid(report))
                        Logger.log("valid: " + report.creditCard.lastFourDigits + " " + report.chargeDate.ToShortDateString());
                    //else
                    //    Logger.log("NOT valid: " + report.creditCard.lastFourDigits + " " + report.chargeDate.ToShortDateString());
                }
            }
            Logger.log("finish!");
        }

        public static void loadAllInputReports()
        {
            string[] isracardFiles = Directory.GetFiles(@"C:\Users\Itay\Desktop\דוחות אשראי\Isracard\", "*.xls");
            ReportReader reportReader = new IsracardReportReader();

            foreach (string file in isracardFiles)
            {
                StreamReader sr = reportReader.getStreamReader(file);
                CreditCardReport cardReport = reportReader.readCreditCardReportFile(sr);
                cardReport.transactions.Clear();

                string cardNumber = getCreditCardNumber(file);

                cardReport.creditCard = Database.getCreditCardByLastDigits(cardNumber);

                Database.addCreditCardReport(cardReport);
            }

            string[] calFiles = Directory.GetFiles(@"C:\Users\Itay\Desktop\דוחות אשראי\Cal\", "*.xls");
            reportReader = new CalReportReader();

            foreach (string file in calFiles)
            {
                StreamReader sr = reportReader.getStreamReader(file);
                CreditCardReport cardReport = reportReader.readCreditCardReportFile(sr);
                Database.addCreditCardReport(cardReport);
            }
        }

        public static void addOldTransactionsToReports()
        {
            foreach (KeyValuePair<DateTime, Month> pair in Database.months)
            {
                foreach (Transaction t in ((Month)pair.Value).getTransactions())
                {
                    if (t.paymentInfo == null)
                        Logger.log(t.date.ToShortDateString() + " " + t.businessName);
                    else
                    {
                        PaymentType type = t.paymentInfo.getPaymentType();
                        if (type == PaymentType.Poalim)
                            continue;
                        string last4 = t.paymentInfo.getPaymentId();
                        DateTime date = t.paymentInfo.getEndDate();
                        CreditCard card = Database.getCreditCardByLastDigits(last4);
                        CreditCardReport report = Database.getCreditCardReport(card, date);
                        if (report == null)
                        {
                            //CreditCardReport r = new CreditCardReport();
                            //r.creditCard = card;
                            //r.chargeDate = date;
                            //r.transactions.Add(t);
                            //Database.creditCardReportsList.Add(r);
                        }
                        else
                            report.transactions.Add(t);
                    }
                }
            }
            //Database.
        }


        private static string getCreditCardNumber(string fullPath)
        {
            string[] parts = fullPath.Split('\\');
            string filename = parts[parts.Count() - 1]; // for example: "2011_10_3267.xls"

            string[] numbers = Regex.Split(filename, @"\D+");
            return numbers[2];
        }
    }
}
