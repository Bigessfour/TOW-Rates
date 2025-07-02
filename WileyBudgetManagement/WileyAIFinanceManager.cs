using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Services.Enhanced;
using WileyBudgetManagement.Services.Calculations;

namespace WileyBudgetManagement.Services
{
    /// <summary>
    /// Main interface for Town of Wiley AI-enhanced utility management
    /// Combines AI natural language queries with sophisticated calculations
    /// Provides both AI insights and basic mathematical fallbacks
    /// </summary>
    public class WileyAIFinanceManager : IDisposable
    {
        private readonly AIEnhancedQueryService? _aiQueryService;
        private readonly AIEnhancedCalculations? _enhancedCalculations;
        private readonly bool _isAIAvailable;

        public WileyAIFinanceManager()
        {
            try
            {
                _aiQueryService = new AIEnhancedQueryService();
                _enhancedCalculations = new AIEnhancedCalculations();
                _isAIAvailable = true;
            }
            catch (Exception)
            {
                _isAIAvailable = false;
                // Continue with basic calculations only
            }
        }

        /// <summary>
        /// AI-enhanced rate optimization with smart fallbacks
        /// Priority: Customer affordability and revenue adequacy
        /// </summary>
        public async Task<RateOptimizationResult> OptimizeUtilityRates(
            EnterpriseContext enterpriseData,
            RateOptimizationGoals goals)
        {
            // Use enhanced calculations service (which handles AI vs basic internally)
            if (_enhancedCalculations != null)
            {
                var result = await _enhancedCalculations.OptimizeRates(enterpriseData, goals);

                // Add AI narrative analysis if available
                if (_isAIAvailable && _aiQueryService != null)
                {
                    try
                    {
                        var narrativeQuery = $@"Explain the rate optimization recommendation for {enterpriseData.Name}.
                                              Current rate: ${enterpriseData.RequiredRate:F2}
                                              Target revenue: ${goals.TargetRevenue:N2}
                                              Customer base: {enterpriseData.CustomerBase:N0}
                                              Provide clear reasoning and customer impact analysis.";

                        var aiResponse = await _aiQueryService.QueryRateOptimization(enterpriseData, narrativeQuery);
                        if (aiResponse.Success)
                        {
                            result.AIAnalysis = aiResponse.Analysis;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.AIAnalysis = $"Basic calculation completed. AI narrative unavailable: {ex.Message}";
                    }
                }

                return result;
            }

            // Fallback to very basic calculation if all else fails
            return new RateOptimizationResult
            {
                Success = false,
                Error = "Unable to initialize calculation services",
                CalculationMethod = "Error - No calculation method available"
            };
        }

        /// <summary>
        /// Customer affordability analysis with demographic insights
        /// Focuses on protecting vulnerable customer populations
        /// </summary>
        public async Task<AffordabilityAnalysisResult> AnalyzeCustomerAffordability(
            EnterpriseContext enterpriseData,
            decimal proposedRate,
            List<CustomerDemographics>? customerData = null)
        {
            customerData ??= GetDefaultDemographics(enterpriseData);

            if (_enhancedCalculations != null)
            {
                var result = await _enhancedCalculations.AnalyzeCustomerAffordability(
                    enterpriseData, customerData, proposedRate);

                // Add AI social impact analysis if available
                if (_isAIAvailable && _aiQueryService != null)
                {
                    try
                    {
                        var socialQuery = $@"Analyze the social impact of a ${proposedRate:F2} monthly rate for {enterpriseData.Name}.
                                           Consider rural Colorado demographics, fixed incomes, and seasonal employment.
                                           Recommend assistance programs and rate structure alternatives.";

                        var aiResponse = await _aiQueryService.ProcessGeneralQuery(socialQuery, enterpriseData);
                        if (aiResponse.Success)
                        {
                            result.SocioeconomicImpactAnalysis = aiResponse.Analysis;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.SocioeconomicImpactAnalysis = $"Basic affordability calculation completed. AI analysis unavailable: {ex.Message}";
                    }
                }

                return result;
            }

            return new AffordabilityAnalysisResult
            {
                Success = false,
                Error = "Affordability analysis service unavailable"
            };
        }

        /// <summary>
        /// Financial anomaly detection with pattern recognition
        /// Identifies unusual spending patterns and potential issues
        /// </summary>
        public async Task<AnomalyDetectionResult> DetectFinancialAnomalies(
            EnterpriseContext enterpriseData,
            List<FinancialDataPoint>? historicalData = null)
        {
            historicalData ??= GenerateBasicHistoricalData(enterpriseData);

            if (_enhancedCalculations != null)
            {
                var result = await _enhancedCalculations.DetectFinancialAnomalies(enterpriseData, historicalData);

                // Add AI context analysis if available
                if (_isAIAvailable && _aiQueryService != null)
                {
                    try
                    {
                        var contextQuery = $@"Analyze potential causes of financial anomalies in {enterpriseData.Name}.
                                            Consider seasonal patterns, equipment needs, regulatory changes, and economic factors.
                                            Provide practical investigation steps and prevention strategies.";

                        var aiResponse = await _aiQueryService.QueryAnomalyDetection(enterpriseData, contextQuery);
                        if (aiResponse.Success)
                        {
                            result.AnomalyAnalysis += "\n\nAI Context Analysis:\n" + aiResponse.Analysis;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.AnomalyAnalysis += $"\n\nNote: AI context analysis unavailable: {ex.Message}";
                    }
                }

                return result;
            }

            return new AnomalyDetectionResult
            {
                Success = false,
                Error = "Anomaly detection service unavailable"
            };
        }

        /// <summary>
        /// Revenue forecasting with economic trend consideration
        /// Projects future revenue under various scenarios
        /// </summary>
        public async Task<RevenueForecastResult> ForecastRevenue(
            EnterpriseContext enterpriseData,
            int forecastMonths = 12,
            List<HistoricalRevenueData>? historicalData = null)
        {
            historicalData ??= GenerateBasicRevenueData(enterpriseData, forecastMonths);

            if (_enhancedCalculations != null)
            {
                var result = await _enhancedCalculations.ForecastRevenue(enterpriseData, historicalData, forecastMonths);

                // Add AI economic context if available
                if (_isAIAvailable && _aiQueryService != null)
                {
                    try
                    {
                        var economicQuery = $@"Provide economic context for {forecastMonths}-month revenue forecast for {enterpriseData.Name}.
                                             Consider Colorado economic trends, utility industry changes, and demographic shifts.
                                             Assess risks and opportunities for municipal utilities.";

                        var aiResponse = await _aiQueryService.QueryRevenueForecast(enterpriseData, economicQuery);
                        if (aiResponse.Success)
                        {
                            result.KeyAssumptions.Add("AI Economic Analysis: " + aiResponse.Analysis);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.KeyAssumptions.Add($"Note: AI economic context unavailable: {ex.Message}");
                    }
                }

                return result;
            }

            return new RevenueForecastResult
            {
                Success = false,
                Error = "Revenue forecasting service unavailable"
            };
        }

        /// <summary>
        /// Regulatory compliance checking with current requirements
        /// Ensures municipal utility operations meet all regulations
        /// </summary>
        public async Task<ComplianceCheckResult> CheckRegulatoryCompliance(
            EnterpriseContext enterpriseData,
            List<RegulatoryRequirement>? requirements = null)
        {
            requirements ??= GetColoradoUtilityRequirements();

            if (_enhancedCalculations != null)
            {
                var result = await _enhancedCalculations.CheckRegulatoryCompliance(enterpriseData, requirements);

                // Add AI regulatory update context if available
                if (_isAIAvailable && _aiQueryService != null)
                {
                    try
                    {
                        var complianceQuery = $@"Review current Colorado municipal utility regulations for {enterpriseData.Name}.
                                               Identify any recent regulatory changes or upcoming requirements.
                                               Provide guidance on maintaining compliance and best practices.";

                        var aiResponse = await _aiQueryService.ProcessGeneralQuery(complianceQuery, enterpriseData);
                        if (aiResponse.Success)
                        {
                            result.ComplianceAnalysis += "\n\nRegulatory Update Analysis:\n" + aiResponse.Analysis;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.ComplianceAnalysis += $"\n\nNote: Regulatory update analysis unavailable: {ex.Message}";
                    }
                }

                return result;
            }

            return new ComplianceCheckResult
            {
                Success = false,
                Error = "Compliance checking service unavailable"
            };
        }

        /// <summary>
        /// General AI query for natural language financial questions
        /// Provides conversational interface to financial data
        /// </summary>
        public async Task<string> AskFinancialQuestion(string question, EnterpriseContext enterpriseData)
        {
            if (_isAIAvailable && _aiQueryService != null)
            {
                try
                {
                    var response = await _aiQueryService.ProcessGeneralQuery(question, enterpriseData);
                    return response.Success ? response.Analysis : response.Error;
                }
                catch (Exception ex)
                {
                    return $"AI service unavailable: {ex.Message}. Please use specific calculation methods instead.";
                }
            }

            return "AI query service is not available. Please use the specific calculation methods for rate optimization, affordability analysis, etc.";
        }

        #region Helper Methods

        private List<CustomerDemographics> GetDefaultDemographics(EnterpriseContext enterpriseData)
        {
            // Default demographics for rural Colorado community
            return new List<CustomerDemographics>
            {
                new CustomerDemographics
                {
                    CustomerSegment = "Residential - Low Income",
                    AverageHouseholdIncome = 35000m,
                    NumberOfCustomers = (int)(enterpriseData.CustomerBase * 0.25m),
                    UtilityBurdenPercentage = 0.06m,
                    GeographicArea = "Rural",
                    VulnerabilityFactors = new List<string> { "Fixed income", "Seasonal employment" }
                },
                new CustomerDemographics
                {
                    CustomerSegment = "Residential - Moderate Income",
                    AverageHouseholdIncome = 55000m,
                    NumberOfCustomers = (int)(enterpriseData.CustomerBase * 0.50m),
                    UtilityBurdenPercentage = 0.04m,
                    GeographicArea = "Rural",
                    VulnerabilityFactors = new List<string> { "Agricultural dependency" }
                },
                new CustomerDemographics
                {
                    CustomerSegment = "Residential - Higher Income",
                    AverageHouseholdIncome = 75000m,
                    NumberOfCustomers = (int)(enterpriseData.CustomerBase * 0.20m),
                    UtilityBurdenPercentage = 0.025m,
                    GeographicArea = "Rural"
                },
                new CustomerDemographics
                {
                    CustomerSegment = "Commercial/Agricultural",
                    AverageHouseholdIncome = 0m, // Not applicable
                    NumberOfCustomers = (int)(enterpriseData.CustomerBase * 0.05m),
                    UtilityBurdenPercentage = 0.03m,
                    GeographicArea = "Rural",
                    VulnerabilityFactors = new List<string> { "Market volatility", "Weather dependency" }
                }
            };
        }

        private List<FinancialDataPoint> GenerateBasicHistoricalData(EnterpriseContext enterpriseData)
        {
            var data = new List<FinancialDataPoint>();
            var random = new Random(42); // Consistent seed for reproducible results
            var baseValue = enterpriseData.YearToDateSpending / Math.Max(DateTime.Now.Month, 1);

            for (int i = 12; i >= 1; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var seasonalMultiplier = GetSeasonalMultiplier(date.Month);
                var randomVariation = 0.85m + ((decimal)random.NextDouble() * 0.3m); // ±15% variation
                var value = baseValue * seasonalMultiplier * randomVariation;

                data.Add(new FinancialDataPoint
                {
                    Date = date,
                    Value = value,
                    Category = "Monthly Spending",
                    Description = $"Estimated monthly spending for {date:MMM yyyy}"
                });
            }

            return data;
        }

        private List<HistoricalRevenueData> GenerateBasicRevenueData(EnterpriseContext enterpriseData, int months)
        {
            var data = new List<HistoricalRevenueData>();
            var random = new Random(42);
            var baseRevenue = enterpriseData.TotalRevenue / 12m; // Monthly average

            for (int i = months; i >= 1; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var seasonalMultiplier = GetSeasonalMultiplier(date.Month);
                var randomVariation = 0.9m + ((decimal)random.NextDouble() * 0.2m); // ±10% variation
                var revenue = baseRevenue * seasonalMultiplier * randomVariation;

                data.Add(new HistoricalRevenueData
                {
                    Date = date,
                    Revenue = revenue,
                    CustomerCount = enterpriseData.CustomerBase,
                    AverageRate = enterpriseData.RequiredRate,
                    RevenueType = "Total"
                });
            }

            return data;
        }

        private decimal GetSeasonalMultiplier(int month)
        {
            // Typical utility seasonal patterns for Colorado
            return month switch
            {
                12 or 1 or 2 => 1.15m, // Winter - higher usage
                3 or 4 or 5 => 0.95m,   // Spring - moderate usage
                6 or 7 or 8 => 1.25m,   // Summer - peak usage
                9 or 10 or 11 => 1.05m, // Fall - moderate usage
                _ => 1.0m
            };
        }

        private List<RegulatoryRequirement> GetColoradoUtilityRequirements()
        {
            return new List<RegulatoryRequirement>
            {
                new RegulatoryRequirement
                {
                    Name = "Reserve Fund Requirement",
                    Type = "Reserves",
                    Description = "Maintain adequate financial reserves",
                    MinValue = 100000m, // Example minimum
                    IsMandatory = true,
                    Authority = "State",
                    RecommendedAction = "Build reserves to recommended levels"
                },
                new RegulatoryRequirement
                {
                    Name = "Rate Setting Process",
                    Type = "Process",
                    Description = "Follow proper rate setting procedures",
                    IsMandatory = true,
                    Authority = "State",
                    RecommendedAction = "Ensure public hearings and proper notice"
                },
                new RegulatoryRequirement
                {
                    Name = "Budget Reporting",
                    Type = "Reporting",
                    Description = "Submit required financial reports",
                    IsMandatory = true,
                    Authority = "State",
                    RecommendedAction = "File annual budget and financial statements"
                }
            };
        }

        #endregion

        public void Dispose()
        {
            _aiQueryService?.Dispose();
            _enhancedCalculations?.Dispose();
        }
    }
}
