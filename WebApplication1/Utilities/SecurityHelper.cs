using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Utilities
{
    public static class SecurityHelper
    {
        public static byte[] HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
