using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Beefeater;

namespace csmacnz.Coveralls
{
    public class Crypto
    {
        public static NotNull<string> CalculateMd5Digest(string data)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(data);
            var hash = md5.ComputeHash(inputBytes);

            return hash.Select(b => b.ToString("X2")).Aggregate((current, next) => current + next);
        }
    }
}
