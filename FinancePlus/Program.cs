using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FinancePlus.PersistentLayer;
using FinancePlus.Storage;

namespace FinancePlus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
         
            Database.load();

            //DataReader.read();

            Application.Run(new MonthForm());
            //Application.Run(new ReportForm());

            //foreach (KeyValuePair<DateTime, Month> pair in Database.months)
            //{
            //    ReportWriter.write(pair.Value, "C:\\Users\\Itay\\Desktop\\דוחות אשראי\\report.xml");
            //    break;
            //}

        }
    }
}
