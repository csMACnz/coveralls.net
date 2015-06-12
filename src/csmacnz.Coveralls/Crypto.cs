using System.Linq;
using System.Security.Cryptography;

namespace csmacnz.Coveralls
{
    public class Crypto
    {
        public static string CalculateMD5Digest(string data)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(data);
            byte[] hash = md5.ComputeHash(inputBytes);

            return hash.Select(b => b.ToString("X2")).Aggregate((current, next)=>current+next);
        }
    }
}