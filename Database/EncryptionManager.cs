using System.Security.Cryptography;
using System.Text;
using System;

namespace NEA_Project_Oubliette.Database
{
    internal static class EncryptionManager
    {
        private static string HASH = "k^FW@13?";

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
            {
                byte[] keyBytes = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(HASH));

                using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider { Key = keyBytes, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripleDES.CreateEncryptor();
                    byte[] resultBytes = transform.TransformFinalBlock(plainTextBytes, 0, plainText.Length);
                    return Convert.ToBase64String(resultBytes, 0, resultBytes.Length);
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            using (MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
            {
                byte[] keyBytes = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(HASH));

                using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider { Key = keyBytes, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripleDES.CreateDecryptor();
                    byte[] resultBytes = transform.TransformFinalBlock(cipherTextBytes, 0, cipherTextBytes.Length);
                    return UTF8Encoding.UTF8.GetString(resultBytes);
                }
            }
        }
    }
}