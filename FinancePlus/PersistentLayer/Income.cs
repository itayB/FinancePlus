using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class Income
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
        public bool filter;
        public string comment;
    }
}
