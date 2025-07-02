using System;
using System.IO;
using System.Linq;
using WileyBudgetManagement.Tools;
using static WileyBudgetManagement.Tools.CodeImprovementManager;

namespace WileyBudgetManagement
{
    /// <summary>
    /// Quick console-based analysis for Town of Wiley code improvements
    /// Use this for rapid assessment without the full UI
    /// </summary>
    class QuickAnalysisRunner
    {
        static void Main(string[] args)
        {
            var targetFile = @"c:\Users\steve.mckitrick\Desktop\Rate Study\WileyBudgetManagement\AIEnhancedQueryService.cs";

            if (!File.Exists(targetFile))
            {
                Console.WriteLine($"❌ File not found: {targetFile}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("🏛️  TOWN OF WILEY - QUICK CODE ANALYSIS");
            Console.WriteLine("=" + new string('=', 50));

            try
            {
                var manager = new CodeImprovementManager(targetFile);
                var plan = manager.AnalyzeAndGenerateImprovements();

                PrintQuickSummary(plan);
                PrintImprovementsByCategory(plan);
                PrintRecommendedActionPlan(plan);

                Console.WriteLine("\n💡 Next Steps:");
                Console.WriteLine("   1. Run LaunchImprovementTool.exe for interactive review");
                Console.WriteLine("   2. Focus on Critical and High priority items first");
                Console.WriteLine("   3. Review municipal compliance improvements");
                Console.WriteLine("   4. Test thoroughly in development environment");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Analysis failed: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void PrintQuickSummary(ImprovementPlan plan)
        {
            Console.WriteLine($"📁 File: AIEnhancedQueryService.cs");
            Console.WriteLine($"📊 Total Improvements: {plan.Improvements.Count}");
            Console.WriteLine($"⏱️  Estimated Hours: {plan.Improvements.Sum(i => i.EstimatedHours):F1}");
            Console.WriteLine();

            // Priority breakdown
            Console.WriteLine("🎯 Priority Breakdown:");
            foreach (ImprovementPriority priority in Enum.GetValues<ImprovementPriority>())
            {
                var count = plan.Improvements.Count(i => i.Priority == priority);
                var hours = plan.Improvements.Where(i => i.Priority == priority).Sum(i => i.EstimatedHours);

                if (count > 0)
                {
                    var emoji = priority switch
                    {
                        ImprovementPriority.Critical => "🚨",
                        ImprovementPriority.High => "⚠️",
                        ImprovementPriority.Medium => "🔶",
                        ImprovementPriority.Low => "🟡",
                        _ => "📋"
                    };
                    Console.WriteLine($"   {emoji} {priority}: {count} items ({hours:F1} hours)");
                }
            }
            Console.WriteLine();
        }

        static void PrintImprovementsByCategory(ImprovementPlan plan)
        {
            Console.WriteLine("📂 Improvements by Category:");

            var categories = plan.Improvements.GroupBy(i => i.Category);
            foreach (var category in categories.OrderBy(g => g.Key))
            {
                Console.WriteLine($"   🔸 {category.Key}: {category.Count()} items");

                // Show top 2 items per category
                var topItems = category.OrderBy(i => i.Priority).Take(2);
                foreach (var item in topItems)
                {
                    Console.WriteLine($"      • {item.Title} [{item.Priority}]");
                }

                if (category.Count() > 2)
                {
                    Console.WriteLine($"      ... and {category.Count() - 2} more");
                }
                Console.WriteLine();
            }
        }

        static void PrintRecommendedActionPlan(ImprovementPlan plan)
        {
            Console.WriteLine("🎯 RECOMMENDED ACTION PLAN:");

            // Phase 1: Critical items
            var critical = plan.Improvements.Where(i => i.Priority == ImprovementPriority.Critical).ToList();
            if (critical.Any())
            {
                Console.WriteLine("📍 PHASE 1 - Critical (Do First):");
                foreach (var item in critical)
                {
                    Console.WriteLine($"   ✅ {item.Title} ({item.EstimatedHours:F1}h)");
                    Console.WriteLine($"      💡 {item.Description}");
                }
                Console.WriteLine();
            }

            // Phase 2: High priority
            var high = plan.Improvements.Where(i => i.Priority == ImprovementPriority.High).ToList();
            if (high.Any())
            {
                Console.WriteLine("📍 PHASE 2 - High Priority (Municipal Focus):");
                foreach (var item in high.Take(3)) // Show top 3
                {
                    Console.WriteLine($"   🔧 {item.Title} ({item.EstimatedHours:F1}h)");
                }
                if (high.Count > 3)
                {
                    Console.WriteLine($"   ... and {high.Count - 3} more high-priority items");
                }
                Console.WriteLine();
            }

            // Estimate total for critical + high
            var priorityHours = critical.Sum(i => i.EstimatedHours) + high.Sum(i => i.EstimatedHours);
            Console.WriteLine($"💰 Investment for Priority Items: {priorityHours:F1} hours");
            Console.WriteLine($"🎯 Expected ROI: Improved maintainability, security, and municipal compliance");
        }
    }
}
