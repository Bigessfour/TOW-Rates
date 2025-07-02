using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace WileyBudgetManagement
{
    public static class DebugHelper
    {
        private static readonly string LogDirectory = @"C:\temp\WileyDebug";
        private static readonly string MainLogFile = Path.Combine(LogDirectory, "dashboard_debug.log");
        private static readonly string ErrorLogFile = Path.Combine(LogDirectory, "errors.log");
        private static readonly string PerformanceLogFile = Path.Combine(LogDirectory, "performance.log");

        static DebugHelper()
        {
            Directory.CreateDirectory(LogDirectory);
        }

        public static void LogButtonClick(string buttonText)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var message = $"[{timestamp}] 🔘 Button clicked: {buttonText}\n";

            File.AppendAllText(MainLogFile, message);
            Console.WriteLine(message.TrimEnd());
        }

        public static void LogAction(string actionName, string details = "")
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var message = $"[{timestamp}] ⚡ {actionName}";
            if (!string.IsNullOrEmpty(details))
                message += $" - {details}";
            message += "\n";

            File.AppendAllText(MainLogFile, message);
            Console.WriteLine(message.TrimEnd());
        }

        public static void LogError(Exception ex, string context = "")
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var separator = new string('=', 60);

            var errorDetails = $"\n{separator}\n" +
                              $"[{timestamp}] *** CRITICAL ERROR ***\n" +
                              $"Context: {context}\n" +
                              $"Type: {ex.GetType().Name}\n" +
                              $"Message: {ex.Message}\n" +
                              $"Stack trace:\n{ex.StackTrace}\n";

            if (ex.InnerException != null)
            {
                errorDetails += $"Inner exception: {ex.InnerException.GetType().Name}\n" +
                               $"Inner message: {ex.InnerException.Message}\n";
            }

            errorDetails += $"{separator}\n\n";

            // Log to both main log and dedicated error log
            File.AppendAllText(MainLogFile, errorDetails);
            File.AppendAllText(ErrorLogFile, errorDetails);
            Console.WriteLine(errorDetails);
        }

        public static void LogPerformance(string operation, TimeSpan duration)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var message = $"[{timestamp}] ⏱️ {operation}: {duration.TotalMilliseconds:F2}ms\n";

            File.AppendAllText(PerformanceLogFile, message);
            Console.WriteLine(message.TrimEnd());
        }

        public static void LogFormCreation(string formType, bool success, Exception? error = null)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var status = success ? "✅ SUCCESS" : "❌ FAILED";
            var message = $"[{timestamp}] 🖼️ Form Creation - {formType}: {status}\n";

            File.AppendAllText(MainLogFile, message);
            Console.WriteLine(message.TrimEnd());

            if (!success && error != null)
            {
                LogError(error, $"Form creation failed: {formType}");
            }
        }

        public static void ClearLogs()
        {
            try
            {
                if (File.Exists(MainLogFile)) File.Delete(MainLogFile);
                if (File.Exists(ErrorLogFile)) File.Delete(ErrorLogFile);
                if (File.Exists(PerformanceLogFile)) File.Delete(PerformanceLogFile);

                LogAction("Logs cleared", "Fresh debugging session started");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clear logs: {ex.Message}");
            }
        }

        public static void OpenLogDirectory()
        {
            try
            {
                Process.Start("explorer.exe", LogDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open log directory: {ex.Message}");
            }
        }

        public static void ShowErrorSummary()
        {
            try
            {
                if (!File.Exists(ErrorLogFile))
                {
                    MessageBox.Show("No errors logged yet!", "Error Summary",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var errors = File.ReadAllText(ErrorLogFile);
                var errorCount = errors.Split(new[] { "CRITICAL ERROR" }, StringSplitOptions.None).Length - 1;

                var summary = $"Total Errors Found: {errorCount}\n\n" +
                             $"Log Files Location: {LogDirectory}\n\n" +
                             "Recent errors:\n" +
                             errors.Substring(Math.Max(0, errors.Length - 1000));

                MessageBox.Show(summary, "Error Summary",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to show error summary: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
