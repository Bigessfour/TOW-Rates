using System;
using System.IO;
using System.Windows.Forms;
using WileyBudgetManagement.Tools;

namespace WileyBudgetManagement
{
    /// <summary>
    /// Launcher for the Town of Wiley Code Improvement Tool
    /// Simple entry point for analyzing and improving municipal software
    /// </summary>
    class ImprovementToolLauncher
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Console.WriteLine("🏛️  TOWN OF WILEY BUDGET MANAGEMENT - CODE IMPROVEMENT TOOL");
            Console.WriteLine("=" + new string('=', 70));

            // Default target file
            var defaultTarget = @"c:\Users\steve.mckitrick\Desktop\Rate Study\WileyBudgetManagement\AIEnhancedQueryService.cs";

            string targetFile;

            // Allow command line argument or use default
            if (args.Length > 0 && File.Exists(args[0]))
            {
                targetFile = args[0];
                Console.WriteLine($"📁 Analyzing file from command line: {Path.GetFileName(targetFile)}");
            }
            else if (File.Exists(defaultTarget))
            {
                targetFile = defaultTarget;
                Console.WriteLine($"📁 Analyzing default target: {Path.GetFileName(targetFile)}");
            }
            else
            {
                // Let user select file
                using var openDialog = new OpenFileDialog
                {
                    Title = "Select C# File to Analyze",
                    Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
                    InitialDirectory = @"c:\Users\steve.mckitrick\Desktop\Rate Study"
                };

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    targetFile = openDialog.FileName;
                    Console.WriteLine($"📁 Analyzing selected file: {Path.GetFileName(targetFile)}");
                }
                else
                {
                    Console.WriteLine("❌ No file selected. Exiting...");
                    return;
                }
            }

            try
            {
                Console.WriteLine("🔍 Starting analysis...");

                // Create the improvement manager
                var manager = new CodeImprovementManager(targetFile);

                // Generate improvement plan
                var plan = manager.AnalyzeAndGenerateImprovements();

                // Display summary
                Console.WriteLine($"✅ Analysis complete!");
                Console.WriteLine($"   📊 Total improvements found: {plan.Improvements.Count}");
                Console.WriteLine($"   🚨 Critical priority: {plan.CriticalCount}");
                Console.WriteLine($"   ⚠️  High priority: {plan.HighCount}");
                Console.WriteLine($"   ⏱️  Estimated total hours: {plan.Improvements.Sum(i => i.EstimatedHours):F1}");
                Console.WriteLine();

                // Show top 3 improvements
                var topImprovements = plan.Improvements
                    .OrderBy(i => i.Priority)
                    .Take(3);

                Console.WriteLine("🔝 Top 3 Priority Improvements:");
                foreach (var improvement in topImprovements)
                {
                    Console.WriteLine($"   [{improvement.Priority}] {improvement.Title} ({improvement.EstimatedHours:F1}h)");
                }
                Console.WriteLine();

                Console.WriteLine("🖥️  Launching interactive review interface...");

                // Launch the interactive review form
                manager.LaunchImprovementReview(plan);

                Console.WriteLine("✅ Review completed. Check output files for implementation details.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during analysis: {ex.Message}");
                MessageBox.Show($"Analysis failed: {ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                    "Town of Wiley - Analysis Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            Console.WriteLine("\n⏸️  Press any key to exit...");
            Console.ReadKey();
        }
    }
}
