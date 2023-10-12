namespace Utilities.Extensions
{
    public static class SecurityExtensions
    {
        public static string HideInfo(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Length > 10)
                return string.Concat(text.AsSpan(0, 2), "***", text.AsSpan(6, text.Length - 6));

            return "***";
        }
    }
}
