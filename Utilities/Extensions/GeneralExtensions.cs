using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class GeneralExtensions
    {
        public static string GetDescription<T>(this T source)
        {
            if (source is null)
                return "";

            var fi = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;

            return source.ToString();
        }

        public static string FirstCharToUpper(this string input)
            => input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        public static bool IsNumber(this string strNumber)
        {
            if (string.IsNullOrEmpty(strNumber))
                return false;

            string strExp = "[0-9]{" + strNumber.Length + "}";
            return Regex.IsMatch(strNumber, strExp);
        }

        public static bool IsGuid(this string inputString)
        {
            try
            {
                var guid = new Guid(inputString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsBase64(this string base64String)
        {
            if (base64String == null || base64String.Length == 0 || base64String.Length % 4 != 0
               || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception)
            {
                // Handle the exception
            }
            return false;
        }

        public static List<string> GetConstants(this Type type)
        {
            var properties = type.GetFields();

            return properties.Select(x => x.Name).ToList();
        }

        public static List<string> ToArrayWithDelimiter(this string text, string delimiter)
        {
            if (string.IsNullOrEmpty(delimiter))
                return new List<string>();

            return text
                ?.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                .ToList() ?? new List<string>();
        }

        public static string ToStringWithDelimiter(this List<string> list, string delimiter)
        {
            if (string.IsNullOrEmpty(delimiter))
                return string.Empty;

            return list != null
                ? string.Join(delimiter, list)
                : string.Empty;
        }

        public static string ReplaceLineBreaks(this string content)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            return Regex.Replace(content, @"\r\n?|\n", "<br />");
        }

        public static bool EqualsIgnoreCase(this string s, string o)
        {
            return string.Equals(s, o, StringComparison.OrdinalIgnoreCase);
        }
    }
}
