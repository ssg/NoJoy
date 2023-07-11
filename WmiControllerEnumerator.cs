/*
Copyright 2018 Sedat Kapanoglu

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace NoJoy
{
    internal class WmiControllerEnumerator
    {
        private const string gameControllerId = "HID_DEVICE_SYSTEM_GAME";
        private const string saitekHidSuffix = " (HID)";
        private const string usbSuffix = " (USB)";

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
            const string hidQuery = @"SELECT Manufacturer,Name,HardwareID,DeviceID,Status " +
                "FROM Win32_PnPEntity WHERE PNPClass='HIDClass'";

            var searcher = new ManagementObjectSearcher(hidQuery);
            var results = searcher.Get();
            var controllers = new MainWindowViewModel();
            foreach (var result in results)
            {
                var device = new DeviceInfo(result);
                if (!device.IsGameController)
                {
                    continue;
                }
                Debug.WriteLine($"Testing game controller: {device.FullName}");

                // Saitek hack to identify correct device
                var newDevice = identifySaitekParent(device);
                if (newDevice != null)
                {
                    device = newDevice;
                }

                controllers.Items.Add(device.ToGameController());
                Debug.WriteLine($"Added {device.FullName}");
            }
            return controllers;
        }

        private static DeviceInfo identifySaitekParent(DeviceInfo device)
        {
            if (!device.Name.EndsWith(saitekHidSuffix))
            {
                return null;
            }
            Debug.WriteLine($"Saitek hack: looking for HID'less parent for '{device.FullName}'");

            // Saitek X55
            string suffixlessName = device.Name.Replace(saitekHidSuffix, String.Empty);

            // Saitek X52
            string nameWithUsbSuffix = device.Name.Replace(saitekHidSuffix, usbSuffix);

            var newDevice = findDeviceByNames(suffixlessName, nameWithUsbSuffix);
            if (newDevice != null)
            {
                Debug.WriteLine($"Identified correct parent as '{newDevice.FullName}'");
                return newDevice;
            }

            Debug.WriteLine($"Saitek hack: failed to identify correct parent for '{device.FullName}'");
            return null;
        }

        private static DeviceInfo findDeviceByNames(params string[] validNames)
        {
            string whereClause = String.Join(" OR ", validNames.Select(n => $"Name='{n}'"));
            string usbQuery = "SELECT Manufacturer,Name,HardwareID,DeviceID,Status " +
                $"FROM Win32_PnPEntity WHERE {whereClause}";
            var searcher = new ManagementObjectSearcher(usbQuery);
            var results = searcher.Get();
            foreach (var result in results)
            {
                // return the first result as we favor suffixless over (USB) suffix
                return new DeviceInfo(result);
            }
            return null;
        }
    }
}