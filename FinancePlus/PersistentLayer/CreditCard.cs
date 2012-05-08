using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class CreditCard
    {
        // Fields
        public PaymentType paymentType;
        public string lastFourDigits;
        //public DateTime validFromDate;
        public DateTime expiryDate; // the day represents the charge day of the month
        public string description;
        public string owner;
        public BankAccount bank;
    }
}
