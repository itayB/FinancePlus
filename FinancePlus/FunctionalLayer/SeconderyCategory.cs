using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    class SeconderyCategory
    {
        // Fields
        public string name;
        static Dictionary<string, SeconderyCategory> categories = new Dictionary<string, SeconderyCategory>();
    }
}
