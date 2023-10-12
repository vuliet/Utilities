using Utilities.Exceptions;

namespace Utilities.Extensions
{
    public static class DecimalExtensions
    {
        public static string ConcatPrecision(this decimal value)
        {
            try
            {
                var str = value.ToString("F8");
                var precisions = str.Split('.')[1].Reverse();
                var precision = 8;

                foreach (var c in precisions)
                {
                    if (int.Parse(c.ToString()) > 0)
                        break;

                    precision--;
                }

                return value.ToString($"F{precision}");
            }
            catch (AppException)
            {
                return value.ToString("F8");
            }
        }

        public static decimal TruncateDecimal(this decimal value, uint precision)
        {
            try
            {
                if (precision > 8)
                    precision = 8;

                if (precision == 0)
                    return Math.Truncate(value);

                var scale = (decimal)Math.Pow(10, precision);

                return Math.Truncate(value * scale) / scale;
            }
            catch (AppException)
            {
                return value;
            }
        }
    }
}
