using System;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    public static class ScenarioCalculator
    {
        // TODO: Refine scenario calculations based on City Council input and additional seasonal factors.

        // Scenario 1: New Trash Truck: $350,000, 12-year lifespan, 2.67% rate impact
        public static decimal CalculateScenario1(SanitationDistrict district)
        {
            // Apply 2.67% rate impact and seasonal adjustment
            decimal baseRate = district.RequiredRate * 1.0267m;
            return baseRate * (district.SeasonalRevenueFactor > 0 ? district.SeasonalRevenueFactor : 1);
        }

        // Scenario 2: Reserve Fund: $50,000 over 5 years, 0.83% rate impact
        public static decimal CalculateScenario2(SanitationDistrict district)
        {
            decimal baseRate = district.RequiredRate * 1.0083m;
            return baseRate * (district.SeasonalRevenueFactor > 0 ? district.SeasonalRevenueFactor : 1);
        }

        // Scenario 3: Grant Repayment: $100,000, 5 years, 3% interest, 1.83% rate impact
        public static decimal CalculateScenario3(SanitationDistrict district)
        {
            decimal baseRate = district.RequiredRate * 1.0183m;
            return baseRate * (district.SeasonalRevenueFactor > 0 ? district.SeasonalRevenueFactor : 1);
        }

        // Update all scenarios for a district
        public static void UpdateScenarios(SanitationDistrict district)
        {
            district.Scenario1 = CalculateScenario1(district);
            district.Scenario2 = CalculateScenario2(district);
            district.Scenario3 = CalculateScenario3(district);
        }
    }
}
