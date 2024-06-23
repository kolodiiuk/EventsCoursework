using System;
using System.Diagnostics;
using Events.Utilities;

namespace Events.Models;

public class NotificationManager
{
    public static Result ShowNotification(string title, string message)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo(
                "notify-send", $"\"{title}\" \"{message}\"")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            var process = Process.Start(processStartInfo);
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                return Result.Success("Notification shown successfully.");
            }

            return Result.Fail(
                "Error showing notification. Exit code: " + process.ExitCode);
        }
        catch (Exception ex)
        {
            return Result.Fail("Error showing notification: " + ex.Message);
        }
    }
}
