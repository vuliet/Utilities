using System.Globalization;
using System.Reflection;

namespace Utilities.Utilities
{
    public class CompareUtils : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var compare = CompareInfo.GetCompareInfo("en-US");
            return compare.Compare(x, y, CompareOptions.Ordinal);
        }
    }

    public class CompareV2Utils
    {
        public static List<string> CompareObjects<T>(T obj1, T obj2)
        {
            List<string> differences = new List<string>();

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value1 = property.GetValue(obj1);
                object value2 = property.GetValue(obj2);

                if (property.GetValue(value2) is null)
                    continue;

                if (!Equals(value1, value2))
                    differences.Add(property.Name);
            }

            return differences;
        }

        public static List<string> CompareAndUpdateObjects<T>(T obj1, T obj2)
        {
            List<string> differences = new List<string>();

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value1 = property.GetValue(obj1);
                object value2 = property.GetValue(obj2);

                if (property.GetValue(value2) is null)
                    continue;

                if (!Equals(value1, value2))
                {
                    differences.Add(property.Name);
                    property.SetValue(obj1, value2);
                }
            }

            return differences;
        }
    }
}
