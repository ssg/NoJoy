using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NoJoy
{
    public static class PowerShell
    {
        private const string elevatedVerb = "runas";
        private const int timeoutMilliseconds = 30_000;

        public static bool RunElevated(string cmd)
        {
            var tempPath = Path.GetTempFileName();
            string args = $"-NoProfile -NonInteractive -Command {cmd} 2>&1 > {tempPath}";
            var pi = new ProcessStartInfo("powershell.exe", args)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = elevatedVerb,
                UseShellExecute = true,
                CreateNoWindow = true,
            };
            var process = Process.Start(pi);
            if (!process.WaitForExit(timeoutMilliseconds))
            {
                Debug.WriteLine("PowerShell timed out");
                return false;
            }
            if (process.ExitCode != 0)
            {
                string errorMessage = File.ReadLines(tempPath).First();
                Debug.WriteLine($"Error disabling the device: {errorMessage}");
                return false;
            }
            return true;
        }
    }
}