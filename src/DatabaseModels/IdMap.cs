using LiteDB;
using SpartanShield.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield.DatabaseModels;

public struct IdMap
{
    [BsonId]
    public Guid Id { get; set; } = Guid.Empty;
    public ObjectType Type { get; set; } = ObjectType.None;

    public IdMap(Guid id, ObjectType type)
    {
        Id = id;
        Type = type;
    }

    public IdMap() { }
}
