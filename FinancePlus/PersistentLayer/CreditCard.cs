﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class CreditCard
    {
        // Fields
        string lastFourDigits;
        DateTime validFromDate;
        DateTime expiryDate;
        string description;
        string owner;
        BankAccount bank;
    }
}
