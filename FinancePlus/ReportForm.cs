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
    public partial class ReportForm : Form
    {
        private HashSet<System.Windows.Forms.DataGridViewTextBoxColumn> monthsColumns = new HashSet<System.Windows.Forms.DataGridViewTextBoxColumn>();

        public ReportForm()
        {
            InitializeComponent();
            addColumns();
            fillData();
        }

        private void addColumns()
        {
            foreach (KeyValuePair<DateTime, Month> pair in Database.months)
            {
                System.Windows.Forms.DataGridViewTextBoxColumn col = new System.Windows.Forms.DataGridViewTextBoxColumn();
                col.HeaderText = pair.Key.ToShortDateString();
                col.Name = "month" + pair.Key.ToShortDateString();
                col.ReadOnly = true;
                col.Tag = pair.Key;
                monthsColumns.Add(col);
            }

            // add average column
            System.Windows.Forms.DataGridViewTextBoxColumn colAverage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colAverage.HeaderText = "ממוצע";
            //colAverage.Name = "ממוצע";
            colAverage.ReadOnly = true;
            monthsColumns.Add(colAverage);

            this.dataGridView1.Columns.AddRange(monthsColumns.ToArray());
        }

        public void fillData()
        {
            //dataGridView1.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //dataGridView1.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //dataGridView1.Rows.Add(new object[] { 1, "Course 1","Sub 1\nSub 2\nSub3", "90\n95\n85" });
            foreach (Category category in Database.categories)
            {
                object[] row = new object[monthsColumns.Count + 3];
                row[0] = category.primary;
                row[1] = category.secondary;
                int i = 2;
                double average = 0;
                foreach (System.Windows.Forms.DataGridViewTextBoxColumn col in monthsColumns)
                {
                    if (col.Tag == null)
                        continue;

                    row[i] = Database.getTotalBillingPrice((DateTime)col.Tag, category);
                    average += (double)row[i];
                    i++;
                }
                // calc average
                row[i] = Math.Round(average / (monthsColumns.Count - 1), 2);
                int rowNum = dataGridView1.Rows.Add(row);

                // colorize relevant rows
                for(int j = 2 ; j < monthsColumns.Count + 2 ; j++)
                    if ((double)row[i] < (double)row[j])
                        dataGridView1.Rows[rowNum].Cells[j].Style.ForeColor = Color.Red;
            }

            // create sum last row
            object[] totalRow = new object[monthsColumns.Count + 3];
            totalRow[0] = Database.SUM;
            foreach (System.Windows.Forms.DataGridViewTextBoxColumn col in monthsColumns)
                if (col.Tag != null)
                    totalRow[col.Index] = Database.getMonth((DateTime)col.Tag).getTotalExpenses();// Database.getTotalBillingPrice((DateTime)col.Tag);
            dataGridView1.Rows.Add(totalRow);

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Font = new Font(dataGridView1.Font.FontFamily, dataGridView1.Font.Size, FontStyle.Bold);
            foreach (DataGridViewCell c in dataGridView1.Rows[dataGridView1.Rows.Count-1].Cells)
                c.Style.ApplyStyle(style);
        }
    }
}
