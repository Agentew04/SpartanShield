using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using Newtonsoft.Json;

namespace SpartanShield.Managers
{
    public class FileManager
    {
        public static FileManager Instance { get; set; } = new();

        private List<string> _folders = new();

        private string _storagePath = string.Empty;

        private byte[] _key = Array.Empty<byte>();

        private readonly byte[] _defaultIV = new byte[16]
        {
            1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16
        };

        public void AddFolder(string folder)
        {
            _folders.Add(folder);
        }
        public void RemoveFolder(string folder)
        {
            _folders.Remove(folder);
        }

        public IEnumerable<string> GetFolders() => _folders;

        public void Initialize(string username, string password)
        {
            var mainpath = Environment.CurrentDirectory;
            _storagePath = mainpath + "/storage/";
            var folderspath = mainpath + "/folderinfo.json";

            if (!File.Exists(folderspath))
            {
                using var s = File.CreateText(folderspath);
                s.Write(@"[]");
                s.Close();
                _folders = new();
            }
            else
            {
                var rawjson = File.ReadAllText(folderspath);
                var jsonDeserialized = JsonConvert.DeserializeObject<List<string>>(rawjson);
                if(jsonDeserialized is null)
                {
                    _folders = new();
                }
                else
                {
                    _folders = jsonDeserialized;
                }
            }

            // generate key 
            _key = Utils.DeriveKeyFromString(password, username);
        }
        public async void Encrypt()
        {
            foreach(var folder in _folders)
            {
                // get zip stream
                using ZipFile zipfile = new();
                zipfile.AddDirectory(folder);
                using MemoryStream zipStream = new();
                zipfile.Save(zipStream);
                zipStream.Seek(0, SeekOrigin.Begin);

                // encrypt stream
                using var encryptedStream = Utils.EncryptStream(zipStream, new(_key, _defaultIV));
                encryptedStream.Seek(0,SeekOrigin.Begin);

                // save stream
                using var fileStream = File.OpenWrite(_storagePath + folder);
                await encryptedStream.CopyToAsync(fileStream);

                // delete orignal file
                File.Delete(folder);
            }
        }
        public void Decrypt()
        {
            foreach(var folder in _folders)
            {
                // read encrypted stream
                using var encryptedStream = new FileStream(_storagePath + folder, FileMode.Open);

                // decrypt zip & unzip
                using var decryptedStream = Utils.DecryptStream(encryptedStream, new(_key, _defaultIV));
                decryptedStream.Seek(0, SeekOrigin.Begin);
                using ZipFile zip = ZipFile.Read(decryptedStream);
                zip.ExtractAll(folder);

                // delete encrypted folder
                File.Delete(_storagePath + folder);
            }
        }
    }
}
