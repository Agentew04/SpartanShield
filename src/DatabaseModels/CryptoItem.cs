using LiteDB;
using System;
using System.Collections.Generic;

namespace SpartanShield
{
    public class CryptoItem
    {
        /// <summary>
        /// The <see cref="Guid"/> of the user that is the owner of this file
        /// </summary>
        public Guid Owner { get; set; }

        /// <summary>
        /// The <see cref="Guid"/> of this item
        /// </summary>
        [BsonId]
        public Guid Id { get; set; }

        /// <summary>
        /// The <see cref="Guid"/> of where the item is located(PC/USB)
        /// </summary>
        public Guid ParentDrive { get; set; }

        /// <summary>
        /// If this item is currently encrypted
        /// </summary>
        public bool IsEncrypted { get; set; }

        /// <summary>
        /// The path of the original file
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// A list of file guids that belongs to this item
        /// </summary>
        public List<Guid> Files { get; set; } = new();
    }
}
