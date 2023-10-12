using System.Globalization;

namespace Utilities.Utilities
{
    public class PayCompareUtils : IComparer<string>
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
}
