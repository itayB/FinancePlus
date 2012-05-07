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
    }
}
