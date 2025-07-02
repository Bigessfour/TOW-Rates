using System;

namespace WileyBudgetManagement.Models
{
    public class SanitationDistrict
    {
        public SanitationDistrict()
        {
            Account = string.Empty;
            Label = string.Empty;
            Section = string.Empty;
            Notes = string.Empty;
        }
        public string Account { get; set; }
        public string Label { get; set; }
        public string Section { get; set; }
        public decimal CurrentFYBudget { get; set; }
        public decimal SeasonalAdjustment { get; set; }
        public decimal MonthlyInput { get; set; }
        public decimal SeasonalRevenueFactor { get; set; }
        public decimal YearToDateSpending { get; set; }
        public decimal PercentOfBudget { get; set; }
        public decimal BudgetRemaining { get; set; }
        public decimal GoalAdjustment { get; set; }
        public decimal ReserveTarget { get; set; }
        public decimal Scenario1 { get; set; }
        public decimal Scenario2 { get; set; }
        public decimal Scenario3 { get; set; }
        public decimal PercentAllocation { get; set; }
        public decimal RequiredRate { get; set; }
        public decimal MonthlyUsage { get; set; }
        public decimal TimeOfUseFactor { get; set; }
        public decimal CustomerAffordabilityIndex { get; set; }
        public decimal QuarterlySummary { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal Total { get; set; }
        public decimal PriorFYActual { get; set; }
        public decimal TwoYearsPriorActual { get; set; }
        public string Notes { get; set; }
    }
}
