using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FinancePlus.PersistentLayer
{
    static class Logger
    {
        public static void log(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}
