using System;
using System.Collections.Generic;

namespace WileyBudgetManagement.Models
{
    public class EnterpriseContext
    {
        public string Municipality { get; set; } = "Town of Wiley";
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public bool HasRateStudyData { get; set; }
        public Dictionary<string, object> FinancialSummary { get; set; } = new();
        public Dictionary<string, object> UtilityData { get; set; } = new();
        public List<string> RecentTransactions { get; set; } = new();

        public string GetSummary()
        {
            return $@"Municipality: {Municipality}
Last Updated: {LastUpdated:yyyy-MM-dd}
Rate Study Data Available: {HasRateStudyData}
Financial Accounts: {FinancialSummary.Count}
Utility Services: {UtilityData.Count}
Recent Activity: {RecentTransactions.Count} transactions";
        }
    }
}
Recent Activity: { RecentTransactions.Count}
transactions";
        }
    }
}
