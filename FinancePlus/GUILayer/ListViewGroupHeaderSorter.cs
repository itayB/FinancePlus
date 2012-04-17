using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FinancePlus.GUILayer
{
    public class ListViewGroupHeaderSorter : IComparer<ListViewGroup>
    {
        private bool _ascending = true;
        public ListViewGroupHeaderSorter(bool ascending)
        {
            _ascending = ascending;
        }

        #region IComparer<ListViewGroup> Members

        public int Compare(ListViewGroup x, ListViewGroup y)
        {
            if (_ascending)
                return string.Compare(((ListViewGroup)x).Header, ((ListViewGroup)y).Header);
            else
                return string.Compare(((ListViewGroup)y).Header, ((ListViewGroup)x).Header);
        }

        #endregion
    }
}
