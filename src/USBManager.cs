using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usb.Events;

namespace SpartanShield
{
    public static class USBManager
    {
        public class SpartanFile
        {
            public Guid Guid { get; set; }
            public bool IsDecrypted { get; set; }
            public List<string> Paths { get; set; } = new();
        }

        public static void Plugged(object? sender, UsbDevice usb)
        {
            if(usb.MountedDirectoryPath == "")
            {
                return; // it's not a usb flash drive
            }

            // its a compatible usb drive
            if (!CheckSpartanFile(usb)) CreateSpartanFile(usb); 




        }

        public static void Unplugged(object? sender, UsbDevice e)
        {
            //not much we can do here, just delete entries on menu
        }

        private static bool CheckSpartanFile(UsbDevice usb) => File.Exists($"{usb.MountedDirectoryPath}\\.spartan");

        private static SpartanFile CreateSpartanFile(UsbDevice usb)
        {
            string path = $"{usb.MountedDirectoryPath}\\.spartan";
            using FileStream fileStream = File.Create(path);
            using BinaryWriter binaryWriter = new(fileStream);
            Guid guid = Guid.NewGuid();
            binaryWriter.Write(guid.ToByteArray()); // id
            binaryWriter.Write(true); // its decrypted
            binaryWriter.Write(0); // the length of the string list
            // list is empty, don't write anything
            
            File.SetAttributes(path,FileAttributes.Hidden);

            return new()
            {
                Guid = guid,
                IsDecrypted = true,
                Paths = new()
            };
        }
    
        private static SpartanFile ReadSpartanFile(UsbDevice usb)
        {
            string path = $"{usb.MountedDirectoryPath}\\.spartan";
            using FileStream fileStream = new(path, FileMode.Open);
            using BinaryReader binaryReader = new(fileStream);
            Guid guid = new(binaryReader.ReadBytes(16));
            bool isdecrypted = binaryReader.ReadBoolean();
            int listLength = binaryReader.ReadInt32();
            List<string> paths = new();
            for (int i = 0; i < listLength; i++)
            {
                paths.Add(binaryReader.ReadString());
            }
            
            return new()
            {
                Guid = guid,
                IsDecrypted = isdecrypted,
                Paths = paths
            };
        }
    
        private static void WriteSpartanFile(UsbDevice usb, SpartanFile spartanFile)
        {
            string filepath = $"{usb.MountedDirectoryPath}\\.spartan";
            using FileStream fileStream = new(filepath, FileMode.Open);
            using BinaryWriter binaryWriter = new(fileStream);
            binaryWriter.Seek(16, SeekOrigin.Begin); // jumps the guid part, should not be edited once defined
            binaryWriter.Write(spartanFile.IsDecrypted); // overrides the old isDecrypted bool
            binaryWriter.Write(spartanFile.Paths.Count); // write list length
            foreach(string path in spartanFile.Paths) 
            {
                binaryWriter.Write(path);
            }
            fileStream.SetLength(fileStream.Position);
        }
    }
}
