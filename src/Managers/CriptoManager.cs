using Ionic.Zip;
using System.IO;
using System.Linq;

namespace SpartanShield
{
    public class CriptoManager
    {
        public static void EncryptItem(Utils.Auth credentials, CryptoItem item)
        {

        }
        private static void EncryptFolder(Utils.Auth auth, string path)
        {
            // read decrypted
            ZipFile zipfile = new();
            zipfile.AddDirectory(path);
            MemoryStream ms = new();
            zipfile.Save(ms);

            // encrypt
            var cs = Utils.EncryptStream(ms, auth);

            // save encrypted
            FileStream fs = new($"{path}.enc", FileMode.Create);
            cs.WriteTo(fs);

            // delete decrypted
            DirectoryInfo directoryInfo = new(path);
            directoryInfo.Delete(true);
        }
        private void DecryptFolder(Utils.Auth auth, string path)
        {
            if (!path.EndsWith(".enc")) return;
            var actualpath = path[0..^4];

            // read encrypted
            using FileStream encStream = new(path, FileMode.Open, FileAccess.Read);

            // decrypt
            using var ms = Utils.DecryptStream(encStream, auth);

            // save decrypted
            using ZipFile zip = ZipFile.Read(ms);
            zip.ExtractAll(actualpath);

            //delete encrypted
            File.Delete(path);
        }

        /// <summary>
        /// Reads a zip file from the disk and encrypts it
        /// </summary>
        /// <param name="zipFilePath">The path of the zip file</param>
        /// <param name="auth">The auth object used to encrypt the file</param>
        /// <returns>A memoryStream with the encrypted data</returns>
        public static MemoryStream EncryptZip(string zipFilePath, Utils.Auth auth)
        {
            using FileStream fs = new(zipFilePath, FileMode.Open);
            return Utils.EncryptStream(fs, auth);
        }

        /// <summary>
        /// Reads a zip file, encrypts it and saves to the disk
        /// </summary>
        /// <param name="zipFilePath">The path for the zip file</param>
        /// <param name="outputPath">The output path for the encrypted file, including filename and extension</param>
        /// <param name="auth">The auth object used to encrypt the file</param>
        public static void EncryptZip(string zipFilePath, string outputPath, Utils.Auth auth)
        {
            using FileStream inputStream = new(zipFilePath, FileMode.Open);
            using var ms = Utils.EncryptStream(inputStream, auth);
            using FileStream outputStream = new(outputPath, FileMode.Create);
            ms.CopyTo(outputStream);
        }

        /// <summary>
        /// Zips a folder and saves it to the memory. The stream is not seeked back to beggining after write
        /// </summary>
        /// <param name="filepath">The path of the folder that will be zipped</param>
        /// <returns>The memorystream containing the zipped file</returns>
        public static MemoryStream ZipFolder(string filepath)
        {
            ZipFile zip = new();
            MemoryStream ms = new();
            zip.AddDirectory(filepath);
            zip.Save(ms);
            return ms;
        }

        /// <summary>
        /// Zips a folder and saves it to the disk
        /// </summary>
        /// <param name="filepath">The folder that will be zipped</param>
        /// <param name="outputPath">The output path for the zip file, including filename and extension</param>
        public static void ZipFolder(string filepath, string outputPath)
        {
            ZipFile zip = new();
            FileStream fs = new(outputPath,FileMode.Create);
            zip.AddDirectory(filepath);
            zip.Save(fs);
        }
    }
}
