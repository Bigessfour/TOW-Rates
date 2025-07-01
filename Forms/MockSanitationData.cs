using System.Collections.Generic;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    public static class MockSanitationData
    {
        public static List<SanitationDistrict> GetSampleData()
        {
            return new List<SanitationDistrict>
            {
                new SanitationDistrict {
                    Account = "1001", Label = "Residential", Section = "A", CurrentFYBudget = 120000, SeasonalAdjustment = 5000, MonthlyInput = 10000, SeasonalRevenueFactor = 1.05M, YearToDateSpending = 60000, PercentOfBudget = 50, BudgetRemaining = 60000, GoalAdjustment = 2000, ReserveTarget = 10000, Scenario1 = 0, Scenario2 = 0, Scenario3 = 0, PercentAllocation = 40, RequiredRate = 25, MonthlyUsage = 400, TimeOfUseFactor = 1, CustomerAffordabilityIndex = 0.8M, QuarterlySummary = 30000, EntryDate = System.DateTime.Now, Total = 120000 },
                new SanitationDistrict {
                    Account = "1002", Label = "Commercial", Section = "B", CurrentFYBudget = 80000, SeasonalAdjustment = 3000, MonthlyInput = 7000, SeasonalRevenueFactor = 1.02M, YearToDateSpending = 40000, PercentOfBudget = 50, BudgetRemaining = 40000, GoalAdjustment = 1500, ReserveTarget = 8000, Scenario1 = 0, Scenario2 = 0, Scenario3 = 0, PercentAllocation = 30, RequiredRate = 30, MonthlyUsage = 200, TimeOfUseFactor = 1, CustomerAffordabilityIndex = 0.7M, QuarterlySummary = 20000, EntryDate = System.DateTime.Now, Total = 80000 }
            };
        }
    }
}
