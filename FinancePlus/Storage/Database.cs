using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.PersistentLayer;
using System.Xml;
using System.IO;

namespace FinancePlus.Storage
{
    static class Database
    {
        /************************************ Constant Strings ************************************/
        public const string NIS_SIGN = " ש\"ח";
        public const string SUM = "סה\"כ";
        public const string DEFAULT_PRIMARY_CATEGORY = "שונות";
        public const string DEFAULT_SECONDARY_CATEGORY = "אחר";
        public const string INCOME_STRING = "הכנסות";
        public const string EXPENSES_STRING = "הוצאות";
        public const string OPEN_REPORT_STRING = "פתח דו\"ח ";
        public const string ISRACARD_STRING = "ישראכרט";
        public const string CAL_STRING = "כ.א.ל";
        public const string POALIM_STRING = "בנק הפועלים";
        public const string ACCOUNT_NUMBER = "מספר חשבון";
        public const string CARD_NUMBER = "כרטיס מספר";
        public const string WARNING_STRING = "אזהרה";
        public const string ACCESS_FAILED_WARNING_MESSAGE = "הקובץ בשימוש על ידי תהליך אחר, לא ניתן לגשת לקובץ";
        /************************************ End of Constant Strings ************************************/


        /************************************ Main Data Structures ************************************/
        public static Dictionary<DateTime, Month> months = new Dictionary<DateTime, Month>();
        public static HashSet<Category> categories = new HashSet<Category>();
        public static Dictionary<string, Category> categoriesMap = new Dictionary<string, Category>();
        public static HashSet<string> filterMap = new HashSet<string>();
        /************************************ End of Main Data Structures ************************************/


        public static string dataFolder = "C:\\Users\\Itay\\Desktop\\דוחות אשראי\\";
        public static string dataTransactionsFilename = "TransactionsData.xml";
        public static string dataTransactionsFilenameBackup = "TransactionsDataBackup.xml";
        public static string dataCategoriesMapFilename = "CategoriesMapData.xml";
        public static string dataFilterMapFilename = "FilterMapData.xml";


        public static DateTime stringDateToDateTime(string stringDate)
        {
            DateTime date = new DateTime(1, 1, 1);

            try
            {
                // Specify exactly how to interpret the string.
                IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);

                date = DateTime.Parse(stringDate, culture, System.Globalization.DateTimeStyles.AssumeLocal);
                //Console.WriteLine("Year: {0}, Month: {1}, Day {2}", date.Year, date.Month, date.Day);
            }
            catch
            {
            }
            return date;
        }

        public static double getTotalBillingPrice(DateTime date, Category category)
        {
            double total = 0;

            Month month = getMonth(date);
            foreach (Transaction e in month.getExpenses())
                if (Database.getCategory(e).Equals(category))
                    total += e.billingPrice;

            return total;
        }

        public static Category getCategory(string primary, string secondary)
        {
            foreach (Category c in categories)
                if (c.primary.Equals(primary) && c.secondary.Equals(secondary))
                    return c;

            Category cat = new Category(primary, secondary);
            Database.categories.Add(cat);
            return cat;
        }

        public static Category getCategory(Transaction e)
        {
            string name = e.businessName;
            Category category;

            if (e.category != null)
                return e.category;

            if (!categoriesMap.TryGetValue(name, out category))
            {

                return getDefaultCategory();
                //category = new Category();
                // i should add it to category list..
            }
            return category;
        }

        private static Category getDefaultCategory()
        {
            foreach (Category c in categories)
                if (c.CompareTo(new Category()) == 0)
                    return c;

            Category newC = new Category();
            categories.Add(newC);
            return newC;
        }

        public static Month getMonth(DateTime date)
        {
            DateTime monthDate = new DateTime(date.Year,date.Month,1);
            Month month;

            if (!months.TryGetValue(monthDate, out month))
            {
                month = new Month();
                months.Add(monthDate, month);
            }

            return month;
        }


        internal static void updateAllTransactionFilter(Transaction exp)
        {
            filterMap.Add(exp.businessName);
        }

        internal static void updateAllExpensesCategory(Transaction t, Category selectedCategory)
        {
            categoriesMap.Add(t.businessName, selectedCategory);
        }

        internal static void updateExpenseCategory(Transaction t, Category selectedCategory)
        {
            t.category = selectedCategory;
        }

        public static string removeScripts(string content)
        {
            string result = "";

            string[] parts = content.Split(new string[] { "<script", "</script>", "<SCRIPT", "</SCRIPT>" }, StringSplitOptions.None);
            for (int i = 0; i < parts.Length; i=i+2)
            {
                result += parts[i]; 
            }

            return result;
        }


        public static void load()
        {
            loadTransactions();
            loadCategoriesMap();
            loadFilterMap();
        }

        public static void save()
        {
            try
            {
                // will work only if the file exist (fails in first time - for example)
                System.IO.File.Copy(dataFolder + dataTransactionsFilename, dataFolder + dataTransactionsFilenameBackup, true);
            }
            catch { }
            saveTransactionsToXML();
            saveCategoriesMapToXML();
            saveFilterMapToXML();
        }

        public static void saveTransactionsToXML()
        {
           XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
           xmlWriterSettings.NewLineOnAttributes = true;
           xmlWriterSettings.Indent = true;

           using (XmlWriter writer = XmlWriter.Create(dataFolder + dataTransactionsFilename, xmlWriterSettings))
           {
                writer.WriteStartDocument();
                writer.WriteStartElement("Months");

                foreach (KeyValuePair<DateTime, Month> pair in months)
                {
                    writer.WriteStartElement("Month");
                    writer.WriteElementString("Date", ((DateTime)pair.Key).ToShortDateString());
                    writer.WriteStartElement("Transactions");
                    foreach (Transaction e in ((Month)pair.Value).getTransactions())
                    {
                        writer.WriteStartElement("Transaction");
                        writer.WriteElementString("Date", e.date.ToShortDateString());
                        writer.WriteElementString("BusinessName", e.businessName);
                        writer.WriteElementString("TransactionPrice", e.transactionPrice.ToString());
                        writer.WriteElementString("BillingPrice", e.billingPrice.ToString());
                        //writer.WriteElementString("PaymentID", e.creditCardNumber);
                        if (e.category != null)
                        {
                            writer.WriteElementString("PrimaryCategory", e.category.primary);
                            writer.WriteElementString("SecondaryCategory", e.category.secondary);
                        }
                        if (e.paymentInfo != null)
                        {
                            writer.WriteStartElement("PaymentInfo");
                            writer.WriteElementString("Id", e.paymentInfo.getPaymentId());
                            writer.WriteElementString("Name", e.paymentInfo.getPaymentType().ToString());
                            writer.WriteElementString("StartDate", e.paymentInfo.getStartDate().ToShortDateString());
                            writer.WriteElementString("EndDate", e.paymentInfo.getEndDate().ToShortDateString());
                            writer.WriteEndElement();
                        }
                        writer.WriteElementString("ReceiptID", e.receiptId.ToString());
                        writer.WriteElementString("Type", e.type.ToString());
                        if (e.details != null && !e.details.Equals(""))
                            writer.WriteElementString("Details", e.details);
                        if (e.filter)
                            writer.WriteElementString("Filter", null);
                        if (e.comment != null && !e.comment.Equals(""))
                            writer.WriteElementString("Comment", e.comment);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
           }
        }

        public static void saveCategoriesMapToXML()
        {
           XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
           xmlWriterSettings.NewLineOnAttributes = true;
           xmlWriterSettings.Indent = true;

           using (XmlWriter writer = XmlWriter.Create(dataFolder + dataCategoriesMapFilename, xmlWriterSettings))
            //using (XmlWriter writer = XmlWriter.Create(new StreamWriter(filename, false, Encoding.UTF8)))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("TransactionsMap");

                foreach (KeyValuePair<string, Category> pair in categoriesMap)
                {
                    writer.WriteStartElement("TransactionMap");
                    writer.WriteElementString("BusinessName", pair.Key);
                    writer.WriteElementString("PrimaryCategory", pair.Value.primary);
                    writer.WriteElementString("SecondaryCategory", pair.Value.secondary);
                    writer.WriteEndElement();
                    //writer.WriteString("\n");
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public static void saveFilterMapToXML()
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(dataFolder + dataFilterMapFilename, xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("FiltersMap");

                foreach (string businessName in filterMap)
                {
                    writer.WriteStartElement("FilterMap");
                    writer.WriteElementString("BusinessName", businessName);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public static void loadTransactions()
        {
            if (!File.Exists(dataFolder + dataTransactionsFilename))
                return;

            StreamReader sr = new StreamReader(dataFolder + dataTransactionsFilename, Encoding.GetEncoding("ISO-8859-8"));
            using (XmlReader reader = XmlReader.Create(sr))
            {
                Month month = null;
                DateTime date = new DateTime();
                Transaction e = null;

                while (reader.Read())
                {
                    // Get element name and switch on it.
                    switch (reader.Name)
                    {
                        case "Month":
                            if (reader.IsStartElement())
                            {
                                month = new Month();
                                reader.ReadToFollowing("Date");
                                if (reader.Read())
                                    date = stringDateToDateTime(reader.Value.Trim());
                            }
                            else
                            {
                                Database.months.Add(date, month);
                            }
                            break;
                        case "Transaction":
                            if (reader.IsStartElement())
                            {
                                XmlReader inner = reader.ReadSubtree();

                                e = new Transaction();
                                while (inner.Read())
                                {
                                    if (inner.IsStartElement())
                                    {
                                        switch (inner.Name)
                                        {
                                            case "Date":
                                                if (inner.Read())
                                                    e.date = stringDateToDateTime(inner.Value.Trim());
                                                break;
                                            case "BusinessName":
                                                if (inner.Read())
                                                    e.businessName = inner.Value.Trim();
                                                break;
                                            case "TransactionPrice":
                                                if (inner.Read())
                                                    e.transactionPrice = Double.Parse(inner.Value.Trim());
                                                break;
                                            case "BillingPrice":
                                                if (inner.Read())
                                                    e.billingPrice = Double.Parse(inner.Value.Trim());
                                                break;
                                            //case "PaymentID":
                                            //    if (inner.Read())
                                            //        e.creditCardNumber = inner.Value.Trim();
                                            //    break;
                                            case "PrimaryCategory":
                                                string primary = null;
                                                string secondary = null;
                                                if (inner.Read())
                                                    primary = inner.Value.Trim();
                                                if (reader.ReadToFollowing("SecondaryCategory"))
                                                    if (inner.Read())
                                                    {
                                                        secondary = inner.Value.Trim();
                                                        e.category = Database.getCategory(primary, secondary);
                                                    }
                                                break;
                                            case "PaymentInfo":
                                                string paymentId = null;
                                                string paymentTypeString = null;
                                                string startDateString = null;
                                                string endDateString = null;

                                                while (inner.Read())
                                                {
                                                    if (inner.IsStartElement())
                                                    {
                                                        switch (inner.Name)
                                                        {
                                                            case "Id":
                                                                if (inner.Read())
                                                                    paymentId = inner.Value.Trim();
                                                                break;
                                                            case "Name":
                                                                if (inner.Read())
                                                                    paymentTypeString = inner.Value.Trim();
                                                                break;
                                                            case "StartDate":
                                                                if (inner.Read())
                                                                    startDateString = inner.Value.Trim();
                                                                break;
                                                            case "EndDate":
                                                                if (inner.Read())
                                                                    endDateString = inner.Value.Trim();
                                                                break;
                                                        }
                                                    }
                                                    if (inner.Name.Equals("PaymentInfo"))
                                                    {
                                                        DateTime startDate = stringDateToDateTime(startDateString);
                                                        DateTime endDate = stringDateToDateTime(endDateString);
                                                        PaymentType paymentType = PaymentInfo.parsePaymentTypeString(paymentTypeString);
                                                        e.paymentInfo = month.getPaymentInfo(paymentId,paymentType,startDate,endDate);
                                                        break;
                                                    }
                                                }
                                                break;
                                            case "ReceiptID":
                                                if (inner.Read())
                                                    e.receiptId = Convert.ToInt32(inner.Value.Trim());
                                                break;
                                            case "Type":
                                                if (inner.Read())
                                                    e.setType(inner.Value.Trim());
                                                break;
                                            case "Details":
                                                if (inner.Read())
                                                    e.details = inner.Value.Trim();
                                                break;
                                            case "Filter":
                                                e.filter = true;
                                                break;
                                            case "Comment":
                                                if (inner.Read())
                                                    e.comment = inner.Value.Trim();
                                                break;
                                        }
                                    }
                                }
                            }
                            reader.Skip();

                            if (e.paymentInfo == null)
                                Console.Write(e.businessName + "\n");

                            month.addTransaction(e);
                            break;

                    }  

                }
            }
            sr.Close();
            sr.Dispose();
        }


        public static void loadCategoriesMap()
        {
            if (!File.Exists(dataFolder + dataCategoriesMapFilename))
                return;

            using (XmlReader reader = XmlReader.Create(new StreamReader(dataFolder + dataCategoriesMapFilename, Encoding.GetEncoding("ISO-8859-8"))))
            {
                string businessName = null;
                string primaryCategory = null;
                string secondaryCategory = null;

                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.Name)
                        {
                            //case "Expense":
                            //    businessName = 
                            //    break;
                            case "BusinessName":
                                if (reader.Read())
                                    businessName = reader.Value.Trim();
                                break;
                            case "PrimaryCategory":
                                if (reader.Read())
                                    primaryCategory = reader.Value.Trim();
                                break;
                            case "SecondaryCategory":
                                if (reader.Read())
                                {
                                    secondaryCategory = reader.Value.Trim();
                                    Category c = getCategory(primaryCategory, secondaryCategory);
                                    Database.categoriesMap.Add(businessName, c);
                                }
                                break;
                        }
                    }

                }
            }

        }


        public static void loadFilterMap()
        {
            if (!File.Exists(dataFolder + dataFilterMapFilename))
                return;

            using (XmlReader reader = XmlReader.Create(new StreamReader(dataFolder + dataFilterMapFilename, Encoding.GetEncoding("ISO-8859-8"))))
            {
                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.Name)
                        {
                            case "BusinessName":
                                if (reader.Read())
                                    Database.filterMap.Add(reader.Value.Trim());
                                break;
                        }
                    }
                }
            }
        }
    }
}
