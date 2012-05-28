using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FinancePlus.PersistentLayer;

namespace FinancePlus.GUILayer
{
    static class GUIHandler
    {
        public static void addListViewItem(ListView listView, string[] strings, object[] objects)
        {
            if (strings.Count() != objects.Count())
            {
                Logger.log("Error in addListViewItem");
                return;
            }

            ListViewItem lvi = new ListViewItem();
            for (int i = 0; i < objects.Count(); i++)
            {
                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(lvi, strings[i]);
                subItem.Tag = objects[i];
                lvi.SubItems.Insert(i, subItem);
            }
            listView.Items.Add(lvi);
        }

        public static void listView_OnColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e, ListView listView)
        {
            ListViewSorter sorter = (ListViewSorter)listView.ListViewItemSorter;

            if (!(listView.ListViewItemSorter is ListViewSorter))
                return;

            if (sorter.LastSort == e.Column)
            {
                if (listView.Sorting == SortOrder.Ascending)
                    listView.Sorting = SortOrder.Descending;
                else
                    listView.Sorting = SortOrder.Ascending;
            }
            else
            {
                listView.Sorting = SortOrder.Descending;
            }
            sorter.ByColumn = e.Column;

            listView.Sort();
        }
    }
}
