using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.Constants;

namespace Utilities.Utilities
{
    public static class RandomUtils
    {
        public static string RandomString(int length = 10)
        {
            var rand = new Random();
            const string pool = ConstantHelpers.PoolRandomString;
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
            const string pool = ConstantHelpers.PoolRandomStringNumber;
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

            var store = new List<string>();
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
            if (numericLength + lCaseLength + uCaseLength + specialLength < 8)
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
}
