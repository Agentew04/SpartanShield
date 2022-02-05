using System;
using System.IO;

namespace SpartanShield
{
    public class CryptoItem
    {
        public string Name { get; set; } = string.Empty;

        public Guid Id { get; set; }

        public string Path { get; set; } = string.Empty;

        public bool IsDirectory { get; set; }

        public bool IsEncrypted { get; set; }

        public bool IsInRemovableDrive { get; set; }

        public Guid OwnerId { get; set; }

        public static CryptoItem ReadItem(BinaryReader reader) => new()
        {
            Name = reader.ReadString(),
            Id = new Guid(reader.ReadBytes(16)),
            Path = reader.ReadString(),
            IsDirectory = reader.ReadBoolean(),
            IsEncrypted = reader.ReadBoolean(),
            IsInRemovableDrive = reader.ReadBoolean(),
            OwnerId = new Guid(reader.ReadBytes(16)),
        };
        public static void WriteItem(BinaryWriter writer, CryptoItem item)
        {
            writer.Write(item.Name);
            writer.Write(item.Id.ToByteArray());
            writer.Write(item.Path);
            writer.Write(item.IsDirectory);
            writer.Write(item.IsEncrypted);
            writer.Write(item.IsInRemovableDrive);
            writer.Write(item.OwnerId.ToByteArray());
        }
    }
}
