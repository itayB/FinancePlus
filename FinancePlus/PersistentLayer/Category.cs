using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.Storage;

namespace FinancePlus.PersistentLayer
{
    public class Category : IComparable<Category>
    {
        // Fields
        public string primary;
        public string secondary;

        // Constructors
        public Category()
        {
            this.primary = Database.DEFAULT_PRIMARY_CATEGORY;
            this.secondary = Database.DEFAULT_SECONDARY_CATEGORY;
        }

        public Category(string primary, string secondary)
        {
            this.primary = primary;
            this.secondary = secondary;
        }

        public int CompareTo(Category other)
        {
            if (this.primary.CompareTo(other.primary) == 0)
            {
                return this.secondary.CompareTo(other.secondary);
            }

            return this.primary.CompareTo(other.primary);
        }

    }
}
