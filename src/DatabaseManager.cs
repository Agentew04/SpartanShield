using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;

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
        var col = db.GetCollection<CryptoItem>("items");
        return col.FindById(id);
    }

    /// <summary>
    /// Adds a new item to the files collection
    /// </summary>
    /// <param name="item">The item that will be added</param>
    public static void AddItem(CryptoItem item)
    {
        using LiteDatabase db = new(DatabasePath);
        var col = db.GetCollection<CryptoItem>("items");
        col.Insert(item);
    }

    /// <summary>
    /// Checks if a item with the specified Id already exists
    /// </summary>
    /// <param name="id">The id that will be searched</param>
    /// <returns></returns>
    public static bool ItemExists(Guid id)
    {
        using LiteDatabase db = new(DatabasePath);
        var col = db.GetCollection<CryptoItem>("items");
        return col.Exists(x => x.Id == id);
    }

    /// <summary>
    /// Returns a list with all <see cref="CryptoItem"/> inside the database
    /// </summary>
    /// <returns></returns>
    [Obsolete("Should not be used as it exposes too much information")]
    public static IEnumerable<CryptoItem> GetItems()
    {
        using LiteDatabase db = new(DatabasePath);
        var col = db.GetCollection<CryptoItem>("items");
        return col.FindAll();
    }

    /// <summary>
    /// Adds a file by its Stream and a Id
    /// </summary>
    /// <param name="stream">The file stream that will be added</param>
    /// <param name="id">The id that will be used to identify the file</param>
    public static void AddFile(Stream stream, Guid id)
    {
        using LiteDatabase db = new(DatabasePath);
        var fs = db.GetStorage<Guid>();
        fs.Upload(id, id.ToString(), stream);
    }

    /// <summary>
    /// Adds a file stream to the database, generating a new id automagically
    /// </summary>
    /// <param name="stream">The data stream that will be added</param>
    /// <returns>The id that was generated</returns>
    public static Guid AddFile(Stream stream)
    {
        Guid guid = Guid.NewGuid();
        using LiteDatabase db = new(DatabasePath);
        var fs = db.GetStorage<Guid>();
        fs.Upload(guid, guid.ToString(), stream);
        return guid;
    }

    /// <summary>
    /// Adds a file by its filepath and <see cref="Guid"/>
    /// </summary>
    /// <param name="stream">The pathh to the file that will be added</param>
    /// <param name="id">The id that will be used to identify the file</param>
    public static void AddFile(string filepath, Guid id)
    {
        using LiteDatabase db = new(DatabasePath);
        var fs = db.GetStorage<Guid>();
        FileStream file = new(filepath, FileMode.Open);
        fs.Upload(id, id.ToString(), file);
    }

    /// <summary>
    /// Adds a file by its path to the database, generating a new id automagically
    /// </summary>
    /// <param name="stream">The path to the file</param>
    /// <returns>The id that was generated</returns>
    public static Guid AddFile(string filepath)
    {
        Guid guid = Guid.NewGuid();
        FileStream fileStream = new(filepath, FileMode.Open);
        using LiteDatabase db = new(DatabasePath);
        var fs = db.GetStorage<Guid>();
        fs.Upload(guid, guid.ToString(), fileStream);
        return guid;
    }

    /// <summary>
    /// Gets a file from the database from its id
    /// </summary>
    /// <param name="id">The id that will be searched</param>
    /// <returns>A <see cref="MemoryStream"/> with the binary data</returns>
    public static MemoryStream GetFile(Guid id)
    {
        using LiteDatabase db = new(DatabasePath);
        var fs = db.GetStorage<Guid>();
        MemoryStream ms = new();
        fs.Download(id, ms);
        return ms;
    }

    /// <summary>
    /// Searches the file storage and sees if a file with the <see cref="Guid"/> exists
    /// </summary>
    /// <param name="id">The id that will searched</param>
    /// <returns>A <see cref="bool"/> representing if the file exists or not</returns>
    public static bool FileExists(Guid id)
    {
        using LiteDatabase db = new(DatabasePath);
        var fs = db.GetStorage<Guid>();
        return fs.Exists(id);
    }

    #endregion

    #region idmapping

    #endregion
}
