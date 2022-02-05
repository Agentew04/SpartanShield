using System;
using System.Collections.Generic;
using System.IO;
using Usb.Events;

namespace SpartanShield
{
    public static class USBManager
    {

#pragma warning disable IDE0060
        public static void Plugged(object? sender, UsbDevice usb)
        {
            if (usb.MountedDirectoryPath == "")
            {
                return; // it's not a usb flash drive
            }

            // its a compatible usb driv
            SpartanFile spartanFile = SpartanFileExists(usb) ? ReadSpartanFile(usb) : CreateSpartanFile(usb);

            // add usb drive contents to the main list
            // TODO

        }

        public static void Unplugged(object? sender, UsbDevice e)
        {
            //not much we can do here, just delete entries on menu

        }
#pragma warning restore IDE0060

        private static bool SpartanFileExists(UsbDevice usb) => File.Exists($"{usb.MountedDirectoryPath}\\.spartan");

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

            File.SetAttributes(path, FileAttributes.Hidden);

            return new()
            {
                Id = guid,
                IsDecrypted = true,
                Paths = new()
            };
        }

        private static SpartanFile ReadSpartanFile(UsbDevice usb)
        {
            string path = $"{usb.MountedDirectoryPath}\\.spartan";
            using FileStream fileStream = new(path, FileMode.Open);
            using BinaryReader binaryReader = new(fileStream);
            return SpartanFile.Read(binaryReader);
        }

        private static void WriteSpartanFile(UsbDevice usb, SpartanFile spartanFile)
        {
            string filepath = $"{usb.MountedDirectoryPath}\\.spartan";
            using FileStream fileStream = new(filepath, FileMode.Open);
            using BinaryWriter binaryWriter = new(fileStream);
            SpartanFile.Write(binaryWriter, spartanFile, true);
            fileStream.SetLength(fileStream.Position); // truncates file, in case there are fewer bytes
        }
        public class SpartanFile
        {
            public Guid Id { get; set; }
            public bool IsDecrypted { get; set; }
            public List<string> Paths { get; set; } = new();

            public static void Write(BinaryWriter writer, SpartanFile item, bool skipGuid = false)
            {
                if (!skipGuid) writer.Write(item.Id.ToByteArray()); // id
                else writer.Seek(16, SeekOrigin.Begin);
                writer.Write(item.IsDecrypted); // overrides the old isDecrypted bool
                writer.Write(item.Paths.Count); // write list length
                foreach (string path in item.Paths)
                {
                    writer.Write(path); // write each path
                }
            }
            public static SpartanFile Read(BinaryReader reader)
            {
                Guid id = new Guid(reader.ReadBytes(16));
                bool isDecrypted = reader.ReadBoolean();
                var listcount = reader.ReadInt32();
                List<string> paths = new();
                for (int i = 0; i < listcount; i++)
                {
                    paths.Add(reader.ReadString());
                }
                return new()
                {
                    Id = id,
                    IsDecrypted = isDecrypted,
                    Paths = paths
                };
            }
        }
    }
}
