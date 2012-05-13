using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.Storage;

namespace FinancePlus.PersistentLayer
{
    public enum PaymentType
    {
        Poalim,
        Isracard,
        Cal,
        Unknown
    };

    public class PaymentInfo
    {
        // fields
        private string paymentId;
        private PaymentType paymentType;
        private DateTime startDate;
        private DateTime endDate;

        // constructor
        public PaymentInfo(string paymentId, PaymentType paymentType, DateTime startDate, DateTime endDate)
        {
            this.paymentId = paymentId;
            this.paymentType = paymentType;
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public string getPaymentId()
        {
            return this.paymentId;
        }

        public PaymentType getPaymentType()
        {
            return this.paymentType;

        }

        public DateTime getStartDate()
        {
            return this.startDate;
        }

        public DateTime getEndDate()
        {
            return this.endDate;
        }

        public static PaymentType parsePaymentTypeString(string name)
        {
            switch (name)
            {
                case "Poalim":
                    return PaymentType.Poalim;
                case "Isracard":
                    return PaymentType.Isracard;
                case "Cal":
                    return PaymentType.Cal;
                default:
                    return PaymentType.Unknown;
            }
        }

        public void setPaymentName(string name)
        {
            switch (name)
            {
                case "Poalim":
                    this.paymentType = PaymentType.Poalim;
                    break;
                case "Isracard":
                    this.paymentType = PaymentType.Isracard;
                    break;
                case "Cal":
                    this.paymentType = PaymentType.Cal;
                    break;
            }
        }

        public override bool Equals(Object o)
        {
            try
            {
                if (o == null || GetType() != o.GetType()) 
                    return false;

                // If parameter cannot be cast to PaymentInfo return false.
                PaymentInfo other = (PaymentInfo)o;
                if ((System.Object)other == null)
                {
                    return false;
                }

                return this.paymentId.Equals(other.paymentId) &&
                       this.paymentType.Equals(other.paymentType) &&
                       this.startDate.Equals(other.startDate) &&
                       this.endDate.Equals(other.endDate);
            }
            catch
            {
                return false;
            }
        }

        public override string ToString()
        {
            string str = "";
            if (this.paymentType.Equals(PaymentType.Poalim))
                str += Database.POALIM_STRING + ": " + Database.ACCOUNT_NUMBER + ":" + this.paymentId;
            else if (this.paymentType.Equals(PaymentType.Cal))
                str += Database.CAL_STRING + ": " + Database.CARD_NUMBER + ":" + this.paymentId;
            else if (this.paymentType.Equals(PaymentType.Isracard))
                str += Database.ISRACARD_STRING + ": " + Database.CARD_NUMBER + ":" + this.paymentId;

            str += " ת. התחלה:" + this.startDate.ToShortDateString() + " ת. סיום:" + this.endDate.ToShortDateString();
            return str;
        }
    }
}
