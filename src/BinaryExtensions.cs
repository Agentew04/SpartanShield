using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield
{
    public static class BinaryExtensions
    {
        /// <summary>
        /// Reads a Guid from a stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>The Guid that has been read</returns>
        public static Guid ReadGuid(this BinaryReader reader) => new(reader.ReadBytes(16));

        /// <summary>
        /// Writes a Guid to a stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="guid">The Guid that will be written</param>
        public static void Write(this BinaryWriter writer, Guid guid) => writer.Write(guid.ToByteArray());

        public static CryptoItem ReadCryptoItem(this BinaryReader reader)
        {
            Guid id = reader.ReadGuid();
            Guid owner = reader.ReadGuid();
            Guid parentDrive = reader.ReadGuid();
            bool isEncrypted = reader.ReadBoolean();
            string path = reader.ReadString();

            var filesCount = reader.ReadInt32();
            var files = new List<Guid>();
            for (int i = 0; i < filesCount; i++) files.Add(reader.ReadGuid());

            return new()
            {
                Id = id,
                Owner = owner,
                ParentDrive = parentDrive,
                IsEncrypted = isEncrypted,
                Path = path,
                Files = files
            };
        }

        public static void WriteCryptoItem(this BinaryWriter writer, CryptoItem item)
        {
            writer.Write(item.Id);
            writer.Write(item.Owner);
            writer.Write(item.ParentDrive);
            writer.Write(item.IsEncrypted);
            writer.Write(item.Path);
            writer.Write(item.Files.Count);
            foreach (var file in item.Files) writer.Write(file);
        }
    }
}
