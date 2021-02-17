using Blueberry.Desktop.WindowsApp.Bluetooth;
using System;

namespace Blueberry.Desktop.ConsolePlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var watcher = new DnaBluetoothLEAdvertisementWatcher(new GattServiceIds());

            watcher.StartedListening += () =>
            {
                Console.WriteLine("Started listening");
            };

            watcher.StoppedListening += () =>
            {
                Console.WriteLine("Stopped listening");
            };

            watcher.NewDeviceDiscovered += (device) =>
            {
                Console.WriteLine($"New device: {device}");
            };

            watcher.DeviceNameChanged += (device) =>
            {
                Console.WriteLine($"Device name Changed : {device}");
            };

            watcher.StartListening();

            Console.ReadLine();


        }
    }
}
