using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.PersistentLayer;
using System.Collections;

namespace FinancePlus.Storage
{
    class CreditCardReport
    {
        public DateTime chargeDate;
        public ArrayList transactions;
        public double total;
        public CreditCard creditCard;
        //public string lastFourDigits;
        //public string bankAccountNumber;
        //public string bankBranchNumber;
        public Dictionary<DateTime, double> totalInternational;

        public CreditCardReport()
        {
            totalInternational = new Dictionary<DateTime, double>();
        }

        public double getTotal()
        {
            double total = this.total;

            foreach (KeyValuePair<DateTime, double> pair in this.totalInternational)
                total += pair.Value;

            return total;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            CreditCardReport p = obj as CreditCardReport;
            if ((System.Object)p == null)
                return false;

            return (this.chargeDate == p.chargeDate) && (this.creditCard == p.creditCard) && (this.total == p.total);
        }
    }
}
