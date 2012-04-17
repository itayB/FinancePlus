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
    public partial class CategoryForm : Form
    {
        public CategoryForm()
        {
            InitializeComponent();
        }


        public void fillData()
        {
            foreach (Category c in Database.categories)
            {
                if (!primaryCategoryComboBox.Items.Contains(c.primary))
                    primaryCategoryComboBox.Items.Add(c.primary);
                //secondaryCategoryComboBox.Items.Add(c.secondary);
            }

            Transaction e = ((MonthForm)this.Tag).getSelectedExpense();
            businessNameLabel.Text = e.businessName;
            if (e.category == null)
            {
                primaryCategoryComboBox.Text = Database.DEFAULT_PRIMARY_CATEGORY;
                secondaryCategoryComboBox.Text = Database.DEFAULT_SECONDARY_CATEGORY;
            }
            else
            {
                primaryCategoryComboBox.Text = e.category.primary;
                secondaryCategoryComboBox.Text = e.category.secondary;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Transaction exp = ((MonthForm)this.Tag).getSelectedExpense();

            Category selectedCategory = Database.getCategory(primaryCategoryComboBox.Text, secondaryCategoryComboBox.Text);

            //exp.category.primary = primaryCategoryComboBox.Text;
            //exp.category.secondary = secondaryCategoryComboBox.Text;

            if (ApplyCategoryCheckBox.Checked == true)
                Database.updateAllExpensesCategory(exp,selectedCategory);
            else
                Database.updateExpenseCategory(exp, selectedCategory);

            // MessageBox.Show(sender.ToString());
            ((MonthForm)this.Tag).updateMonthData();
            this.Close();
        }

        private void primaryCategoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string primary = primaryCategoryComboBox.SelectedItem.ToString();

            secondaryCategoryComboBox.Items.Clear();
            //for(int i=0; i<secondaryCategoryComboBox.Items.Count ; i++)
            //    secondaryCategoryComboBox.Items.RemoveAt(i);

            foreach (Category c in Database.categories)
            {
                if (c.primary.Equals(primary))
                    secondaryCategoryComboBox.Items.Add(c.secondary);
            }
        }

        private void primaryCategoryComboBox_TextChanged(object sender, EventArgs e)
        {
            secondaryCategoryComboBox.Items.Clear();
            //for (int i = 0; i < secondaryCategoryComboBox.Items.Count; i++)
            //    secondaryCategoryComboBox.Items.RemoveAt(i);

            secondaryCategoryComboBox.Text = "";
        }
    }
}
