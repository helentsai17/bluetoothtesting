using System;
using System.Collections.Generic;
using System.Text;

namespace Blueberry.Desktop.WindowsApp.Bluetooth
{
    public class DnaBluetoothLEDevice
    {
        /// <summary>
        /// the time of the broadcast advertisment message of the device 
        /// </summary>
        public DateTimeOffset BroadcastTime { get; }
        /// <summary>
        /// the address of the device 
        /// </summary>
        public ulong Address { get; }

        /// <summary>
        /// the name of the device
        /// </summary>
        public string Name { get; }

        public short SignalStrengthInDB { get; }

        public bool Connected { get; }

        public bool CanPair { get; }

        public bool Pair { get; }

        public string DeviceID { get; }


        public DnaBluetoothLEDevice(ulong address, 
            string name, 
            short rssi, 
            DateTimeOffset broadcastTime,
            bool connect,
            bool canpair,
            bool paired,
            string deviceId)
        {
            Address = address;
            Name = name;
            BroadcastTime = broadcastTime;
            SignalStrengthInDB = rssi;
            Connected = connect;
            CanPair = canpair;
            Pair = paired;
            DeviceID = deviceId;
        }

        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(Name) ? "[No Name]" : Name)}  [{DeviceID}] ({SignalStrengthInDB})";
        }
    }
}
