using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
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
    }

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

    public static class EnCodeExtensisons
    {
        public static string Encrypt(this string plainText, string hashKey)
        {
            var initVector = hashKey.Substring(0, 16);

            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            var initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            var password = new PasswordDeriveBytes(hashKey, null);
            var keyBytes = password.GetBytes(Constants.KeyEncodeSize / 8);
            var symmetricKey = new AesManaged
            {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC
            };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            var cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(this string cipherText, string hashKey)
        {
            var initVector = hashKey.Substring(0, 16);

            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            var initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            var cipherTextBytes = Convert.FromBase64String(cipherText);
            var password = new PasswordDeriveBytes(hashKey, null);
            var keyBytes = password.GetBytes(Constants.KeyEncodeSize / 8);
            var symmetricKey = new AesManaged
            {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC
            };
            var decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];
            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public static string ToSha512Hash(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            using var sha512 = SHA512.Create();
            var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(value));

            var stringBuilder = new StringBuilder();

            foreach (var b in hash)
                stringBuilder.AppendFormat(b.ToString("x2"));

            return stringBuilder.ToString().ToUpper();
        }

        public static string ToSha1Hash(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            using var sha512 = SHA1.Create();
            var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(value));

            var stringBuilder = new StringBuilder();

            foreach (var b in hash)
                stringBuilder.AppendFormat(b.ToString("x2"));

            return stringBuilder.ToString().ToUpper();
        }
    }

    public static class ConvertExtensions
    {
        public static List<int> ConvertStringToListInt(string strIds)
        {
            if (string.IsNullOrEmpty(strIds))
                return new List<int>();

            var lstResult = new List<int>();
            var arrId = strIds.Split(',');

            foreach (var item in arrId)
            {
                if (int.TryParse(item, out int rsid))
                    lstResult.Add(rsid);
            }

            return lstResult;
        }
    }

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
