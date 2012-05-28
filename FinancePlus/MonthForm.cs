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
using FinancePlus.GUILayer;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace FinancePlus
{
    public partial class MonthForm : Form
    {
        public MonthForm()
        {
            InitializeComponent();
            incomesListView.ContextMenuStrip = contextMenuStrip1;
            expenseslistView.ContextMenuStrip = contextMenuStrip1;
            filterListView.ContextMenuStrip = contextMenuStrip1;
            updateMonths();
            CenterToScreen();
        }

        public Transaction getSelectedExpense()
        {
            if (incomesListView.SelectedItems.Count != 0)
                return (Transaction)(incomesListView.SelectedItems[0].Tag);
            if (expenseslistView.SelectedItems.Count != 0)
                return (Transaction)(expenseslistView.SelectedItems[0].Tag);
            if (filterListView.SelectedItems.Count != 0)
                return (Transaction)(filterListView.SelectedItems[0].Tag);

            return null;
        }

        ListViewGroup getGroup(Category category)
        {
            // Check each group if it fits to the item
            foreach (ListViewGroup group in this.expenseslistView.Groups)
            {
                // Compare group's header to selected subitem's text
                if (((Category)group.Tag).CompareTo(category) == 0)
                {
                    return group;
                }
            }
            // Create new group if no proper group was found

            // Create group and specify its header by
            // getting selected subitem's text
            //string total = "סה\"כ: " + "₪" + Database.getTotalBillingPrice(getSelectedMonth(), category);

            string total = Database.SUM + ": " + Database.getTotalBillingPrice(getSelectedFromMonthDateTime(), category) + Database.NIS_SIGN;

            ListViewGroup newGroup = new ListViewGroup(category.primary + ", " + category.secondary + ", " + total);
            newGroup.Tag = category;
            // We need to add the group to the ListView first
            this.expenseslistView.Groups.Add(newGroup);

            return newGroup;
        }

        void updateMonths()
        {
            fromMonthComboBox.Items.Clear();
            tillMonthComboBox.Items.Clear();

            foreach (KeyValuePair<DateTime, Month> pair in Database.months)
            {
                DateTime date= pair.Key;
                fromMonthComboBox.Items.Add(date.ToShortDateString());
                tillMonthComboBox.Items.Add(date.ToShortDateString());
            }

            fromMonthComboBox.SelectedItem = fromMonthComboBox.Items[fromMonthComboBox.Items.Count - 1];
            tillMonthComboBox.SelectedItem = tillMonthComboBox.Items[tillMonthComboBox.Items.Count - 1];
        }


        public void clearAll()
        {
            incomesListView.Items.Clear();
            expenseslistView.Groups.Clear();
            expenseslistView.Items.Clear();
            filterListView.Items.Clear();
            expensesChart.Series[0].Points.Clear();
        }

        public Month getSelectedMonth()
        {
            return Database.getMonth(getSelectedFromMonthDateTime());
        }

        public DateTime getSelectedFromMonthDateTime()
        {
            return Database.stringDateToDateTime(fromMonthComboBox.SelectedItem.ToString());
        }

        public DateTime getSelectedTillMonthDateTime()
        {
            return Database.stringDateToDateTime(tillMonthComboBox.SelectedItem.ToString());
        }

        public void updateMonthData()
        {
            clearAll();
            foreach (KeyValuePair<DateTime, Month> pair in Database.months)
                pair.Value.sortTransactions();
            updateIncomeListView();
            updateTotalIncomes();
            updateExpensesListView();
            updateTotalExpenses();
            updateFilterListView();
            updateTotalFilter();
            updateChart();
        }

        private void updateIncomeListView()
        {
            DateTime date = getSelectedFromMonthDateTime();
            Month month;

            if (!Database.months.TryGetValue(date, out month))
                return;

            foreach (Transaction e in month.getIncomes())
            {
                if (e.filtered())
                    continue;

                ListViewItem lvi = new ListViewItem(e.date.ToShortDateString());
                lvi.SubItems.Add(e.businessName);
                lvi.SubItems.Add("");
                lvi.SubItems.Add(e.transactionPrice.ToString());
                lvi.SubItems.Add(e.details);
                if (e.paymentInfo != null)
                    lvi.SubItems.Add(e.paymentInfo.getPaymentId());
                else if (e.creditCardNumber != null)
                    lvi.SubItems.Add(e.creditCardNumber);
                lvi.SubItems.Add(e.comment);
                //lvi.Group = getGroup(Database.getCategory(e));
                lvi.Tag = e;
                lvi.UseItemStyleForSubItems = false;
                lvi.SubItems[3].ForeColor = Color.LightGreen;
                incomesListView.Items.Add(lvi);
            }

        }

        private void updateFilterListView()
        {
            DateTime date = getSelectedFromMonthDateTime();
            Month month;

            if (!Database.months.TryGetValue(date, out month))
                return;

            foreach (Transaction e in month.getFilter())
            {
                if (e.filtered() == false)
                    continue;

                ListViewItem lvi = new ListViewItem(e.date.ToShortDateString());
                lvi.SubItems.Add(e.businessName);
                lvi.SubItems.Add(e.transactionPrice.ToString());
                lvi.SubItems.Add(e.billingPrice.ToString());
                lvi.SubItems.Add(e.details);
                if (e.paymentInfo != null)
                    lvi.SubItems.Add(e.paymentInfo.getPaymentId());
                else if (e.creditCardNumber != null)
                    lvi.SubItems.Add(e.creditCardNumber);
                lvi.SubItems.Add(e.comment);
                //lvi.Group = getGroup(Database.getCategory(e));
                lvi.Tag = e;
                lvi.UseItemStyleForSubItems = false;
                if (e.type == PersistentLayer.Type.Expense)
                    lvi.SubItems[3].ForeColor = Color.Red;
                else if (e.type == PersistentLayer.Type.Income)
                    lvi.SubItems[3].ForeColor = Color.LightGreen;
                filterListView.Items.Add(lvi);
            }

            //((ListViewGroupSorter)expenseslistView).SortGroups(true);  //Ascending...
            //((ListViewGroupSorter)listView1).SortGroups(false); //Descending...
        }

        private void updateExpensesListView()
        {
            DateTime date = getSelectedFromMonthDateTime();
            Month month;

            if (!Database.months.TryGetValue(date, out month))
                return;

            foreach (Transaction e in month.getExpenses())
            {
                if (e.filtered())
                    continue;

                ListViewItem lvi = new ListViewItem(e.getChargeDate().ToShortDateString());
                lvi.SubItems.Add(e.businessName);
                lvi.SubItems.Add(e.transactionPrice.ToString());
                lvi.SubItems.Add(e.billingPrice.ToString());
                lvi.SubItems.Add(e.details);
                if (e.paymentInfo != null)
                    lvi.SubItems.Add(e.paymentInfo.getPaymentId());
                else if (e.creditCardNumber != null)
                    lvi.SubItems.Add(e.creditCardNumber);
                lvi.SubItems.Add(e.comment);
                lvi.Group = getGroup(Database.getCategory(e));
                lvi.Tag = e;
                lvi.UseItemStyleForSubItems = false;
                if (e.type == PersistentLayer.Type.Expense)
                    lvi.SubItems[3].ForeColor = Color.Red;
                else
                    lvi.SubItems[3].ForeColor = Color.LightGreen;
                expenseslistView.Items.Add(lvi);
            }

            ((ListViewGroupSorter)expenseslistView).SortGroups(true);  //Ascending...
            //((ListViewGroupSorter)listView1).SortGroups(false); //Descending...
        }

        private void updateChart()
        {
            Dictionary<string, double> data = new Dictionary<string, double>();
            //List<ChartCategoty> list = new List<ChartCategoty>();
            DateTime date = getSelectedFromMonthDateTime();
            Month month;
            double total;
            double newTotal;

            if (!Database.months.TryGetValue(date, out month))
                return;

            foreach (Transaction e in month.getExpenses())
            {
                string prim = Database.getCategory(e).primary;
                if (!data.TryGetValue(prim, out total))
                {
                    data.Add(prim, e.billingPrice);
                }
                else
                {
                    newTotal = total + e.billingPrice;
                    data.Remove(prim);
                    data.Add(prim, newTotal);
                }
            }

            double[] yValues = data.Values.ToArray();//{ 3400, 1500, 500, 2000 };
            //string[] xValues = data.Keys.ToArray();//{ "", "הדס", "CCC","" };
            //chart1.Series[0].Points.DataBindXY(xValues, yValues);
            //chart1.Series[0].Points.Add(yValues);
            expensesChart.Series[0].Points.DataBindY(yValues);
            int i = 0;
            foreach (KeyValuePair<string, double> pair in data)
            {
                expensesChart.Series[0].Points[i].LegendText = pair.Key;
                expensesChart.Series[0].Points[i].ToolTip = pair.Key + ", " + pair.Value + Database.NIS_SIGN;
                i++;
            }
            //chart1.Series[0].Points[0].ToolTip = "tooltip 1";
            //chart1.Series[0].Points[0].LegendText = "שכירות";
            //chart1.Series[0].Points[1].LegendText = "הוצאות שוטפות";
            //chart1.Series[0].Points[2].LegendText = "תקשורת";
            //chart1.Series[0].Points[3].LegendText = "תחבורה";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (expenseslistView.SelectedItems.Count == 0)
                return;

            //listView1.SelectedItems[0].SubItems[0].Text;
        }

        private void updateTotalIncomes()
        {
            totalIncomesLabel.Text = "" + getSelectedMonth().getTotalIncomes().ToString("N", new CultureInfo("en-US")) + Database.NIS_SIGN;
        }

        private void updateTotalExpenses()
        {
            totalExpensesLabel.Text = "" + getSelectedMonth().getTotalExpenses().ToString("N", new CultureInfo("en-US")) + Database.NIS_SIGN;
        }

        public void updateTotalFilter()
        {
            totalFilterLabel.Text = "" + getSelectedMonth().getTotalFilter().ToString("N", new CultureInfo("en-US")) + Database.NIS_SIGN;

        }

        private void updatePaymentsInfoStatus()
        {
            string str = "";
            Month m = getSelectedMonth();
            foreach (PaymentInfo p in m.paymentsInfoList)
                if (p != null)
                    str += p.ToString() + "\n";

            sizeLabel.Text = str;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatePaymentsInfoStatus();

            if (this.tabControl.SelectedTab.Equals(this.monthTabPage))
                updateMonthData();
            else if (this.tabControl.SelectedTab.Equals(this.monthsCompTabPage))
                updateMonthsCompData();
            else if (this.tabControl.SelectedTab.Equals(this.categoryCompTabPage))
            {
                categoryCompListView.Items.Clear();
                updateCategoryCompRows();
                updateCategoryCompChart();
            }
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (expenseslistView.SelectedItems.Count == 0)
                return;

            CategoryForm cf = new CategoryForm();

            cf.Tag = this;// listView1.SelectedItems[0].Tag; // SubItems[1].Text;
            cf.fillData();
            cf.Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Save data to file?", "Application 1", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                //MessageBox.Show("App won´t close");
            }
            if (result == DialogResult.Yes)
            {
                Database.save();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (expenseslistView.SelectedItems.Count == 0 && incomesListView.SelectedItems.Count == 0)
                return;

            FilterForm ff = new FilterForm();

            ff.Tag = this;// listView1.SelectedItems[0].Tag; // SubItems[1].Text;
            ff.fillData();
            ff.Show();
            
            //Transaction selected = getSelectedExpense();
            //selected.filter = true;
            //updateData();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (expenseslistView.SelectedItems.Count == 0 && incomesListView.SelectedItems.Count == 0 && filterListView.SelectedItems.Count == 0)
                return;

            CommentForm cf = new CommentForm();

            cf.Tag = this;// listView1.SelectedItems[0].Tag; // SubItems[1].Text;
            
            cf.Show();
            cf.updateData();

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (incomesListView.Focused)
            {
                contextMenuStrip1.Items[0].Visible = false;
                contextMenuStrip1.Items[1].Visible = true;
                contextMenuStrip1.Items[2].Visible = true;
            }
            else if (expenseslistView.Focused)
            {
                contextMenuStrip1.Items[0].Visible = true;
                contextMenuStrip1.Items[1].Visible = true;
                contextMenuStrip1.Items[2].Visible = true;
            }
            else if (filterListView.Focused)
            {
                contextMenuStrip1.Items[0].Visible = false;
                contextMenuStrip1.Items[1].Visible = false;
                contextMenuStrip1.Items[2].Visible = true;
            }
        }

        private void loadFilePoalimToolStripButton_Click(object sender, EventArgs e)
        {
            //loadFileToolStripButton_Click(new PoalimReportReader());
        }

        private void loadFileIsracardToolStripButton_Click(object sender, EventArgs e)
        {
            loadFileToolStripButton_Click(new IsracardReportReader());
        }

        private void loadFileCalToolStripButton_Click(object sender, EventArgs e)
        {
            loadFileToolStripButton_Click(new CalReportReader());
        }

        private void loadFileToolStripButton_Click(ReportReader reportReader)
        {
            openFileDialog1.Title = Database.OPEN_REPORT_STRING + reportReader.getOpenFileDialogTitle();
            DialogResult result = openFileDialog1.ShowDialog();

            if (result != DialogResult.OK)
                return;

            string file = openFileDialog1.FileName;
            StreamReader sr = reportReader.getStreamReader(file);
            while (sr == null)
            {
                DialogResult result1 = MessageBox.Show(Database.ACCESS_FAILED_WARNING_MESSAGE,
                                                        Database.WARNING_STRING,
                                                        MessageBoxButtons.RetryCancel);
                if (result1 == DialogResult.Retry)
                    sr = reportReader.getStreamReader(file);
                else
                    return;
            }

            if (reportReader.isCreditCardReader())
            {
                CreditCardReport cardReport = reportReader.readCreditCardReportFile(sr);

                if (reportReader.getPaymentType() == PaymentType.Isracard)
                {
                    CreditCardChooserForm form = new CreditCardChooserForm();
                    form.Tag = cardReport;

                    // the ShowDialog waits until the form will be closed
                    form.ShowDialog();
                }

                // TODO: card report validation
                if (reportReader.isValid(cardReport))
                {
                    Database.addCreditCardReport(cardReport);
                    // add transaction to db
                }

            }
            else
            {
                //DataReader.readFile(sr, reportReader, file);
            }
            updateMonths();
            updateMonthData();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (this.tabControl.SelectedTab.Equals(this.monthTabPage))
            {
                this.tillMonthComboBox.Visible = false;
                this.tillMonthlabel.Visible = false;
            }
            else
            {
                this.tillMonthComboBox.Visible = true;
                this.tillMonthlabel.Visible = true;
            }

            if (this.tabControl.SelectedTab.Equals(this.monthsCompTabPage))
                updateMonthsCompData();
            else if (this.tabControl.SelectedTab.Equals(this.monthTabPage))
                updateMonthData();
            else if (this.tabControl.SelectedTab.Equals(this.categoryCompTabPage))
                updateCategoryCompData();
            else if (this.tabControl.SelectedTab.Equals(this.reportsTabPage))
                updateReportsData();
        }

        /************************************************** Months Compare **************************************************/

        public void updateMonthsCompData()
        {
            clearAllMonthsComp();
            updateColumns();
            updateMonthsCompRows();
            updateMonthsExpensesChart();
            updateMonthsIncomesExpenesChart();
            updateMonthsSavings();
        }

        public void clearAllMonthsComp()
        {
            monthsCompListView.Columns.Clear();
            monthsCompListView.Items.Clear();
            foreach (Series s in monthsExpensesChart.Series)
                s.Points.Clear();
            monthsExpensesChart.Series.Clear();
            foreach (Series s in monthsIncomesExpensesChart.Series)
                s.Points.Clear();
            monthsIncomesExpensesChart.Series.Clear();
            foreach (Series s in savingsChart.Series)
                s.Points.Clear();
            savingsChart.Series.Clear();
        }

        public ColumnHeader addColumn(string name, string text, int width, object data)
        {
            ColumnHeader ch = new ColumnHeader();
            ch.Name = name;
            ch.Text = text;
            ch.Width = width;
            ch.Tag = data;

            return ch;
        }

        private void updateColumns()
        {
            DateTime fromDate = getSelectedFromMonthDateTime();
            DateTime tillDate = getSelectedTillMonthDateTime();

            if (fromDate > tillDate)
                return;

            ColumnHeader secCategoryCol = addColumn("secondaryCategory", "קטגוריה משנית", 140, null);
            monthsCompListView.Columns.Add(secCategoryCol);

            for (DateTime date = fromDate; date <= tillDate; date = date.AddMonths(1))
            {
                Month month;
                if (Database.months.TryGetValue(date, out month))
                {
                    ColumnHeader ch = addColumn("month" + date.ToShortDateString(), date.ToShortDateString(), 70, date);
                    monthsCompListView.Columns.Add(ch);
                }
            }
            /*
            foreach (KeyValuePair<DateTime, Month> pair in Database.months)
            {
                ColumnHeader ch = addColumn("month" + pair.Key.ToShortDateString(), pair.Key.ToShortDateString(), 70, pair.Key);
                monthsCompListView.Columns.Add(ch);
            }
            */
            ColumnHeader averageCol = addColumn("average", "ממוצע", 70, null);
            monthsCompListView.Columns.Add(averageCol);

            /*
            // add average column
            System.Windows.Forms.DataGridViewTextBoxColumn colAverage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colAverage.HeaderText = "ממוצע";
            //colAverage.Name = "ממוצע";
            colAverage.ReadOnly = true;
            monthsColumns.Add(colAverage);

            this.dataGridView1.Columns.AddRange(monthsColumns.ToArray());
            */
        }

        private void updateMonthsCompIncomeRow()
        {
            ListViewItem lvi = new ListViewItem("");
            foreach (ColumnHeader col in monthsCompListView.Columns)
            { 
                Month month;
                if (col.Tag != null && Database.months.TryGetValue((DateTime)col.Tag, out month))
                {
                    lvi.SubItems.Add(month.getTotalIncomes().ToString("N", new CultureInfo("en-US")));
                }
            }
            lvi.Group = getCategoryGroup("הכנסות");
            monthsCompListView.Items.Add(lvi);

        }

        private void updateMonthsCompRows()
        {
            updateMonthsCompIncomeRow();

            foreach (Category category in Database.categories)
            {
                ListViewItem lvi = new ListViewItem(category.secondary);
                foreach (ColumnHeader col in monthsCompListView.Columns)
                {
                    if (col.Tag != null)
                    {
                        lvi.SubItems.Add(Database.getTotalBillingPrice((DateTime)col.Tag, category).ToString("N", new CultureInfo("en-US")));
                    }
                }
                lvi.Group = getCategoryGroup(category.primary);
                monthsCompListView.Items.Add(lvi);
            }
        }

        private void updateMonthsExpensesChart()
        {
            foreach (ListViewGroup group in monthsCompListView.Groups)
            {
                if (group.Header.ToString().StartsWith(Database.INCOME_STRING))
                    continue;
                Series s = new Series();
                s.ChartType = SeriesChartType.StackedColumn;
                s.LegendText = group.Tag.ToString();
                monthsExpensesChart.Series.Add(s);
                
                foreach (ColumnHeader col in monthsCompListView.Columns)
                {
                    double total = 0;
                    if (col.Tag != null)
                    {
                        foreach (ListViewItem item in group.Items)
                            total += Double.Parse(item.SubItems[col.Index].Text);
                        DataPoint point = new DataPoint();
                        point.SetValueXY(((DateTime)col.Tag).ToShortDateString(), total);
                        point.ToolTip = group.Header + total.ToString("N", new CultureInfo("en-US")) + " " + Database.NIS_SIGN;
                        s.Points.Add(point);
                    }
                    
                }
              
            }
        }

        private void updateMonthsIncomesExpenesChart()
        {
            Series incomesSeries = new Series();
            incomesSeries.Color = Color.LimeGreen;
            incomesSeries.LegendText = Database.INCOME_STRING;
            monthsIncomesExpensesChart.Series.Add(incomesSeries);
            
            Series expensesSeries = new Series();
            expensesSeries.Color = Color.Red;
            expensesSeries.LegendText = Database.INCOME_STRING;
            monthsIncomesExpensesChart.Series.Add(expensesSeries);
           

            foreach (ColumnHeader col in monthsCompListView.Columns)
            {
                if (col.Tag != null)
                {
                    double incomesTotal = 0;
                    double expensesTotal = 0;
                    foreach (ListViewGroup group in monthsCompListView.Groups)
                    {
                        double total = 0;
                        foreach (ListViewItem item in group.Items)
                            total += Double.Parse(item.SubItems[col.Index].Text);

                        if (group.Header.ToString().StartsWith(Database.INCOME_STRING))
                            incomesTotal += total;
                        else
                            expensesTotal += total;
                    }

                    DataPoint pIncomes = new DataPoint();
                    pIncomes.SetValueXY(((DateTime)col.Tag).ToShortDateString(), incomesTotal);
                    pIncomes.ToolTip = Database.INCOME_STRING + ":" + incomesTotal.ToString("N", new CultureInfo("en-US")) + " " + Database.NIS_SIGN;
                    incomesSeries.Points.Add(pIncomes);

                    DataPoint pExpenses = new DataPoint();
                    pExpenses.SetValueXY(((DateTime)col.Tag).ToShortDateString(), expensesTotal);
                    pExpenses.ToolTip = Database.EXPENSES_STRING + ":" + expensesTotal.ToString("N", new CultureInfo("en-US")) + " " + Database.NIS_SIGN;
                    expensesSeries.Points.Add(pExpenses);
                }
            }
        }

        private void updateMonthsSavings()
        {
            Series savingsSeries = new Series();
            savingsSeries.Color = Color.LimeGreen;
            savingsSeries.LegendText = "יתרה";
            savingsChart.Series.Add(savingsSeries);

            foreach (ColumnHeader col in monthsCompListView.Columns)
            {
                if (col.Tag != null)
                {
                    double incomesTotal = 0;
                    double expensesTotal = 0;
                    foreach (ListViewGroup group in monthsCompListView.Groups)
                    {
                        double total = 0;
                        foreach (ListViewItem item in group.Items)
                            total += Double.Parse(item.SubItems[col.Index].Text);

                        if (group.Header.ToString().StartsWith(Database.INCOME_STRING))
                            incomesTotal += total;
                        else
                            expensesTotal += total;
                    }

                    double savings = incomesTotal - expensesTotal;
                    DataPoint pSavings = new DataPoint();
                    if (savings < 0)
                        pSavings.Color = Color.Red;

                    pSavings.SetValueXY(((DateTime)col.Tag).ToShortDateString(), savings);
                    pSavings.ToolTip = "יתרה" + ":" + savings.ToString("N", new CultureInfo("en-US")) + " " + Database.NIS_SIGN;
                    savingsSeries.Points.Add(pSavings);
                }
            }
        }

        ListViewGroup getCategoryGroup(string primaryCategory)
        {
            // Check each group if it fits to the item
            foreach (ListViewGroup group in this.monthsCompListView.Groups)
            {
                // Compare group's header to selected subitem's text
                if (((string)group.Tag).CompareTo(primaryCategory) == 0)
                {
                    return group;
                }
            }

            ListViewGroup newGroup = new ListViewGroup(primaryCategory + ": ");
            newGroup.Tag = primaryCategory;
            // We need to add the group to the ListView first
            this.monthsCompListView.Groups.Add(newGroup);

            return newGroup;
        }

        private void tillMonthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl.SelectedTab.Equals(this.monthsCompTabPage))
                updateMonthsCompData();
            else if (this.tabControl.SelectedTab.Equals(this.categoryCompTabPage))
            {
                //updateCategoryCompData();
                categoryCompListView.Items.Clear();
                updateCategoryCompRows();
                updateCategoryCompChart();
            }
        }


        /************************************************** Category Compare **************************************************/

        public void updateCategoryCompData()
        {
            clearAllCategoryComp();
            updatePrimaryCategoryComboBox();
            updateCategoryCompChart();
        }

        public void clearAllCategoryComp()
        {
            primaryCategoryComboBox.Items.Clear();
            secondaryCategoryComboBox.Items.Clear();
            categoryCompListView.Items.Clear();
            categoryChart.Series[0].Points.Clear();
        }

        public void updatePrimaryCategoryComboBox()
        {
            List<string> items = new List<string>();

            foreach (Category c in Database.categories)
            {
                if (!items.Contains(c.primary))
                    items.Add(c.primary);
            }

            items.Sort();
            primaryCategoryComboBox.Items.AddRange(items.ToArray());
        }

        private void primaryCategoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string primary = primaryCategoryComboBox.SelectedItem.ToString();

            secondaryCategoryComboBox.Items.Clear();

            List<string> items = new List<string>();
            foreach (Category c in Database.categories)
            {
                if (c.primary.Equals(primary))
                    items.Add(c.secondary);
            }

            items.Sort();
            secondaryCategoryComboBox.Items.AddRange(items.ToArray());
        }

        private void secondaryCategoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            categoryCompListView.Items.Clear();
            updateCategoryCompRows();
            updateCategoryCompChart();
        }

        private void updateCategoryCompRows()
        {
            DateTime fromDate = getSelectedFromMonthDateTime();
            DateTime tillDate = getSelectedTillMonthDateTime();

            if (fromDate > tillDate)
                return;

            this.categoryCompListView.Groups.Clear();

            for (DateTime date = fromDate; date <= tillDate; date = date.AddMonths(1))
            {
                Month month;
                if (Database.months.TryGetValue(date, out month))
                {
                    foreach (Transaction e in month.getExpenses())
                    {
                        if (e.filtered())
                            continue;
                            
                        if (!Database.getCategory(e).primary.Equals(primaryCategoryComboBox.Text) ||
                            !Database.getCategory(e).secondary.Equals(secondaryCategoryComboBox.Text))
                            continue;

                        ListViewItem lvi = new ListViewItem(e.getChargeDate().ToShortDateString());
                        lvi.SubItems.Add(e.businessName);
                        lvi.SubItems.Add(e.transactionPrice.ToString());
                        lvi.SubItems.Add(e.billingPrice.ToString());
                        lvi.SubItems.Add(e.details);
                        if (e.paymentInfo != null)
                            lvi.SubItems.Add(e.paymentInfo.getPaymentId());
                        else if (e.creditCardNumber != null)
                            lvi.SubItems.Add(e.creditCardNumber);
                        lvi.SubItems.Add(e.comment);
                        lvi.Group = getCategoryCompGroup(date);// new ListViewGroup(date.ToShortDateString());
                        lvi.Group.Items.Add(lvi);
                        categoryCompListView.Groups.Add(lvi.Group);
                        lvi.Tag = e;
                        lvi.UseItemStyleForSubItems = false;
                        lvi.SubItems[3].ForeColor = Color.Red;
                        categoryCompListView.Items.Add(lvi);
                    }

                    //((ListViewGroupSorter)categoryCompListView).SortGroups(true);  //Ascending...
                    //((ListViewGroupSorter)listView1).SortGroups(false); //Descending...

                }
            }
        }

        ListViewGroup getCategoryCompGroup(DateTime date)
        {
            // Check each group if it fits to the item
            foreach (ListViewGroup group in this.categoryCompListView.Groups)
            {
                // Compare group's header to selected subitem's text
                if (((DateTime)group.Tag).CompareTo(date) == 0)
                {
                    return group;
                }
            }
            // Create new group if no proper group was found

            // Create group and specify its header by
            // getting selected subitem's text
            //string total = "סה\"כ: " + "₪" + Database.getTotalBillingPrice(getSelectedMonth(), category);

            Category category = Database.getCategory(primaryCategoryComboBox.Text, secondaryCategoryComboBox.Text);

            string total = "סה\"כ: " + Database.getTotalBillingPrice(date, category) + Database.NIS_SIGN;

            ListViewGroup newGroup = new ListViewGroup(date.ToShortDateString() + ", " + total);
            newGroup.Tag = date;
            // We need to add the group to the ListView first
            this.categoryCompListView.Groups.Add(newGroup);

            return newGroup;
        }


        private void updateCategoryCompChart()
        {
            DateTime fromDate = getSelectedFromMonthDateTime();
            DateTime tillDate = getSelectedTillMonthDateTime();

            if (fromDate > tillDate)
                return;

            Dictionary<DateTime, double> data = new Dictionary<DateTime, double>();
            double total, newTotal;

            for (DateTime date = fromDate; date <= tillDate; date = date.AddMonths(1))
            {
                Month month;
                if (Database.months.TryGetValue(date, out month))
                {
                    foreach (Transaction e in month.getExpenses())
                    {
                        if (e.filtered())
                            continue;

                        if (!Database.getCategory(e).primary.Equals(primaryCategoryComboBox.Text) ||
                            !Database.getCategory(e).secondary.Equals(secondaryCategoryComboBox.Text))
                            continue;

                        if (!data.TryGetValue(date, out total))
                        {
                            data.Add(date, e.billingPrice);
                        }
                        else
                        {
                            newTotal = total + e.billingPrice;
                            data.Remove(date);
                            data.Add(date, newTotal);
                        }
                    }

                }
            }

            double[] yValues = data.Values.ToArray();
            HashSet<string> xValues = new HashSet<string>();
            foreach(DateTime d in data.Keys)
                xValues.Add(d.ToShortDateString());
            categoryChart.Series[0].Points.DataBindXY(xValues.ToArray(), yValues);
            
            int i = 0;
            foreach (KeyValuePair<DateTime, double> pair in data)
            {
                //chart2.Series[0].Points[i].LegendText = pair.Key.ToShortDateString();
                categoryChart.Series[0].Points[i].ToolTip = pair.Key.ToShortDateString() +", " + pair.Value + Database.NIS_SIGN;
               // DataPoint item = new DataPoint();
                //item.ToolTip = pair.Key.ToShortDateString() +", " + pair.Value + Database.NIS_SIGN;
                //item.LegendText = pair.Key.ToShortDateString();
                //chart2.Series[0].Points.Add(item);
                i++;
            }
            //chart1.Series[0].Points[0].ToolTip = "tooltip 1";
            //chart1.Series[0].Points[0].LegendText = "שכירות";
            //chart1.Series[0].Points[1].LegendText = "הוצאות שוטפות";
            //chart1.Series[0].Points[2].LegendText = "תקשורת";
            //chart1.Series[0].Points[3].LegendText = "תחבורה";
        }

        /************************************************** Bank & Credit-Cards reports **************************************************/


    }
}
