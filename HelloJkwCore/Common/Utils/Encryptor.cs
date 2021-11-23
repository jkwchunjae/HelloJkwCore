﻿using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Encryptor
    {
        public static string Encrypt(this string plainText, string password)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (password == null || password.Length <= 0)
                throw new ArgumentNullException("password");

            password = Enumerable.Range(0, 16).Select(x => password).StringJoin("");
            var key = password.Take(16).Select(x => (byte)x).ToArray();
            var iv = password.Take(16).Select(x => (byte)x).ToArray();

            using Aes aes = Aes.Create();
            ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

            using MemoryStream msEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            var encrypted = msEncrypt.ToArray();
            var cipherText = Convert.ToHexString(encrypted).ToLower();
            return cipherText;
        }

        public static string Decrypt(this string cipherText, string password)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (password == null || password.Length <= 0)
                throw new ArgumentNullException("password");

            var cipherData = Convert.FromHexString(cipherText);

            password = Enumerable.Range(0, 16).Select(x => password).StringJoin("");
            var key = password.Take(16).Select(x => (byte)x).ToArray();
            var iv = password.Take(16).Select(x => (byte)x).ToArray();

            using Aes aes = Aes.Create();
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);

            using MemoryStream msDecrypt = new MemoryStream(cipherData);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            var plainText = srDecrypt.ReadToEnd();
            return plainText;
        }
    }
}
