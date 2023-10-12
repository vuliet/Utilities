namespace Utilities.Utilities
{
    public static class NumberUtils
    {
        public static string ConvertPriceToBc(double price, int numberOfDecimal = 0)
            => ((decimal)(price * Math.Pow(10, 18))).ToString($"F{numberOfDecimal}");

        /// <summary>
        /// convert to số dài
        /// </summary>
        public static string ConvertPriceToBc(decimal price, int numberOfDecimal = 0)
            => ((decimal)((double)price * Math.Pow(10, 18))).ToString($"F{numberOfDecimal}");

        /// <summary>
        /// convert to số ngắn
        /// </summary>
        public static decimal ConvertToSystemPrice(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            if (double.TryParse(value, out var price))
                return (decimal)(price / Math.Pow(10, 18));

            return 0;
        }
    }
}
