using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Usb.Events;

namespace SpartanShield
{
    /// <summary>
    /// A class to integrate <see cref="UsbDevice"/> and <see cref="DriveInfo"/>
    /// </summary>
    public class Usb
    {
        #region fields

        private readonly UsbDevice? usbDevice;
        private readonly DriveInfo? driveInfo;

        #endregion

        #region properties

        /// <summary>
        /// Returns the full root directory of this usb. If its invalid, returns <see cref="string.Empty"/>
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// Returns the letter of this Usb device
        /// </summary>
        public string Letter { get; set; }

        /// <summary>
        /// The universal Id for this pendrive, store under the .spartan file
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// If the USB is connected and ready to use
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (usbDevice != null) return usbDevice.IsMounted && !usbDevice.IsEjected;
                if (driveInfo != null) return driveInfo.IsReady;
                return false;
            }
        }

        /// <summary>
        /// Represents if the USB is currently encrypted or decrypted
        /// </summary>
        public bool IsDecrypted { get; set; }

        /// <summary>
        /// The list of items contained in this pen drive
        /// </summary>
        public List<CryptoItem> Items { get; set; } = new();

        #endregion

        #region constructors

        public Usb(DriveInfo driveInfo)
        {
            this.driveInfo = driveInfo;

            // get root dir
            RootDirectory = driveInfo.Name;

            // get letter
            Regex letterRegex = new(@"\w(?=:\\)");
            Letter = letterRegex.Match(driveInfo.Name).Value;

            LoadInfo();
        }

        public Usb(UsbDevice usbDevice)
        {
            this.usbDevice = usbDevice;

            // get root dir
            RootDirectory = $"{usbDevice.MountedDirectoryPath}\\";

            // get letter
            Regex letterRegex = new(@"\w(?=:\\)");
            Letter = letterRegex.Match(RootDirectory).Value;

            LoadInfo();
        }

        #endregion

        #region methods

        private void LoadInfo()
        {
            var path = $"{RootDirectory}.spartan";
            using FileStream fileStream = new(path, FileMode.Open);
            using BinaryReader reader = new(fileStream);
            Id = reader.ReadGuid();
            IsDecrypted = reader.ReadBoolean();
            Items = ReadItemsList(reader);

        }

        /// <summary>
        /// Saves the current Usb(object) state to the Usb(drive) file
        /// </summary>
        public void SaveInfo()
        {
            var path = $"{RootDirectory}.spartan";
            using FileStream fileStream = new(path, FileMode.OpenOrCreate, FileAccess.Write);
            using BinaryWriter writer = new(fileStream);
            writer.Write(Id);
            writer.Write(IsDecrypted);
            WriteItemsList(writer, Items);
        }

        private static List<CryptoItem> ReadItemsList(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            var items = new List<CryptoItem>();
            for (int i = 0; i < count; i++) items.Add(reader.ReadCryptoItem());
            return items;
        }

        private static void WriteItemsList(BinaryWriter writer, List<CryptoItem> list)
        {
            writer.Write(list.Count);
            foreach (var item in list) writer.Write(item);
        }
        
        #endregion

    }
}
