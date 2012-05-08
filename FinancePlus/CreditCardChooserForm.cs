using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FinancePlus.Storage;
using FinancePlus.PersistentLayer;

namespace FinancePlus
{
    public partial class CreditCardChooserForm : Form
    {
        public CreditCardChooserForm()
        {
            InitializeComponent();
            updateCreditCardListBox();
        }

        private void updateCreditCardListBox()
        {
            foreach (CreditCard card in Database.creditCardsList)
            {
                if (card.paymentType == PaymentType.Isracard)
                {
                    ListViewItem lvi = new ListViewItem(card.lastFourDigits);
                    lvi.SubItems.Add(card.description);
                    lvi.Tag = card;
                    creditCardsListView.Items.Add(lvi);
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (this.Tag == null)
            {
                MessageBox.Show("Error: CreditCardReport in Tag data object.");
                return;
            }

            CreditCardReport cardReport = (CreditCardReport)this.Tag;

            if (creditCardsListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("יש לבחור כרטיס אשראי מהרשימה.");
                return;
            }

            cardReport.creditCard = (CreditCard)(creditCardsListView.SelectedItems[0].Tag);
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
