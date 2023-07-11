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
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NoJoy
{
    public static class PowerShell
    {
        private const string elevatedVerb = "runas";
        private const int timeoutMilliseconds = 30_000;

        public static PowerShellOperationResult RunElevated(string cmd)
        {
            PowerShellOperationResult result;
            string tempPath = Path.GetTempFileName();
            string args = $"-NoProfile -NonInteractive -Command {cmd} 2>&1 > {tempPath}";
            var pi = new ProcessStartInfo("powershell.exe", args) {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = elevatedVerb,
                UseShellExecute = true,
                CreateNoWindow = true,
            };
            var process = Process.Start(pi);
            if (!process.WaitForExit(timeoutMilliseconds))
            {
                string errorMessage = "PowerShell timed out";
                Debug.WriteLine(errorMessage);
                result = new PowerShellOperationResult(false, errorMessage);
            }
            else if (process.ExitCode != 0)
            {
                string errorMessage = File.ReadLines(tempPath).FirstOrDefault();
                Debug.WriteLine($"Error disabling the device: {errorMessage}");
                result = new PowerShellOperationResult(false, errorMessage ?? "-no error message generated-");
            }
            else
            {
                result = new PowerShellOperationResult(true);
            }
            return result;
        }
    }
}