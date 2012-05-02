using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.Storage;

namespace FinancePlus.PersistentLayer
{
    public class Month
    {
        // Fields
        private List<Transaction> transactions;
        public HashSet<PaymentInfo> paymentsInfoList;


        // Constructor
        public Month()
        {
            this.transactions = new List<Transaction>();
            this.paymentsInfoList = new HashSet<PaymentInfo>();
        }

        public void sortTransactions()
        {
            this.transactions.Sort(new TransactionsSorter());
        }

        public HashSet<Transaction>  getExpenses()
        {
            HashSet<Transaction> expenses = getTransactionsByType(Type.Expense, false);
            expenses.UnionWith(getTransactionsByType(Type.Credit, false));
            return expenses;
        }

        public HashSet<Transaction> getIncomes()
        {
            return getTransactionsByType(Type.Income,false);
        }

        public HashSet<Transaction> getFilter()
        {
            HashSet<Transaction> trans = new HashSet<Transaction>();
            foreach (Transaction t in this.transactions)
                if (t.filtered())
                    trans.Add(t);

            return trans;
        }

        public double getTotalExpenses()
        {
            return getTotal(Type.Expense, false);
        }

        public double getTotalIncomes()
        {
            return getTotal(Type.Income, false);
        }

        public double getTotalFilter()
        {
            return Math.Abs(getTotal(Type.Income, true) - getTotal(Type.Expense, true));
        }

        private double getTotal(Type type, bool filter)
        {
            HashSet<Transaction> trans = getTransactionsByType(type,filter);
            double total = 0;
            foreach (Transaction t in trans)
                total += t.billingPrice;

            return total;
        }

        private HashSet<Transaction> getTransactionsByType(Type type, bool filter)
        {
            HashSet<Transaction> trans = new HashSet<Transaction>();
            foreach (Transaction t in this.transactions)
                if (t.type == type && t.filtered() == filter)
                    trans.Add(t);

            return trans;
        }

        public void addTransaction(Transaction t)
        {
            transactions.Add(t);
        }

        public List<Transaction> getTransactions()
        {
            return this.transactions;
        }

        public PaymentInfo getPaymentInfo(string paymentId,PaymentType paymentType,DateTime startDate,DateTime endDate)
        {
            PaymentInfo paymentInfo = new PaymentInfo(paymentId, paymentType, startDate, endDate);

            foreach (PaymentInfo p in this.paymentsInfoList)
                if (p.Equals(paymentInfo))
                    return p;

            this.paymentsInfoList.Add(paymentInfo);
            return paymentInfo;
        }
    }
}
