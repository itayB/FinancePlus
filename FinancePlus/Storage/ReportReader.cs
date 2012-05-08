using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FinancePlus.PersistentLayer;

namespace FinancePlus.Storage
{
    abstract class ReportReader
    {
        /* Abstract methods */
        public abstract PaymentType getPaymentType();
        public abstract string getOpenFileDialogTitle();
        public abstract CreditCardReport readCreditCardReportFile(StreamReader sr);
        /* End of Abstract methods */


        /* Other methods */
        public virtual bool isCreditCardReader()
        {
            return false;
        }

        public virtual bool isBankAccountReader()
        {
            return false;
        }

        public virtual bool isValid(CreditCardReport report)
        {
            if (report == null)
            {
                Logger.log("CreditCardReport is empty.");
                return false;
            }

            if (report.creditCard == null)
            {
                Logger.log("CreditCard is empty.");
                return false;
            }

            double total = this.getTotalPrice(report);
            if (total != report.total)
            {
                Logger.log("Total transactions price (" + total + ") is different than appear in the report (" + report.total + ")");
                return false;
            }
            return true;
        }

        public StreamReader getStreamReader(string filename)
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

        protected double getTotalPrice(CreditCardReport report)
        {
            if (report == null || report.transactions == null)
                return 0;

            double total = 0;
            foreach (Transaction t in report.transactions)
                total += t.billingPrice;

            return total;
        }
        /* End of Other methods */
    }
}
