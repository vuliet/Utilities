using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Utilities.Exceptions;

namespace Utilities.Utilities
{
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
}
