using LiteDB;
using SpartanShield.DatabaseModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpartanShield.Managers;

public static class DatabaseManager
{

    public static string DatabasePath { get; set; } = $"{Environment.CurrentDirectory}/config/spartan.db";

    private static LiteDatabase GetDB() => new(DatabasePath);

    #region User

    /// <summary>
    /// Returns the hash of a user
    /// </summary>
    /// <param name="username">The username that will be searched</param>
    /// <returns></returns>
    public static string GetUserHash(string username)
    {
        using var db = GetDB();
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
        using var db = GetDB();
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
        using var db = GetDB();
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
        using var db = GetDB();
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
        using var db = GetDB();
        var col = db.GetCollection<User>("users");
        var exists = col.Exists(x => x.Id == id);
        return exists;
    }

    /// <summary>
    /// Deletes a user from the database based on its id
    /// </summary>
    /// <param name="id"></param>
    public static void RemoveUser(Guid id)
    {
        using var db = GetDB();
        var col = db.GetCollection<User>("users");
        col.Delete(id);
    }

    /// <summary>
    /// Deletes a user based on its username
    /// </summary>
    /// <param name="username">The name of the user that will be deleted</param>
    public static void RemoveUser(string username)
    {
        using var db = GetDB();
        var col = db.GetCollection<User>("users");
        col.DeleteMany(x => x.Username == username);
    }

    #endregion

    #region items
    /// <summary>
    /// Returns a <see cref="CryptoItem"/> by its <see cref="Guid"/>
    /// </summary>
    /// <param name="id">The id that will looked up</param>
    /// <returns>The item that has been found or null if it does not exists</returns>
    public static CryptoItem? GetItem(Guid id)
    {
        using var db = GetDB();
        var col = db.GetCollection<CryptoItem>("items");
        return col.FindById(id);
    }

    /// <summary>
    /// Change the different fields of a item in the database, does nothing if it doesn't exists
    /// </summary>
    /// <param name="item">The item to be edited</param>
    public static void EditItem(CryptoItem item)
    {
        bool exists = ItemExists(item.Id);
        if (!exists) return;
        using var db = GetDB();
        var col = db.GetCollection<CryptoItem>("items");
        col.Update(item);
    }

    /// <summary>
    /// Adds a new item to the files collection
    /// </summary>
    /// <param name="item">The item that will be added</param>
    public static void AddItem(CryptoItem item)
    {
        using var db = GetDB();
        var col = db.GetCollection<CryptoItem>("items");
        col.Insert(item);
        OnItemChanged(new()
        {
            HasBeenAdded = true,
            Item = item
        });
    }

    /// <summary>
    /// Checks if a item with the specified Id already exists
    /// </summary>
    /// <param name="id">The id that will be searched</param>
    /// <returns></returns>
    public static bool ItemExists(Guid id)
    {
        using var db = GetDB();
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
        using var db = GetDB();
        var col = db.GetCollection<CryptoItem>("items");
        return col.FindAll();
    }

    /// <summary>
    /// Removes a item from the database based on its id
    /// </summary>
    /// <param name="id"></param>
    public static void RemoveItem(Guid id)
    {
        using var db = GetDB();
        var col = db.GetCollection<CryptoItem>("items");
        col.Delete(id);
    }

    /// <summary>
    /// Removes all items containing the ids in the IEnumerable
    /// </summary>
    /// <param name="ids">The ids that will be removed</param>
    /// <returns>How many items where removed</returns>
    public static int RemoveItem(IEnumerable<Guid> ids)
    {
        using var db = GetDB();
        var col = db.GetCollection<CryptoItem>("items");
        return col.DeleteMany(x => ids.Contains(x.Id));
    }

    #endregion

    #region files

    /// <summary>
    /// Adds a file by its Stream and a Id
    /// </summary>
    /// <param name="stream">The file stream that will be added</param>
    /// <param name="id">The id that will be used to identify the file</param>
    public static void AddFile(Stream stream, Guid id)
    {
        using var db = GetDB();
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
        using var db = GetDB();
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
        using var db = GetDB();
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
        using var db = GetDB();
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
        using var db = GetDB();
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
        using var db = GetDB();
        var fs = db.GetStorage<Guid>();
        return fs.Exists(id);
    }

    /// <summary>
    /// Removes a file from the database based on its id
    /// </summary>
    /// <param name="id"></param>
    public static void RemoveFile(Guid id)
    {
        using var db = GetDB();
        var fs = db.GetStorage<Guid>();
        fs.Delete(id);
    }

    #endregion

    #region idmapping

    /// <summary>
    /// Adds a Id mapping to the database. If the id already exists, nothing is changed
    /// </summary>
    /// <param name="id">The id that will be mapped</param>
    /// <param name="type">The <see cref="ObjectType"/> that the id corresponds</param>
    public static void AddIdMapping(Guid id, ObjectType type)
    {
        using var db = GetDB();
        var col = db.GetCollection<IdMap>("idmap");
        if (!col.Exists(x => x.Id == id)) // does not use method IdMappingExists because the file is locked 
            col.Insert(new IdMap(id, type));
    }

    /// <summary>
    /// Gets the <see cref="ObjectType"/> that an id corresponds to
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> that will be searched in the database</param>
    /// <returns>An <see cref="ObjectType"/> enum representing the type associated with the given <see cref="Guid"/></returns>
    public static ObjectType GetIdMapping(Guid id)
    {
        using var db = GetDB();
        var col = db.GetCollection<IdMap>("idmap");
        col.EnsureIndex(x => x.Id);
        return col.Query()
            .Where(x => x.Id == id)
            .Select(x => x.Type)
            .FirstOrDefault();
    }

    /// <summary>
    /// Checks if a IdMapping already exists
    /// </summary>
    /// <param name="id">The id that will be searched</param>
    /// <returns>The result of the operation</returns>
    public static bool IdMappingExists(Guid id)
    {
        using var db = GetDB();
        var col = db.GetCollection<IdMap>("idmap");
        return col.Exists(x => x.Id == id);
    }

    #endregion

    #region events

    public delegate void ItemsChangedHandler(ItemsChangedArgs e);

    /// <summary>
    /// Is raised when the persistent storage changes
    /// </summary>
    public static event ItemsChangedHandler? ItemsChanged;

    private static void OnItemChanged(ItemsChangedArgs args)
    {
        ItemsChanged?.Invoke(args);
    }

    #endregion
}

// ADDITIONAL CLASSES!!

public enum ObjectType
{
    None,
    User,
    File,
    Item,
    Drive,
    Computer
}

public class ItemsChangedArgs : EventArgs
{
    public bool HasBeenAdded { get; set; } = false;
    public bool HasBeenRemoved { get; set; } = false;
    public bool HasBeenEdited { get; set; } = false;

    /// <summary>
    /// Its null if a item has been deleted
    /// </summary>
    public CryptoItem? Item { get; set; } = null;

    /// <summary>
    /// Its null if no item was removed
    /// </summary>
    public Guid? RemovedItemId { get; set; } = null;
}
