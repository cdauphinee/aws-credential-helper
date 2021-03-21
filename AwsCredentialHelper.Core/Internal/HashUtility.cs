using System.Text;
using System.Security.Cryptography;

namespace AwsCredentialHelper.Core.Internal
{
    internal static class HashUtility
    {
        public static byte[] CalculateSHA256(string message, byte[] key = null)
        {
            using HashAlgorithm hashAlgorithm = (key == null) 
                ? SHA256.Create()
                : new HMACSHA256(key);

            var bytes = Encoding.UTF8.GetBytes(message);
            return hashAlgorithm.ComputeHash(bytes);
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
