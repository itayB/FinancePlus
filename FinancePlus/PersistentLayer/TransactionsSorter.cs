using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class TransactionsSorter : IComparer<Transaction>
    {
        public int Compare(Transaction t1, Transaction t2)
        {
            return t1.getChargeDate().CompareTo(t2.getChargeDate());
            //return obj2.CompareTo(obj1);
        }
    }
}
