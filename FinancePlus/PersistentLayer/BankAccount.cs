using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class BankAccount
    {
        // Fields
        public int hashCode;
        public string accountNumber;    /* 6 digits */
        public string branchNumber;     /* 3 digits */
        public string bankNumber;       /* 2 digits */
        public string bankName;
        public string branchName;
        public string owner;
        //public DateTime validFromDate;
        //public DateTime expiryDate;
        //public string description;
    }
}
