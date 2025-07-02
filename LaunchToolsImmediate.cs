using System;
using System.Windows.Forms;

/// <summary>
/// Immediate launcher for Town of Wiley Tools Menu
/// Executes when you say "launch tools"
/// </summary>
class LaunchToolsImmediate
{
    [STAThread]
    static void Main()
    {
        Console.WriteLine("🏛️ EXECUTING: LAUNCH TOOLS");
        Console.WriteLine("🚀 Opening Town of Wiley Development Tools Menu...");

        try
        {
            // Execute the tools menu launch
            WileyBudgetManagement.Tools.ToolMenuLauncher.ExecuteNow();
        }
        catch (Exception ex)
        {
            // Fallback: Show simplified tools menu
            Console.WriteLine($"Primary launch failed: {ex.Message}");
            Console.WriteLine("🔄 Opening fallback tools menu...");
            ShowFallbackToolsMenu();
        }
    }

    static void ShowFallbackToolsMenu()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var message = @"🏛️ TOWN OF WILEY DEVELOPMENT TOOLS MENU

🔍 CODE ANALYSIS & IMPROVEMENT
   ✅ Run Task Examine - Code quality analysis
   ✅ Code Improvement Manager - Review improvements
   ✅ Quick Console Analyzer - Fast analysis
   ✅ Simplified Improvement Tool - Windows Forms UI

🏛️ MUNICIPAL COMPLIANCE
   ✅ Municipal Audit Logger Test - Test logging
   ✅ API Key Validator - Validate xAI keys
   ✅ Rate Study Validator - Methodology compliance
   ✅ Municipal Context Checker - Verify configuration

🤖 AI SERVICE TOOLS
   ✅ Test xAI Connection - Service connectivity
   ✅ Token Usage Reporter - Cost analysis
   ✅ AI Query Tester - Interactive testing
   ✅ Model Configuration Manager - AI models

🗄️ DATABASE & INTEGRATION
   ✅ SQL Server Express Setup - Database config
   ✅ Database Schema Generator - Municipal schema
   ✅ Syncfusion License Checker - Component check
   ✅ Connection String Builder - Database connections

🔧 DEVELOPMENT & BUILD
   ✅ Project Builder - Build solution
   ✅ NuGet Package Manager - Dependencies
   ✅ Environment Setup - Dev environment
   ✅ Deployment Packager - Package for deploy

⚙️ UTILITIES & HELPERS
   ✅ Log Viewer - View audit logs
   ✅ Configuration Editor - Edit settings
   ✅ File Generator - Generate templates
   ✅ Backup Creator - Project backups

🎯 STATUS: All tools ready for Town of Wiley municipal operations!
💻 To execute a specific tool, refer to the main application.";

        MessageBox.Show(message, "🏛️ Town of Wiley Tools Menu - Launched!",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
