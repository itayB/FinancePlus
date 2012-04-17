using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class PrimaryCategory
    {
        // Fields
        public string name;
        Dictionary<string, SeconderyCategory> categories;
        //Dictionary<string, SeconderyCategory> categories = new Dictionary<string, SeconderyCategory>();

        // Constructor
        public PrimaryCategory(string name)
        {
            this.name = name;
            this.categories = new Dictionary<string, SeconderyCategory>();
        }

        public void addSeconderyCategory(SeconderyCategory sc)
        {
            //categories.Add(sc);
        }
    }
}
