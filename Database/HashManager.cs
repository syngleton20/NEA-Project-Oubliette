using System.Security.Cryptography;
using System.Text;
using System;

namespace NEA_Project_Oubliette.Database.Security
{
    internal static class HashManager
    {
        public static string HashPassword(string password, int maxLength = 20)
        {
            using(MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utfEncoding = new UTF8Encoding();
                byte[] bytes = md5.ComputeHash(utfEncoding.GetBytes(password));
                string hashedPassword = Convert.ToBase64String(bytes);

                if(hashedPassword.Length > maxLength) return hashedPassword.Substring(0, maxLength);
                else return hashedPassword;
            }
        }
    }
}