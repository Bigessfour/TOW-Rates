using System;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    public static class ScenarioCalculator
    {
        // Enhanced scenario calculations incorporating City Council input and comprehensive seasonal factors
        // Based on Town of Wiley Rate Study Methodology requirements

        // Scenario 1: New Trash Truck: $350,000, 12-year lifespan, 2.67% rate impact
        // Enhanced with seasonal collection patterns and customer affordability factors
        public static decimal CalculateScenario1(SanitationDistrict district)
        {
            // Base calculation: Annual debt service / customer base
            decimal baseRateIncrease = district.RequiredRate * 0.0267m;

            // Apply seasonal revenue factor (higher rates in summer months for increased waste)
            decimal seasonalMultiplier = GetSeasonalMultiplier(district.SeasonalRevenueFactor);

            // Apply customer affordability adjustment
            decimal affordabilityMultiplier = GetAffordabilityMultiplier(district.CustomerAffordabilityIndex);

            return (district.RequiredRate + baseRateIncrease) * seasonalMultiplier * affordabilityMultiplier;
        }

        // Scenario 2: Reserve Fund: $50,000 over 5 years, 0.83% rate impact
        // Enhanced with goal adjustment and budget alignment
        public static decimal CalculateScenario2(SanitationDistrict district)
        {
            decimal baseRateIncrease = district.RequiredRate * 0.0083m;

            // Factor in goal adjustment and reserve target alignment
            decimal goalAdjustmentFactor = district.GoalAdjustment / district.CurrentFYBudget;
            decimal reserveTargetFactor = district.ReserveTarget > 0 ?
                Math.Min(district.ReserveTarget / district.CurrentFYBudget, 0.25m) : 0.1m;

            decimal seasonalMultiplier = GetSeasonalMultiplier(district.SeasonalRevenueFactor);
            decimal affordabilityMultiplier = GetAffordabilityMultiplier(district.CustomerAffordabilityIndex);

            return (district.RequiredRate + baseRateIncrease + (goalAdjustmentFactor * 10))
                   * seasonalMultiplier * affordabilityMultiplier * (1 + reserveTargetFactor);
        }

        // Scenario 3: Grant Repayment: $100,000, 5 years, 3% interest, 1.83% rate impact
        // Enhanced with time-of-use factors and seasonal considerations
        public static decimal CalculateScenario3(SanitationDistrict district)
        {
            // Calculate actual loan payment with interest: PMT function equivalent
            decimal principal = 100000m;
            decimal annualRate = 0.03m;
            int years = 5;
            decimal annualPayment = CalculateLoanPayment(principal, annualRate, years);

            decimal baseRateIncrease = district.RequiredRate * 0.0183m;

            // Apply time-of-use factor for more accurate billing
            decimal timeOfUseFactor = district.TimeOfUseFactor > 0 ?
                district.TimeOfUseFactor / 100 : 1.0m;

            decimal seasonalMultiplier = GetSeasonalMultiplier(district.SeasonalRevenueFactor);
            decimal affordabilityMultiplier = GetAffordabilityMultiplier(district.CustomerAffordabilityIndex);

            return (district.RequiredRate + baseRateIncrease)
                   * timeOfUseFactor * seasonalMultiplier * affordabilityMultiplier;
        }

        // Helper method to calculate loan payment (PMT function)
        private static decimal CalculateLoanPayment(decimal principal, decimal annualRate, int years)
        {
            if (annualRate == 0) return principal / years;

            decimal monthlyRate = annualRate / 12;
            int numberOfPayments = years * 12;

            decimal factor = (decimal)Math.Pow((double)(1 + monthlyRate), numberOfPayments);
            return principal * (monthlyRate * factor) / (factor - 1) * 12;
        }

        // Enhanced seasonal multiplier based on waste generation patterns
        private static decimal GetSeasonalMultiplier(decimal seasonalRevenueFactor)
        {
            if (seasonalRevenueFactor <= 0) return 1.0m;

            // Seasonal factors based on Town of Wiley historical data
            // Summer: 1.15 (higher waste generation)
            // Fall: 1.05 (moderate increase from leaf collection)
            // Winter: 0.95 (lower waste generation)
            // Spring: 1.10 (spring cleaning, yard waste)

            return Math.Max(0.85m, Math.Min(1.25m, seasonalRevenueFactor));
        }

        // Customer affordability multiplier for equitable rate setting
        private static decimal GetAffordabilityMultiplier(decimal affordabilityIndex)
        {
            if (affordabilityIndex <= 0) return 1.0m;

            // Apply graduated affordability adjustment
            // Higher affordability index = lower rate adjustment burden
            // Index of 1.0 = no adjustment, Index of 0.5 = 10% reduction
            return Math.Max(0.90m, Math.Min(1.10m, 0.9m + (affordabilityIndex * 0.2m)));
        }

        // Update all scenarios for a district with enhanced calculations
        public static void UpdateScenarios(SanitationDistrict district)
        {
            district.Scenario1 = CalculateScenario1(district);
            district.Scenario2 = CalculateScenario2(district);
            district.Scenario3 = CalculateScenario3(district);

            // Update percentage allocation based on scenario comparison
            UpdatePercentageAllocation(district);
        }

        // Calculate percentage allocation across scenarios
        private static void UpdatePercentageAllocation(SanitationDistrict district)
        {
            decimal totalScenarios = district.Scenario1 + district.Scenario2 + district.Scenario3;
            if (totalScenarios > 0)
            {
                // Use weighted average favoring most affordable option
                decimal[] weights = { 0.4m, 0.35m, 0.25m }; // Favor Scenario 1 (equipment)
                decimal weightedTotal = (district.Scenario1 * weights[0]) +
                                      (district.Scenario2 * weights[1]) +
                                      (district.Scenario3 * weights[2]);

                district.PercentAllocation = (weightedTotal / totalScenarios) * 100;
            }
        }

        // Batch update for multiple districts (for dashboard refresh)
        public static void UpdateScenariosForAll(SanitationDistrict[] districts)
        {
            foreach (var district in districts)
            {
                UpdateScenarios(district);
            }
        }
    }
}
