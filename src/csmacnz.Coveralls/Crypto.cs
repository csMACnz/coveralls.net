using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Beefeater;

namespace csmacnz.Coveralls
{
    public static class Crypto
    {
        public static NotNull<string> CalculateMd5Digest(string data)
        {
#pragma warning disable CA5351 // Do not use insecure cryptographic algorithm MD5.
            var md5 = MD5.Create();
#pragma warning restore CA5351 // Do not use insecure cryptographic algorithm MD5.
            var inputBytes = Encoding.ASCII.GetBytes(data);
            var hash = md5.ComputeHash(inputBytes);

            return hash.Select(b => b.ToString("X2")).Aggregate((current, next) => current + next);
        }
    }
}
