using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FinancePlus.Storage;

namespace FinancePlus.PersistentLayer
{
    public enum Type
    {
	    Expense,
	    Income,
	    Credit
    };

    public class Transaction
    {
        // Fields
        public DateTime date;
        public String businessName;
        public double transactionPrice;
        public double billingPrice;
        public int receiptId;
        public string details;
        public Category category;
        public string creditCardNumber;
        public PaymentInfo paymentInfo;
        public bool filter;
        public string comment;
        public Type type;

        // Constructor
        public Transaction()
        {
            filter = false;
        }

        public DateTime getChargeDate()
        {
            int paymentNum = 0;
            if (this.details != null && this.details.StartsWith("תשלום"))
            {
                string[] numbers = Regex.Split(this.details, @"\D+");
                if (numbers.Count() == 3)
                    paymentNum = int.Parse(numbers[1]) - 1;
            }

            return new DateTime(this.date.Year, this.date.Month, this.date.Day).AddMonths(paymentNum);
        }

        public void setType(string type)
        {
            switch (type)
            {
                case "Expense":
                    this.type = Type.Expense;
                    break;
                case "Income":
                    this.type = Type.Income;
                    break;
                case "Credit":
                    this.type = Type.Credit;
                    break;
            }
        }

        public bool filtered()
        {
            if (this.filter)
                return true;

            return Database.filterMap.Contains(this.businessName);
        }

        public string toString()
        {
            return date + " "
                + businessName + " "
                + transactionPrice + " "
                + billingPrice + " "
                + receiptId + " "
                + details;
        }

    }
}
