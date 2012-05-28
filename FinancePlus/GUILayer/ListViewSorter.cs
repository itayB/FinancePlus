using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FinancePlus.GUILayer
{
    public class ListViewSorter : System.Collections.IComparer
    {
        int Column = 0;
        int LastColumn = 0;

        public int Compare(object o1, object o2)
        {
            if (!(o1 is ListViewItem))
                return (0);
            if (!(o2 is ListViewItem))
                return (0);

            ListViewItem lvi1 = (ListViewItem)o2;
            string str1 = lvi1.SubItems[ByColumn].Text;
            object tag1 = lvi1.SubItems[ByColumn].Tag;
            ListViewItem lvi2 = (ListViewItem)o1;
            string str2 = lvi2.SubItems[ByColumn].Text;
            object tag2 = lvi2.SubItems[ByColumn].Tag;

            int result;

            if (tag1 != null && tag2 != null)
            {
                if (tag1 is DateTime && tag2 is DateTime)
                {
                    DateTime date1 = (DateTime)tag1;
                    DateTime date2 = (DateTime)tag2;
                    if (lvi1.ListView.Sorting == SortOrder.Ascending)
                        result = date1.CompareTo(date2);
                    else
                        result = date2.CompareTo(date1);
                    LastSort = ByColumn;
                    return result;
                }
                else if (tag1 is int && tag2 is int)
                {
                    int d1 = (int)tag1;
                    int d2 = (int)tag2;
                    if (lvi1.ListView.Sorting == SortOrder.Ascending)
                        result = d1.CompareTo(d2);
                    else
                        result = d2.CompareTo(d1);
                    LastSort = ByColumn;
                    return result;
                }
                else if (tag1 is double && tag2 is double)
                {
                    double d1 = (double)tag1;
                    double d2 = (double)tag2;
                    if (lvi1.ListView.Sorting == SortOrder.Ascending)
                        result = d1.CompareTo(d2);
                    else
                        result = d2.CompareTo(d1);
                    LastSort = ByColumn;
                    return result;
                }
            }

            if (lvi1.ListView.Sorting == SortOrder.Ascending)
                result = String.Compare(str1, str2);
            else
                result = String.Compare(str2, str1);

            LastSort = ByColumn;

            return (result);
        }


        public int ByColumn
        {
            get { return Column; }
            set { Column = value; }
        }

        public int LastSort
        {
            get { return LastColumn; }
            set { LastColumn = value; }
        }

    }   

}
