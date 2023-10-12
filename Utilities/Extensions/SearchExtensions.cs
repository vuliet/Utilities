using System.Globalization;
using System.Text;

namespace Utilities.Extensions
{
    public static class SearchExtensions
    {
        public static string ToSearchString(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = text.ToLower();
            text = text.Replace(" ", "").ToLower();
            text = text.Replace("đ", "d").ToLower();

            var special = Constants.SpecialCharacters;

            text = special.Aggregate(text, (current, sp) => current.Replace(sp, ""));
            text = text.Normalize(NormalizationForm.FormD);
            var chars = text
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        public static string ToSlug(this string text)
        {
            var specialCharacters = new[]
{
                "`", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "_", "=", "+",
                "{", "}", "[", "]", ";", ":", "'", "\"", "<", ",", ">", ".", "?", "/", "\\", "|"
            };

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = text.ToLower();
            text = text.Replace(" ", "-").ToLower();
            text = text.Replace("đ", "d").ToLower();

            var special = Constants.SpecialCharacters;

            text = special.Aggregate(text, (current, sp) => current.Replace(sp, ""));
            text = text.Normalize(NormalizationForm.FormD);
            var chars = text
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
}
