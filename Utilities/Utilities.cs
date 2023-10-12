using System.Collections.Concurrent;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

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
                            throw new Exception($"Sorry, too many attempts. Please try again in {remainSeconds} seconds.");
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
            catch (Exception)
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

        #region CheckSum
        public static bool IsValidDataUseHmacSHA256(object data, string currentSignature, string checksumKey)
        {
            var sortData = SortUtils.SortObjDataByAlphabet(data);
            var stringifyData = string.Join("&", sortData.Select(kv => $"{kv.Key}={kv.Value}"));

            using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringifyData));
            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hash == currentSignature;
        }

        #endregion
    }

    public static class StringUtils
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
    }

    public static class RandomUtils
    {
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
}
