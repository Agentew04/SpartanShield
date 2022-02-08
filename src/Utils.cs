using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield
{
    public static class Utils
    {
        public enum HashSecurity
        {
            Unsafe = 1,
            Safe = 17,
            Safer = 18,
            Extreme = 20
        }
        public static string AppFolder { get; private set; } = $"{Environment.CurrentDirectory}/config";
        public static string UsersFile { get; private set; } = $"{AppFolder}/users.json";
        public static string ItemsFile { get; private set; } = $"{AppFolder}/items.json";

        public static byte[] CreateKeyFromString(string input, string? salt = null)
        {
            //get input bytes
            byte[] inputbytes = Encoding.UTF8.GetBytes(input);
            byte[] saltbytes;
            if (salt != null) saltbytes = Encoding.UTF8.GetBytes(salt);
            else saltbytes = new byte[16];

            // Generate the hash
            Rfc2898DeriveBytes pbkdf2 = new(inputbytes, saltbytes, iterations: 5000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32); //32 bytes length is 256 bits
        }

        public static byte[] CreateIV()
        {
            using Aes aes = Aes.Create();
            return aes.IV;
        }

        public static string HashString(string str, HashSecurity difficulty)
        {
            int hashnum = (int)Math.Pow(2, (double)difficulty); // the bigger this number, the harder is to bruteforce
            SHA512 sha512 = SHA512.Create();
            for (int i = 0; i < hashnum; i++)
            {
                str = RawHash(str, sha512);
            }
            return str;
        }

        private static string RawHash(string s, HashAlgorithm hash)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            byte[] hashOutput = hash.ComputeHash(bytes);
            return BitConverter.ToString(hashOutput).Replace("-", "").ToLower();
        }


        public record Auth(byte[] Key, byte[] IV);

        /// <summary>
        /// Encrypts a Stream using AES. Keysize is 256bit and IV is 128bit
        /// </summary>
        /// <param name="inStream">The stream that will be encrypted</param>
        /// <param name="auth">The Auth object</param>
        /// <returns>A stream containing all cypher data</returns>
        public static MemoryStream EncryptStream(Stream inStream, Auth auth)
        {
            using Aes aes = Aes.Create();
            aes.Key = auth.Key ?? aes.Key;
            aes.IV = auth.IV ?? aes.IV;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform encryptor = aes.CreateEncryptor();
            MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            using StreamWriter swEncrypt = new(cryptoStream);
            swEncrypt.Write(inStream);
            return memoryStream;
        }

        /// <summary>
        /// Decrypts a <see cref="Stream"/> containing cyphertext. Uses AES with 2048 as Key and Block size.
        /// </summary>
        /// <param name="inStream">The encrypted <see cref="Stream"/></param>
        /// <param name="auth">A object containing Key and IV</param>
        /// <returns>A <see cref="Stream"/> containing decrypted data</returns>
        public static MemoryStream DecryptStream(Stream inStream, Auth auth)
        {
            using Aes aes = Aes.Create();
            aes.Key = auth.Key ?? aes.Key;
            aes.IV = auth.IV ?? aes.IV;
            aes.KeySize = 2048;
            aes.BlockSize = 2048;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aes.CreateDecryptor();
            using CryptoStream cryptoStream = new(inStream, decryptor, CryptoStreamMode.Read);
            using MemoryStream memoryStream = new();
            CopyStream(cryptoStream, memoryStream);
            return memoryStream;
        }

        /// <summary>
        /// Copies a <see cref="Stream"/> to another in chunks
        /// </summary>
        /// <param name="input">The input</param>
        /// <param name="output">The output</param>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }

        }

        
    }
}
