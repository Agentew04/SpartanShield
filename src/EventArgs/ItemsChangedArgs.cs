using System;
using System.Collections.Generic;
using System.Linq;

namespace SpartanShield.EventArguments;

public class ItemsChangedArgs : EventArgs
{
    public bool HasBeenAdded { get; set; } = false;
    public bool HasBeenRemoved { get; set; } = false;
    public bool HasBeenEdited { get; set; } = false;

    public bool IsBulk { get; set; } = false;

    /// <summary>
    /// Its null if a item has been deleted
    /// </summary>
    public CryptoItem? Item { get; set; } = null;

    /// <summary>
    /// Its <see cref="Enumerable.Empty{CryptoItem}"/> if <see cref="IsBulk"/> is false
    /// </summary>
    public IEnumerable<CryptoItem> Items { get; set; } = Enumerable.Empty<CryptoItem>();

    /// <summary>
    /// Its null if no item was removed
    /// </summary>
    public Guid? RemovedItemId { get; set; } = null;
}

