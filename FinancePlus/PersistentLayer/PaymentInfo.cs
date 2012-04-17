using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
