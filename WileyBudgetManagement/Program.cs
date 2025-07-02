using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WileyBudgetManagement.Forms;

namespace WileyBudgetManagement;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Set up global exception handling
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += Application_ThreadException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        try
        {
            Console.WriteLine("Starting Wiley Budget Management Application...");
            Console.WriteLine($"Current Time: {DateTime.Now}");
            Console.WriteLine($"Working Directory: {Environment.CurrentDirectory}");

            // Enable console for debugging
            AllocConsole();

            // Register Syncfusion license key
            try
            {
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzkzMTc1M0AzMjM2MmUzMDJlMzBrQ0Q3YU1NR0JkaWJmdUsvQWQ2U3Erd0R6Q3VyVHE0eGJvSGtlYkpqZFVZPQ==");
                Console.WriteLine("Syncfusion license registered successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Syncfusion license registration warning: {ex.Message}");
                LogException("Syncfusion License", ex);
                // Continue anyway - this shouldn't prevent the app from starting
            }

            // Enable Windows visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // SQL Server schema setup: see Database/Setup.sql for fields: Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput, SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining, GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3, PercentAllocation, RequiredRate, MonthlyUsage, TimeOfUseFactor, CustomerAffordabilityIndex, QuarterlySummary, EntryDate, Total.

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Console.WriteLine("Creating Test Runner...");

            // Launch the TestRunner instead of the main application form
            Application.Run(new TestRunner());

            Console.WriteLine("Application ended normally");
        }
        catch (Exception ex)
        {
            var errorMessage = $"Application startup error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
            Console.WriteLine(errorMessage);

            // Show error dialog if possible
            try
            {
                MessageBox.Show(errorMessage, "Wiley Budget Management - Startup Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                // If MessageBox fails, just write to console
                Console.WriteLine("Failed to show error dialog");
            }

            // Exit with error code
            Environment.Exit(1);
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        var errorMessage = $"Thread Exception: {e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}";
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {errorMessage}");
        LogException("Thread Exception", e.Exception);

        // Show error dialog
        var result = MessageBox.Show($"An error occurred:\n{e.Exception.Message}\n\nWould you like to continue?",
            "Application Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

        if (result == DialogResult.No)
        {
            Application.Exit();
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        var errorMessage = $"Unhandled Exception: {ex?.Message ?? e.ExceptionObject.ToString()}\n\nStack Trace:\n{ex?.StackTrace ?? "N/A"}";
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {errorMessage}");

        if (ex != null)
            LogException("Unhandled Exception", ex);

        // Show error dialog
        MessageBox.Show($"A critical error occurred:\n{ex?.Message ?? "Unknown error"}\n\nThe application will now close.",
            "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);

        Environment.Exit(1);
    }

    private static void LogException(string context, Exception ex)
    {
        try
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WileyBudgetManagement", "Logs");
            Directory.CreateDirectory(logPath);

            var logFile = Path.Combine(logPath, $"error_{DateTime.Now:yyyyMMdd}.log");
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}: {ex.Message}\nStack Trace:\n{ex.StackTrace}\n\n";

            File.AppendAllText(logFile, logEntry);
            Console.WriteLine($"Error logged to: {logFile}");
        }
        catch (Exception logEx)
        {
            Console.WriteLine($"Failed to log exception: {logEx.Message}");
        }
    }
}