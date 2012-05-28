using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class CreditCard
    {
        // Fields
        public int hashCode;
        public PaymentType paymentType;
        public string lastFourDigits;
        //public DateTime validFromDate;
        public DateTime expiryDate; // the day represents the charge day of the month
        public string description;
        public string owner;
        public BankAccount bank;

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            CreditCard p = obj as CreditCard;
            if ((System.Object)p == null)
                return false;

            return (this.paymentType == p.paymentType) && (this.lastFourDigits.Equals(p.lastFourDigits)) && (this.expiryDate.Equals(p.expiryDate)) && (this.owner.Equals(p.owner));
        }

        public override string ToString()
        {
            return lastFourDigits + ", " + this.description;
        }
    }
}
