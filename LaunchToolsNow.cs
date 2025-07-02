using System;
using System.Windows.Forms;
using WileyBudgetManagement.Tools;

/// <summary>
/// Immediate launcher for Town of Wiley Tools Menu
/// Run this to instantly launch the development tools
/// </summary>
class LaunchToolsNow
{
    [STAThread]
    static void Main()
    {
        Console.WriteLine("🏛️ TOWN OF WILEY BUDGET MANAGEMENT");
        Console.WriteLine("🚀 LAUNCHING DEVELOPMENT TOOLS MENU");
        Console.WriteLine("=" + new string('=', 50));
        Console.WriteLine();

        try
        {
            // Launch the tools menu immediately
            ToolMenuLauncher.LaunchNow();

            Console.WriteLine("📋 Tools menu is now active!");
            Console.WriteLine("Press any key to keep console open...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
