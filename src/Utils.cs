using System;
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

        public static byte[] DeriveKeyFromString(string input, string? salt = null)
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

        /// <summary>
        /// Hashes a string 2^difficulty times using SHA-512
        /// </summary>
        /// <param name="str">The string that will be hashed</param>
        /// <param name="difficulty">Determines the amount of times the string will be hashed</param>
        /// <returns></returns>
        public static string HashString(string str, HashSecurity difficulty = HashSecurity.Unsafe)
        {
            int hashnum = (int)Math.Pow(2, (double)difficulty); // the bigger this number, the harder is to bruteforce
            using SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < hashnum; i++)
            {
                str = HashBytesToString(bytes, sha512);
            }
            return str;
        }

        /// <summary>
        /// Hashes a file based on its content using SHA-512
        /// </summary>
        /// <param name="filepath">The path of the file</param>
        /// <returns>The hash in a string format</returns>
        public static async Task<string> HashFileAsync(string filepath)
        {
            // TODO use its length too
            using SHA512 sha512 = SHA512.Create();
            FileInfo fileInfo = new(filepath);
            FileStream fileStream = fileInfo.OpenRead();
            byte[] result = await sha512.ComputeHashAsync(fileStream);
            return ByteToString(result);
        }

        /// <summary>
        /// Converts a byte array to its string representation
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLower();

        /// <summary>
        /// Should be used often, so pass the same hash class
        /// </summary>
        /// <param name="bytes">The bytes that will be hashed</param>
        /// <param name="hash">The hashAlgorithm</param>
        /// <returns>The hash in a string format</returns>
        public static string HashBytesToString(byte[] bytes, HashAlgorithm hash) => ByteToString(hash.ComputeHash(bytes));

        /// <summary>
        /// Should be used often, so pass the same hash class
        /// </summary>
        /// <param name="bytes">The bytes that will be hashed</param>
        /// <param name="hash">The hashAlgorithm</param>
        /// <returns>The hash</returns>
        public static byte[] HashBytes(byte[] bytes, HashAlgorithm hash) => hash.ComputeHash(bytes);

        /// <summary>
        /// Represents a Key and IV
        /// </summary>
        /// <param name="Key">The key in bytes[]</param>
        /// <param name="IV"></param>
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
