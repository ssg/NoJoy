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
using System.Text;

namespace NoJoy
{
    public static class PnpUtil
    {
        private const string elevatedVerb = "runas";
        private const int timeoutMilliseconds = 30_000;

        public static ShellResult RunElevated(string args)
        {
            var pi = new ProcessStartInfo("pnputil.exe", args)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = elevatedVerb,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };
            var process = new Process
            {
                StartInfo = pi
            };
            var stdout = new StringBuilder();
            process.OutputDataReceived += (sender, arg) => stdout.AppendLine(arg.Data);
            process.ErrorDataReceived += (sender, arg) => stdout.AppendLine(arg.Data);
            _ = process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (!process.WaitForExit(timeoutMilliseconds))
            {
                string errorMessage = "Execution timeout";
                Debug.WriteLine(errorMessage);
                return new ShellResult(false, errorMessage);
            }

            if (process.ExitCode != 0)
            {
                string errorMessage = stdout.ToString().Trim();
                Debug.WriteLine($"Error disabling the device: {errorMessage}");
                return new ShellResult(false, errorMessage);
            }

            return new ShellResult(success: true);
        }
    }
}