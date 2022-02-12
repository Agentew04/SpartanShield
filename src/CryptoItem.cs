using LiteDB;
using System;
using System.Collections.Generic;

namespace SpartanShield
{
    public class CryptoItem
    {
        public Guid Owner { get; set; }
        [BsonId]
        public Guid Id { get; set; }
        public Guid ParentDrive { get; set; }
        public bool IsEncrypted { get; set; }
        public string? Path { get; set; }
        public List<Guid> Files { get; set; } = new();
    }
}
