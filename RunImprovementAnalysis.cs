using System;
using System.Windows.Forms;
using WileyBudgetManagement.Tools;

class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var targetFile = @"c:\Users\steve.mckitrick\Desktop\Rate Study\WileyBudgetManagement\AIEnhancedQueryService.cs";

        try
        {
            var manager = new CodeImprovementManager(targetFile);
            var plan = manager.AnalyzeAndGenerateImprovements();

            Console.WriteLine($"Found {plan.Improvements.Count} improvement opportunities");
            Console.WriteLine($"Critical: {plan.CriticalCount}, High: {plan.HighCount}");

            manager.LaunchImprovementReview(plan);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Analysis Failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
