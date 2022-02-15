using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Usb.Events;

namespace SpartanShield.Managers;

public static class USBManager
{
    public static List<Usb> UsbDevices { get; set; } = new();
    private static IUsbEventWatcher UsbEventWatcher { get; set; } = new UsbEventWatcher();


    public static void StartManager()
    {
        UsbEventWatcher.UsbDeviceAdded += Plugged;
        UsbEventWatcher.UsbDeviceRemoved += Unplugged;
        UsbDevices.AddRange(DriveInfo.GetDrives()                                        // get all drives and adds it to the list
                            .Where(x => x.DriveType == DriveType.Removable && x.IsReady) // selects only usable usbs
                            .Select(x => new Usb(x)));                                   // converts to Usb class
        var newItems = UsbDevices.Where(x => x.Items != null && x.Items.Count > 0)
            .SelectMany(x => x.Items);

        DatabaseManager.AddItem(newItems);
    }

    public static void Plugged(object? _, UsbDevice usbDevice)
    {
        Usb usb = new(usbDevice);
        UsbDevices.Add(usb);
        var path = $"{usb.RootDirectory}";
        if (path == "") return; // it's not a usb flash drive 

        // its a compatible usb driv

        DatabaseManager.AddIdMapping(usb.Id, ObjectType.Drive);
        foreach (var item in usb.Items) DatabaseManager.AddItem(item);

    }

    public static void Unplugged(object? _, UsbDevice e)
    {
        // remove unplugged usbs
        List<Usb> toBeRemovedDrives = new();
        foreach (Usb usb in UsbDevices) if (!usb.IsConnected) toBeRemovedDrives.Add(usb);
        foreach (Usb usbDevice in toBeRemovedDrives) UsbDevices.Remove(usbDevice);

        // delete the entries that were in the usb from the database
        SyncAllUSBDrives(toBeRemovedDrives);
    }

    private static void SyncAllUSBDrives(List<Usb> removedUsbs)
    {
        var ids = removedUsbs
            .SelectMany(usb => usb.Items)
            .Select(usb => usb.Id);
        DatabaseManager.RemoveItem(ids);
    }
}
