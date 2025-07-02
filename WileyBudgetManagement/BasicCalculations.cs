using System;
using System.Collections.Generic;
using System.Linq;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Services;

namespace WileyBudgetManagement.Services.Calculations
{
    /// <summary>
    /// Basic 9th-grade level calculations for Town of Wiley utility management
    /// These serve as fallback calculations when AI services are unavailable
    /// </summary>
    public class BasicCalculations : IDisposable
    {
        /// <summary>
        /// Basic rate optimization using simple math: Total Costs ÷ Number of Customers
        /// </summary>
        public RateOptimizationResult CalculateBasicRateOptimization(
            EnterpriseContext enterpriseData,
            RateOptimizationGoals goals)
        {
            // Simple calculation: Required Revenue ÷ Customer Count = Base Rate
            decimal baseRate = 0;
            if (enterpriseData.CustomerBase > 0)
            {
                baseRate = goals.TargetRevenue / enterpriseData.CustomerBase;
            }

            // Add 10% buffer for safety (basic percentage calculation)
            decimal recommendedRate = baseRate * 1.10m;

            // Check if rate increase is within limits
            decimal currentRate = enterpriseData.RequiredRate;
            decimal rateIncrease = 0;
            if (currentRate > 0)
            {
                rateIncrease = (recommendedRate - currentRate) / currentRate;
            }

            // If increase is too high, cap it at maximum allowed
            if (rateIncrease > goals.MaxRateIncreasePercent)
            {
                recommendedRate = currentRate * (1 + goals.MaxRateIncreasePercent);
            }

            return new RateOptimizationResult
            {
                Success = true,
                OptimizedRates = new List<OptimizedRate>
                {
                    new OptimizedRate
                    {
                        ServiceType = "Base Service",
                        CurrentRate = currentRate,
                        RecommendedRate = recommendedRate,
                        RateChange = recommendedRate - currentRate,
                        Justification = $"Simple calculation: ${goals.TargetRevenue:N0} ÷ {enterpriseData.CustomerBase} customers + 10% buffer",
                        ConfidenceLevel = 0.7m
                    }
                },
                RevenueProjection = new RevenueProjection
                {
                    ProjectedAnnualRevenue = recommendedRate * enterpriseData.CustomerBase * 12,
                    RevenueChange = (recommendedRate - currentRate) * enterpriseData.CustomerBase * 12
                },
                CustomerImpactAssessment = new CustomerImpactAssessment
                {
                    AverageMonthlyIncrease = recommendedRate - currentRate,
                    PercentageIncrease = rateIncrease,
                    EstimatedCustomerLoss = rateIncrease > 0.15m ? enterpriseData.CustomerBase * 0.05m : 0 // Assume 5% loss if >15% increase
                },
                ConfidenceScore = 0.7m
            };
        }

        /// <summary>
        /// Basic affordability calculation: Rate ÷ Average Income
        /// </summary>
        public AffordabilityAnalysisResult CalculateBasicAffordability(
            EnterpriseContext enterpriseData,
            decimal proposedRate)
        {
            // Assume average household income of $50,000 for Wiley, CO (basic assumption)
            decimal averageHouseholdIncome = 50000m;
            decimal monthlyIncome = averageHouseholdIncome / 12m;

            // Calculate annual utility cost
            decimal annualUtilityCost = proposedRate * 12m;
            
            // Affordability percentage (should be less than 2% of income for utilities)
            decimal affordabilityPercentage = annualUtilityCost / averageHouseholdIncome;

            // Simple classification
            string affordabilityRating;
            if (affordabilityPercentage <= 0.02m) // 2% or less
                affordabilityRating = "Highly Affordable";
            else if (affordabilityPercentage <= 0.04m) // 2-4%
                affordabilityRating = "Affordable";
            else if (affordabilityPercentage <= 0.06m) // 4-6%
                affordabilityRating = "Moderately Affordable";
            else
                affordabilityRating = "Challenging Affordability";

            // Estimate vulnerable customers (simple percentage based on rate)
            decimal vulnerablePercentage = Math.Min(affordabilityPercentage * 10, 0.3m); // Max 30%

            return new AffordabilityAnalysisResult
            {
                Success = true,
                OverallAffordabilityScore = Math.Max(0, 1 - affordabilityPercentage * 20), // Scale to 0-1
                VulnerableCustomerPercentage = vulnerablePercentage,
                AffordabilityRating = affordabilityRating,
                AverageMonthlyBurden = proposedRate,
                PercentageOfIncome = affordabilityPercentage,
                RecommendedAssistancePrograms = GetBasicAssistancePrograms(affordabilityPercentage),
                CalculationMethod = "Basic Income Percentage Method"
            };
        }

        /// <summary>
        /// Basic anomaly detection using standard deviation (2-sigma rule)
        /// </summary>
        public AnomalyDetectionResult DetectBasicAnomalies(List<FinancialDataPoint> historicalData)
        {
            if (historicalData == null || historicalData.Count < 3)
            {
                return new AnomalyDetectionResult
                {
                    Success = false,
                    Error = "Insufficient data for anomaly detection"
                };
            }

            // Calculate mean and standard deviation
            decimal mean = historicalData.Average(d => d.Value);
            decimal variance = historicalData.Sum(d => (d.Value - mean) * (d.Value - mean)) / historicalData.Count;
            decimal standardDeviation = (decimal)Math.Sqrt((double)variance);

            // Find outliers (more than 2 standard deviations from mean)
            var anomalies = new List<DetectedAnomaly>();
            decimal lowerBound = mean - (2 * standardDeviation);
            decimal upperBound = mean + (2 * standardDeviation);

            foreach (var dataPoint in historicalData)
            {
                if (dataPoint.Value < lowerBound || dataPoint.Value > upperBound)
                {
                    decimal deviationFromMean = Math.Abs(dataPoint.Value - mean);
                    decimal severityScore = deviationFromMean / standardDeviation;

                    anomalies.Add(new DetectedAnomaly
                    {
                        Date = dataPoint.Date,
                        Value = dataPoint.Value,
                        ExpectedValue = mean,
                        Deviation = deviationFromMean,
                        SeverityScore = severityScore,
                        AnomalyType = dataPoint.Value > upperBound ? "High Value" : "Low Value",
                        Description = $"Value ${dataPoint.Value:N2} is {severityScore:F1} standard deviations from mean ${mean:N2}"
                    });
                }
            }

            return new AnomalyDetectionResult
            {
                Success = true,
                DetectedAnomalies = anomalies,
                AnomalyAnalysis = $"Found {anomalies.Count} anomalies using 2-sigma rule. Mean: ${mean:N2}, Std Dev: ${standardDeviation:N2}",
                SeverityAssessment = new SeverityAssessment
                {
                    OverallSeverity = anomalies.Count > 3 ? "High" : anomalies.Count > 1 ? "Medium" : "Low",
                    CriticalAnomalies = anomalies.Count(a => a.SeverityScore > 3),
                    HighSeverityAnomalies = anomalies.Count(a => a.SeverityScore > 2 && a.SeverityScore <= 3),
                    MediumSeverityAnomalies = anomalies.Count(a => a.SeverityScore > 1.5m && a.SeverityScore <= 2),
                    LowSeverityAnomalies = anomalies.Count(a => a.SeverityScore <= 1.5m)
                },
                CalculationMethod = "2-Sigma Statistical Analysis"
            };
        }

        /// <summary>
        /// Basic revenue forecast using linear trend (y = mx + b)
        /// </summary>
        public RevenueForecastResult CalculateBasicRevenueForecast(
            List<HistoricalRevenueData> historicalData,
            int forecastMonths)
        {
            if (historicalData == null || historicalData.Count < 2)
            {
                return new RevenueForecastResult
                {
                    Success = false,
                    Error = "Insufficient historical data for forecasting"
                };
            }

            // Simple linear regression: find slope and intercept
            var sortedData = historicalData.OrderBy(d => d.Date).ToList();
            
            // Convert dates to months since start for easier calculation
            var startDate = sortedData.First().Date;
            var dataPoints = sortedData.Select((d, index) => new
            {
                X = index, // Month number
                Y = d.Revenue
            }).ToList();

            // Calculate slope (m) and intercept (b) for y = mx + b
            int n = dataPoints.Count;
            decimal sumX = dataPoints.Sum(p => p.X);
            decimal sumY = dataPoints.Sum(p => p.Y);
            decimal sumXY = dataPoints.Sum(p => p.X * p.Y);
            decimal sumXX = dataPoints.Sum(p => p.X * p.X);

            decimal slope = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);
            decimal intercept = (sumY - slope * sumX) / n;

            // Generate forecasts
            var projections = new List<decimal>();
            for (int i = 1; i <= forecastMonths; i++)
            {
                decimal futureMonth = n + i - 1; // Continue from last data point
                decimal projection = slope * futureMonth + intercept;
                projections.Add(Math.Max(0, projection)); // Don't allow negative revenue
            }

            // Calculate simple confidence intervals (±10% of projected value)
            var confidenceIntervals = projections.Select(p => new ConfidenceInterval
            {
                LowerBound = p * 0.9m,
                UpperBound = p * 1.1m,
                ConfidenceLevel = 0.8m // 80% confidence for basic calculation
            }).ToList();

            return new RevenueForecastResult
            {
                Success = true,
                MonthlyProjections = projections,
                ConfidenceIntervals = confidenceIntervals,
                ForecastAccuracy = 0.75m, // Moderate accuracy for linear trend
                ScenarioAnalysis = new ScenarioAnalysis
                {
                    OptimisticScenario = projections.Select(p => p * 1.15m).ToList(), // +15%
                    RealisticScenario = projections,
                    PessimisticScenario = projections.Select(p => p * 0.85m).ToList() // -15%
                },
                KeyAssumptions = new List<string>
                {
                    "Linear growth trend continues",
                    "No major economic disruptions",
                    "Customer base remains stable",
                    $"Historical growth rate: {slope:F2} per month"
                },
                CalculationMethod = "Linear Trend Analysis (y = mx + b)"
            };
        }

        /// <summary>
        /// Basic compliance check using simple rule validation
        /// </summary>
        public ComplianceCheckResult CheckBasicCompliance(
            EnterpriseContext enterpriseData,
            List<RegulatoryRequirement> requirements)
        {
            var complianceGaps = new List<string>();
            var recommendedActions = new List<string>();
            var passedChecks = 0;
            var totalChecks = 0;

            foreach (var requirement in requirements ?? new List<RegulatoryRequirement>())
            {
                totalChecks++;
                bool compliant = CheckSingleRequirement(enterpriseData, requirement);
                
                if (compliant)
                {
                    passedChecks++;
                }
                else
                {
                    complianceGaps.Add($"{requirement.Name}: {requirement.Description}");
                    recommendedActions.Add($"Address {requirement.Name} - {requirement.RecommendedAction}");
                }
            }

            // Basic financial compliance checks
            totalChecks += 4;

            // Check 1: Budget utilization should be reasonable
            if (enterpriseData.PercentOfBudgetUsed > 0.95m)
            {
                complianceGaps.Add("Budget Utilization: Exceeding safe budget limits");
                recommendedActions.Add("Implement budget controls and monitor spending closely");
            }
            else
            {
                passedChecks++;
            }

            // Check 2: Revenue should cover expenses
            if (enterpriseData.TotalRevenue < enterpriseData.TotalExpenses)
            {
                complianceGaps.Add("Revenue Adequacy: Revenue insufficient to cover expenses");
                recommendedActions.Add("Review rate structure or reduce expenses");
            }
            else
            {
                passedChecks++;
            }

            // Check 3: Should have some reserves
            if (enterpriseData.BudgetRemaining < enterpriseData.TotalBudget * 0.05m)
            {
                complianceGaps.Add("Reserve Requirements: Insufficient financial reserves");
                recommendedActions.Add("Build financial reserves to 5% of annual budget minimum");
            }
            else
            {
                passedChecks++;
            }

            // Check 4: Customer rates should be reasonable
            if (enterpriseData.RequiredRate > 0 && enterpriseData.CustomerAffordabilityIndex < 0.7m)
            {
                complianceGaps.Add("Rate Reasonableness: Rates may be unaffordable for customers");
                recommendedActions.Add("Review rate structure for customer affordability");
            }
            else
            {
                passedChecks++;
            }

            decimal complianceScore = totalChecks > 0 ? (decimal)passedChecks / totalChecks : 1.0m;

            return new ComplianceCheckResult
            {
                Success = true,
                OverallComplianceScore = complianceScore,
                CompliancePercentage = complianceScore,
                ComplianceGaps = complianceGaps,
                RecommendedActions = recommendedActions,
                RegulatoryRiskAssessment = GetRiskAssessment(complianceScore),
                ComplianceAnalysis = $"Basic compliance check: {passedChecks}/{totalChecks} requirements met ({complianceScore:P1} compliance)",
                CalculationMethod = "Basic Rule-Based Validation"
            };
        }

        #region Validation Methods

        public RateOptimizationResult ValidateRateOptimization(RateOptimizationResult aiResult)
        {
            // Basic sanity checks on AI results
            foreach (var rate in aiResult.OptimizedRates)
            {
                // Ensure rate is reasonable (not negative, not too high)
                if (rate.RecommendedRate < 0)
                {
                    rate.RecommendedRate = Math.Max(rate.CurrentRate, 0);
                    rate.Justification += " (Adjusted: Rate cannot be negative)";
                }

                if (rate.RecommendedRate > rate.CurrentRate * 2) // More than 100% increase
                {
                    rate.RecommendedRate = rate.CurrentRate * 1.5m; // Cap at 50% increase
                    rate.Justification += " (Adjusted: Rate increase capped at 50%)";
                }
            }

            return aiResult;
        }

        public AffordabilityAnalysisResult ValidateAffordabilityAnalysis(AffordabilityAnalysisResult aiResult)
        {
            // Ensure affordability score is in valid range
            aiResult.OverallAffordabilityScore = Math.Min(1.0m, Math.Max(0.0m, aiResult.OverallAffordabilityScore));
            
            // Ensure vulnerable percentage is reasonable
            aiResult.VulnerableCustomerPercentage = Math.Min(0.5m, Math.Max(0.0m, aiResult.VulnerableCustomerPercentage));

            return aiResult;
        }

        public AnomalyDetectionResult ValidateAnomalyDetection(AnomalyDetectionResult aiResult)
        {
            // Add basic statistical validation to AI results
            if (aiResult.DetectedAnomalies?.Count > 0)
            {
                var values = aiResult.DetectedAnomalies.Select(a => a.Value).ToList();
                decimal mean = values.Average();
                decimal maxDeviation = values.Max(v => Math.Abs(v - mean));
                
                // Flag if AI found anomalies that aren't statistically significant
                if (maxDeviation < mean * 0.1m) // Less than 10% deviation
                {
                    aiResult.AnomalyAnalysis += " (Note: Anomalies may not be statistically significant)";
                }
            }

            return aiResult;
        }

        public RevenueForecastResult ValidateRevenueForecast(RevenueForecastResult aiResult)
        {
            // Ensure projections are reasonable
            for (int i = 0; i < aiResult.MonthlyProjections.Count; i++)
            {
                if (aiResult.MonthlyProjections[i] < 0)
                {
                    aiResult.MonthlyProjections[i] = 0;
                }
                
                // Flag unrealistic growth (more than 20% per month)
                if (i > 0 && aiResult.MonthlyProjections[i] > aiResult.MonthlyProjections[i-1] * 1.2m)
                {
                    aiResult.KeyAssumptions.Add($"Warning: Month {i+1} shows high growth rate");
                }
            }

            return aiResult;
        }

        public ComplianceCheckResult ValidateComplianceCheck(ComplianceCheckResult aiResult)
        {
            // Ensure compliance score is in valid range
            aiResult.OverallComplianceScore = Math.Min(1.0m, Math.Max(0.0m, aiResult.OverallComplianceScore));
            
            return aiResult;
        }

        #endregion

        #region Helper Methods

        private List<string> GetBasicAssistancePrograms(decimal affordabilityPercentage)
        {
            var programs = new List<string>();

            if (affordabilityPercentage > 0.04m) // More than 4% of income
            {
                programs.Add("Low-income discount program (10-20% rate reduction)");
                programs.Add("Extended payment plans (up to 12 months)");
            }

            if (affordabilityPercentage > 0.06m) // More than 6% of income
            {
                programs.Add("Emergency assistance fund");
                programs.Add("Community support programs");
            }

            // Always include basic programs
            programs.Add("Budget billing to spread costs evenly");
            programs.Add("Water conservation education and rebates");

            return programs;
        }

        private bool CheckSingleRequirement(EnterpriseContext enterpriseData, RegulatoryRequirement requirement)
        {
            // Simple rule-based checking
            switch (requirement.Type?.ToLower())
            {
                case "budget":
                    return enterpriseData.PercentOfBudgetUsed <= requirement.Threshold;
                case "revenue":
                    return enterpriseData.TotalRevenue >= requirement.MinValue;
                case "reserves":
                    return enterpriseData.BudgetRemaining >= requirement.MinValue;
                default:
                    return true; // Assume compliant if we can't check
            }
        }

        private string GetRiskAssessment(decimal complianceScore)
        {
            if (complianceScore >= 0.9m)
                return "Low Risk - Strong compliance record";
            else if (complianceScore >= 0.75m)
                return "Medium Risk - Some compliance gaps identified";
            else if (complianceScore >= 0.5m)
                return "High Risk - Multiple compliance issues";
            else
                return "Critical Risk - Significant compliance failures";
        }

        #endregion

        public void Dispose()
        {
            // Nothing to dispose for basic calculations
        }
    }
}
