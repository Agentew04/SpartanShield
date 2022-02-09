using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpartanShield;

public class FileManager
{
    private static void CheckFile()
    {
        string[] paths = new[]
        {
                Utils.UsersFile,
                Utils.ItemsFile
            };
        if (!Directory.Exists(Utils.AppFolder))
        {
            Directory.CreateDirectory(Utils.AppFolder);
        }
        foreach (string path in paths)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }
    }

    #region itemsfile


    /// <summary>
    ///  Read the contents of the file
    /// </summary>
    /// <returns></returns>
    private static List<CryptoItem> ReadItemsFile()
    {
        CheckFile();
        var json = File.ReadAllText(Utils.ItemsFile);
        var itens = JsonConvert.DeserializeObject<List<CryptoItem>>(json);
        return itens ?? new();
    }

    /// <summary>
    /// Writes the list to the ItemsFile
    /// </summary>
    /// <param name="items"></param>
    private static void WriteItemsFile(List<CryptoItem> items)
    {
        CheckFile();
        var json = JsonConvert.SerializeObject(items);
        File.WriteAllText(Utils.ItemsFile, json);
    }

    /// <summary>
    /// Returns all crypto items, including USB ones
    /// </summary>
    /// <returns></returns>
    public static List<CryptoItem> GetItems()
    {
        return ReadItemsFile();
    }

    /// <summary>
    /// Adds a CryptoItem to the current persistent storage
    /// </summary>
    /// <param name="item"></param>
    public static void AddItem(CryptoItem item)
    {
        var list = GetItems();
        var exists = list.Any(x => x.Id == item.Id);
        if (exists) list[list.FindIndex(x => x.Id == item.Id)] = item;
        else list.Add(item);
        WriteItemsFile(list);
        RaiseItemsChanged(new()
        {
            HasBeenAdded = true
        });
    }

    /// <summary>
    /// Adds many items to the persistent storage
    /// </summary>
    /// <param name="items">The items to be added</param>
    public static void AddItem(IEnumerable<CryptoItem> items)
    {
        var list = GetItems();
        foreach (var item in items)
        {
            var exists = list.Any(x => x.Id == item.Id);
            if (exists) list[list.FindIndex(x => x.Id == item.Id)] = item;
            else list.Add(item);
        }
        WriteItemsFile(list);
        RaiseItemsChanged(new()
        {
            HasBeenAdded = true
        });
    }

    /// <summary>
    /// Removes a CryptoItem from the persistent storage with the specificed Id
    /// </summary>
    /// <param name="id">The Id that will be removed</param>
    /// <returns>A bool representing if the item was removed or not</returns>
    public static bool RemoveItem(Guid id)
    {
        var list = GetItems();
        var removed = list.RemoveAll(x => x.Id == id) != 0;
        WriteItemsFile(list);
        RaiseItemsChanged(new()
        {
            HasBeenRemoved = true
        });
        return removed;
    }

    /// <summary>
    /// Removes many items from persistent storage based on their ids
    /// </summary>
    /// <param name="ids">The ids that will be removed</param>
    /// <returns>How many items were removed</returns>
    public static int RemoveItem(IEnumerable<Guid> ids)
    {
        var list = GetItems();
        var removed = list.RemoveAll(x => ids.Contains(x.Id));
        RaiseItemsChanged(new()
        {
            HasBeenRemoved = true
        });
        return removed;
    }

    #endregion itemsfile

    #region event

    public delegate void ItemsChangedHandler(object sender, ItemsChangedArgs e);

    /// <summary>
    /// Is raised when the persistent storage changes
    /// </summary>
    public static event ItemsChangedHandler? ItemsChanged;

    protected static void RaiseItemsChanged(ItemsChangedArgs args)
    {
        ItemsChanged?.Invoke(new(), args);
    }

    #endregion
}
public class ItemsChangedArgs : EventArgs
{
    public bool HasBeenAdded { get; set; } = false;
    public bool HasBeenRemoved { get; set; } = false;
    public bool HasBeenEdited { get; set; } = false;
}
