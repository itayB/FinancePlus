using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FinancePlus.PersistentLayer;

namespace FinancePlus
{
    public partial class CommentForm : Form
    {
        public CommentForm()
        {
            InitializeComponent();
        }

        public void updateData()
        {
            Transaction exp = ((MonthForm)this.Tag).getSelectedExpense();

            businessNameLabel.Text = exp.businessName;
            if (exp.comment != null)
                commentTextBox.Text = exp.comment;

            commentTextBox.Focus();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Transaction exp = ((MonthForm)this.Tag).getSelectedExpense();

            exp.comment = commentTextBox.Text;

            ((MonthForm)this.Tag).updateMonthData();
            this.Close();
        }
    }
}
