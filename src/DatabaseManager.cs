using LiteDB;
using System;
using System.Collections.Generic;

namespace SpartanShield;
public class User
{
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<Guid> Files { get; set; } = new();
}
public static class DatabaseManager
{

    public static string DatabasePath { get; set; } = $"{Environment.CurrentDirectory}/config/spartan.db";

    #region User

    /// <summary>
    /// Returns the hash of a user
    /// </summary>
    /// <param name="username">The username that will be searched</param>
    /// <returns></returns>
    public static string GetUserHash(string username)
    {
        using LiteDatabase db = new(DatabasePath);
        var col = db.GetCollection<User>("users");
        var hash = col.Query()
            .Where(x => x.Username == username)
            .Select(x => x.PasswordHash)
            .FirstOrDefault();
        return hash;
    }

    /// <summary>
    /// Returns the hash of a user
    /// </summary>
    /// <param name="id">The unique id that will be searched</param>
    /// <returns></returns>
    public static string GetUserHash(Guid id)
    {
        using LiteDatabase db = new(DatabasePath);
        var col = db.GetCollection<User>("users");
        var hash = col.Query()
            .Where(x => x.Id == id)
            .Select(x => x.PasswordHash)
            .FirstOrDefault();
        return hash;
    }

    /// <summary>
    /// Creates a new user with a random id and sets its hash
    /// </summary>
    /// <param name="username">The username of the user</param>
    /// <param name="hash">The password hash of the user</param>
    public static void SetUserHash(string username, string hash)
    {
        using LiteDatabase db = new(DatabasePath);
        var col = db.GetCollection<User>("users");
        User user = new()
        {
            Username = username,
            PasswordHash = hash
        };
        col.Insert(user);
    }

    /// <summary>
    /// Checks if a user exists
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public static bool UserExists(string username)
    {
        using LiteDatabase db = new(DatabasePath);
        var col = db.GetCollection<User>("users");
        var exists = col.Exists(x => x.Username == username);
        return exists;
    }

    /// <summary>
    /// Checks if a user exists
    /// </summary>
    /// <param name="id">The id of the user</param>
    /// <returns></returns>
    public static bool UserExists(Guid id)
    {
        using LiteDatabase db = new(DatabasePath);
        var col = db.GetCollection<User>("users");
        var exists = col.Exists(x => x.Id == id);
        return exists;
    }

    #endregion

    #region files
    /// <summary>
    /// Returns a <see cref="CryptoItem"/> by its <see cref="Guid"/>
    /// </summary>
    /// <param name="id">The id that will looked up</param>
    /// <returns>The item that has been found or null if it does not exists</returns>
    public static CryptoItem? GetItem(Guid id)
    {
        using LiteDatabase db = new(DatabasePath);
        var col = db.GetCollection<CryptoItem>("files");
        return col.FindById(id);
    }

    #endregion

    #region idmapping

    #endregion
}
