using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;

namespace SpartanShield
{
    public class EncryptionManager
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
            FileStream fs = new($"{path}.enc",FileMode.Create);
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
            using FileStream encStream = new(path,FileMode.Open,FileAccess.Read);

            // decrypt
            using var ms = Utils.DecryptStream(encStream, auth);

            // save decrypted
            using ZipFile zip = ZipFile.Read(ms);
            zip.ExtractAll(actualpath);

            //delete encrypted
            File.Delete(path);
        }
    }
}
