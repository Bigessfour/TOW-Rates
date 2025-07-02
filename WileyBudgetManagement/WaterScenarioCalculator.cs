using System;
using System.Collections.Generic;
using System.Linq;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    /// <summary>
    /// Water Enterprise Scenario Calculator
    /// Implements comprehensive water infrastructure scenarios based on Rate Study Methodology
    /// Scenarios: Treatment Plant ($750K), Pipeline Replacement ($200K), Quality Upgrades ($125K)
    /// </summary>
    public static class WaterScenarioCalculator
    {
        // Water Infrastructure Investment Constants
        public const decimal TREATMENT_PLANT_COST = 750000m;      // 20-year project
        public const decimal PIPELINE_REPLACEMENT_COST = 200000m; // 10-year project  
        public const decimal QUALITY_UPGRADE_COST = 125000m;      // 8-year project

        // Interest rates for financial calculations
        public const decimal TREATMENT_PLANT_INTEREST_RATE = 0.04m; // 4% for long-term municipal bonds
        public const decimal PIPELINE_INTEREST_RATE = 0.035m;       // 3.5% for infrastructure loans
        public const decimal QUALITY_INTEREST_RATE = 0.03m;        // 3% for short-term improvements

        // Town of Wiley Water System Parameters
        public const int WATER_CUSTOMER_BASE = 850;                // Approximate water customers
        public const decimal MINIMUM_INFRASTRUCTURE_ALLOCATION = 0.15m; // 15% minimum for infrastructure
        public const decimal MINIMUM_QUALITY_ALLOCATION = 0.05m;   // 5% minimum for quality assurance
        public const decimal EPA_COMPLIANCE_FACTOR = 1.08m;        // 8% compliance overhead

        /// <summary>
        /// Calculate comprehensive water scenarios for all infrastructure investments
        /// </summary>
        /// <param name="waterData">Current water enterprise data</param>
        /// <returns>Updated water data with calculated scenarios</returns>
        public static List<SanitationDistrict> CalculateWaterScenarios(List<SanitationDistrict> waterData)
        {
            if (waterData == null || !waterData.Any())
                return waterData ?? new List<SanitationDistrict>();

            try
            {
                // Calculate monthly payment amounts for each scenario
                var treatmentPlantMonthly = CalculateMonthlyPayment(TREATMENT_PLANT_COST, TREATMENT_PLANT_INTEREST_RATE, 20);
                var pipelineMonthly = CalculateMonthlyPayment(PIPELINE_REPLACEMENT_COST, PIPELINE_INTEREST_RATE, 10);
                var qualityUpgradeMonthly = CalculateMonthlyPayment(QUALITY_UPGRADE_COST, QUALITY_INTEREST_RATE, 8);

                foreach (var district in waterData)
                {
                    CalculateScenarioImpacts(district, treatmentPlantMonthly, pipelineMonthly, qualityUpgradeMonthly);
                }

                return waterData;
            }
            catch (Exception ex)
            {
                // Log error and return original data
                System.Diagnostics.Debug.WriteLine($"Water scenario calculation error: {ex.Message}");
                return waterData;
            }
        }

        /// <summary>
        /// Calculate monthly payment for infrastructure investment using PMT formula
        /// </summary>
        /// <param name="principal">Total investment amount</param>
        /// <param name="annualRate">Annual interest rate (as decimal)</param>
        /// <param name="years">Loan term in years</param>
        /// <returns>Monthly payment amount</returns>
        private static decimal CalculateMonthlyPayment(decimal principal, decimal annualRate, int years)
        {
            if (principal <= 0 || annualRate < 0 || years <= 0)
                return 0;

            decimal monthlyRate = annualRate / 12;
            int totalPayments = years * 12;

            if (monthlyRate == 0)
                return principal / totalPayments; // No interest case

            // PMT formula: P * [r(1+r)^n] / [(1+r)^n - 1]
            decimal factor = (decimal)Math.Pow((double)(1 + monthlyRate), totalPayments);
            return principal * (monthlyRate * factor) / (factor - 1);
        }

        /// <summary>
        /// Calculate scenario-specific impacts for a water district account
        /// </summary>
        /// <param name="district">Water district account</param>
        /// <param name="treatmentPlantMonthly">Monthly cost for treatment plant</param>
        /// <param name="pipelineMonthly">Monthly cost for pipeline replacement</param>
        /// <param name="qualityUpgradeMonthly">Monthly cost for quality upgrades</param>
        private static void CalculateScenarioImpacts(SanitationDistrict district,
            decimal treatmentPlantMonthly, decimal pipelineMonthly, decimal qualityUpgradeMonthly)
        {
            decimal baseMonthly = district.MonthlyInput;

            switch (district.Section?.ToUpper())
            {
                case "REVENUE":
                    CalculateRevenueScenarios(district, baseMonthly, treatmentPlantMonthly, pipelineMonthly, qualityUpgradeMonthly);
                    break;

                case "OPERATING":
                    CalculateOperatingScenarios(district, baseMonthly, treatmentPlantMonthly, pipelineMonthly, qualityUpgradeMonthly);
                    break;

                case "INFRASTRUCTURE":
                    CalculateInfrastructureScenarios(district, baseMonthly, treatmentPlantMonthly, pipelineMonthly, qualityUpgradeMonthly);
                    break;

                case "QUALITY":
                    CalculateQualityScenarios(district, baseMonthly, treatmentPlantMonthly, pipelineMonthly, qualityUpgradeMonthly);
                    break;

                default:
                    // Default case for undefined sections
                    district.Scenario1 = baseMonthly;
                    district.Scenario2 = baseMonthly;
                    district.Scenario3 = baseMonthly;
                    break;
            }

            // Apply adjustment factors
            ApplyWaterFactors(district);

            // Ensure scenarios are not negative
            district.Scenario1 = Math.Max(0, district.Scenario1);
            district.Scenario2 = Math.Max(0, district.Scenario2);
            district.Scenario3 = Math.Max(0, district.Scenario3);
        }

        /// <summary>
        /// Calculate revenue scenarios - revenue must increase to cover infrastructure investments
        /// </summary>
        private static void CalculateRevenueScenarios(SanitationDistrict district, decimal baseMonthly,
            decimal treatmentPlantMonthly, decimal pipelineMonthly, decimal qualityUpgradeMonthly)
        {
            decimal allocationFactor = district.PercentAllocation > 0 ? district.PercentAllocation : 0.25m;

            // Scenario 1: Water Treatment Plant ($750,000, 20-year)
            // Revenue needs to cover debt service plus operational impact
            district.Scenario1 = baseMonthly + (treatmentPlantMonthly * allocationFactor * 1.05m); // 5% operational overhead

            // Scenario 2: Pipeline Replacement ($200,000, 10-year)
            // Revenue increase to cover pipeline investment
            district.Scenario2 = baseMonthly + (pipelineMonthly * allocationFactor * 1.03m); // 3% operational overhead

            // Scenario 3: Quality Upgrades ($125,000, 8-year)
            // Revenue to cover quality improvements
            district.Scenario3 = baseMonthly + (qualityUpgradeMonthly * allocationFactor * 1.02m); // 2% operational overhead

            // Apply EPA compliance factor for water sales
            if (district.Account?.StartsWith("W301") == true) // Water Sales account
            {
                district.Scenario1 *= EPA_COMPLIANCE_FACTOR;
                district.Scenario2 *= EPA_COMPLIANCE_FACTOR;
                district.Scenario3 *= EPA_COMPLIANCE_FACTOR;
            }
        }

        /// <summary>
        /// Calculate operating expense scenarios - direct operational impacts
        /// </summary>
        private static void CalculateOperatingScenarios(SanitationDistrict district, decimal baseMonthly,
            decimal treatmentPlantMonthly, decimal pipelineMonthly, decimal qualityUpgradeMonthly)
        {
            // Operating expenses have direct impacts from infrastructure changes

            // Scenario 1: Treatment Plant - significant operational impact
            decimal treatmentImpact = GetOperatingImpactFactor(district, "TREATMENT");
            district.Scenario1 = baseMonthly + (treatmentPlantMonthly * treatmentImpact);

            // Scenario 2: Pipeline Replacement - moderate operational impact
            decimal pipelineImpact = GetOperatingImpactFactor(district, "PIPELINE");
            district.Scenario2 = baseMonthly + (pipelineMonthly * pipelineImpact);

            // Scenario 3: Quality Upgrades - varies by account type
            decimal qualityImpact = GetOperatingImpactFactor(district, "QUALITY");
            district.Scenario3 = baseMonthly + (qualityUpgradeMonthly * qualityImpact);

            // Add goal adjustment for improved efficiency
            if (district.GoalAdjustment > 0)
            {
                district.Scenario1 += district.GoalAdjustment * 0.10m; // 10% of goal adjustment
                district.Scenario2 += district.GoalAdjustment * 0.15m; // 15% of goal adjustment
                district.Scenario3 += district.GoalAdjustment * 0.05m; // 5% of goal adjustment
            }
        }

        /// <summary>
        /// Calculate infrastructure scenarios - direct investment impacts
        /// </summary>
        private static void CalculateInfrastructureScenarios(SanitationDistrict district, decimal baseMonthly,
            decimal treatmentPlantMonthly, decimal pipelineMonthly, decimal qualityUpgradeMonthly)
        {
            // Infrastructure accounts directly absorb investment costs

            // Scenario 1: Treatment Plant Investment
            district.Scenario1 = baseMonthly + treatmentPlantMonthly;

            // Scenario 2: Pipeline Replacement Investment
            district.Scenario2 = baseMonthly + pipelineMonthly;

            // Scenario 3: Quality Infrastructure Investment
            district.Scenario3 = baseMonthly + qualityUpgradeMonthly;

            // Add goal adjustments for infrastructure improvements
            if (district.GoalAdjustment > 0)
            {
                district.Scenario1 += district.GoalAdjustment * 0.80m; // Major infrastructure gets 80% of goal
                district.Scenario2 += district.GoalAdjustment * 0.60m; // Pipeline gets 60% of goal
                district.Scenario3 += district.GoalAdjustment * 0.40m; // Quality gets 40% of goal
            }

            // Apply minimum infrastructure allocation validation
            ValidateInfrastructureAllocation(district);
        }

        /// <summary>
        /// Calculate quality assurance scenarios - regulatory and compliance impacts
        /// </summary>
        private static void CalculateQualityScenarios(SanitationDistrict district, decimal baseMonthly,
            decimal treatmentPlantMonthly, decimal pipelineMonthly, decimal qualityUpgradeMonthly)
        {
            // Quality accounts have varying impacts based on infrastructure changes

            // Scenario 1: Treatment Plant - increased quality monitoring
            district.Scenario1 = baseMonthly + (treatmentPlantMonthly * 0.08m); // 8% quality oversight

            // Scenario 2: Pipeline Replacement - moderate quality impact
            district.Scenario2 = baseMonthly + (pipelineMonthly * 0.05m); // 5% quality oversight

            // Scenario 3: Quality Upgrades - major impact on quality programs
            district.Scenario3 = baseMonthly + qualityUpgradeMonthly; // Full quality investment

            // EPA compliance adjustments
            if (district.Account?.Contains("EPA") == true || district.Account?.Contains("405") == true)
            {
                district.Scenario1 *= EPA_COMPLIANCE_FACTOR;
                district.Scenario2 *= EPA_COMPLIANCE_FACTOR;
                district.Scenario3 *= EPA_COMPLIANCE_FACTOR;
            }
        }

        /// <summary>
        /// Get operating impact factor based on account type and scenario
        /// </summary>
        private static decimal GetOperatingImpactFactor(SanitationDistrict district, string scenarioType)
        {
            string accountCode = district.Account?.ToUpper() ?? "";

            return scenarioType switch
            {
                "TREATMENT" => accountCode switch
                {
                    var code when code.Contains("418") => 0.25m, // Water Plant Utilities - high impact
                    var code when code.Contains("425") => 0.20m, // Water Treatment Chemicals - high impact
                    var code when code.Contains("415") => 0.15m, // Water System Repairs - moderate impact
                    var code when code.Contains("430") => 0.08m, // Insurance - moderate impact
                    _ => 0.05m // Default low impact
                },
                "PIPELINE" => accountCode switch
                {
                    var code when code.Contains("415") => 0.20m, // Water System Repairs - high impact
                    var code when code.Contains("491") => 0.15m, // Vehicle Fuel - moderate impact
                    var code when code.Contains("418") => 0.10m, // Utilities - moderate impact
                    _ => 0.03m // Default low impact
                },
                "QUALITY" => accountCode switch
                {
                    var code when code.Contains("405") => 0.15m, // Quality Testing - high impact
                    var code when code.Contains("425") => 0.12m, // Treatment Chemicals - moderate impact
                    var code when code.Contains("413") => 0.10m, // Training - moderate impact
                    _ => 0.02m // Default low impact
                },
                _ => 0.05m // Default factor
            };
        }

        /// <summary>
        /// Apply water-specific adjustment factors
        /// </summary>
        private static void ApplyWaterFactors(SanitationDistrict district)
        {
            // Time-of-use factor adjustments (peak vs off-peak usage)
            if (district.TimeOfUseFactor > 0 && district.TimeOfUseFactor != 1.0m)
            {
                district.Scenario1 *= district.TimeOfUseFactor;
                district.Scenario2 *= district.TimeOfUseFactor;
                district.Scenario3 *= district.TimeOfUseFactor;
            }

            // Customer affordability index adjustments
            if (district.CustomerAffordabilityIndex > 0 && district.CustomerAffordabilityIndex != 1.0m)
            {
                district.Scenario1 *= district.CustomerAffordabilityIndex;
                district.Scenario2 *= district.CustomerAffordabilityIndex;
                district.Scenario3 *= district.CustomerAffordabilityIndex;
            }

            // Seasonal revenue factor for water usage patterns
            if (district.Section == "Revenue" && district.SeasonalRevenueFactor != 1.0m)
            {
                district.Scenario1 *= district.SeasonalRevenueFactor;
                district.Scenario2 *= district.SeasonalRevenueFactor;
                district.Scenario3 *= district.SeasonalRevenueFactor;
            }
        }

        /// <summary>
        /// Validate infrastructure allocation meets minimum requirements
        /// </summary>
        private static void ValidateInfrastructureAllocation(SanitationDistrict district)
        {
            // Ensure infrastructure investments meet minimum allocation requirements
            decimal minimumInfrastructure = district.CurrentFYBudget * MINIMUM_INFRASTRUCTURE_ALLOCATION;

            if (district.Scenario1 < minimumInfrastructure)
                district.Scenario1 = Math.Max(district.Scenario1, minimumInfrastructure);

            if (district.Scenario2 < minimumInfrastructure)
                district.Scenario2 = Math.Max(district.Scenario2, minimumInfrastructure);

            if (district.Scenario3 < minimumInfrastructure)
                district.Scenario3 = Math.Max(district.Scenario3, minimumInfrastructure);
        }

        /// <summary>
        /// Calculate required water rates for customer billing
        /// </summary>
        /// <param name="district">Water district account</param>
        /// <param name="totalExpenses">Total water system expenses</param>
        /// <param name="totalRevenue">Total water system revenue</param>
        /// <returns>Required rate per customer per month</returns>
        public static decimal CalculateWaterRequiredRate(SanitationDistrict district, decimal totalExpenses, decimal totalRevenue)
        {
            try
            {
                decimal calculatedRate = 0;

                switch (district.Section?.ToUpper())
                {
                    case "REVENUE":
                        calculatedRate = CalculateRevenueRequiredRate(district, totalExpenses, totalRevenue);
                        break;

                    case "OPERATING":
                        calculatedRate = district.CurrentFYBudget / WATER_CUSTOMER_BASE / 12;
                        break;

                    case "INFRASTRUCTURE":
                        // Infrastructure costs spread over customer base with goal adjustment
                        calculatedRate = (district.CurrentFYBudget + district.GoalAdjustment) / WATER_CUSTOMER_BASE / 12;
                        break;

                    case "QUALITY":
                        // Quality assurance costs
                        calculatedRate = district.CurrentFYBudget / WATER_CUSTOMER_BASE / 12;
                        break;

                    default:
                        calculatedRate = 0;
                        break;
                }

                // Apply adjustment factors
                if (district.TimeOfUseFactor > 0 && district.TimeOfUseFactor != 1.0m)
                    calculatedRate *= district.TimeOfUseFactor;

                if (district.CustomerAffordabilityIndex > 0 && district.CustomerAffordabilityIndex != 1.0m)
                    calculatedRate *= district.CustomerAffordabilityIndex;

                return Math.Max(0, calculatedRate);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Calculate revenue-based required rate
        /// </summary>
        private static decimal CalculateRevenueRequiredRate(SanitationDistrict district, decimal totalExpenses, decimal totalRevenue)
        {
            if (totalRevenue <= 0 || WATER_CUSTOMER_BASE <= 0) return 0;

            decimal revenueShare = district.CurrentFYBudget / totalRevenue;
            return (totalExpenses * revenueShare) / WATER_CUSTOMER_BASE / 12;
        }

        /// <summary>
        /// Get water enterprise summary statistics
        /// </summary>
        /// <param name="waterData">Water enterprise data</param>
        /// <returns>Dictionary of summary statistics</returns>
        public static Dictionary<string, object> GetWaterSummaryStatistics(List<SanitationDistrict> waterData)
        {
            var stats = new Dictionary<string, object>();

            if (waterData?.Any() != true)
                return stats;

            try
            {
                stats["TotalRevenue"] = waterData.Where(d => d.Section == "Revenue").Sum(d => d.CurrentFYBudget);
                stats["TotalOperatingExpenses"] = waterData.Where(d => d.Section == "Operating").Sum(d => d.CurrentFYBudget);
                stats["TotalInfrastructureInvestment"] = waterData.Where(d => d.Section == "Infrastructure").Sum(d => d.CurrentFYBudget);
                stats["TotalQualityInvestment"] = waterData.Where(d => d.Section == "Quality").Sum(d => d.CurrentFYBudget);
                stats["TotalExpenses"] = waterData.Where(d => d.Section != "Revenue").Sum(d => d.CurrentFYBudget);
                stats["NetSurplusDeficit"] = (decimal)stats["TotalRevenue"] - (decimal)stats["TotalExpenses"];
                stats["AverageRequiredRate"] = waterData.Where(d => d.RequiredRate > 0).DefaultIfEmpty().Average(d => d?.RequiredRate ?? 0);
                stats["TotalYTDSpending"] = waterData.Sum(d => d.YearToDateSpending);
                stats["TotalMonthlyUsage"] = waterData.Sum(d => d.MonthlyUsage);
                stats["InfrastructurePercentage"] = Math.Round(((decimal)stats["TotalInfrastructureInvestment"] / ((decimal)stats["TotalRevenue"] + (decimal)stats["TotalExpenses"])) * 100, 2);
                stats["QualityPercentage"] = Math.Round(((decimal)stats["TotalQualityInvestment"] / ((decimal)stats["TotalRevenue"] + (decimal)stats["TotalExpenses"])) * 100, 2);
                stats["CustomerBase"] = WATER_CUSTOMER_BASE;
                stats["TreatmentPlantMonthlyPayment"] = CalculateMonthlyPayment(TREATMENT_PLANT_COST, TREATMENT_PLANT_INTEREST_RATE, 20);
                stats["PipelineMonthlyPayment"] = CalculateMonthlyPayment(PIPELINE_REPLACEMENT_COST, PIPELINE_INTEREST_RATE, 10);
                stats["QualityUpgradeMonthlyPayment"] = CalculateMonthlyPayment(QUALITY_UPGRADE_COST, QUALITY_INTEREST_RATE, 8);

                // Validation flags
                stats["InfrastructureAdequate"] = (decimal)stats["InfrastructurePercentage"] >= (MINIMUM_INFRASTRUCTURE_ALLOCATION * 100);
                stats["QualityAdequate"] = (decimal)stats["QualityPercentage"] >= (MINIMUM_QUALITY_ALLOCATION * 100);
                stats["BudgetBalanced"] = (decimal)stats["NetSurplusDeficit"] >= 0;
            }
            catch (Exception ex)
            {
                stats["Error"] = $"Calculation error: {ex.Message}";
            }

            return stats;
        }

        /// <summary>
        /// Validate water enterprise data against Rate Study Methodology requirements
        /// </summary>
        /// <param name="waterData">Water enterprise data to validate</param>
        /// <returns>List of validation messages</returns>
        public static List<string> ValidateWaterEnterprise(List<SanitationDistrict> waterData)
        {
            var validationMessages = new List<string>();

            if (waterData?.Any() != true)
            {
                validationMessages.Add("ERROR: No water enterprise data found");
                return validationMessages;
            }

            var stats = GetWaterSummaryStatistics(waterData);

            // Infrastructure allocation validation
            if (stats.ContainsKey("InfrastructurePercentage"))
            {
                decimal infraPercent = (decimal)stats["InfrastructurePercentage"];
                if (infraPercent < (MINIMUM_INFRASTRUCTURE_ALLOCATION * 100))
                {
                    validationMessages.Add($"WARNING: Infrastructure allocation ({infraPercent:F1}%) below recommended minimum ({MINIMUM_INFRASTRUCTURE_ALLOCATION * 100:F0}%)");
                }
            }

            // Quality allocation validation
            if (stats.ContainsKey("QualityPercentage"))
            {
                decimal qualityPercent = (decimal)stats["QualityPercentage"];
                if (qualityPercent < (MINIMUM_QUALITY_ALLOCATION * 100))
                {
                    validationMessages.Add($"WARNING: Quality allocation ({qualityPercent:F1}%) below recommended minimum ({MINIMUM_QUALITY_ALLOCATION * 100:F0}%)");
                }
            }

            // Budget balance validation
            if (stats.ContainsKey("NetSurplusDeficit"))
            {
                decimal surplus = (decimal)stats["NetSurplusDeficit"];
                if (surplus < 0)
                {
                    validationMessages.Add($"WARNING: Water enterprise showing deficit of ${Math.Abs(surplus):N2}");
                }
            }

            // Account validation
            var requiredSections = new[] { "Revenue", "Operating", "Infrastructure", "Quality" };
            foreach (var section in requiredSections)
            {
                if (!waterData.Any(d => d.Section?.Equals(section, StringComparison.OrdinalIgnoreCase) == true))
                {
                    validationMessages.Add($"WARNING: No accounts found for {section} section");
                }
            }

            // Rate validation
            var averageRate = waterData.Where(d => d.RequiredRate > 0).DefaultIfEmpty().Average(d => d?.RequiredRate ?? 0);
            if (averageRate > 50) // Arbitrary high threshold
            {
                validationMessages.Add($"WARNING: Average required rate (${averageRate:F2}) seems high");
            }

            if (validationMessages.Count == 0)
            {
                validationMessages.Add("SUCCESS: Water enterprise validation passed");
            }

            return validationMessages;
        }
    }
}
