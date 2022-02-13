using LiteDB;
using System;
using System.Collections.Generic;

namespace SpartanShield.DatabaseModels;

public class User
{
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<Guid> Files { get; set; } = new();
}
