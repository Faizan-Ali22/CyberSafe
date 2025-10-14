using System.Security.Cryptography;
using System.Text;

namespace DA_Assets.Extensions
{
    public static class CryptoExtensions
    {
        public static string CreateShortGuid(this string value, out string result)
        {
            if (string.IsNullOrWhiteSpace(value))
                value = System.Guid.NewGuid().ToString().Split('-')[0];

            result = value;
            return value;
        }

        public static byte[] GetSHA1Hash(this string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] result = sha1.ComputeHash(bytes);
                return result;
            }
        }

        public static string ToHEX(this byte[] bytes)
        {
            StringBuilder buffer = new StringBuilder(bytes.Length * 2);
            foreach (var byt in bytes)
            {
                buffer.Append(byt.ToString("X2"));
            }

            return buffer.ToString();
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}
