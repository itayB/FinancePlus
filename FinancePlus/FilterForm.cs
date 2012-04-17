using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FinancePlus.PersistentLayer;
using FinancePlus.Storage;

namespace FinancePlus
{
    public partial class FilterForm : Form
    {
        public FilterForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Transaction exp = ((MonthForm)this.Tag).getSelectedExpense();

            if (checkBox1.Checked == true)
                Database.updateAllTransactionFilter(exp);
            else
                exp.filter = true;

            ((MonthForm)this.Tag).updateMonthData();
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void fillData()
        {
            Transaction e = ((MonthForm)this.Tag).getSelectedExpense();
            businessNameLabel.Text = e.businessName;
        }
    }
}
