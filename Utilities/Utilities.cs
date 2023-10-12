using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Drawing;
using System.Globalization;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class DataTimeUtils
    {
        public static long NowMilis()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static string NewGuidStr()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static long NowDateMilis()
        {
            var now = NowMilis();

            return now - now % (long)TimeSpan.FromDays(1).TotalMilliseconds;
        }

        public static long YesterdayDateMilis()
        {
            return NowDateMilis() - (long)TimeSpan.FromHours(24).TotalMilliseconds;
        }

        public static DateTime DateTimeFromMilis(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddMilliseconds(unixTimeStamp).ToUniversalTime();
        }

        public static DateTime DateTimeFromSeconds(long seconds)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddSeconds(seconds).ToUniversalTime();
        }

        public static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        public static DateTime NowDate()
        {
            return DateTime.UtcNow;
        }

        public static long DateTimeToUnixInMilliSeconds(DateTime dateTime)
        {
            return (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static long DateTimeToUnixSeconds(DateTime dateTime)
        {
            return (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime DateTimeToRemoveMinutes(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }

        public static bool TimeBetween(long requestTime, long? min, long? max)
        {
            if (min is null || max is null)
                return false;
            return requestTime > min && requestTime < max;
        }

        public static bool TimeBetween(long? min, long? max)
        {
            long current = NowMilis();
            return TimeBetween(current, min, max);
        }

        public static bool TimeBetween(DateTime requestTime, DateTime? min, DateTime? max)
        {
            if (min is null || max is null)
                return false;
            return requestTime > min && requestTime < max;
        }

        public static bool TimeBetween(DateTime? min, DateTime? max)
        {
            DateTime current = DateTime.UtcNow;
            return TimeBetween(current, min, max);
        }
    }

    public static class EnviromentUtils
    {
        public static string GetConfig(string code)
        {
            IConfigurationRoot configuration = ConfigCollection.Instance.GetConfiguration();
            var value = configuration[code];
            return value ?? string.Empty;
        }
        public static string GetConfig(IConfiguration configuration, string code)
        {
            var value = configuration[code];
            return value ?? string.Empty;
        }
        public static string GetEnv()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        }
        public static bool IsDevelopment() => GetEnv().Equals("Development");
        public static bool IsLocal() => GetEnv().Equals("Local");
        public static bool IsProduction() => GetEnv().Equals("Production");
    }

    public static class ValidationUtils
    {
        private static readonly Dictionary<string, ConcurrentDictionary<string, long>> ProtectActionData = new();

        /// <summary>
        /// ProtectAction only use for single host application
        /// </summary>
        public static void ProtectAction(string action, string entity, int circleInSeconds = 60)
        {
            var now = DataTimeUtils.NowMilis();
            lock (ProtectActionData)
            {
                if (ProtectActionData.TryGetValue(action, out var requests))
                {
                    if (requests.TryGetValue(entity, out var last))
                    {
                        if (last > now - TimeSpan.FromSeconds(circleInSeconds).TotalMilliseconds)
                        {
                            var remainSeconds = circleInSeconds - (now - last) / 1000;
                            throw new AppException($"Sorry, too many attempts. Please try again in {remainSeconds} seconds.");
                        }

                        requests[entity] = now;
                    }
                    else
                    {
                        requests.TryAdd(entity, now);
                    }
                }
                else
                {
                    var newRequests = new ConcurrentDictionary<string, long>();
                    newRequests.TryAdd(entity, now);
                    ProtectActionData.TryAdd(action, newRequests);
                }
            }
        }

        /// <summary>
        /// ProtectAction only use for single host application but return
        /// </summary>
        public static bool CheckHasProtectAction(string action, string entity, int circleInSeconds = 60)
        {
            var now = DataTimeUtils.NowMilis();
            lock (ProtectActionData)
            {
                if (ProtectActionData.TryGetValue(action, out var requests))
                {
                    if (requests.TryGetValue(entity, out var last))
                    {
                        if (last > now - TimeSpan.FromSeconds(circleInSeconds).TotalMilliseconds)
                            return true;

                        requests[entity] = now;
                    }
                    else
                    {
                        requests.TryAdd(entity, now);
                    }
                }
                else
                {
                    var newRequests = new ConcurrentDictionary<string, long>();
                    newRequests.TryAdd(entity, now);
                    ProtectActionData.TryAdd(action, newRequests);
                }
            }

            return false;
        }

        public static bool IsValidEmail(string value)
        {
            if (IsUnicode(value))
                return false;

            try
            {
                var _ = new MailAddress(value);
                return true;
            }
            catch (AppException)
            {
                return false;
            }
        }

        public static bool IsPasswordValid(string password)
        {
            // Define password validation rules
            var requiredLength = 6;
            var maxLength = 128;

            // Check each rule
            if (string.IsNullOrEmpty(password)
                || password.Length < requiredLength
                || password.Length > maxLength)
            {
                return false;
            }

            return true;
        }

        public static bool IsUnicode(string input)
        {
            var asciiBytesCount = Encoding.ASCII.GetByteCount(input);
            var unicodBytesCount = Encoding.UTF8.GetByteCount(input);
            return asciiBytesCount != unicodBytesCount;
        }

        public static long GenerateNumberByMilisecond()
        {
            long _currentId = DataTimeUtils.NowMilis();
            Interlocked.Increment(ref _currentId);
            return _currentId;
        }

        public static bool IsImage(byte[] fileBytes)
        {
            var headers = new List<byte[]>
            {
                Encoding.ASCII.GetBytes("BM"),      // BMP
                Encoding.ASCII.GetBytes("GIF"),     // GIF
                new byte[] { 137, 80, 78, 71 },     // PNG
                new byte[] { 73, 73, 42 },          // TIFF
                new byte[] { 77, 77, 42 },          // TIFF
                new byte[] { 255, 216, 255, 224 },  // JPEG
                new byte[] { 255, 216, 255, 225 }   // JPEG CANON
            };

            return headers.Any(x => x.SequenceEqual(fileBytes.Take(x.Length)));
        }

        public static bool IsValidJsonObject(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
                return false;

            var value = stringValue.Trim();

            if (value.StartsWith("{") && value.EndsWith("}"))
            {
                try
                {
                    JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }

        public static bool IsValidJsonArray(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                try
                {
                    JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }

        #region CheckSum
        public static string GetRequestRaw(SortedList<string, string> requestData)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in requestData)
            {
                var value = kv.Value is null ? string.Empty : kv.Value;
                data.Append(kv.Key + "=" + value + "&");
            }

            //remove last '&'
            if (data.Length > 0)
                data.Remove(data.Length - 1, 1);

            return data.ToString();
        }

        public static bool IsValidDataUseHmacSHA256(object data, string currentSignature, string checksumKey)
        {
            var sortData = SortUtils.SortObjDataByAlphabet(data);
            var stringifyData = string.Join("&", sortData.Select(kv => $"{kv.Key}={kv.Value}"));

            return VerifyHmacSha256Hex(stringifyData, currentSignature, checksumKey);
        }

        public static bool VerifyHmacSha256Hex(string data, string hexSignature, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            string computedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return computedSignature == hexSignature;
        }

        #endregion
    }

    public static class RandomUtils
    {
        public static string RandomString(int length = 10)
        {
            var rand = new Random();
            const string pool = Constants.PoolRandomString;
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var c = pool[rand.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }

        public static string RandomStringNumber(int length = 10)
        {
            var rand = new Random();
            const string pool = Constants.PoolRandomStringNumber;
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var c = pool[rand.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }

        public static T? RandomFromListAny<T>(List<T> listNumber)
        {
            if (!listNumber.Any())
                return default;

            var index = new Random().Next(listNumber.Count);

            return listNumber[index];
        }

        public static decimal RandomFrom(decimal min, decimal max, int decimals)
        {
            var power = (decimal)Math.Pow(10, decimals);

            var intMin = (int)(min * power);
            var intMax = (int)(max * power);

            var result = (decimal)new Random().Next(intMin, intMax);

            return result / power;
        }

        public static string GenerateTransactionNo(string prefix1)
        {
            string numbersSalt = DateTime.Now.ToString("ddMMyy");
            return prefix1 + numbersSalt;
        }

        public static string GenerateTransactionNo()
        {
            var random = new Random();

            var store = new List<String>();
            string numbersSalt = DateTime.Now.ToString("HHmmssddMMyy");
            string numbersToUse = "0123456789";
            string numbersToUseEx = "123456789";

            MatchEvaluator RandomNumber = delegate (Match m)
            {
                return numbersToUse[random.Next(numbersToUse.Length)].ToString();
            };

            MatchEvaluator RandomNumberEx = delegate (Match m)
            {
                return numbersToUseEx[random.Next(numbersToUseEx.Length)].ToString();
            };

            MatchEvaluator RandomSalt = delegate (Match m)
            {
                var x = ReRandom(store, random.Next(numbersSalt.Length), numbersSalt);

                return numbersSalt[x].ToString();
            };

            return Regex.Replace("X", "X", RandomNumberEx)
                + Regex.Replace("XXX", "X", RandomNumber)
                + Regex.Replace("XXXXXXXXXXXX", "X", RandomSalt);
        }

        public static int ReRandom(List<string> store, int x, string numbersSalt)
        {
            var random = new Random();

            var e = (from s in store
                     where s == x.ToString()
                     select s).FirstOrDefault();
            if (e == null)
            {
                store.Add(x.ToString());
                return x;
            }
            else
            {
                var x1 = random.Next(numbersSalt.Length);
                return ReRandom(store, x1, numbersSalt);
            }
        }

        public static string GenerateNewRandom()
        {
            Random generator = new Random();
            string result = generator.Next(0, 1000000).ToString("D6");

            if (result.Distinct().Count() == 1)
                result = GenerateNewRandom();

            return result;
        }

        public static string PassowrdRandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        public static string PasswordCreateSalt512_UNUSED_REPLACE_WITH_ACCOUNTHELPER()
        {
            var message = PassowrdRandomString(512, false);
            return BitConverter.ToString(SHA512.HashData(Encoding.ASCII.GetBytes(message))).Replace("-", "");
        }

        public static string RandomPassword(
            int numericLength,
            int lCaseLength,
            int uCaseLength,
            int specialLength)
        {
            Random random = new Random();

            //char set random
            string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
            string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
            string PASSWORD_CHARS_NUMERIC = "1234567890";
            string PASSWORD_CHARS_SPECIAL = "!@#$%^&*()-+<>?";
            if ((numericLength + lCaseLength + uCaseLength + specialLength) < 8)
                return string.Empty;
            else
            {
                //get char
                var strNumeric = new string(Enumerable.Repeat(PASSWORD_CHARS_NUMERIC, numericLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var strUper = new string(Enumerable.Repeat(PASSWORD_CHARS_UCASE, uCaseLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var strSpecial = new string(Enumerable.Repeat(PASSWORD_CHARS_SPECIAL, specialLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var strLower = new string(Enumerable.Repeat(PASSWORD_CHARS_LCASE, lCaseLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                //result : ký tự số + chữ hoa + chữ thường + các ký tự đặc biệt > 8
                var strResult = strNumeric + strUper + strSpecial + strLower;
                return strResult;
            }
        }
    }

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

    public static class EnumUtils
    {
        public static bool EnumContainsKey<T>(string value) where T : Enum
        {
            var props = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(x => x.ToString())
                .ToList();

            return props.Contains(value);
        }
    }

    public static class ReadFileUtils
    {
        public static Task<string> ReadFileFromAssemblyAsync(string projectName, string pathName, Type type)
        {
            var assembly = Assembly.GetAssembly(type);

            if (assembly is null)
                return Task.FromResult(string.Empty);

            var resourceName = $"{projectName}.{pathName}";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEndAsync();
        }
    }

    public static class SortUtils
    {
        public static SortedDictionary<string, object> SortObjDataByAlphabet(object obj)
        {
            var properties = obj.GetType().GetProperties();
            var sortedObject = new SortedDictionary<string, object>();

            foreach (var property in properties)
                sortedObject[property.Name] = property.GetValue(obj) ?? "";

            return sortedObject;
        }
    }

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

    public static class ImageUtils
    {
        public static Image ResizeImage(Image imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }
    }
}
