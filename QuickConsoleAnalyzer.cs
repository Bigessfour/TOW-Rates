using System;
using System.IO;
using System.Linq;
using WileyBudgetManagement.Tools;
using static WileyBudgetManagement.Tools.CodeImprovementManager;

namespace WileyBudgetManagement
{
    /// <summary>
    /// Quick console-based code analyzer for Town of Wiley Budget Management
    /// Method 1: Simple, reliable, no external dependencies
    /// </summary>
    class QuickConsoleAnalyzer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🏛️  TOWN OF WILEY BUDGET MANAGEMENT");
            Console.WriteLine("📋 QUICK CODE IMPROVEMENT ANALYZER");
            Console.WriteLine("=" + new string('=', 60));
            Console.WriteLine();

            // Target file
            var targetFile = @"c:\Users\steve.mckitrick\Desktop\Rate Study\WileyBudgetManagement\AIEnhancedQueryService.cs";

            // Allow command line override
            if (args.Length > 0 && File.Exists(args[0]))
            {
                targetFile = args[0];
                Console.WriteLine($"📁 Using command line file: {Path.GetFileName(targetFile)}");
            }
            else
            {
                Console.WriteLine($"📁 Analyzing default file: {Path.GetFileName(targetFile)}");
            }

            // Check if file exists
            if (!File.Exists(targetFile))
            {
                Console.WriteLine($"❌ ERROR: File not found at {targetFile}");
                Console.WriteLine("   Please ensure the AIEnhancedQueryService.cs file exists.");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            try
            {
                Console.WriteLine("🔍 Starting analysis...");
                Console.WriteLine();

                // Create the improvement manager and analyze
                var manager = new CodeImprovementManager(targetFile);
                var plan = manager.AnalyzeAndGenerateImprovements();

                // Display results
                DisplayAnalysisResults(plan);
                DisplayTopImprovements(plan);
                DisplayQuickActionPlan(plan);
                DisplayNextSteps();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ANALYSIS FAILED: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Stack trace:");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\n" + "=" + new string('=', 60));
            Console.WriteLine("⏸️  Press any key to exit...");
            Console.ReadKey();
        }

        static void DisplayAnalysisResults(ImprovementPlan plan)
        {
            Console.WriteLine("📊 ANALYSIS RESULTS");
            Console.WriteLine("-" + new string('-', 30));
            Console.WriteLine($"   Total Improvements Found: {plan.Improvements.Count}");
            Console.WriteLine($"   Estimated Total Hours: {plan.Improvements.Sum(i => i.EstimatedHours):F1}");
            Console.WriteLine();

            // Priority breakdown
            Console.WriteLine("🎯 Priority Breakdown:");
            var priorities = plan.Improvements.GroupBy(i => i.Priority).OrderBy(g => g.Key);
            foreach (var group in priorities)
            {
                var count = group.Count();
                var hours = group.Sum(i => i.EstimatedHours);
                var emoji = group.Key switch
                {
                    ImprovementPriority.Critical => "🚨",
                    ImprovementPriority.High => "⚠️",
                    ImprovementPriority.Medium => "🔶",
                    ImprovementPriority.Low => "🟡",
                    _ => "📋"
                };
                Console.WriteLine($"   {emoji} {group.Key}: {count} items ({hours:F1} hours)");
            }
            Console.WriteLine();

            // Category breakdown
            Console.WriteLine("📂 Category Breakdown:");
            var categories = plan.Improvements.GroupBy(i => i.Category).OrderBy(g => g.Key);
            foreach (var group in categories)
            {
                Console.WriteLine($"   🔸 {group.Key}: {group.Count()} items");
            }
            Console.WriteLine();
        }

        static void DisplayTopImprovements(ImprovementPlan plan)
        {
            Console.WriteLine("🔝 TOP PRIORITY IMPROVEMENTS");
            Console.WriteLine("-" + new string('-', 40));

            var topImprovements = plan.Improvements
                .Where(i => i.Priority <= ImprovementPriority.High)
                .OrderBy(i => i.Priority)
                .ThenBy(i => i.EstimatedHours);

            if (!topImprovements.Any())
            {
                Console.WriteLine("   ✅ No critical or high priority issues found!");
                Console.WriteLine();
                return;
            }

            foreach (var improvement in topImprovements)
            {
                var priorityEmoji = improvement.Priority == ImprovementPriority.Critical ? "🚨" : "⚠️";
                Console.WriteLine($"{priorityEmoji} [{improvement.Priority}] {improvement.Title}");
                Console.WriteLine($"   ⏱️  Time: {improvement.EstimatedHours:F1} hours");
                Console.WriteLine($"   📝 {improvement.Description}");
                Console.WriteLine($"   💡 Why: {improvement.Rationale}");
                Console.WriteLine();
            }
        }

        static void DisplayQuickActionPlan(ImprovementPlan plan)
        {
            Console.WriteLine("🎯 QUICK ACTION PLAN FOR TOWN OF WILEY");
            Console.WriteLine("-" + new string('-', 45));

            var critical = plan.Improvements.Where(i => i.Priority == ImprovementPriority.Critical).ToList();
            var high = plan.Improvements.Where(i => i.Priority == ImprovementPriority.High).ToList();

            if (critical.Any())
            {
                Console.WriteLine("📍 IMMEDIATE ACTION REQUIRED (Critical):");
                foreach (var item in critical)
                {
                    Console.WriteLine($"   🚨 {item.Title} ({item.EstimatedHours:F1}h)");
                }
                Console.WriteLine();
            }

            if (high.Any())
            {
                Console.WriteLine("📍 NEXT PHASE (High Priority):");
                foreach (var item in high.Take(3))
                {
                    Console.WriteLine($"   ⚠️  {item.Title} ({item.EstimatedHours:F1}h)");
                }
                if (high.Count > 3)
                {
                    Console.WriteLine($"   ... and {high.Count - 3} more high-priority items");
                }
                Console.WriteLine();
            }

            // Municipal-specific recommendations
            var municipalItems = plan.Improvements.Where(i =>
                i.Category == ImprovementCategory.MunicipalCompliance ||
                i.Category == ImprovementCategory.Security).ToList();

            if (municipalItems.Any())
            {
                Console.WriteLine("🏛️  MUNICIPAL COMPLIANCE FOCUS:");
                foreach (var item in municipalItems.Take(2))
                {
                    Console.WriteLine($"   🔒 {item.Title}");
                }
                Console.WriteLine();
            }

            // Investment summary
            var priorityHours = critical.Sum(i => i.EstimatedHours) + high.Sum(i => i.EstimatedHours);
            if (priorityHours > 0)
            {
                Console.WriteLine($"💰 INVESTMENT SUMMARY:");
                Console.WriteLine($"   ⏱️  Priority items: {priorityHours:F1} hours");
                Console.WriteLine($"   💵 Estimated value: High ROI for municipal compliance");
                Console.WriteLine();
            }
        }

        static void DisplayNextSteps()
        {
            Console.WriteLine("🚀 NEXT STEPS");
            Console.WriteLine("-" + new string('-', 20));
            Console.WriteLine("1. 📋 Review the improvements listed above");
            Console.WriteLine("2. 🔧 Start with Critical priority items first");
            Console.WriteLine("3. 🏛️  Focus on municipal compliance and security");
            Console.WriteLine("4. 🧪 Test each change in development environment");
            Console.WriteLine("5. 📈 Deploy to Town of Wiley production after validation");
            Console.WriteLine();
            Console.WriteLine("💡 For interactive review, consider building the full UI tool");
            Console.WriteLine("   or manually apply the improvements shown above.");
        }
    }
}
