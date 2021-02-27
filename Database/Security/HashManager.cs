using System.Security.Cryptography;
using System.Text;
using System;

namespace NEA_Project_Oubliette.Database.Security
{
    ///<summary>Provides methods for hashing data</summary>
    internal static class HashManager
    {
        ///<summary>Hashes a password so that it cannot be easily viewed in plaintext again</summary>
        public static string HashPassword(string password, int maxLength = 20)
        {
            using(MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utfEncoding = new UTF8Encoding();
                byte[] bytes = md5.ComputeHash(utfEncoding.GetBytes(password)); // Hashes each byte for each character of the password using the MD5 hashing algorithm
                string hashedPassword = Convert.ToBase64String(bytes); // Converts the hashed bytes back into UTF8 encoded text

                // Truncates the hashed password if its length is longer than the maxLength argument
                if(hashedPassword.Length > maxLength) return hashedPassword.Substring(0, maxLength);
                else return hashedPassword;
            }
        }
    }
}