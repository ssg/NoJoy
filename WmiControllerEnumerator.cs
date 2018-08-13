using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace NoJoy
{
    internal class WmiControllerEnumerator
    {
        private const string gameControllerId = "HID_DEVICE_SYSTEM_GAME";

        private const string hidQuery =
            @"SELECT Manufacturer,Name,HardwareID,DeviceID,Status FROM Win32_PnPEntity
              WHERE PNPClass='HIDClass'";
        private const string saitekHidSuffix = " (HID)";

        private class DeviceInfo
        {
            public string[] HardwareIds { get; set; }
            public string Manufacturer { get; set; }
            public string Name { get; set; }
            public string DeviceId { get; set; }
            public string Status { get; set; }

            public string FullName => $"{Manufacturer} {Name}";

            public GameControllerState State => Status == "OK" ? GameControllerState.Enabled
                                                               : GameControllerState.Disabled;

            public bool IsGameController => HardwareIds.Contains(gameControllerId);

            public DeviceInfo(ManagementBaseObject result)
            {
                Manufacturer = result["Manufacturer"].ToString();
                Name = result["Name"].ToString();
                HardwareIds = (string[])result["HardwareID"];
                DeviceId = result["DeviceId"].ToString();
                Status = result["Status"].ToString();
            }

            public GameController ToGameController()
            {
                return new GameController
                {
                    Name = Name,
                    DeviceId = DeviceId,
                    State = State,
                };
            }
        }

        public MainWindowViewModel GetControllers()
        {
            var searcher = new ManagementObjectSearcher(hidQuery);
            var results = searcher.Get();
            var controllers = new MainWindowViewModel();
            foreach (var result in results)
            {
                var device = new DeviceInfo(result);
                Debug.WriteLine($"Testing: {device.FullName}");
                if (!device.IsGameController)
                {
                    continue;
                }

                // Saitek hack to identify correct device
                if (device.Name.EndsWith(saitekHidSuffix))
                {
                    var newDevice = identifySaitekParent(results, device);
                    if (newDevice != null)
                    {
                        device = newDevice;
                    }
                }

                controllers.Items.Add(device.ToGameController());
                Debug.WriteLine($"Added {device.FullName}");
            }
            return controllers;
        }

        private static DeviceInfo identifySaitekParent(ManagementObjectCollection results, DeviceInfo device)
        {
            Debug.WriteLine($"Saitek hack: looking for HID'less parent for {device.FullName}");
            string searchFor = device.Name.Replace(saitekHidSuffix, String.Empty);
            DeviceInfo newDevice = null;
            foreach (var subResult in results)
            {
                var newInfo = new DeviceInfo(subResult);
                if (newInfo.Name == searchFor)
                {
                    newDevice = newInfo;
                    break;
                }
            }
            if (newDevice == null)
            {
                Debug.WriteLine($"Saitek hack: failed to identify correct parent for {device.FullName}");
                return null;
            }
            Debug.WriteLine($"Identified correct parent as {newDevice.FullName}");
            return newDevice;
        }
    }
}