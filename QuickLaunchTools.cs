using System;
using System.Windows.Forms;

/// <summary>
/// Quick launcher for Town of Wiley Tools Menu
/// Compile and run: csc /target:winexe QuickLaunchTools.cs
/// </summary>
class QuickLaunchTools
{
    [STAThread]
    static void Main()
    {
        try
        {
            Console.WriteLine("🏛️ Launching Town of Wiley Tools Menu...");

            // This would reference the actual ToolMenuForm when compiled together
            // For now, show a simplified menu
            ShowSimplifiedMenu();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Launch failed: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    static void ShowSimplifiedMenu()
    {
        var menu = $@"🏛️ TOWN OF WILEY DEVELOPMENT TOOLS MENU

🔍 CODE ANALYSIS & IMPROVEMENT
   • Run Task Examine - Code quality analysis
   • Code Improvement Manager - Review improvements
   • Quick Console Analyzer - Fast analysis
   • Simplified Improvement Tool - Windows Forms UI

🏛️ MUNICIPAL COMPLIANCE  
   • Municipal Audit Logger Test - Test logging
   • API Key Validator - Validate xAI keys
   • Rate Study Validator - Methodology compliance
   • Municipal Context Checker - Verify configuration

🤖 AI SERVICE TOOLS
   • Test xAI Connection - Service connectivity
   • Token Usage Reporter - Cost analysis
   • AI Query Tester - Interactive testing
   • Model Configuration Manager - AI models

🗄️ DATABASE & INTEGRATION
   • SQL Server Express Setup - Database config
   • Database Schema Generator - Municipal schema
   • Syncfusion License Checker - Component check
   • Connection String Builder - Database connections

🔧 DEVELOPMENT & BUILD
   • Project Builder - Build solution
   • NuGet Package Manager - Dependencies
   • Environment Setup - Dev environment
   • Deployment Packager - Package for deploy

⚙️ UTILITIES & HELPERS
   • Log Viewer - View audit logs
   • Configuration Editor - Edit settings
   • File Generator - Generate templates
   • Backup Creator - Project backups

Status: All tools available for Town of Wiley municipal operations!";

        MessageBox.Show(menu, "Town of Wiley Tools Menu",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
