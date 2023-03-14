using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cryptography
{
    public static class PasswordHashArgorithm
    {
        public static string GetHashedPassword(string plainPassword, string salt)
        {
            var sha512 = SHA512.Create();
            var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes((salt ?? "") + plainPassword));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
