using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LogInLogOut.Models;
using System.Security.Cryptography;

namespace LogInLogOut.Extensions
{
    public static class LoginExtensions
    {
        public static string HashPassword(this Login login, string password, byte[] salt)
        {
            Rfc2898DeriveBytes rng = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] passwordHash = rng.GetBytes(20);

            byte[] hashAndSalt = new byte[36];// 36 because hashed password = 20 bytes and salt = 16 bytes
            Array.Copy(salt, hashAndSalt, 16);
            Array.Copy(passwordHash, 0, hashAndSalt, 16, 20);

            return Convert.ToBase64String(hashAndSalt);
        }
    }
}