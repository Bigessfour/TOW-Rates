using System;
using System.Collections.Generic;
using System.Linq;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Services
{
    /// <summary>
    /// Represents enterprise context data for AI analysis
    /// Used to provide comprehensive financial data to the AI service
    /// </summary>
    public class EnterpriseContext
    {
        /// <summary>
        /// Enterprise name (e.g., "Water Enterprise", "Trash & Recycling")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Municipality name (e.g., "Town of Wiley")
        /// </summary>
        public string Municipality { get; set; } = "Town of Wiley";

        /// <summary>
        /// Description of the data scope and context
        /// </summary>
        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether this enterprise has rate study data available
        /// </summary>
        public bool HasRateStudyData { get; set; } = false;

        /// <summary>
        /// Total budget amount for this enterprise
        /// </summary>
        public decimal TotalBudget { get; set; }

        /// <summary>
        /// Total revenue for this enterprise
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Total expenses for this enterprise
        /// </summary>
        public decimal TotalExpenses { get; set; }

        /// <summary>
        /// Number of customers served by this enterprise
        /// </summary>
        public int CustomerBase { get; set; }

        /// <summary>
        /// Year-to-date spending total
        /// </summary>
        public decimal YearToDateSpending { get; set; }

        /// <summary>
        /// Percentage of budget used
        /// </summary>
        public decimal PercentOfBudgetUsed { get; set; }

        /// <summary>
        /// Budget remaining
        /// </summary>
        public decimal BudgetRemaining { get; set; }

        /// <summary>
        /// Average monthly usage (if applicable)
        /// </summary>
        public decimal AverageMonthlyUsage { get; set; }

        /// <summary>
        /// Required rate per customer/unit
        /// </summary>
        public decimal RequiredRate { get; set; }

        /// <summary>
        /// Customer affordability index (0.0 to 1.0)
        /// </summary>
        public decimal CustomerAffordabilityIndex { get; set; }

        /// <summary>
        /// Seasonal adjustment factor
        /// </summary>
        public decimal SeasonalAdjustment { get; set; }

        /// <summary>
        /// Time of use factor for rate calculations
        /// </summary>
        public decimal TimeOfUseFactor { get; set; }

        /// <summary>
        /// Reserve target amount
        /// </summary>
        public decimal ReserveTarget { get; set; }

        /// <summary>
        /// List of all accounts in this enterprise
        /// </summary>
        public List<SanitationDistrict> Accounts { get; set; } = new List<SanitationDistrict>();

        /// <summary>
        /// Key financial metrics for quick analysis
        /// </summary>
        public Dictionary<string, decimal> KeyMetrics { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// Risk factors and concerns
        /// </summary>
        public List<string> RiskFactors { get; set; } = new List<string>();

        /// <summary>
        /// Compliance status and notes
        /// </summary>
        public string ComplianceStatus { get; set; } = string.Empty;

        /// <summary>
        /// Last update timestamp
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public EnterpriseContext()
        {
            Municipality = "Town of Wiley";
            HasRateStudyData = false;
            InitializeKeyMetrics();
        }

        private void InitializeKeyMetrics()
        {
            KeyMetrics["BudgetVariance"] = 0;
            KeyMetrics["RevenueProjection"] = 0;
            KeyMetrics["ExpenseProjection"] = 0;
            KeyMetrics["CashFlow"] = 0;
            KeyMetrics["CustomerGrowthRate"] = 0;
            KeyMetrics["OperationalEfficiency"] = 0;
        }

        /// <summary>
        /// Calculate and update key metrics based on account data
        /// </summary>
        public void CalculateMetrics()
        {
            if (Accounts?.Count > 0)
            {
                TotalBudget = 0;
                TotalRevenue = 0;
                TotalExpenses = 0;
                YearToDateSpending = 0;
                BudgetRemaining = 0;

                foreach (var account in Accounts)
                {
                    TotalBudget += account.CurrentFYBudget;
                    YearToDateSpending += account.YearToDateSpending;
                    BudgetRemaining += account.BudgetRemaining;

                    if (account.Section?.ToLower() == "revenue")
                    {
                        TotalRevenue += account.CurrentFYBudget;
                    }
                    else
                    {
                        TotalExpenses += account.CurrentFYBudget;
                    }
                }

                // Calculate derived metrics
                if (TotalBudget > 0)
                {
                    PercentOfBudgetUsed = YearToDateSpending / TotalBudget;
                    KeyMetrics["BudgetVariance"] = (TotalBudget - YearToDateSpending) / TotalBudget;
                }

                KeyMetrics["RevenueProjection"] = TotalRevenue;
                KeyMetrics["ExpenseProjection"] = TotalExpenses;
                KeyMetrics["CashFlow"] = TotalRevenue - TotalExpenses;

                // Calculate operational efficiency
                if (TotalExpenses > 0)
                {
                    KeyMetrics["OperationalEfficiency"] = TotalRevenue / TotalExpenses;
                }

                // Update averages
                if (Accounts.Count > 0)
                {
                    AverageMonthlyUsage = Accounts.Where(a => a.MonthlyUsage > 0).Average(a => a.MonthlyUsage);
                    RequiredRate = Accounts.Where(a => a.RequiredRate > 0).Average(a => a.RequiredRate);
                    CustomerAffordabilityIndex = Accounts.Where(a => a.CustomerAffordabilityIndex > 0).Average(a => a.CustomerAffordabilityIndex);
                    SeasonalAdjustment = Accounts.Average(a => a.SeasonalAdjustment);
                    TimeOfUseFactor = Accounts.Where(a => a.TimeOfUseFactor > 0).Any() ?
                        Accounts.Where(a => a.TimeOfUseFactor > 0).Average(a => a.TimeOfUseFactor) : 1.0m;
                    ReserveTarget = Accounts.Sum(a => a.ReserveTarget);
                }

                // Set HasRateStudyData based on available data
                HasRateStudyData = Accounts.Any(a => a.RequiredRate > 0 || a.MonthlyUsage > 0 || a.CustomerAffordabilityIndex > 0);
            }

            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Get a summary string for AI analysis
        /// </summary>
        public string GetSummary()
        {
            CalculateMetrics();

            return $@"Municipality: {Municipality}
Enterprise: {Name}
Scope: {Scope}
Has Rate Study Data: {HasRateStudyData}
Customer Base: {CustomerBase:N0} customers
Total Budget: ${TotalBudget:N2}
Total Revenue: ${TotalRevenue:N2}
Total Expenses: ${TotalExpenses:N2}
YTD Spending: ${YearToDateSpending:N2} ({PercentOfBudgetUsed:P1} of budget)
Budget Remaining: ${BudgetRemaining:N2}
Required Rate: ${RequiredRate:N2} per unit
Customer Affordability: {CustomerAffordabilityIndex:P1}
Seasonal Adjustment: ${SeasonalAdjustment:N2}
Reserve Target: ${ReserveTarget:N2}
Accounts: {Accounts.Count} line items
Last Updated: {LastUpdated:yyyy-MM-dd HH:mm}";
        }

        /// <summary>
        /// Get financial ratios for analysis
        /// </summary>
        public Dictionary<string, decimal> GetFinancialRatios()
        {
            var ratios = new Dictionary<string, decimal>();

            if (TotalRevenue > 0)
            {
                ratios["ExpenseRatio"] = TotalExpenses / TotalRevenue;
                ratios["RevenuePerCustomer"] = TotalRevenue / Math.Max(CustomerBase, 1);
            }

            if (TotalBudget > 0)
            {
                ratios["BudgetUtilization"] = YearToDateSpending / TotalBudget;
                ratios["BudgetPerCustomer"] = TotalBudget / Math.Max(CustomerBase, 1);
            }

            if (CustomerBase > 0)
            {
                ratios["ExpensePerCustomer"] = TotalExpenses / CustomerBase;
                ratios["UsagePerCustomer"] = AverageMonthlyUsage / Math.Max(CustomerBase, 1);
            }

            return ratios;
        }

        /// <summary>
        /// Assess risk factors based on current metrics
        /// </summary>
        public void AssessRiskFactors()
        {
            RiskFactors.Clear();

            if (PercentOfBudgetUsed > 0.9m)
            {
                RiskFactors.Add("High budget utilization - approaching budget limits");
            }

            if (KeyMetrics["CashFlow"] < 0)
            {
                RiskFactors.Add("Negative cash flow - expenses exceed revenue");
            }

            if (CustomerAffordabilityIndex < 0.8m)
            {
                RiskFactors.Add("Low customer affordability - rates may be too high");
            }

            if (ReserveTarget > 0 && BudgetRemaining < ReserveTarget)
            {
                RiskFactors.Add("Insufficient reserves - below target reserve level");
            }

            if (Math.Abs(SeasonalAdjustment) > TotalBudget * 0.1m)
            {
                RiskFactors.Add("High seasonal variability - revenue fluctuation risk");
            }

            if (KeyMetrics["OperationalEfficiency"] < 1.0m)
            {
                RiskFactors.Add("Operational inefficiency - revenue below expenses");
            }
        }

        /// <summary>
        /// Generate recommendations based on current state
        /// </summary>
        public List<string> GetRecommendations()
        {
            var recommendations = new List<string>();
            AssessRiskFactors();

            if (PercentOfBudgetUsed > 0.85m)
            {
                recommendations.Add("Monitor remaining budget closely - consider expense controls");
            }

            if (KeyMetrics["CashFlow"] < 0)
            {
                recommendations.Add("Implement revenue enhancement strategies or reduce expenses");
            }

            if (CustomerAffordabilityIndex < 0.85m)
            {
                recommendations.Add("Review rate structure for customer affordability impact");
            }

            if (ReserveTarget > BudgetRemaining)
            {
                recommendations.Add("Prioritize building reserves for financial stability");
            }

            if (KeyMetrics["OperationalEfficiency"] > 1.2m)
            {
                recommendations.Add("Consider rate reduction or service expansion opportunities");
            }

            return recommendations;
        }
    }
}
