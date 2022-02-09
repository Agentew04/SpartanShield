using System;
using System.Collections.Generic;
using System.IO;
using Usb.Events;
using System.Linq;
#pragma warning disable IDE0060
namespace SpartanShield
{
    public static class USBManager
    {

        public static void Plugged(object? sender, UsbDevice usb)
        {
            var path = usb.MountedDirectoryPath+"\\";
            if (path == "")
            {
                return; // it's not a usb flash drive
            }

            // its a compatible usb driv
            SpartanFile spartanFile = SpartanFileExists(path) ? ReadSpartanFile(path) : CreateSpartanFile(path);
            
            FileManager.AddItem(spartanFile.Items);

        }

        public static void Unplugged(object? sender, UsbDevice e)
        {
            //not much we can do here, just delete entries on menu
            SyncAllUSBDrives();
        }

        private static void SyncAllUSBDrives()
        {
            // TODO fix all this too
            //var usbsPaths = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Removable && x.IsReady).Select(x=>x.RootDirectory.FullName);
            //var items = FileManager.GetItems();
            //List<Guid> pluggedIds = new();
            //foreach (var usbPath in usbsPaths) pluggedIds.Add(ReadSpartanFile(usbPath).Id);
            //var idsToBeRemoved = items.Where(x => x.IsInRemovableDrive && !pluggedIds.Contains(x.OwnerId)).Select(x => x.Id) ;
            //FileManager.RemoveItem(idsToBeRemoved);
        }

        private static bool SpartanFileExists(string rootDir) => File.Exists($"{rootDir}.spartan");

        private static SpartanFile CreateSpartanFile(string rootDir)
        {
            string path = $"{rootDir}.spartan";
            using FileStream fileStream = File.Create(path);
            using BinaryWriter binaryWriter = new(fileStream);
            SpartanFile spartanfile = new()
            {
                Id = Guid.NewGuid(),
                IsDecrypted = true,
                Items = new()
            };
            binaryWriter.Write(spartanfile.ToByteArray());
            File.SetAttributes(path, FileAttributes.Hidden);

            return spartanfile;
        }

        private static SpartanFile ReadSpartanFile(string rootDir)
        {
            string path = $"{rootDir}.spartan";
            using FileStream fileStream = new(path, FileMode.Open);
            using BinaryReader binaryReader = new(fileStream);
            return SpartanFile.Read(binaryReader);
        }

        private static void WriteSpartanFile(string rootDir, SpartanFile spartanFile)
        {
            string filepath = $"{rootDir}.spartan";
            using FileStream fileStream = new(filepath, FileMode.Open);
            using BinaryWriter binaryWriter = new(fileStream);
            binaryWriter.Write(spartanFile.ToByteArray());
            fileStream.SetLength(fileStream.Position); // truncates file, in case there are fewer bytes
        }
        public class SpartanFile
        {
            public Guid Id { get; set; }
            public bool IsDecrypted { get; set; }
            public List<CryptoItem> Items { get; set; } = new();

            public byte[] ToByteArray()
            {
                MemoryStream ms = new();
                BinaryWriter binaryWriter = new(ms);
                Write(binaryWriter, this);
                return ms.ToArray();
            }

            public static void Write(BinaryWriter writer, SpartanFile item, bool skipGuid = false)
            {
                if (!skipGuid) writer.Write(item.Id.ToByteArray()); // id
                else writer.Seek(16, SeekOrigin.Begin);
                writer.Write(item.IsDecrypted); // overrides the old isDecrypted bool
                writer.Write(item.Items.Count); // write list length
                foreach (var itemInList in item.Items)
                {
                    // TODO fix this
                    //CryptoItem.WriteItem(writer, itemInList);
                }
            }
            public static SpartanFile Read(BinaryReader reader)
            {
                Guid id = new(reader.ReadBytes(16));
                bool isDecrypted = reader.ReadBoolean();
                var listcount = reader.ReadInt32();
                List<CryptoItem> items = new();
                for (int i = 0; i < listcount; i++)
                {
                    // TODO fix this
                    //items.Add(CryptoItem.ReadItem(reader));
                }
                return new()
                {
                    Id = id,
                    IsDecrypted = isDecrypted,
                    Items = items
                };
            }
        }
    }
}
