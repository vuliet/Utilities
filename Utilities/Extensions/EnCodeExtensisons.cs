using System.Security.Cryptography;
using System.Text;
using Utilities.Constants;

namespace Utilities.Extensions
{
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
            var keyBytes = password.GetBytes(ConstantHelpers.KeyEncodeSize / 8);
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
            var keyBytes = password.GetBytes(ConstantHelpers.KeyEncodeSize / 8);
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
}
