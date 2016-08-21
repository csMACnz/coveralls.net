using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Beefeater;

namespace csmacnz.Coveralls
{
    public class Crypto
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static NotNull<string> CalculateMD5Digest(string data)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(data);
            byte[] hash = md5.ComputeHash(inputBytes);

            return hash.Select(b => b.ToString("X2")).Aggregate((current, next)=>current+next);
        }
    }
}