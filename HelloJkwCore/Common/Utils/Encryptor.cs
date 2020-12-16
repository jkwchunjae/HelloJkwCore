using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Encryptor
    {
        public static string Encrypt(this string plainText, string key)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");

            key = Enumerable.Range(0, 16).Select(x => key).StringJoin("");

            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = key.Take(16).Select(x => (byte)x).ToArray();
                rijAlg.IV = key.Take(16).Select(x => (byte)x).ToArray();

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            //return HttpUtility.UrlEncode(encrypted);
            return encrypted.Select(x => x.ToString("x2")).StringJoin("");
        }

        public static string Decrypt(this string cipherText, string key)
        {

            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");

            key = Enumerable.Range(0, 16).Select(x => key).StringJoin("");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = key.Take(16).Select(x => (byte)x).ToArray();
                rijAlg.IV = key.Take(16).Select(x => (byte)x).ToArray();

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                //var cipherData = HttpUtility.UrlDecodeToBytes(cipherText);
                var cipherData = Enumerable.Range(0, cipherText.Length / 2)
                    .Select(i => byte.Parse(cipherText.Substring(i * 2, 2), NumberStyles.HexNumber))
                    .ToArray();
                using (MemoryStream msDecrypt = new MemoryStream(cipherData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
