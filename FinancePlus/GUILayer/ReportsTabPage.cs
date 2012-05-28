using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.Storage;
using FinancePlus.PersistentLayer;
using System.Windows.Forms;
using FinancePlus.GUILayer;

namespace FinancePlus
{
    public partial class MonthForm
    {
        public void updateReportsData()
        {
            creditCardReportsListView.ListViewItemSorter = new ListViewSorter();
            creditCardsListView.ListViewItemSorter = new ListViewSorter();
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

        public void updateCreditCardsList1()
        {
            foreach (CreditCard card in Database.creditCardsList)
            {
                ListViewItem lvi = new ListViewItem(card.lastFourDigits);
                lvi.SubItems.Add(card.description);
                lvi.SubItems.Add(card.owner);
                lvi.SubItems.Add(Common.toShortMonthYearString(card.expiryDate));
                lvi.SubItems.Add(ToShortDayString(card.expiryDate));
                if (card.bank != null)
                    lvi.SubItems.Add(card.bank.accountNumber);
                creditCardsListView.Items.Add(lvi);
            }
        }

        public void updateCreditCardsList()
        {
            foreach (CreditCard card in Database.creditCardsList)
            {
                string[] strings = { card.lastFourDigits, 
                                     card.description, 
                                     card.owner,
                                     Common.toShortMonthYearString(card.expiryDate), 
                                     ToShortDayString(card.expiryDate),
                                     card.bank.accountNumber};

                object[] objects = { card.lastFourDigits, 
                                     card.description, 
                                     card.owner,
                                     card.expiryDate, 
                                     card.expiryDate.Day,
                                     card.bank.accountNumber};

                GUIHandler.addListViewItem(creditCardsListView, strings, objects);
            }
        }

        public void updateCreditCardReportsList()
        {
            foreach (CreditCardReport cardReport in Database.getCreditCardReportsList())
            {
                string[] strings = { Common.toShortMonthYearString(cardReport.chargeDate), 
                                     cardReport.creditCard.lastFourDigits, 
                                     cardReport.creditCard.description,
                                     cardReport.transactions.Count.ToString(), 
                                     cardReport.getTotal().ToString()};
                
                object[] objects = { cardReport.chargeDate, 
                                     cardReport.creditCard.lastFourDigits, 
                                     cardReport.creditCard.description,
                                     cardReport.transactions.Count, 
                                     cardReport.getTotal()};

                GUIHandler.addListViewItem(creditCardReportsListView,strings,objects);
            }
        }


        public string ToShortDayString(DateTime date)
        {
            if (date.Day < 10)
                return "0" + date.Day;

            return "" + date.Day;
        }

        private void creditCardReportsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            GUIHandler.listView_OnColumnClick(sender, e, creditCardReportsListView);
        }

        private void creditCardsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            GUIHandler.listView_OnColumnClick(sender, e, creditCardsListView);
        }
    }
}
