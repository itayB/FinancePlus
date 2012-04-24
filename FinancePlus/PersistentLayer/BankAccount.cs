using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class BankAccount
    {
        // Fields
        string accountNumber;    /* 6 digits */
        string bankBranchNumber; /* 3 digits */
        string bankNumber;       /* 2 digits */
        DateTime validFromDate;
        DateTime expiryDate;
        string description;
        string owner;
    }
}
