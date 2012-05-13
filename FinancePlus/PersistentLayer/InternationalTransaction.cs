using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class InternationalTransaction : Transaction
    {
        // Fields
        public DateTime chargeDate;
        public string city;
        public string originalCurrency;
        public string billingCurrency;

    }
}
