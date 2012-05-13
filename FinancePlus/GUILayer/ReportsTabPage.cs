using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.Storage;
using FinancePlus.PersistentLayer;
using System.Windows.Forms;

namespace FinancePlus
{
    public partial class MonthForm
    {
        public void updateReportsData()
        {
            clearAllReportsData();
            updateBankAccountsList();
            updateCreditCardsList();
            updateCreditCardReportsList();
        }

        public void clearAllReportsData()
        {
            bankAccountsListView.Items.Clear();
            creditCardsListView.Items.Clear();
            creditCardReportsListView.Items.Clear();
        }

        public void updateBankAccountsList()
        {
            foreach (BankAccount bank in Database.bankAccounts)
            {
                ListViewItem lvi = new ListViewItem(bank.accountNumber);
                lvi.SubItems.Add(bank.branchNumber);
                lvi.SubItems.Add(bank.bankNumber);
                lvi.SubItems.Add(bank.bankName);
                lvi.SubItems.Add(bank.branchName);
                lvi.SubItems.Add(bank.owner);
                bankAccountsListView.Items.Add(lvi);
            }
        }

        public void updateCreditCardsList()
        {
            foreach (CreditCard card in Database.creditCardsList)
            {
                ListViewItem lvi = new ListViewItem(card.lastFourDigits);
                lvi.SubItems.Add(card.description);
                lvi.SubItems.Add(card.owner);
                lvi.SubItems.Add(ToShortMonthYearString(card.expiryDate));
                lvi.SubItems.Add(ToShortDayString(card.expiryDate));
                if (card.bank != null)
                    lvi.SubItems.Add(card.bank.accountNumber);
                creditCardsListView.Items.Add(lvi);
            }
        }

        public void updateCreditCardReportsList()
        {
            foreach (CreditCardReport cardReport in Database.creditCardReportsList)
            {
                ListViewItem lvi = new ListViewItem(ToShortMonthYearString(cardReport.chargeDate));
                if (cardReport.creditCard != null)
                {
                    lvi.SubItems.Add(cardReport.creditCard.lastFourDigits);
                    lvi.SubItems.Add(cardReport.creditCard.description);
                }
                else
                {
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                }
                lvi.SubItems.Add(cardReport.transactions.Count.ToString());
                lvi.SubItems.Add(cardReport.getTotal().ToString());
                creditCardReportsListView.Items.Add(lvi);
            }
        }

        public string ToShortDayString(DateTime date)
        {
            if (date.Day < 10)
                return "0" + date.Day;

            return "" + date.Day;
        }

        public string ToShortMonthYearString(DateTime date)
        {
            string res = "";

            if (date.Month < 10)
                res += "0";
            res += date.Month + "/" + date.Year; 

            return res; 
        }

    }
}
