using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield
{
    public class FileManager
    {
        private static void CheckFile()
        {
            if (!Directory.Exists(Utils.AppFolder))
            {
                Directory.CreateDirectory(Utils.AppFolder);
            }
            if (!File.Exists(Utils.UsersFile))
            {
                File.Create(Utils.UsersFile).Close();
            }
        }

        /// <summary>
        /// Returns all crypto items, including USB ones
        /// </summary>
        /// <returns></returns>
        public static List<CryptoItem> GetItems()
        {
            CheckFile();
            using var fs = File.OpenRead(Utils.ItemsFile);
            using BinaryReader binaryReader = new(fs);
            var items = new List<CryptoItem>();
            var count = binaryReader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                items.Add(CryptoItem.ReadItem(binaryReader));
            }
            return items;
        }

        /// <summary>
        /// Adds a CryptoItem to the current app list
        /// </summary>
        /// <param name="item"></param>
        public static void AddItem(CryptoItem item)
        {
            var alreadyExists = GetItems().Any(x => x.Id == item.Id);
            if (alreadyExists)
            {
                var conflictantItem= GetItems().Where(x => x.Id == item.Id).First();
                ConflictProp conflicts=null;
                if (conflictantItem.Name != item.Name) // name conflict
                {
                    conflicts &= ConflictProp.Name;
                }
                if(conflictantItem.Path != item.Path)
                {
                    
                }
                return;
            }

        }

        [Flags]
        private enum ConflictProp
        {
            
            Name = 1,
            IsDirectory =2,
            IsEncrypted = 4,
            IsInRemovableDrive = 8,
            OwnerId = 16,
            Path=32
        }
    }
}
