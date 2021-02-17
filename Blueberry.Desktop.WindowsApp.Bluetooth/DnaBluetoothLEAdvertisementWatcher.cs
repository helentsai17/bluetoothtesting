using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Blueberry.Desktop.WindowsApp.Bluetooth
{
    public class DnaBluetoothLEAdvertisementWatcher
    {
        private readonly BluetoothLEAdvertisementWatcher mWatcher;

        private readonly Dictionary<string, DnaBluetoothLEDevice> mDiscoveredDevices = new Dictionary<string, DnaBluetoothLEDevice>();

        private readonly GattServiceIds mGattServiceIds;

        private readonly object mThreadLock = new object();
        public bool Listening => mWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;


        public IReadOnlyCollection<DnaBluetoothLEDevice> DiscoveredDevices
        {
            get
            {
                lock (mThreadLock)
                {
                    //convert to read only list 
                    return mDiscoveredDevices.Values.ToList().AsReadOnly();
                }
            }
        }

        public event Action StoppedListening = () => { };

        public event Action StartedListening = () => { };

        public event Action<DnaBluetoothLEDevice> DeviceDiscovered = (device) => { };

        public event Action<DnaBluetoothLEDevice> NewDeviceDiscovered = (device) => { };

        public event Action<DnaBluetoothLEDevice> DeviceNameChanged = (device) => { };


        public DnaBluetoothLEAdvertisementWatcher(GattServiceIds gattIds)
        {

            mGattServiceIds = gattIds ?? throw new ArgumentNullException(nameof(gattIds));
            mWatcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };

            mWatcher.Received += WatcherAdvertisementReceivedAsync;

            //Listen out for when the watcher stops listening
            mWatcher.Stopped += (watcher, e) =>
            {
                StoppedListening();
            };

        }

        private async void WatcherAdvertisementReceivedAsync(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var device = await GetBluetoothLEDeviceAsync(
                args.BluetoothAddress,
                args.Timestamp,
                args.RawSignalStrengthInDBm);

            if (device == null)
                return;

            var newDiscovery = false;
            var existingName = default(string);

            lock (mThreadLock)
            {
                newDiscovery = !mDiscoveredDevices.ContainsKey(device.DeviceID);
                if (!newDiscovery)
                {
                    existingName = mDiscoveredDevices[device.DeviceID].Name;
                } 
            }

            //if not discovery yet than it's new


            var nameChanged = !newDiscovery && !string.IsNullOrEmpty(device.Name) &&
                existingName != device.Name;

            lock (mThreadLock)
            {
               
                mDiscoveredDevices[device.DeviceID] = device;
            }

            DeviceDiscovered(device);

            if (nameChanged)
                DeviceNameChanged(device);

            if (newDiscovery)
                NewDeviceDiscovered(device);

        }


        private async Task<DnaBluetoothLEDevice> GetBluetoothLEDeviceAsync(ulong address, DateTimeOffset broadcastTime, short rssi)
        {
            ;
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(address).AsTask();

            if (device == null)
                return null;

            // device name
            // var name = device.Name;

           
            var gatt = await device.GetGattServicesAsync().AsTask();

            
            
            //if there have any service 
            if(gatt.Status == GattCommunicationStatus.Success)
            {
                //loop into each GATT service 
                foreach (var service in gatt.Services)
                {
                    var gattProfileId = service.Uuid;
                    
                }
            }
            return new DnaBluetoothLEDevice
                (
                deviceId: device.DeviceId,
                address: device.BluetoothAddress,
                name:device.Name,
                broadcastTime: broadcastTime,
                rssi: rssi,
                connect:device.ConnectionStatus == BluetoothConnectionStatus.Connected,
                canpair: device.DeviceInformation.Pairing.CanPair,
                paired:device.DeviceInformation.Pairing.IsPaired
                );
        }

        // starts listening for advertisement
        public void StartListening()
        {
            // if already listening 
            if (Listening)
                return;

            mWatcher.Start();

            StartedListening();
        }

        //stops listening for advertisment
        public void StopListening()
        {
            if (!Listening)
                return;

            mWatcher.Stop();
        }
    }
}
