using System.Security.Cryptography;
using System.Text;
using System;

namespace NEA_Project_Oubliette.Database.Security
{
    ///<summary>Provides methods for encrypting and decrypting email addresses</summary>
    internal static class EncryptionManager
    {
        private static string HASH = "k^FW@13?";

        ///<summary>Encrypts an email address using the MD5 hashing algorithm</summary>
        public static string EncryptEmailAddress(string plainText)
        {
            string[] parts = plainText.Split('@');

            // Initialises two byte arrays for characters on either side of the @ symbol
            byte[] leftPlainTextBytes = Encoding.UTF8.GetBytes(parts[0]);
            byte[] rightPlainTextBytes = Encoding.UTF8.GetBytes(parts[1]);

            using(MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
            {
                byte[] keyBytes = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(HASH));

                using(TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider { Key = keyBytes, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripleDES.CreateEncryptor();

                    byte[] leftCipherTextBytes = transform.TransformFinalBlock(leftPlainTextBytes, 0, leftPlainTextBytes.Length);
                    byte[] rightCipherTextBytes = transform.TransformFinalBlock(rightPlainTextBytes, 0, rightPlainTextBytes.Length);

                    // Converts the encrypted byte arrays back into UTF8 encoded text and delimits both parts with an @ symbol
                    return Convert.ToBase64String(leftCipherTextBytes, 0, leftCipherTextBytes.Length) + '@' + Convert.ToBase64String(rightCipherTextBytes, 0, rightCipherTextBytes.Length);
                }
            }
        }

        ///<summary>Decrypts an email address using the MD5 hashing algorithm</summary>
        public static string DecryptEmailAddress(string cipherText)
        {
            string[] parts = cipherText.Split('@');

            // Initialises two byte arrays for characters on either side of the @ symbol
            byte[] leftCipherTextBytes = Convert.FromBase64String(parts[0]);
            byte[] rightCipherTextBytes = Convert.FromBase64String(parts[1]);

            using(MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
            {
                byte[] keyBytes = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(HASH));

                using(TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider { Key = keyBytes, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripleDES.CreateDecryptor();

                    byte[] leftPlainTextBytes = transform.TransformFinalBlock(leftCipherTextBytes, 0, leftCipherTextBytes.Length);
                    byte[] rightPlainTextBytes = transform.TransformFinalBlock(rightCipherTextBytes, 0, rightCipherTextBytes.Length);

                    // Converts the encrypted byte arrays back into UTF8 encoded text and delimits both parts with an @ symbol
                    return UTF8Encoding.UTF8.GetString(leftPlainTextBytes) + '@' + UTF8Encoding.UTF8.GetString(rightPlainTextBytes);
                }
            }
        }
    }
}