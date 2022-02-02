using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield
{
    public static class Utils
    {
        public static string AppFolder { get; private set; } = $"{Environment.CurrentDirectory}/config";
        public static string UsersFile { get; private set; } = $"{AppFolder}/users.dat";

        public static byte[] CreateKeyFromString(string input) => Convert.FromHexString(HashString(input));

        public static string HashString(string str)
        {
            int hashnum = 100_000; // the bigger this number, the harder is to bruteforce
            for (int i = 0; i < hashnum; i++)
            {
                str = RawHash(str);
            }
            return str;
        }

        private static string RawHash(string s)
        {
            //sha512
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            byte[] hash = sha512.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }


        public record Auth(byte[] Key, byte[] IV);

        /// <summary>
        /// Encrypts a Stream using AES. Key and Block size is 2048 for safety.
        /// </summary>
        /// <param name="inStream">The stream that will be encrypted</param>
        /// <param name="auth">The Auth object</param>
        /// <returns>A stream containing all cypher data</returns>
        public static MemoryStream EncryptStream(Stream inStream, Auth auth)
        {
            using Aes aes = Aes.Create();
            aes.Key = auth.Key ?? aes.Key;
            aes.IV = auth.IV ?? aes.IV;
            aes.KeySize = 2048;
            aes.BlockSize = 2048;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform encryptor = aes.CreateEncryptor();
            using MemoryStream memoryStream = new();
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

        private static bool CreateUsersFile()
        {
            try
            {
                if (File.Exists(UsersFile)) return true;
                else File.Create(UsersFile).Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static bool WriteUsersFile(Dictionary<string,string> dict)
        {
            try
            {
                if (!File.Exists(UsersFile)) CreateUsersFile();
                var json = JsonConvert.SerializeObject(dict);
                File.WriteAllText(UsersFile, json);
                return true;
            }catch (Exception)
            {
                return false;
            }
        }
        private static Dictionary<string,string> ReadUsersFile()
        {
            if (!File.Exists(UsersFile)) CreateUsersFile();
            var json = File.ReadAllText(UsersFile);
            var dict = JsonConvert.DeserializeObject<Dictionary<string,string>>(json);
            return dict ?? new();
        }
        public static string GetUserHash(string user)
        {
            var dict = ReadUsersFile();
            return dict[user];
        }
        public static void SetUserHash(string user, string hash)
        {
            if (!File.Exists(UsersFile)) CreateUsersFile();
            var dict = ReadUsersFile();
            dict[user] = hash;
            WriteUsersFile(dict);

        }
    }
}
