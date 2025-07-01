namespace WileyBudgetManagement;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // SQL Server schema setup: see Database/Setup.sql for fields: Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput, SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining, GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3, PercentAllocation, RequiredRate, MonthlyUsage, TimeOfUseFactor, CustomerAffordabilityIndex, QuarterlySummary, EntryDate, Total.
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new WileyBudgetManagement.Forms.DashboardForm());
    }
}